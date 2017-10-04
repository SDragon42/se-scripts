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

            GridTerminalSystem.SearchBlocksOfName(gateTag, _gateBlocks, IsTaggedStation);
            GridTerminalSystem.SearchBlocksOfName(gateTag, _armLights, IsLightOnTransferArm);
            var _armRotor = GetFirstBlockInList<IMyMotorAdvancedStator>(_gateBlocks, IsOnTransferArm);
            var _armPiston = GetFirstBlockInList<IMyPistonBase>(_gateBlocks, IsOnTransferArm);
            var _armConnector = GetFirstBlockInList<IMyShipConnector>(_gateBlocks, IsOnTransferArm);
            var _terminalPiston = GetFirstBlockInList<IMyPistonBase>(_gateBlocks, IsOnTerminal);
            GridTerminalSystem.SearchBlocksOfName(gateTag, _terminalDoors, IsDoorOnTerminal);

            var CanSendConnectedMessage = false;
            var CanSendDisconnectedMessage = false;

            var newState = HookupState.Disconnecting;
            if (carriage.Connect) {
                var completed = ConnectArm(_armRotor, _armPiston, _armConnector, _terminalPiston);
                completed &= ExtendRamp(_armRotor, _armPiston, _armConnector, _terminalPiston);
                newState = completed ? HookupState.Connected : HookupState.Connecting;
            } else {
                var completed = DisconnectArm(_armRotor, _armPiston, _armConnector, _terminalPiston);
                completed &= RetractRamp(_armRotor, _armPiston, _armConnector, _terminalPiston);
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

        bool ConnectArm(IMyMotorAdvancedStator _armRotor, IMyPistonBase _armPiston, IMyShipConnector _armConnector, IMyPistonBase _terminalPiston) {
            // turn on lights
            foreach (var light in _armLights) {
                ((IMyFunctionalBlock)light).Enabled = true;
            }

            // rotate arm
            if (_armRotor != null) {
                var currAngle = Math.Round(_armRotor.Angle, ElevatorConst.RADIAN_ROUND_DIGITS);
                var maxAngle = Math.Round(_armRotor.UpperLimit, ElevatorConst.RADIAN_ROUND_DIGITS);
                if (currAngle < maxAngle) {
                    _armRotor.SafetyLock = false;
                    _armRotor.SetValueFloat("Velocity", ElevatorConst.ROTOR_VELOCITY);
                    return false; // not in position yet
                }
                _armRotor.SafetyLock = true;
            }

            if (_armConnector == null) return false;
            // extend pistion - extends till the piston can connect
            if (_armConnector.Status == MyShipConnectorStatus.Unconnected) {
                if (_armPiston != null) {
                    _armPiston.SafetyLock = false;
                    _armPiston.Extend();
                }
            } else {
                if (_armPiston != null)
                    _armPiston.SafetyLock = true;
                _armConnector.Connect();
            }

            return (_armConnector.Status == MyShipConnectorStatus.Connected);
        }
        bool DisconnectArm(IMyMotorAdvancedStator _armRotor, IMyPistonBase _armPiston, IMyShipConnector _armConnector, IMyPistonBase _terminalPiston) {
            // retract piston
            if (_armConnector == null) return false;
            _armConnector.Disconnect();
            if (_armPiston != null) {
                if (_armPiston.CurrentPosition > _armPiston.MinLimit) {
                    _armPiston.SafetyLock = false;
                    _armPiston.Retract();
                    return false; // not fully retracted
                }
                _armPiston.SafetyLock = true;
            }

            // rotate arm
            if (_armRotor != null) {
                var currAngle = Math.Round(_armRotor.Angle, ElevatorConst.RADIAN_ROUND_DIGITS);
                var minAngle = Math.Round(_armRotor.LowerLimit, ElevatorConst.RADIAN_ROUND_DIGITS);
                if (currAngle > minAngle) {
                    _armRotor.SafetyLock = false;
                    _armRotor.SetValueFloat("Velocity", ElevatorConst.ROTOR_VELOCITY * -1);
                    return false; // not fully retracted
                }
                _armRotor.SafetyLock = true;
            }

            // turn off lights
            foreach (var light in _armLights) {
                ((IMyFunctionalBlock)light).Enabled = false;
            }

            return true;
        }

        bool ExtendRamp(IMyMotorAdvancedStator _armRotor, IMyPistonBase _armPiston, IMyShipConnector _armConnector, IMyPistonBase _terminalPiston) {
            // extend piston
            if (_terminalPiston != null) {
                _terminalPiston.Extend();
                if (_terminalPiston.CurrentPosition < _terminalPiston.MaxLimit) return false;
            }

            // open doors
            if (_terminalDoors != null) {
                foreach (var b in _terminalDoors) {
                    var door = (IMyDoor)b;
                    if (door.Status != DoorStatus.Open && door.Status != DoorStatus.Opening) {
                        door.Enabled = true;
                        door.OpenDoor();
                    }
                }
            }

            return true;
        }
        bool RetractRamp(IMyMotorAdvancedStator _armRotor, IMyPistonBase _armPiston, IMyShipConnector _armConnector, IMyPistonBase _terminalPiston) {
            // Close doors
            if (_terminalDoors != null) {
                foreach (var b in _terminalDoors) {
                    var door = (IMyDoor)b;
                    if (door.Status != DoorStatus.Closing && door.Status != DoorStatus.Closed) {
                        door.Enabled = true;
                        door.CloseDoor();
                    }
                }
                foreach (var b in _terminalDoors) {
                    var door = (IMyDoor)b;
                    if (door.Status != DoorStatus.Closed)
                        return false;
                }

                // lock doors
                foreach (var b in _terminalDoors) {
                    var door = (IMyDoor)b;
                    if (door.Status == DoorStatus.Closed)
                        door.Enabled = false;
                }
            }

            // retract piston
            if (_terminalPiston != null) {
                _terminalPiston.Retract();
                return !(_terminalPiston.CurrentPosition > _terminalPiston.MinLimit);
            }

            return true;
        }

    }
}
