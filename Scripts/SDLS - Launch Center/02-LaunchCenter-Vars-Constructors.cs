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
    partial class Program : MyGridProgram {

        // Modules
        readonly RunningSymbol _runSymbol = new RunningSymbol();
        readonly Logging _log = new Logging(20);

        //Lists
        readonly List<IEnumerator<bool>> _Operations = new List<IEnumerator<bool>>();
        readonly List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();

        bool boomBooster = false;
        bool boomCargo = false;
        bool boomCrew = false;

        public Program() {
            this.Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo("Launch Center " + _runSymbol.GetSymbol(Runtime));
            try {
                // Process Arguments
                ProcessArguments(argument);

                // Run State Machines
                if ((updateSource & UpdateType.Update10) == UpdateType.Update10) {
                    RunStateMachines();
                }

                // Display LOG
                Echo(_log.GetLogText());
            } catch (Exception ex) {
                Echo(ex.ToString());
                throw;
            }
        }

        void ProcessArguments(string argument) {
            if (argument.Length == 0) return;
            _log.AppendLine($"{DateTime.Now.ToShortTimeString()} - {argument}");
            switch (argument?.ToLower()) {
                case "extend":
                    MoveBoomBooster(true);
                    MoveBoomOrbiter(true);
                    break;
                case "extend-booster":
                    MoveBoomBooster(true);
                    break;
                case "extend-orbiter":
                    MoveBoomOrbiter(true);
                    break;
                case "extend-crew":
                    break;
                case "retract":
                    MoveBoomBooster(false);
                    MoveBoomOrbiter(false);
                    break;
                case "retract-booster":
                    MoveBoomBooster(false);
                    break;
                case "retract-orbiter":
                    MoveBoomOrbiter(false);
                    break;
                case "retract-crew":
                    break;
            }
        }

        public void RunStateMachines() {
            var idx = 0;
            //Echo($"# OPS: {_Operations.Count:N0}");
            while (idx < _Operations.Count) {
                Echo($"OP # {idx} of {_Operations.Count}");
                var op = _Operations[idx];
                if (op.MoveNext() && op.Current) {
                    idx++;
                } else {
                    _Operations.RemoveAt(idx);
                    op.Dispose();
                    op = null;
                    _log.AppendLine($"Removed OP at {idx}");
                }
            }
        }

        void MoveBoomBooster(bool extend) {
            //if (boomBooster) return;
            //boomBooster = true;
            var op = extend
                ? ConnectBoom("[boom-booster]", 1f, RotorLimit.Low)
                : RetractBoom("[boom-booster]", 1f, RotorLimit.High);
            _Operations.Add(op);
        }
        void MoveBoomOrbiter(bool extend) {
            //if (boomCargo) return;
            //boomCargo = true;
            var op = extend
                ? ConnectBoom("[boom-orbiter]", 1f, RotorLimit.High)
                : RetractBoom("[boom-orbiter]", 1f, RotorLimit.Low);
            _Operations.Add(op);
        }


        bool Connector_Disconnect(IMyShipConnector connector) {
            if (!Collect.IsConnectorConnected(connector))
                return false;
            connector.Disconnect();
            return true;
        }
        bool Connector_Connect(IMyShipConnector connector) {
            if (Collect.IsConnectorConnected(connector))
                return false;
            if (!Collect.IsConnectorConnectable(connector))
                return true;
            connector.Connect();
            return true;
        }

        bool Rotor_RotateTo(IMyMotorStator rotor, float velocity, double limit) {
            var currAngle = Math.Round(rotor.Angle, 3);
            var diff = currAngle - limit;

            if (diff == 0.0) return false;

            velocity = velocity * (currAngle > limit ? -1 : 1);
            rotor.TargetVelocityRPM = velocity;

            return true;
        }

        IEnumerator<bool> ConnectBoom(string boomTag, float velocity, RotorLimit toLimit) {
            GridTerminalSystem.GetBlocksOfType(_blocks, b => Collect.IsTagged(b, boomTag));

            var rotor = _blocks.Where(b => b is IMyMotorAdvancedStator).FirstOrDefault() as IMyMotorAdvancedStator;
            var connector = _blocks.Where(Collect.IsConnector).FirstOrDefault() as IMyShipConnector;

            if (rotor == null || connector == null)
                yield return false;

            var limit = Math.Round(
                (toLimit == RotorLimit.Low) ? rotor.LowerLimitRad : rotor.UpperLimitRad,
                3);

            while (Connector_Disconnect(connector)) yield return true;
            while (Rotor_RotateTo(rotor, velocity, limit)) yield return true;
            while (Connector_Connect(connector)) yield return true;
            //boomBooster = false;
        }
        IEnumerator<bool> RetractBoom(string boomTag, float velocity, RotorLimit toLimit) {
            GridTerminalSystem.GetBlocksOfType(_blocks, b => Collect.IsTagged(b, boomTag));

            var rotor = _blocks.Where(b => b is IMyMotorAdvancedStator).FirstOrDefault() as IMyMotorAdvancedStator;
            var connector = _blocks.Where(Collect.IsConnector).FirstOrDefault() as IMyShipConnector;

            if (rotor == null || connector == null)
                yield return false;

            var limit = Math.Round(
                (toLimit == RotorLimit.Low) ? rotor.LowerLimitRad : rotor.UpperLimitRad,
                3);

            while (Connector_Disconnect(connector)) yield return true;
            while (Rotor_RotateTo(rotor, velocity, limit)) yield return true;
            //boomBooster = false;
        }

        enum RotorLimit { Low, High }
    }
}
