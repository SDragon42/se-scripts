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
        readonly RunningSymbol _running = new RunningSymbol();
        readonly TagRegex _tag = new TagRegex();
        readonly Logging _debugLog;
        readonly StateMachineSets _operations = new StateMachineSets();
        readonly AutoDoorCloser _doorCloser = new AutoDoorCloser();

        readonly string _instructions;

        readonly char[] _argSplitter = new char[] { ' ' };
        string _commandKey;
        string _commandArgs;
        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);

        //readonly List<IMyShipMergeBlock> allMerges = new List<IMyShipMergeBlock>();
        readonly List<IMyShipMergeBlock> _myMerges = new List<IMyShipMergeBlock>();
        readonly List<IMyShipConnector> _myConnectors = new List<IMyShipConnector>();
        readonly List<IMyThrust> _trainThrusters = new List<IMyThrust>();
        readonly List<IMyGyro> _trainGyros = new List<IMyGyro>();
        readonly List<IMyDoor> _doorList = new List<IMyDoor>();
        readonly List<IMyTerminalBlock> _tempBlocks = new List<IMyTerminalBlock>();
        IMyRadioAntenna _myAntenna;
        IMyTextSurface _debugOutput;

        double _timeToReload = 0;
        bool _isMerged;
        bool _onStandby;

        Action<string> Debug = (t) => { };

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            _commands.Add("set-id", SetGridID);
            _commands.Add("disconnect", Disconnect);

            // Instructions
            _instructions = "Script Commands\n" + string.Join("\n", _commands.Keys);

            _tag.SetTagRegex(TAG_PREFIX);

            // Debug Logging Module Config
            _debugLog = new Logging(40);
            Debug = (t) => _debugLog.AppendLine(t);
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            try {
                var argumentParts = argument.Split(_argSplitter, 2);
                _commandKey = argumentParts[0];
                _commandArgs = argumentParts.Length < 2 ? string.Empty : argumentParts[1];

                LoadConfig();
                LoadBlocks();

                _isMerged = CheckIfMerged();
                _onStandby = _tag.IsOtherProgramOnDuty(GridTerminalSystem, Me, IsEngineProgramBlock);

                Echo("Union Space Transit " + (_onStandby ? "[ON STANDBY]" : _running.GetSymbol()));
                Echo("Configure script in 'Custom Data'\n");

                _operations.RunAll();

                Echo(_instructions);

                if (_isMerged) {
                    SetAntenna(!_onStandby);
                    if (!_onStandby) {
                        SetGridName(trainName);
                        foreach (var t in _trainThrusters) if (Me.IsSameConstructAs(t) && IsThrusterIon(t)) t.Enabled = true;
                        foreach (var g in _trainGyros) if (Me.IsSameConstructAs(g)) g.Enabled = true;
                    }
                } else {
                    SetGridName(gridName);
                    SetAntenna(true);
                    if (!_onStandby) {
                        foreach (var t in _trainThrusters) t.Enabled = false;
                        foreach (var g in _trainGyros) g.Enabled = false;
                    }
                }

                if (!_onStandby) _doorCloser.CloseOpenDoors(Runtime, _doorList, Me);

                SetGridID();

                if (_commands.ContainsKey(_commandKey)) _commands[_commandKey]?.Invoke();


                if (_onStandby && !_operations.HasTasks) {
                    Runtime.UpdateFrequency = UpdateFrequency.Update100;
                    return;
                }
                Runtime.UpdateFrequency = UpdateFrequency.Update10;

            } finally {
                SaveConfig();
                var debugText = _debugLog.ToString();
                Echo(debugText);
                _debugOutput?.WriteText(debugText);
            }
        }

        void SetGridName(string name) {
            if (!string.IsNullOrEmpty(name) && Me.CubeGrid.CustomName != name) Me.CubeGrid.CustomName = name;
        }
        void SetAntenna(bool enabled) {
            if (_myAntenna == null) return;
            _myAntenna.Enabled = enabled;
            _myAntenna.EnableBroadcasting = enabled;
            _myAntenna.ShowShipName = _isMerged;
        }


        void LoadBlocks() {
            _timeToReload -= Runtime.TimeSinceLastRun.TotalSeconds;
            var skipLoad = _timeToReload > 0.0;
            if (!skipLoad) _timeToReload = BLOCK_RELOAD_TIME;
            Echo($"Time to reload: {Math.Round(Math.Max(_timeToReload, 0)):N0} seconds");
            if (skipLoad) return;

            //GridTerminalSystem.GetBlocksOfType(allMerges, b => Me.IsSameConstructAs(b));
            GridTerminalSystem.GetBlocksOfType(_myMerges, b => Me.IsSameConstructAs(b) && IsMyGrid(b));
            GridTerminalSystem.GetBlocksOfType(_myConnectors, b => Me.IsSameConstructAs(b) && IsMyGrid(b));
            GridTerminalSystem.GetBlocksOfType(_trainThrusters, b => Me.IsSameConstructAs(b) && IsTagged(b, "[Train]"));
            GridTerminalSystem.GetBlocksOfType(_trainGyros, b => Me.IsSameConstructAs(b) && IsTagged(b, "[Train]"));
            GridTerminalSystem.GetBlocksOfType(_doorList, b => Me.IsSameConstructAs(b) && IsHumanDoor(b));

            _myAntenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(b => Me.IsSameConstructAs(b) && IsMyGrid(b));

            _debugOutput = GridTerminalSystem.GetBlockWithName("DEBUG") as IMyTextSurface;
        }

        void SetGridID() {
            //Debug("SetGridID()");
            if (_isMerged) return;
            if (gridId == Me.CubeGrid.EntityId) return;
            gridId = Me.CubeGrid.EntityId;
            isEngine = IsEngineGrid();

            GridTerminalSystem.GetBlocksOfType(_tempBlocks, b => Me.IsSameConstructAs(b) && (b is IMyShipConnector || b is IMyShipMergeBlock || b is IMyRadioAntenna));
            Debug($"  # Blocks: {_tempBlocks.Count}");
            foreach (var blk in _tempBlocks) {
                var ini = new MyIni();
                if (!ini.TryParse(blk.CustomData)) ini.EndContent = Me.CustomData;
                ini.Add(Key_GridId, gridId, " Unique ID for this grid");
                ini.Set(Key_GridId, gridId);
                blk.CustomData = ini.ToString();
            }
        }

        bool CheckIfMerged() {
            GridTerminalSystem.GetBlocksOfType<IMyShipMergeBlock>(_tempBlocks, Me.IsSameConstructAs);
            return _tempBlocks.Any(b => ((IMyShipMergeBlock)b).IsMerged());
        }

        bool IsEngineGrid() {
            GridTerminalSystem.GetBlocksOfType<IMyCockpit>(_tempBlocks, Me.IsSameConstructAs);
            var hasCockpit = _tempBlocks.Count > 0;
            GridTerminalSystem.GetBlocksOfType<IMyShipMergeBlock>(_tempBlocks, Me.IsSameConstructAs);
            var isTrain = _tempBlocks.Any(b => ((IMyShipMergeBlock)b).IsConnected);
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

        // Disconnect used for local grid only.
        void Disconnect() {
            Disconnect(_commandArgs);
        }
        void Disconnect(string blockTag) {
            Debug("Disconnect: " + blockTag);
            var seqKey = "disconnect" + blockTag;
            if (_operations.HasTask(seqKey)) return;

            _operations.Add(seqKey, SEQ_DisconnectConnector(blockTag), true);
            _operations.Add(seqKey, SEQ_DisconnectMerge(blockTag));
            _operations.Add(seqKey, SEQ_Delay(DISCONNECT_ENABLE_DELAY));
            _operations.Add(seqKey, SEQ_EnableConnector(blockTag));
            _operations.Add(seqKey, SEQ_Delay(DISCONNECT_ENABLE_DELAY));
            _operations.Add(seqKey, SEQ_AwaitConnectorClear(blockTag));
            _operations.Add(seqKey, SEQ_Delay(DISCONNECT_ENABLE_DELAY));
            _operations.Add(seqKey, SEQ_EnableMerge(blockTag));
        }

    }
}
