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
        public void Main(string argument, UpdateType updateSource) {
            UpTime += Runtime.TimeSinceLastRun;
            TagSelf();
            //NameGrid(Me);
            NameGrids();
            if ((updateSource.HasFlag(UpdateType.Update10)))
                Echo("SDLS " + Running.GetSymbol(Runtime));
            else
                Echo("SDLS");

            try {
                //LoadBlocks();
                ProcessArguments(argument.ToLower());
                if (RunSequences())
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                Echo(Log.GetLogText());
            } catch (Exception ex) {
                Echo(ex.ToString());
                throw;
            } finally {
                Debug?.UpdateDisplay();
            }
        }

        void ProcessArguments(string argument) {
            if (Commands.ContainsKey(argument)) Commands[argument]?.Invoke();
        }

        bool RunSequences() {
            var hasTasks = false;

            //GravityAlign.Run();
            //hasTasks |= GravityAlign.HasTasks;

            //Operations.RunAll();
            //hasTasks |= Operations.HasTasks;

            return hasTasks;
        }


        void ScanGrids() {
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(TmpBlocks, b => GRID.IsNamed(b.CubeGrid));

            var grids = TmpBlocks
                .Select(b => new { b.CubeGrid.EntityId, b.CubeGrid.CustomName })
                .Distinct()
                .ToList();

            grids.ForEach(g => Echo($"Grid: {g.CustomName}"));
        }

        void TagSelf() {
            if (IsSDLS(Me)) return;
            Me.CustomName = Me.CustomName.Trim() + " " + TAG.GRID;
        }
        void NameGrids() {
            var progs = new List<IMyProgrammableBlock>();
            GridTerminalSystem.GetBlocksOfType(progs);
            progs.ForEach(NameGrid);
        }
        void NameGrid(IMyProgrammableBlock block) {
            if (GRID.IsNamed(block.CubeGrid)) return;

            // POD
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(TmpBlocks, b => OnSameGrid(block, b) && Collect.IsSmallBlockLargeCargoContainer(b));
            if (TmpBlocks.Count > 0) {
                GridTerminalSystem.GetBlocksOfType<IMyCockpit>(TmpBlocks, b => OnSameGrid(block, b));
                block.CubeGrid.CustomName = (TmpBlocks.Count > 0)
                    ? GRID.PILOTED_POD
                    : GRID.POD;
                return;
            }

            // ID Stage 2
            GridTerminalSystem.GetBlocksOfType<IMyThrust>(TmpBlocks, b => OnSameGrid(block, b) && Collect.IsThrusterHydrogen(b) && b.BlockDefinition.SubtypeId.Contains("Large"));
            if (TmpBlocks.Count == 0) {
                block.CubeGrid.CustomName = GRID.STAGE2;
                return;
            }

            // ID Stage 1 Core / Booster
            GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(TmpBlocks, b => OnSameGrid(block, b) && Collect.IsTagged(b, TAG.BOOSTER_CLAMP));
            block.CubeGrid.CustomName = (TmpBlocks.Count > 0)
                ? GRID.STAGE1C
                : GRID.STAGE1B;
        }


    }
}
