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
            UpTime += Runtime.TimeSinceLastRun;
            if ((updateSource.HasFlag(UpdateType.Update10))) {
                Echo(Running.GetSymbol(Runtime));
            }

            if (!Me.CubeGrid.CustomName.EndsWith(GRID_TAG))
                Me.CubeGrid.CustomName += " " + GRID_TAG;

            try {
                LoadBlocks();
                ProcessArguments(argument.ToLower());
                Operations.RunAll();
                Echo(Log.GetLogText());
            } catch (Exception ex) {
                Echo(ex.ToString());
                throw;
            } finally {
                Debug?.UpdateDisplay();
            }
            //if (updateSource.HasFlag(UpdateType.Terminal)) {
            //    switch (argument.ToLower()) {
            //        case CMD_STANDBY:
            //            Runtime.UpdateFrequency = UpdateFrequency.None;
            //            break;
            //        default:
            //            Runtime.UpdateFrequency |= UpdateFrequency.Update10;
            //            break;
            //    }
            //}
        }

        void LoadBlocks(bool force = false) {
            if (BlocksLoaded && !force) return;

            Remote = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>(IsSameGrid);
            Antenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(IsSameGrid);
            Beacon = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyBeacon>(IsSameGrid);

            Orientation.Init(Remote);

            GridTerminalSystem.GetBlocksOfType(Parachutes, IsSameGrid);
            GridTerminalSystem.GetBlocksOfType(Gyros, IsSameGrid);
            GridTerminalSystem.GetBlocksOfType(LandingGears, IsSameGrid);
            GridTerminalSystem.GetBlocksOfType(LaunchClamps, b => IsSameGrid(b) && Collect.IsTagged(b, TAG_LAUNCH_CLAMP));
            GridTerminalSystem.GetBlocksOfType(StageClamps, b => IsSameGrid(b) && Collect.IsTagged(b, TAG_STAGING_CLAMP));

            // Thrusters
            ManeuverThrusters.Clear();
            StageThrusters.Clear();
            AscentThrusters.Clear();
            GridTerminalSystem.GetBlocksOfType(AllThrusters, b => {
                if (!IsSameGrid(b)) return false;
                //if (Collect.IsTagged(b, TAG_MANEUVER)) ManeuverThrusters.Add(b);
                if (Collect.IsTagged(b, TAG_MAIN)) AscentThrusters.Add(b);
                return true;
            });

            // Staging Thrusters
            StageThrusters.AddRange(AllThrusters.Where(Orientation.IsUp)); // slow down thrusters
            if (StageClamps.Count > 0) {
                var a = StageClamps[0].Orientation.TransformDirectionInverse(Base6Directions.Direction.Down);
                var st = ManeuverThrusters.Where(t => a == t.Orientation.TransformDirection(Base6Directions.Direction.Forward));
                StageThrusters.AddRange(st);
            }

            Debug.AppendLine($"Gyros: {Gyros.Count}");
            Debug.AppendLine($"Parachutes: {Parachutes.Count}");
            Debug.AppendLine($"Launch Clamps: {LaunchClamps.Count}");
            Debug.AppendLine($"Stage Clamps: {StageClamps.Count}");
            Debug.AppendLine($"Ascent T: {AscentThrusters.Count}");
            Debug.AppendLine($"ManeuverThrusters T: {ManeuverThrusters.Count}");
            Debug.AppendLine($"Staging T: {StageThrusters.Count}");
            //Debug.AppendLine($"Landing1 T: {LandingThrusters1.Count}");
            //Debug.AppendLine($"Landing2 T: {LandingThrusters2.Count}");
            //Debug.AppendLine($"Landing3 T: {LandingThrusters3.Count}");

            BlocksLoaded = true;
        }


        bool IsSameGrid(IMyTerminalBlock b) => Me.CubeGrid == b.CubeGrid;
        bool IsOnRocket(IMyTerminalBlock b) => rocketParts.Contains(b.CubeGrid);

        void ProcessArguments(string argument) {
            if (!Commands.ContainsKey(argument)) return;
            Commands[argument]?.Invoke();
        }


        void TurnOff() {
            Runtime.UpdateFrequency = UpdateFrequency.None;
            AllThrusters.ForEach(t => t.Enabled = false);
        }

        void ManualControl() {
            Runtime.UpdateFrequency = UpdateFrequency.None;
            AllThrusters.ForEach(t => t.Enabled = true);
        }

        void ScanGrids() {
            GridTerminalSystem.GetBlocks(TempBlocks);

            var grids = TempBlocks
                .Select(b => new { b.CubeGrid.EntityId, b.CubeGrid.CustomName })
                .Distinct()
                .ToList()
                ;

            grids.ForEach(g => Echo($"Grid: {g.CustomName}"));
        }


    }
}
