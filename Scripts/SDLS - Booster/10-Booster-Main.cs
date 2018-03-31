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
        public void Main(string argument, UpdateType updateSource) {
            Echo(Running.GetSymbol(Runtime));

            try {
                LoadBlocks();

                // Process Arguments
                ProcessArguments(argument);

                // Run State Machines
                if ((updateSource & UpdateType.Update10) == UpdateType.Update10) {
                    Operations.RunAll();
                }

                // Display LOG
                Echo(Log.GetLogText());
            } catch (Exception ex) {
                Echo(ex.ToString());
                throw;
            } finally {
                Debug?.UpdateDisplay();
            }
        }


        void ProcessArguments(string argument) {
            if (argument.Length == 0) return;
            switch (argument?.ToLower()) {
                case CMD_SHUTDOWN:
                    AllThrusters.ForEach(DisableThruster);
                    Operations.RemoveAll();
                    //Operations.Add("boom", ConnectBoom2("launch-pad", 0.5F));
                    break;
                case CMD_STANDBY:
                    //Operations.Add("boom", RetractBoom2("launch-pad", -0.5F), replace: true);
                    LoadBlocks();
                    break;
                case CMD_STAGE:
                    Operations.Add("stage", Sequence_Stage());
                    break;
            }
        }


        bool _loaded = false;
        void LoadBlocks(bool force = false) {
            if (_loaded && !force) return;

            BoosterControl = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>(b => IsSameGrid(b) && b is IMyRemoteControl);
            Orientation.Init(BoosterControl);

            GridTerminalSystem.GetBlocksOfType(Parachutes, IsSameGrid);
            GridTerminalSystem.GetBlocksOfType(Gyros, IsSameGrid);
            GridTerminalSystem.GetBlocksOfType(LaunchClamps, b => IsSameGrid(b) && Collect.IsTagged(b, "[launch-clamp]"));
            GridTerminalSystem.GetBlocksOfType(StageClamps, b => IsSameGrid(b) && Collect.IsTagged(b, "[stage-clamp]"));

            // Thrusters
            ManeuverThrusters.Clear();
            StageThrusters.Clear();
            BreakingThrusters.Clear();
            LandingThrusters1.Clear();
            LandingThrusters2.Clear();
            LandingThrusters3.Clear();
            AscentThrusters.Clear();
            GridTerminalSystem.GetBlocksOfType(AllThrusters, b => {
                if (!IsSameGrid(b)) return false;
                if (Collect.IsTagged(b, "[maneuver]")) ManeuverThrusters.Add(b);
                if (Collect.IsTagged(b, "[land-1]")) LandingThrusters1.Add(b);
                if (Collect.IsTagged(b, "[land-2]")) LandingThrusters2.Add(b);
                if (Collect.IsTagged(b, "[land-3]")) LandingThrusters3.Add(b);
                if (Collect.IsTagged(b, "[breaking]")) BreakingThrusters.Add(b);
                if (Collect.IsTagged(b, "[main]")) AscentThrusters.Add(b);
                return true;
            });

            if (StageClamps.Count > 0) {
                var a = StageClamps[0].Orientation.TransformDirectionInverse(Base6Directions.Direction.Down);
                var st = ManeuverThrusters
                    .Where(t => a == t.Orientation.TransformDirection(Base6Directions.Direction.Forward));
                StageThrusters.AddRange(st);
            }

            //Debug.AppendLine($"Gyros: {Gyros.Count}");
            //Debug.AppendLine($"Parachutes: {Parachutes.Count}");
            //Debug.AppendLine($"Ascent T: {AscentThrusters.Count}");
            //Debug.AppendLine($"ManeuverThrusters T: {ManeuverThrusters.Count}");
            //Debug.AppendLine($"Breaking T: {BreakingThrusters.Count}");
            //Debug.AppendLine($"Landing1 T: {LandingThrusters1.Count}");
            //Debug.AppendLine($"Landing2 T: {LandingThrusters2.Count}");
            //Debug.AppendLine($"Landing3 T: {LandingThrusters3.Count}");
            //Debug.AppendLine($"Staging T: {StageThrusters.Count}");
            //Debug.AppendLine($"Launch Clamps: {LaunchClamps.Count}");
            //Debug.AppendLine($"Stage Clamps: {StageClamps.Count}");

            _loaded = true;
        }

        bool IsSameGrid(IMyTerminalBlock b) => Me.CubeGrid == b.CubeGrid;
    }
}
