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

            if ((updateSource & UpdateType.Update10) == UpdateType.Update10)
                Echo($"{ScriptName} {RSymbol.GetSymbol(Runtime)}");
            else
                Echo(ScriptName);

            Echo($"Rocket: {RocketType}");
            Echo($"Structure: {Structure}");
            Echo($"Mode: {Mode}");
            Echo("--------------------");
            //Echo(Instructions);

            GridTerminalSystem.GetBlocksOfType(ConnectedMerges, b => Me.IsSameConstructAs(b) && b.IsConnected);
            SetGridName();


            if (Commands.ContainsKey(argument)) Commands[argument].Invoke();
            // runtime loop

            SequenceSets.RunAllTasks();

            Log.UpdateDisplay();
        }


        void LoadInAllProgramBlocks(List<IMyTerminalBlock> list) {
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(list, b => Me.IsSameConstructAs(b) && Collect.IsTagged(b, ScriptName));
        }

        void SendCmdToOtherParts(string command) {
            if (Structure != RocketStructure.Pod) return;
            LoadInAllProgramBlocks(TmpBlocks);
            foreach (IMyProgrammableBlock pb in TmpBlocks)
            {
                if (pb == Me) continue;
                pb.TryRun(command);
            }
        }

        Func<IMyTerminalBlock, bool> collecter = null;
        void InitStructure() {
            if (IsStructureInited) return;
            RocketType = RocketStructure.Unknown;
            Structure = GetRocketStructure(Me);
            StructureTag = GetRocketStructureTag(Structure);

            LoadInAllProgramBlocks(TmpBlocks);
            TmpBlocks.ForEach(b => RocketType |= GetRocketStructure(b));

            collecter = (b) => Me.IsSameConstructAs(b) && Collect.IsTagged(b, StructureTag);
            if (Structure == RocketStructure.Pod && RocketType != RocketStructure.Pod)
                collecter = Me.IsSameConstructAs;

            //Action<RocketStructure> addStageTag = (part) => { if (RocketType.HasFlag(part)) StageTags.Enqueue(GetRocketStructureTag(part)); };
            //addStageTag(RocketStructure.Booster);
            //addStageTag(RocketStructure.Stage1);
            //addStageTag(RocketStructure.Stage2);

            IsStructureInited = true;
        }
        RocketStructure GetRocketStructure(IMyTerminalBlock b) {
            if (Collect.IsTagged(b, Cfg.PodTag)) return RocketStructure.Pod;
            else if (Collect.IsTagged(b, Cfg.Stage2Tag)) return RocketStructure.Stage2;
            else if (Collect.IsTagged(b, Cfg.Stage1Tag)) return RocketStructure.Stage1;
            else if (Collect.IsTagged(b, Cfg.BoosterTag)) return RocketStructure.Booster;
            return RocketStructure.Unknown;
        }
        string GetRocketStructureTag(RocketStructure structure) {
            switch (structure) {
                case RocketStructure.Booster: return Cfg.BoosterTag;
                case RocketStructure.Stage1: return Cfg.Stage1Tag;
                case RocketStructure.Stage2: return Cfg.Stage2Tag;
                default: return Cfg.PodTag;
            }
        }

        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(Gyros, collecter);
            GridTerminalSystem.GetBlocksOfType(LandingGears, collecter);
            GridTerminalSystem.GetBlocksOfType(Parachutes, collecter);

            //GridTerminalSystem.Get
        }


        void SetGridName() {
            var gridName = (ConnectedMerges.Count > 0) ? Cfg.GridName_Merged : Cfg.GridName;
            if (gridName.Length == 0) return;
            if (gridName == Me.CubeGrid.CustomName) return;
            Debug($"GridName={gridName}");
            Me.CubeGrid.CustomName = gridName;
            IsStructureInited = false;
        }

        void SetStageMass() {
            if (ConnectedMerges.Count > 0) return;
            var rc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>(b => b.IsSameConstructAs(Me) && Collect.IsTagged(b, StructureTag));
            if (rc == null) { Debug("* stage RC not found *"); return; }

            var dryMass = rc.CalculateShipMass().BaseMass;
            if (dryMass == Cfg.StageDryMass) return;
            Cfg.StageDryMass = dryMass;
            Cfg.Save(Me);
        }





        void Command_Init() {
            Debug("CMD_Init");
            if (Mode != FlightMode.Off) return;
            IsStructureInited = false;
            InitStructure();
            SetStageMass();
        }



        

        void Command_AwaitStaging() {
            Debug("CMD_AwaitStaging");
            if (Structure == RocketStructure.Pod) return;
            Mode = FlightMode.Standby;
            LoadBlocks();
        }

        void Command_Shutdown() {
            Debug("CMD_Shutdown");
            LoadBlocks();
            Mode = FlightMode.Off;
        }
    }
}
