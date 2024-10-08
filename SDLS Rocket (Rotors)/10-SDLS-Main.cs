﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public void Main(string argument, UpdateType updateSource) {
            UpTime += Runtime.TimeSinceLastRun;
            TagSelf();
            NameGrids();
            if ((updateSource & UpdateType.Update10) == UpdateType.Update10)
                Echo("SDLS " + Running.GetSymbol(Runtime));
            else
                Echo("SDLS");

            IsMasterGrid = GRID.IsMaster(Me.CubeGrid);

            try {
                LoadLocalBlocks();
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




        void TagSelf() {
            if (Me.CustomName.Contains(TAG.GRID)) return;
            Me.CustomName = Me.CustomName.Trim() + " " + TAG.GRID;
        }

        void NameGrids(bool force = false) {
            GridTerminalSystem.GetBlocksOfType(GridPrograms);
            GridPrograms.ForEach(b => NameGrid(b, force));
        }
        void NameGrid(IMyProgrammableBlock block, bool force) {
            if (GRID.IsNamed(block.CubeGrid) && !force) return;

            // POD
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(TmpBlocks, b => IsSameGrid(block, b) && Collect.IsSmallBlockLargeCargoContainer(b));
            if (TmpBlocks.Count > 0) {
                GridTerminalSystem.GetBlocksOfType<IMyCockpit>(TmpBlocks, b => IsSameGrid(block, b));
                block.CubeGrid.CustomName = (TmpBlocks.Count > 0)
                    ? GRID.PILOTED_POD
                    : GRID.POD;
                Structure = Structure | RocketStructure.Pod;
                return;
            }

            // ID Stage 2
            GridTerminalSystem.GetBlocksOfType<IMyThrust>(TmpBlocks, b => IsSameGrid(block, b) && Collect.IsThrusterHydrogen(b) && b.BlockDefinition.SubtypeId.Contains("Large"));
            if (TmpBlocks.Count == 0) {
                block.CubeGrid.CustomName = GRID.STAGE2;
                Structure = Structure | RocketStructure.Stage2;
                return;
            }

            // ID Stage 1 Core / Booster
            GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(TmpBlocks, b => IsSameGrid(block, b) && Collect.IsTagged(b, TAG.BOOSTER_CLAMP));
            if (TmpBlocks.Count > 0) {
                block.CubeGrid.CustomName = GRID.STAGE1C;
                Structure = Structure | RocketStructure.CoreBooster;
            } else {
                block.CubeGrid.CustomName = GRID.STAGE1B;
                Structure = Structure | RocketStructure.SideBooster;
            }
        }


        void SetParticleEmitters(bool turnOn, params IMyCubeGrid[] grids) {

            GridTerminalSystem.GetBlocksOfType(TmpBlocks, b => {
                if (!grids.Contains(b.CubeGrid)) return false;
                return (b.BlockDefinition.SubtypeId == "SmallParticleEmitter");
            });

            TmpBlocks.ForEach(b => {
                //Debug.AppendLine(b.BlockDefinition.TypeIdString);
                //Debug.AppendLine("   " + b.BlockDefinition.SubtypeId);
                ((IMyFunctionalBlock)b).Enabled = turnOn;
            });
        }
    }
}
