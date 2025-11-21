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

        // Ship Commands
        const string CMD_InitializeShip = "start";
        const string CMD_SetShipID = "setid";
        const string CMD_SetShipID4AllBlocks = "setid4all";
        const string CMD_Pressurize = "pressurize";
        const string CMD_Depressurize = "depressurize";
        const string CMD_Disconnect = "disconnect";
        const string CMD_Heartbeat = "heartbeat";

        // Ship ID Constants
        const string ShipID_Prefix = "DHI-";
        const string ShipID_Suffix = ":";
        const string ShipID_NoValue = "#";

        // Door Closer Constants
        const int MaxNumCycles2KeepDoorOpen = 3;
        const string ExcludeTag = "[Excluded]";
        const string AirtightHangerDoorName = "Airtight Hangar Door";

        const int MaxNumCyclesBeforeLockingConnectors = 3;

        // Airlock Doors
        const string AirlockDoor_Interior = "interior";
        const string AirlockDoor_Exterior = "exterior";

        // Oxygen Management Constants
        const double MinO2FillPercentage = 0.3;
        const double MaxO2FillPercentage = 0.8;

        // ############################################################


        List<string> _airlockKeys = new List<string>();
        Dictionary<string, IActionSequence> _airlockSequence = new Dictionary<string, IActionSequence>();

        List<string> _mergeKeys = new List<string>();
        Dictionary<string, IActionSequence> _mergeDecoupleSequence = new Dictionary<string, IActionSequence>();

        Dictionary<IMyDoor, int> _openDoors = new Dictionary<IMyDoor, int>();


        int _shipID_Prefix_Length;
        int _shipID_Suffix_Length;
        string _currentShipID;
        string _command;
        string _commandData;

        void Main(string argument) {
            _shipID_Prefix_Length = ShipID_Prefix.Length;
            _shipID_Suffix_Length = ShipID_Suffix.Length;
            _currentShipID = GetShipIdFromBlock(Me);

            ProcessArgument(argument);

            switch (_command) {
                case CMD_Pressurize:
                    StartAirlockSequence(false);
                    break;
                case CMD_Depressurize:
                    StartAirlockSequence(true);
                    break;
                case CMD_Disconnect:
                    StartMergeDecoupleSequence();
                    break;
                case CMD_SetShipID:
                    SetShipIdOnBlocks(GetAllBlocksOnThisShip());
                    break;
                case CMD_SetShipID4AllBlocks:
                    SetShipIdOnBlocks(GetAllBlocks());
                    break;
                case CMD_InitializeShip:
                    StartupShip();
                    break;
                case CMD_Heartbeat:
                    CloseOpenDoors();
                    LockConnectors();
                    CheckO2Levels();
                    RunSequenceCycle(_airlockKeys, _airlockSequence);
                    RunSequenceCycle(_mergeKeys, _mergeDecoupleSequence);
                    break;
                default:
                    Echo("Unknown Commands: " + argument);
                    break;
            }
        }

        private void ProcessArgument(string argument) {
            _command = null;
            _commandData = null;

            var splitArgs = argument.Split(new char[] { ' ' }, 2);

            _command = splitArgs[0].ToLower();
            if (_command == "")
                _command = CMD_Heartbeat;

            if (splitArgs.Length > 1)
                _commandData = splitArgs[1];
        }



        private void StartupShip() {
            var blocks = GetAllBlocksOnThisShip();
            ApplyActionToBlocks(blocks, BlockAction_TurnOn);
        }

        private void StartAirlockSequence(bool depressurize) {
            if (_airlockSequence.ContainsKey(_commandData))
                return;

            var doors = GetAirlockDoorsOnThisShip(_commandData);
            var vents = GetAirlockVentsOnThisShip(_commandData);

            var sequence = new AirlockSequence(_commandData, depressurize, doors, vents);
            _airlockSequence.Add(_commandData, sequence);
            _airlockKeys.Add(_commandData);
        }
        private void StartMergeDecoupleSequence() {
            if (_mergeDecoupleSequence.ContainsKey(_commandData))
                return;

            var merges = GetMergeBlocksOnThisShip(_commandData);
            var connectors = GetConnectorBlocksOnThisShip(_commandData);

            var sequence = new MergeDecoupleSequence(_commandData, merges, connectors);
            _mergeDecoupleSequence.Add(_commandData, sequence);
            _mergeKeys.Add(_commandData);
        }
        private static void RunSequenceCycle(IList<string> keys, IDictionary<string, IActionSequence> sequence) {
            var i = 0;
            while (i < keys.Count) {
                var key = keys[i];
                if (!sequence.ContainsKey(key)) {
                    keys.RemoveAt(i);
                    continue;
                }

                var airlock = sequence[key];
                airlock.RunActions();

                if (airlock.TicksRemaining < 0) {
                    keys.RemoveAt(i);
                    sequence.Remove(key);
                    continue;
                }

                i++;
            }
        }

        private void SetShipIdOnBlocks(IList<IMyTerminalBlock> blocks) {
            var newShipId = GetShipIdFromArguments();

            for (var i = 0; i < blocks.Count; i++) {
                var id = GetShipIdFromBlock(blocks[i]);
                var name = blocks[i].CustomName.Substring(id.Length);
                blocks[i].CustomName = newShipId + name;
            }
        }

        private void CloseOpenDoors() {
            var doorBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyDoor>(doorBlocks, IsValidDoor2Close);

            for (int i = 0; i < doorBlocks.Count; i++) {
                var door = (IMyDoor)doorBlocks[i];

                if (_openDoors.ContainsKey(door)) {
                    if (door.Status != DoorStatus.Open)
                        _openDoors.Remove(door);
                    else {
                        int doorCount;
                        _openDoors.TryGetValue(door, out doorCount);
                        _openDoors.Remove(door);

                        if (doorCount++ < MaxNumCycles2KeepDoorOpen)
                            _openDoors.Add(door, doorCount);
                        else
                            door.GetActionWithName(DoorAction_CloseDoor).Apply(door);
                    }
                } else {
                    if (door.Status == DoorStatus.Open)
                        _openDoors.Add(door, 0);
                }

            }
        }
        private bool IsValidDoor2Close(IMyTerminalBlock block) {
            if (block == null)
                return false;
            if (!(block is IMyDoor))
                return false;

            if (block.CustomName.ToLower().Contains(AirtightHangerDoorName.ToLower()))
                return false;
            if (block.CustomName.ToLower().Contains(ExcludeTag.ToLower()))
                return false;

            return true;
        }

        Dictionary<IMyShipConnector, int> _unlockedConnectors = new Dictionary<IMyShipConnector, int>();
        private void LockConnectors() {
            var connectorBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectorBlocks, IsConnectorValid2Lock);

            for (int i = 0; i < connectorBlocks.Count; i++) {
                var connector = (IMyShipConnector)connectorBlocks[i];

                if (_unlockedConnectors.ContainsKey(connector)) {
                    if (connector.Status == MyShipConnectorStatus.Connected)
                        _unlockedConnectors.Remove(connector);
                    else {
                        int connectorCount;
                        _unlockedConnectors.TryGetValue(connector, out connectorCount);
                        _unlockedConnectors.Remove(connector);

                        if (connectorCount++ < MaxNumCyclesBeforeLockingConnectors)
                            _unlockedConnectors.Add(connector, connectorCount);
                        else
                            connector.GetActionWithName(ConnectorAction_Lock).Apply(connector);
                    }
                } else {
                    if (connector.Status != MyShipConnectorStatus.Connected)
                        _unlockedConnectors.Add(connector, 0);
                }
            }
        }
        private bool IsConnectorValid2Lock(IMyTerminalBlock block) {
            if (block == null)
                return false;
            if (!(block is IMyShipConnector))
                return false;

            if (block.CustomName.ToLower().Contains(ExcludeTag.ToLower()))
                return false;

            return true;
        }


        private void CheckO2Levels() {
            var oxygenTankPercent = GetStoredOxygenPercentage();
            if (oxygenTankPercent >= MinO2FillPercentage && oxygenTankPercent <= MaxO2FillPercentage)
                return;

            var actionMethod = BlockAction_TurnOn;
            if (oxygenTankPercent > MaxO2FillPercentage)
                actionMethod = BlockAction_TurnOff;

            var _oxyGenerators = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(_oxyGenerators);

            ApplyActionToBlocks(_oxyGenerators, actionMethod);
        }
        private double GetStoredOxygenPercentage() {
            var oxygenTanks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(oxygenTanks);

            var value = 0.0;
            for (int i = 0; i < oxygenTanks.Count; i++) {
                var tank = (IMyGasTank)oxygenTanks[i];
                value += tank.FilledRatio;
            }
            return value / oxygenTanks.Count;
        }


        private static void ApplyActionToBlocks(IList<IMyTerminalBlock> blocks, string actionMethod) {
            for (int i = 0; i < blocks.Count; i++) {
                ApplyActionToBlock(blocks[i], actionMethod);
            }
        }
        private static void ApplyActionToBlock(IMyTerminalBlock block, string actionMethod) {
            if (block.HasAction(actionMethod))
                block.ApplyAction(actionMethod);
        }

        private IList<IMyTerminalBlock> GetAllBlocks() {
            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(blocks);
            return blocks;
        }
        private IList<IMyTerminalBlock> GetAllBlocksOnThisShip() {
            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName("", blocks, IsBlockOnThisShip);
            return blocks;
        }
        private IList<IMyTerminalBlock> GetAirlockDoorsOnThisShip(string key) {
            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(key, blocks, IsAirlockDoorOnThisShip);
            return blocks;
        }
        private IList<IMyTerminalBlock> GetAirlockVentsOnThisShip(string key) {
            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(key, blocks, IsAirlockVentOnThisShip);
            return blocks;
        }
        private IList<IMyTerminalBlock> GetMergeBlocksOnThisShip(string key) {
            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(key, blocks, IsMergeBlockOnThisShip);
            return blocks;
        }
        private IList<IMyTerminalBlock> GetConnectorBlocksOnThisShip(string key) {
            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(key, blocks, IsConnectorOnThisShip);
            return blocks;
        }


        private string GetShipIdFromBlock(IMyTerminalBlock block) {
            var idxSuffix = block.CustomName.IndexOf(ShipID_Suffix, _shipID_Prefix_Length);
            if (idxSuffix < 0)
                return "";

            return block.CustomName.Substring(0, idxSuffix + 1);
        }
        private string GetShipIdFromArguments() {
            var value = _commandData;
            if (string.IsNullOrWhiteSpace(value))
                value = ShipID_NoValue;

            return ShipID_Prefix + value.Trim() + ShipID_Suffix;
        }

        private bool IsBlockOnThisShip(IMyTerminalBlock block) => block.CustomName.StartsWith(_currentShipID);
        private bool IsDoor(IMyTerminalBlock block) => block is IMyDoor;
        private bool IsAirlockDoorOnThisShip(IMyTerminalBlock block) {
            var flag = block is IMyDoor;
            flag &= IsBlockOnThisShip(block);
            return flag;
        }
        private bool IsAirlockVentOnThisShip(IMyTerminalBlock block) {
            var flag = block is IMyAirVent;
            flag &= IsBlockOnThisShip(block);
            return flag;
        }
        private bool IsMergeBlockOnThisShip(IMyTerminalBlock block) {
            var flag = block is IMyShipMergeBlock;
            flag &= IsBlockOnThisShip(block);
            return flag;
        }
        private bool IsConnectorOnThisShip(IMyTerminalBlock block) {
            var flag = block is IMyShipConnector;
            flag &= IsBlockOnThisShip(block);
            return flag;
        }


        // Block Actions
        const string BlockAction_TurnOn = "OnOff_On";
        const string BlockAction_TurnOff = "OnOff_Off";
        const string DoorAction_CloseDoor = "Open_Off";
        const string DoorAction_OpenDoor = "Open_On";
        const string VentAction_Pressurize = "Depressurize_Off";
        const string VentAction_Depressurize = "Depressurize_On";
        const string ConnectorAction_Unlock = "Unlock";
        const string ConnectorAction_Lock = "Lock";


        private interface IActionSequence {
            int TicksRemaining { get; }
            void RunActions();
        }
        private class AirlockSequence : IActionSequence {
            private const int STEP_1 = 5;
            private const int STEP_2 = 4;
            private const int STEP_3 = 1;
            private const int STEP_4 = 0;

            public AirlockSequence(string airlockKey, bool depressurize, IList<IMyTerminalBlock> doors, IList<IMyTerminalBlock> vents) {
                _airlockKey = airlockKey;
                _depressurize = depressurize;
                for (var i = 0; i < doors.Count; i++) {
                    var name = doors[i].CustomName.ToLower();
                    if (_outerDoor == null && name.Contains(AirlockDoor_Exterior))
                        _outerDoor = doors[i];
                    else if (_innerDoor == null && name.Contains(AirlockDoor_Interior))
                        _innerDoor = doors[i];
                }
                _vent = vents[0];
                _ticksRemaining = STEP_1;

            }

            private readonly string _airlockKey;
            private readonly bool _depressurize;

            private readonly IMyTerminalBlock _innerDoor;
            private readonly IMyTerminalBlock _outerDoor;
            private readonly IMyTerminalBlock _vent;

            private int _ticksRemaining;

            public bool Depressurize { get { return _depressurize; } }
            public int TicksRemaining { get { return _ticksRemaining; } }

            public void RunActions() {
                switch (TicksRemaining) {
                    case STEP_1:
                        ApplyActionToBlock(_innerDoor, DoorAction_CloseDoor);
                        ApplyActionToBlock(_outerDoor, DoorAction_CloseDoor);
                        if (_depressurize)
                            ApplyActionToBlock(_vent, VentAction_Depressurize);
                        else
                            ApplyActionToBlock(_vent, VentAction_Pressurize);
                        break;

                    case STEP_2:
                        ApplyActionToBlock(_innerDoor, BlockAction_TurnOff);
                        ApplyActionToBlock(_outerDoor, BlockAction_TurnOff);
                        break;

                    case STEP_3:
                        if (_depressurize)
                            ApplyActionToBlock(_outerDoor, BlockAction_TurnOn);
                        else
                            ApplyActionToBlock(_innerDoor, BlockAction_TurnOn);
                        break;

                    case STEP_4:
                        if (_depressurize)
                            ApplyActionToBlock(_outerDoor, DoorAction_OpenDoor);
                        else
                            ApplyActionToBlock(_innerDoor, DoorAction_OpenDoor);
                        break;
                }

                _ticksRemaining--;
            }
        }

        private class MergeDecoupleSequence : IActionSequence {
            private const int STEP_1 = 10;
            //private const int STEP_2 = 8;
            private const int STEP_2b = 9;
            private const int STEP_3 = 0;

            public MergeDecoupleSequence(string mergeKey, IList<IMyTerminalBlock> mergeBlocks, IList<IMyTerminalBlock> connectorBlocks) {
                _mergeKey = mergeKey;
                _merge = mergeBlocks[0];
                _connector = connectorBlocks[0];
                _ticksRemaining = STEP_1;
            }

            private readonly string _mergeKey;

            private readonly IMyTerminalBlock _merge;
            private readonly IMyTerminalBlock _connector;

            private int _ticksRemaining;

            public int TicksRemaining { get { return _ticksRemaining; } }

            public void RunActions() {
                switch (TicksRemaining) {
                    case STEP_1:
                        ApplyActionToBlock(_connector, ConnectorAction_Unlock);
                        //    break;
                        //case STEP_2:
                        ApplyActionToBlock(_connector, BlockAction_TurnOff);
                        break;
                    case STEP_2b:
                        ApplyActionToBlock(_merge, BlockAction_TurnOff);
                        break;
                    case STEP_3:
                        ApplyActionToBlock(_connector, BlockAction_TurnOn);
                        ApplyActionToBlock(_merge, BlockAction_TurnOn);
                        break;

                }

                _ticksRemaining--;
            }
        }

    }
}
