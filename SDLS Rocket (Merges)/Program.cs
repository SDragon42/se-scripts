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
            Cfg.Load(Me);
            InitStructure();
            LoadBlocks();

            if ((updateSource & UpdateType.Update10) == UpdateType.Update10)
                Echo($"{ScriptName} {RSymbol.GetSymbol(Runtime)}");
            else
                Echo(ScriptName);

            Echo($"Struct: {RocketType}");
            Echo($"Struct: {Structure}");
            Echo($"Mode: {Mode}");
            Echo("--------------------");


            //Echo(Instructions);

            CheckMerges();
            if (Commands.ContainsKey(argument)) Commands[argument].Invoke();
            // runtime loop

            SequenceSets.RunAllTasks();

            Log.UpdateDisplay();
        }


        void InitStructure() {
            if (IsStructureInited) return;
            Structure = GetRocketStructure(Me);
            StructureTag = GetRocketStructureTag();
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(TmpBlocks, b => Me.IsSameConstructAs(b));
            TmpBlocks.ForEach(b => RocketType |= GetRocketStructure(b));
            IsStructureInited = true;
        }
        RocketStructure GetRocketStructure(IMyTerminalBlock b) {
            if (b.CustomName.Contains(Cfg.PodTag)) return RocketStructure.Pod;
            else if (b.CustomName.Contains(Cfg.Stage2Tag)) return RocketStructure.Stage2;
            else if (b.CustomName.Contains(Cfg.Stage1Tag)) return RocketStructure.Stage1;
            else if (b.CustomName.Contains(Cfg.BoosterTag)) return RocketStructure.Booster;
            return RocketStructure.Unknown;
        }
        string GetRocketStructureTag() {
            switch (Structure) {
                case RocketStructure.Booster: return Cfg.BoosterTag;
                case RocketStructure.Stage1: return Cfg.Stage1Tag;
                case RocketStructure.Stage2: return Cfg.Stage2Tag;
                default: return Cfg.PodTag;
            }
        }

        void LoadBlocks() {

        }


        void CheckMerges() {
            GridTerminalSystem.GetBlocksOfType(Merges, b => b.IsSameConstructAs(Me) && b.IsConnected);
            if (Merges.Count == 0) SetStageMass();

            if (!Cfg.HasGridNames) return;
            var gridName = (Merges.Count > 0) ? Cfg.GridName_Merged : Cfg.GridName;
            if (gridName.Length == 0) return;
            if (gridName == Me.CubeGrid.CustomName) return;

            Debug($"GridName={gridName}");
            Me.CubeGrid.CustomName = gridName;
            IsStructureInited = false;
        }

        void SetStageMass() {
            var rc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>(b => b.IsSameConstructAs(Me) && Collect.IsTagged(b, StructureTag));
            if (rc == null) { Debug("* stage RC not found *"); return; }

            var dryMass = rc.CalculateShipMass().BaseMass;
            if (dryMass == Cfg.StageDryMass) return;
            Cfg.StageDryMass = dryMass;
            Cfg.Save(Me);
        }





        void CMD_Init() {
            Debug("CMD_Init");
            if (Mode != FlightMode.Off) return;
            IsStructureInited = false;
            InitStructure();
        }



        void CMD_Launch() {
            Debug("CMD_Launch");
            if (Structure != RocketStructure.Pod) return;
            if (Mode != FlightMode.Off) return;
        }

        void CMD_AwaitStaging() {
            Debug("CMD_AwaitStaging");
            if (Structure == RocketStructure.Pod) return;
        }

        void CMD_Shutdown() {
            Debug("CMD_Shutdown");
        }
    }
}
