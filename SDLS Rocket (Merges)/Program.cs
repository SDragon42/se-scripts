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

        public void Main(string argument, UpdateType updateSource) {
            if ((updateSource & UpdateType.Update10) == UpdateType.Update10)
                Echo($"{ScriptName} {RSymbol.GetSymbol(Runtime)}");
            else
                Echo(ScriptName);
            Echo(Instructions);

            Cfg.LoadConfig(Me);

            InitStructure();
            LoadBlocks();
            CheckMerges();

            if (Commands.ContainsKey(argument)) Commands[argument].Invoke();

            Log.UpdateDisplay();
        }

        void CMD_Init() {
            //Debug("CMD_Init");
            IsInited = false;
            InitStructure();
        }
        void InitStructure() {
            if (IsInited) return;
            IsMaster = false;
            Structure = GetRocketStructure(Me);
            if (Structure == RocketStructure.Pod) {
                IsMaster = true;
                GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(TmpBlocks, b => Me.IsSameConstructAs(b));
                TmpBlocks.ForEach(b => Structure |= GetRocketStructure(b));
            }
            IsInited = true;
        }
        RocketStructure GetRocketStructure(IMyTerminalBlock b) {
            if (b.CustomName.Contains(Cfg.PodTag))
                return RocketStructure.Pod;
            else if (b.CustomName.Contains(Cfg.Stage2Tag))
                return RocketStructure.Stage2;
            else if (b.CustomName.Contains(Cfg.Stage1Tag))
                return RocketStructure.Stage1;
            else if (b.CustomName.Contains(Cfg.BoosterTag))
                return RocketStructure.Booster;
            return RocketStructure.Unknown;
        }

        void LoadBlocks() {

        }


        void CheckMerges() {
            if (!Cfg.HasGridNames) return;
            GridTerminalSystem.GetBlocksOfType(Merges, b => b.IsSameConstructAs(Me) && b.IsConnected);

            var gridName = (Merges.Count > 0) ? Cfg.GridName_Merged : Cfg.GridName;
            if (gridName.Length == 0) return;
            if (gridName == Me.CubeGrid.CustomName) return;
            Debug($"GridName={gridName}");
            Me.CubeGrid.CustomName = gridName;
            IsInited = false;
        }






        void CMD_Scan() {
            Debug("CMD_Scan");
        }
        void CMD_Launch() {
            Debug("CMD_Launch");
            if (!IsMaster) return;
        }
        void CMD_AwaitStaging() {
            Debug("CMD_AwaitStaging");
            if (IsMaster) return;
        }
        void CMD_Shutdown() {
            Debug("CMD_Shutdown");
        }
    }
}
