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

        // Modules
        readonly RunningSymbol RunningModule = new RunningSymbol();
        readonly TagRegex TagModule = new TagRegex();
        readonly Logging DebugLogModule;
        readonly StateMachineSets stateMachine = new StateMachineSets();
        readonly AutoDoorCloser doorCloser = new AutoDoorCloser();

        readonly string Instructions;

        readonly char[] ArgumentSplitter = new char[] { ' ' };
        string commandKey;
        string commandArgs;
        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();

        //readonly List<IMyShipMergeBlock> allMerges = new List<IMyShipMergeBlock>();
        readonly List<IMyShipMergeBlock> myMerges = new List<IMyShipMergeBlock>();
        readonly List<IMyShipConnector> myConnectors = new List<IMyShipConnector>();
        readonly List<IMyThrust> trainThrusters = new List<IMyThrust>();
        readonly List<IMyGyro> trainGyros = new List<IMyGyro>();
        readonly List<IMyDoor> doorList = new List<IMyDoor>();
        IMyRadioAntenna myAntenna;
        IMyTextSurface debugOutput;

        double timeToReload = 0;
        bool isMerged;
        bool onStandby;

        Action<string> Debug = (t) => { };

        public Program() {

            Commands.Add("set-id", SetGridID);
            Commands.Add("disconnect", Disconnect);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            TagModule.SetTagRegex(tagPrefix);

            //doorCloser.Debug = (t) => DebugLogModule.AppendLine(t);

            // Debug Logging Module Config
            DebugLogModule = new Logging(40);
            Debug = (t) => DebugLogModule.AppendLine(t);
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            try {
                var argumentParts = argument.Split(ArgumentSplitter, 2);
                commandKey = argumentParts[0];
                commandArgs = argumentParts.Length < 2 ? string.Empty : argumentParts[1];

                LoadConfig();
                LoadBlocks();

                isMerged = CheckIfMerged();
                onStandby = TagModule.IsOtherProgramOnDuty(GridTerminalSystem, Me, IsEngineProgramBlock);

                Echo("Union Space Transit " + (onStandby ? "[ON STANDBY]" : RunningModule.GetSymbol(Runtime)));
                Echo("Configure script in 'Custom Data'\n");

                stateMachine.RunAll();

                Echo(Instructions);

                if (isMerged) {
                    SetAntenna(!onStandby);
                    if (!onStandby) {
                        SetGridName(trainName);
                        foreach (var t in trainThrusters) {
                            if (Collect.IsThrusterIon(t)) if (t.IsSameConstructAs(Me)) t.Enabled = true;
                        }
                        foreach (var g in trainGyros) if (g.IsSameConstructAs(Me)) g.Enabled = true;
                    }
                } else {
                    SetGridName(gridName);
                    SetAntenna(true);
                    if (!onStandby) {
                        foreach (var t in trainThrusters) t.Enabled = false;
                        foreach (var g in trainGyros) g.Enabled = false;
                    }
                }

                if (!onStandby) doorCloser.CloseOpenDoors(Runtime, doorList, Me);

                SetGridID();

                if (Commands.ContainsKey(commandKey)) Commands[commandKey]?.Invoke();


                if (onStandby && !stateMachine.HasTasks) {
                    Runtime.UpdateFrequency = UpdateFrequency.Update100;
                    return;
                }
                Runtime.UpdateFrequency = UpdateFrequency.Update10;

            } finally {
                SaveConfig();
                var debugText = DebugLogModule.ToString();
                Echo(debugText);
                debugOutput?.WriteText(debugText);
            }
        }

        void SetGridName(string name) {
            if (!string.IsNullOrEmpty(name) && Me.CubeGrid.CustomName != name) Me.CubeGrid.CustomName = name;
        }
        void SetAntenna(bool enabled) {
            if (myAntenna == null) return;
            myAntenna.Enabled = enabled;
            myAntenna.EnableBroadcasting = enabled;
            myAntenna.ShowShipName = isMerged;
        }


        void LoadBlocks() {
            timeToReload -= Runtime.TimeSinceLastRun.TotalSeconds;
            var skipLoad = (timeToReload > 0.0);
            if (!skipLoad) timeToReload = BlockReloadTime;
            Echo($"Time to reload: {Math.Round(Math.Max(timeToReload, 0)):N0} seconds");
            if (skipLoad) return;

            //GridTerminalSystem.GetBlocksOfType(allMerges, b => b.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(myMerges, b => b.IsSameConstructAs(Me) && IsMyGrid(b));
            GridTerminalSystem.GetBlocksOfType(myConnectors, b => b.IsSameConstructAs(Me) && IsMyGrid(b));
            GridTerminalSystem.GetBlocksOfType(trainThrusters, b => b.IsSameConstructAs(Me) && Collect.IsTagged(b, "[Train]"));
            GridTerminalSystem.GetBlocksOfType(trainGyros, b => b.IsSameConstructAs(Me) && Collect.IsTagged(b, "[Train]"));
            GridTerminalSystem.GetBlocksOfType(doorList, b => b.IsSameConstructAs(Me) && Collect.IsHumanDoor(b));

            myAntenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(b => b.IsSameConstructAs(Me) && IsMyGrid(b));

            debugOutput = GridTerminalSystem.GetBlockWithName("DEBUG") as IMyTextSurface;
        }

        void SetGridID() {
            //Debug("SetGridID()");
            if (isMerged) return;
            if (gridId == Me.CubeGrid.EntityId) return;
            gridId = Me.CubeGrid.EntityId;
            isEngine = IsEngineGrid();

            GridTerminalSystem.GetBlocksOfType(TmpBlocks, b => b.IsSameConstructAs(Me) && (b is IMyShipConnector || b is IMyShipMergeBlock || b is IMyRadioAntenna));
            Debug($"  # Blocks: {TmpBlocks.Count}");
            foreach (var blk in TmpBlocks) {
                var ini = new MyIni();
                if (!ini.TryParse(blk.CustomData)) ini.EndContent = Me.CustomData;
                ini.Add(Key_GridId, gridId, " Unique ID for this grid");
                ini.Set(Key_GridId, gridId);
                blk.CustomData = ini.ToString();
            }
        }

        bool CheckIfMerged() {
            GridTerminalSystem.GetBlocksOfType<IMyShipMergeBlock>(TmpBlocks, Me.IsSameConstructAs);
            return TmpBlocks.Any(b => ((IMyShipMergeBlock)b).IsMerged());
        }

        bool IsEngineGrid() {
            GridTerminalSystem.GetBlocksOfType<IMyCockpit>(TmpBlocks, Me.IsSameConstructAs);
            var hasCockpit = TmpBlocks.Count > 0;
            GridTerminalSystem.GetBlocksOfType<IMyShipMergeBlock>(TmpBlocks, Me.IsSameConstructAs);
            var isTrain = TmpBlocks.Any(b => ((IMyShipMergeBlock)b).IsConnected);
            return hasCockpit && !isTrain;
        }
        bool IsEngineProgramBlock(IMyProgrammableBlock pb) {
            var tmpIni = new MyIni();
            if (!tmpIni.TryParse(pb.CustomData)) return false;
            if (!tmpIni.ContainsKey(Key_IsEngine)) return false;
            return tmpIni.Get(Key_IsEngine).ToBoolean();
        }

        bool IsMyGrid(IMyTerminalBlock blk) {
            var ini = new MyIni();
            if (!ini.TryParse(blk.CustomData)) return false;
            if (!ini.ContainsKey(Key_GridId)) return false;
            var value = ini.Get(Key_GridId).ToInt64();
            return value == gridId;
        }

        /// <summary>
        /// Disconnect used for local grid only.
        /// </summary>
        void Disconnect() {
            Disconnect(commandArgs);
        }
        void Disconnect(string blockTag) {
            Debug("Disconnect: " + blockTag);
            var seqKey = "disconnect" + blockTag;
            if (stateMachine.HasTask(seqKey)) return;

            stateMachine.Add(seqKey, SEQ_DisconnectConnector(blockTag), true);
            stateMachine.Add(seqKey, SEQ_DisconnectMerge(blockTag));
            stateMachine.Add(seqKey, SEQ_Delay(DisconnectEnableDelayMs));
            stateMachine.Add(seqKey, SEQ_EnableConnector(blockTag));
            stateMachine.Add(seqKey, SEQ_Delay(DisconnectEnableDelayMs));
            stateMachine.Add(seqKey, SEQ_AwaitConnectorClear(blockTag));
            stateMachine.Add(seqKey, SEQ_Delay(DisconnectEnableDelayMs));
            stateMachine.Add(seqKey, SEQ_EnableMerge(blockTag));
        }

    }
}
