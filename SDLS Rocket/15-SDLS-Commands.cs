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

        void CMD_ScanGrids() {
            NameGrids(true);
            ShowGridNames();
        }
        void ShowGridNames() {
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(TmpBlocks, b => GRID.IsNamed(b.CubeGrid));
            var grids = TmpBlocks
                .Select(b => b.CubeGrid)
                .Distinct()
                .ToList();
            grids.ForEach(g => Echo($"Grid: {g.CustomName}"));
        }


        void CMD_Off() {

        }

        void CMD_Standby() {
            if (!IsMasterGrid) return;
            // Await Staging
            
        }

        void CMD_Lanuch() {
            if (!IsMasterGrid) return;
            // Build Launch Sequence
            BuildLaunchSequence();

            QueueGravityAlign.Clear();
            QueueGravityAlign.Add(Sequence_LaunchGravAlign());
        }

        void BuildLaunchSequence() {
            switch (Structure) {
                case RocketStructure.Rocket4:
                    break;
                case RocketStructure.Rocket3:
                    break;
                case RocketStructure.Rocket2:
                    break;
                case RocketStructure.Rocket1:
                    break;
                case RocketStructure.Pod:
                    break;
                default:
                    break;
            }
        }

    }
}
