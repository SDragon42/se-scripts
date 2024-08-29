using System;
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
    partial class Program : MyGridProgram {

        public void Main(string argument, UpdateType updateSource) {
            Cfg.Load(Me);
            InitStructure();
            InitRocketType();

            Echo(ScriptName + " " + RSymbol.GetSymbol(Runtime));

            Echo("Mode: " + Mode);
            Echo("Rocket: " + RocketType);
            Echo("Structure: " + Structure);
            Echo("--------------------");
            //Echo(Instructions);

            GridTerminalSystem.GetBlocksOfType(ConnectedMerges, b => Me.IsSameConstructAs(b) && b.IsConnected);
            SetExecutionRate();
            Echo("Rate: " + Runtime.UpdateFrequency.ToString());
            SetGridName();

            if (Commands.ContainsKey(argument)) Commands[argument].Invoke();

            SequenceSets.RunAll();

            Log.UpdateDisplay();
        }

        private void SetExecutionRate() {
            if (Mode == FlightMode.Off)
                Runtime.UpdateFrequency = UpdateFrequency.Update100;
            else
                Runtime.UpdateFrequency = (Structure == RocketStructure.Pod || ConnectedMerges.Count == 0) ? UpdateFrequency.Update10 : UpdateFrequency.Update100;
        }

        void SendCmdToOtherParts(string command) {
            if (Structure != RocketStructure.Pod) return;
            LoadInAllProgramBlocks(TmpBlocks);
            foreach (IMyProgrammableBlock pb in TmpBlocks)
                if (pb != Me) pb.TryRun(command);
        }

        void InitStructure() {
            if (IsStructureInited) return;
            Structure = GetRocketStructure(Me);
            StructureTag = GetRocketStructureTag(Structure);
            IsStructureInited = true;
        }
        private void InitRocketType() {
            LoadInAllProgramBlocks(TmpBlocks);
            RocketType = RocketStructure.Unknown;
            TmpBlocks.ForEach(b => RocketType |= GetRocketStructure(b));

            collecter = (b) => Me.IsSameConstructAs(b) && Collect.IsTagged(b, StructureTag);
            if (Structure == RocketStructure.Pod && RocketType != RocketStructure.Pod)
                collecter = Me.IsSameConstructAs;
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







        void SetGridName() {
            var gridName = (ConnectedMerges.Count > 0) ? Cfg.GridName_Merged : Cfg.GridName;
            if (gridName.Length == 0) return;
            if (gridName == Me.CubeGrid.CustomName) return;
            Debug("GridName=" + gridName);
            Me.CubeGrid.CustomName = gridName;
            IsStructureInited = false;
            Antenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(b => Collect.IsTagged(b, StructureTag));
            if (Antenna != null)
                Antenna.HudText = gridName;
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

        void Command_CheckReadyToLaunch() {
            Debug("CMD_CheckReadyToLaunch");
            if (Structure != RocketStructure.Pod) return;
            if (Mode != FlightMode.Off) return;

            if (SequenceSets.HasTask("CheckReady")) return;
            SequenceSets.Add("CheckReady", SEQ_CheckReadyToLaunch(), true);
        }

        void Command_Launch() {
            Debug("CMD_Launch");
            if (Structure != RocketStructure.Pod) return;
            if (Mode != FlightMode.Off) return;
            if (SequenceSets.HasTask("Launch")) return;
            Mode = FlightMode.Launch;
            SequenceSets.Add("Launch", SEQ_Launch(), true);

            switch (RocketType) {
                case RocketStructure.Pod:
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.PodTag, null, false));
                    break;
                case RocketStructure.Rocket_S1:
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.Stage1Tag, null, true));
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.PodTag, null, false));
                    break;
                case RocketStructure.Rocket_S2:
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.Stage2Tag, null, true));
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.PodTag, null, false));
                    break;
                case RocketStructure.Rocket_S12:
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.Stage1Tag, null, true));
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.Stage2Tag, null, true));
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.PodTag, null, false));
                    break;
                case RocketStructure.Rocket_S1B:
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.BoosterTag, Cfg.Stage1Tag, true));
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.Stage1Tag, null, true));
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.PodTag, null, false));
                    break;
                case RocketStructure.Rocket_S12B:
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.BoosterTag, Cfg.Stage1Tag, true));
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.Stage1Tag, null, true));
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.Stage2Tag, null, true));
                    SequenceSets.Add("Launch", SEQ_LaunchStage(Cfg.PodTag, null, false));
                    break;
            }

            SendCmdToOtherParts(CMD_AwaitStaging);
        }

        void Command_AwaitStaging() {
            Debug("CMD_AwaitStaging");
            if (Structure == RocketStructure.Pod) return;
            Mode = FlightMode.Standby;
            const string seq = "AwaitStaging";
            if (SequenceSets.HasTask(seq)) return;

            SequenceSets.Add(seq, SEQ_AwaitStaging(), true);
            SequenceSets.Add(seq, Delay(3000));
            SequenceSets.Add(seq, SEQ_ParachuteLanding());
        }

        void Command_Shutdown() {
            Debug("CMD_Shutdown");
            LoadBlocks();
            Mode = FlightMode.Off;
            if (Structure != RocketStructure.Pod) return;
            VectorAlign.SetGyrosOff(Gyros);
            LandingGears.ForEach(b => b.Lock());
            Parachutes.ForEach(b => b.Enabled = false);
            ConnectedMerges.ForEach(b => b.Enabled = true);
            H2Tanks.ForEach(b => b.Stockpile = true);
            AllThrusters.ForEach(b => b.Enabled = false);
            SendCmdToOtherParts(CMD_Shutdown);

        }
    }
}
