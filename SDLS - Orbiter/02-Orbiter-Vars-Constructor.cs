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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        public Program() {
            Debug = new DebugLogging(this) {
                EchoMessages = true,
                Enabled = false
            };

            Fdr = new FlightDataRecorder(new string[] { "" }, 1000) {
                Enabled = false
            };

            Runtime.UpdateFrequency = UpdateFrequency.None;

            Commands.Add(CMD_OFF, TurnOff);
            Commands.Add(CMD_MANUAL, ManualControl);
            Commands.Add(CMD_SCAN, ScanGrids);
            Commands.Add(CMD_ALIGN_LAUNCH, Align_Launch);
            Commands.Add(CMD_ALIGN_LAND, Align_Land);
        }

        public void Save() {
        }

        void LoadRocketPartGrids() {
            rocketParts.Clear();
            var tmp = new List<IMyProgrammableBlock>();
            GridTerminalSystem.GetBlocksOfType(tmp);
        }
        void LoadBlocks(bool force = false) {
            if (BlocksLoaded && !force) return;

            Remote = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>(IsOnThisGrid);
            Antenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(IsOnThisGrid);
            Beacon = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyBeacon>(IsOnThisGrid);

            Orientation.Init(Remote);

            GridTerminalSystem.GetBlocksOfType(Parachutes, IsOnThisGrid);
            GridTerminalSystem.GetBlocksOfType(Gyros, IsOnThisGrid);
            GridTerminalSystem.GetBlocksOfType(LandingGears, IsOnThisGrid);
            GridTerminalSystem.GetBlocksOfType(LaunchClamps, b => IsOnThisGrid(b) && Collect.IsTagged(b, TAG.LAUNCH_CLAMP));
            GridTerminalSystem.GetBlocksOfType(StageClamps, b => IsOnThisGrid(b) && Collect.IsTagged(b, TAG.STAGING_CLAMP));

            // Thrusters
            ManeuverThrusters.Clear();
            StageThrusters.Clear();
            AscentThrusters.Clear();
            GridTerminalSystem.GetBlocksOfType(AllThrusters, b => {
                if (!IsOnThisGrid(b)) return false;
                //if (Collect.IsTagged(b, TAG_MANEUVER)) ManeuverThrusters.Add(b);
                if (Collect.IsTagged(b, TAG.MAIN)) AscentThrusters.Add(b);
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

    }
}
