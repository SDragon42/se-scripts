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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {
        const string TAG_LAUNCH = "[launch]";
        const string TAG_LANDING1 = "landing-1";
        const string TAG_LANDING2 = "landing-2";
        const string TAG_MANEUVER = "[maneuver]";
        const string TAG_STAGE_SEP = "[stage-sep]";
        const string TAG_LAUNCH_CLAMP = "[launch-clamp]";

        const string GPS_LZ1 = "GPS:LZ-1/Grasshopper:-20189.71:18615.27:54263.73:";
        const string GPS_LZ2 = "GPS:LZ-2/Grasshopper:-19891.45:18820.13:54302.99:";

        readonly Vector3D LZ1_Vector;
        readonly Vector3D LZ2_Vector;

        readonly RunningSymbol RunSymbol = new RunningSymbol();
        readonly Logging Log = new Logging(20);

        readonly List<IMyThrust> Thrusters_Launch = new List<IMyThrust>();
        readonly List<IMyThrust> Thrusters_Landing1 = new List<IMyThrust>();
        readonly List<IMyThrust> Thrusters_Landing2 = new List<IMyThrust>();
        readonly List<IMyThrust> Thrusters_Maneuver = new List<IMyThrust>();
        readonly List<IMyThrust> Thrusters_StageSep = new List<IMyThrust>();
        readonly List<IMyMotorStator> LaunchClamps = new List<IMyMotorStator>();

        public Program() {
            LZ1_Vector = VectorHelper.GpsToVector(GPS_LZ1);
            LZ2_Vector = VectorHelper.GpsToVector(GPS_LZ2);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo("GrassHopper " + RunSymbol.GetSymbol(Runtime));
            try {
                // Process Arguments
                ProcessArguments(argument);

                // Run State Machines
                if ((updateSource & UpdateType.Update10) == UpdateType.Update10) {
                    RunStateMachines();
                }

                // Display LOG
                Echo(Log.GetLogText());
            } catch (Exception ex) {
                // Display LOG
                Echo(Log.GetLogText());
                Echo("*************");
                Echo(ex.ToString());
                throw;
            }
        }

        void ProcessArguments(string argument) {
            if (argument.Length == 0) return;
            Log.AppendLine($"{DateTime.Now.ToShortTimeString()} - {argument}");
            switch (argument?.ToLower()) {
                case "go":
                    if (_Operations.Count == 0)
                        _Operations.Add(FlightTest_Hover());
                    break;
                case "abort":
                    while (_Operations.Count > 0) {
                        _Operations[0].Dispose();
                        _Operations.RemoveAt(0);
                    }
                    _Operations.Add(FlightTest_Abort());
                    break;
            }
        }


        readonly List<IEnumerator<bool>> _Operations = new List<IEnumerator<bool>>();
        public void RunStateMachines() {
            var idx = 0;
            while (idx < _Operations.Count) {
                Echo($"OP # {idx} of {_Operations.Count}");
                var op = _Operations[idx];
                if (op.MoveNext() && op.Current) {
                    idx++;
                } else {
                    _Operations.RemoveAt(idx);
                    op.Dispose();
                    op = null;
                    Log.AppendLine($"Removed OP at {idx}");
                }
            }
        }

        IEnumerator<bool> FlightTest_Abort() {
            GtsBlocks(Thrusters_Launch, TAG_LAUNCH);
            GtsBlocks(Thrusters_Landing1, TAG_LANDING1);
            GtsBlocks(Thrusters_Landing2, TAG_LANDING2);
            GtsBlocks(Thrusters_Maneuver, TAG_MANEUVER);
            GtsBlocks(Thrusters_StageSep, TAG_STAGE_SEP);

            yield return true;

            Thrusters_Launch.ForEach(Thruster_Off_NoThrust);
            Thrusters_Maneuver.ForEach(Thruster_Off_NoThrust);
            Thrusters_StageSep.ForEach(Thruster_Off_NoThrust);
        }

        

        double getGravityForceN(IMyShipController sc) {
            var gVector = sc.GetNaturalGravity();
            var gMs = Math.Sqrt(
                Math.Pow(gVector.X, 2) +
                Math.Pow(gVector.Y, 2) +
                Math.Pow(gVector.Z, 2));
            var mass = sc.CalculateShipMass().PhysicalMass;
            var gravForce = mass * gMs;
            return gravForce;
        }

        void Thruster_Off_NoThrust(IMyThrust t) {
            t.Enabled = false;
            t.ThrustOverride = 0f;
        }
        void Thruster_Off_FullThrust(IMyThrust t) {
            t.Enabled = false;
            t.ThrustOverride = t.MaxThrust;
        }
        void Thruster_On(IMyThrust t) => t.Enabled = true;

        void GtsBlocks<T>(List<T> list, string tag) where T : class, IMyTerminalBlock {
            GridTerminalSystem.GetBlocksOfType(list, b => b.CubeGrid == Me.CubeGrid && Collect.IsTagged(b, tag));
        }
    }
}
