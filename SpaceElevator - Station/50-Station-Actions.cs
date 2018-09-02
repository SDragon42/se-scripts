using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        //-------------------------------------------------------------------------------
        //  CARRIAGE DOCK OPERATIONS
        //-------------------------------------------------------------------------------

        void RunCarriageDockDepartureActions(string gateTag, CarriageVars carriage) {
            var CanSendConnectedMessage = false;
            var CanSendDisconnectedMessage = false;

            var newState = HookupState.Disconnecting;
            if (carriage.Connect) {
                var completed = ConnectToCarriage(gateTag, _gateTransfer);
                completed &= ConnectToCarriage(gateTag, _gateTerminal);
                newState = completed ? HookupState.Connected : HookupState.Connecting;
            } else {
                var completed = DisconnectFromCarriage(gateTag, _gateTransfer);
                completed &= DisconnectFromCarriage(gateTag, _gateTerminal);
                newState = completed ? HookupState.Disconnected : HookupState.Disconnecting;
            }

            if (newState == HookupState.Connected && (carriage.GateState == HookupState.Connecting || carriage.SendResponseMsg))
                CanSendConnectedMessage = true;
            if (newState == HookupState.Disconnected && (carriage.GateState == HookupState.Disconnecting || carriage.SendResponseMsg))
                CanSendDisconnectedMessage = true;
            carriage.GateState = newState;

            if (CanSendConnectedMessage && carriage.SendResponseMsg) {
                SendResponseMessage(carriage.GridName, StationResponses.DockingComplete);
                carriage.SendResponseMsg = false;
            }
            if (CanSendDisconnectedMessage && carriage.SendResponseMsg) {
                SendResponseMessage(carriage.GridName, StationResponses.DepartureOk);
                carriage.SendResponseMsg = false;
            }
        }

        bool DisconnectFromCarriage(string gateTag, List<IMyTerminalBlock> blocks) {
            // Doors
            var doors = GetBlocksOfType<IMyDoor>(gateTag, blocks, Collect.IsDoor);
            var allClosed = true;
            foreach (var door in doors) {
                if (door.Status == DoorStatus.Closed) {
                    door.Enabled = false;
                } else {
                    allClosed = false;
                    door.Enabled = true;
                    if (door.Status != DoorStatus.Closing)
                        door.CloseDoor();
                }
            }
            if (!allClosed) return false;

            // Connectors
            var connector = GetBlockOfType<IMyShipConnector>(gateTag, blocks);
            if (connector != null && connector.Status == MyShipConnectorStatus.Connected) {
                connector.Disconnect();
                if (connector.Status == MyShipConnectorStatus.Connected)
                    return false;
            }

            // Pistons
            var piston = GetBlockOfType<IMyPistonBase>(gateTag, blocks);
            if (piston != null) {
                piston.Retract();
                if (piston.CurrentPosition > piston.MinLimit)
                    return false;
            }

            // Rotors
            var rotors = GetBlocksOfType<IMyMotorStator>(gateTag, blocks);
            var terminate = false;
            foreach (var rotor in rotors) {
                var currAngle = Math.Round(rotor.Angle, RotorConstants.RADIAN_ROUND_DIGITS);
                var minAngle = Math.Round(rotor.LowerLimitRad, RotorConstants.RADIAN_ROUND_DIGITS);
                if (currAngle > minAngle) {
                    rotor.TargetVelocityRPM = RotorConstants.ROTOR_VELOCITY * -1;
                    terminate = true;
                }
            }
            if (terminate) return false;

            // Lights
            var lights = GetBlocksOfType<IMyFunctionalBlock>(gateTag, blocks, b => (b is IMyInteriorLight || b is IMyReflectorLight));
            foreach (var light in lights) light.Enabled = false;

            // Done
            return true;
        }
        bool ConnectToCarriage(string gateTag, List<IMyTerminalBlock> blocks) {
            // Lights
            var lights = GetBlocksOfType<IMyFunctionalBlock>(gateTag, blocks, b => (b is IMyInteriorLight || b is IMyReflectorLight));
            foreach (var light in lights) light.Enabled = true;

            // Rotors
            var rotors = GetBlocksOfType<IMyMotorStator>(gateTag, blocks);
            var terminate = false;
            foreach (var rotor in rotors) {
                var currAngle = Math.Round(rotor.Angle, RotorConstants.RADIAN_ROUND_DIGITS);
                var maxAngle = Math.Round(rotor.UpperLimitRad, RotorConstants.RADIAN_ROUND_DIGITS);
                if (currAngle < maxAngle) {
                    rotor.TargetVelocityRPM = RotorConstants.ROTOR_VELOCITY;
                    terminate = true;
                }
            }
            if (terminate) return false;

            // Pistons / Connectors
            var piston = GetBlockOfType<IMyPistonBase>(gateTag, blocks);
            var connector = GetBlockOfType<IMyShipConnector>(gateTag, blocks);
            if (connector == null || connector.Status == MyShipConnectorStatus.Unconnected) {
                if (piston != null) {
                    piston.Extend();
                    if (piston.CurrentPosition < piston.MaxLimit) return false;
                }
            } else {
                connector?.Connect();
            }

            if (connector != null)
                return (connector.Status == MyShipConnectorStatus.Connected);

            // Doors
            var doors = GetBlocksOfType<IMyDoor>(gateTag, blocks, Collect.IsDoor);
            var allOpen = true;
            foreach (var door in doors) {
                if (door.Status == DoorStatus.Open) {
                    door.Enabled = false;
                } else {
                    allOpen = false;
                    door.Enabled = true;
                    if (door.Status != DoorStatus.Opening)
                        door.OpenDoor();
                }
            }
            if (!allOpen) return false;

            // Done
            return true;
        }


        IEnumerable<T> GetBlocksOfType<T>(string gateTag, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) {
            foreach (var b in blocks) {
                if (Collect.IsTagged(b, gateTag) && b is T && (collect?.Invoke(b) ?? true))
                    yield return (T)b;
            }
        }
        T GetBlockOfType<T>(string gateTag, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) {
            foreach (var b in blocks) {
                if (Collect.IsTagged(b, gateTag) && b is T && (collect?.Invoke(b) ?? true))
                    return (T)b;
            }
            return default(T);
        }

    }
}
