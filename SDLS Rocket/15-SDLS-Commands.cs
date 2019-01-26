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

        void ScanGrids() {
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(TmpBlocks, b => GRID.IsNamed(b.CubeGrid));

            var grids = TmpBlocks
                .Select(b => new { b.CubeGrid.EntityId, b.CubeGrid.CustomName })
                .Distinct()
                .ToList();

            grids.ForEach(g => Echo($"Grid: {g.CustomName}"));
        }

    }
}
