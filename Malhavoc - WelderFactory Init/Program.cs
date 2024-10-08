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
    partial class Program : MyGridProgram {

        string GroupKey_AllWelders = string.Empty;
        string GroupKey_AllPistons = string.Empty;
        float Speed_Operation = 0.015f;
        float Speed_MoveToPosition = 1.0F;


        readonly RunningSymbol Running = new RunningSymbol();
        readonly List<IMyPistonBase> PistonList = new List<IMyPistonBase>();
        readonly List<IMyShipWelder> WelderList = new List<IMyShipWelder>();
        readonly StateMachineQueue Operation = new StateMachineQueue();

        string OperationMessage = string.Empty;


        public void Main(string argument, UpdateType updateSource) {
            LoadConfig();
            var autoRun = (updateSource & UpdateType.Update10) == UpdateType.Update10;
            if (autoRun)
                Echo("Running " + Running.GetSymbol(Runtime));

            argument = argument?.ToLower();
            switch (argument) {
                case "extend":
                    Operation.Clear();
                    Operation.Add(SetFactoryState(true));
                    break;
                case "retract":
                    Operation.Clear();
                    Operation.Add(SetFactoryState(false));
                    break;
            }

            Operation.Run();

            if (Operation.HasTasks)
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            else if (autoRun)
                Runtime.UpdateFrequency = UpdateFrequency.Once;
            else
                Runtime.UpdateFrequency = UpdateFrequency.None;

            Echo(OperationMessage);
        }

        IEnumerator<bool> SetFactoryState(bool extend) {
            OperationMessage = extend
                ? "Moving welders to start position"
                : "Moving welders to retracted position";

            Action<IMyPistonBase> MovePistonAction;
            Func<IMyPistonBase, bool> PositionCheckFunc;

            if (extend) {
                OperationMessage = "Moving welders to start position";
                MovePistonAction = (p) => p.Extend();
                PositionCheckFunc = IsExtended;
            } else {
                OperationMessage = "Moving welders to retracted position";
                MovePistonAction = (p) => p.Retract();
                PositionCheckFunc = IsReteacted;
            }

            LoadBlocks();
            yield return true;

            WelderList.ForEach(w => w.Enabled = false);
            yield return true;

            PistonList.ForEach(p => p.Velocity = Speed_MoveToPosition);
            PistonList.ForEach(MovePistonAction);

            var allAtEnd = false;
            do {
                yield return true;
                allAtEnd = PistonList.All(PositionCheckFunc);
            } while (!allAtEnd);

            PistonList.ForEach(p => p.Velocity = Speed_Operation);
            PistonList.ForEach(MovePistonAction);
            yield return true;
            OperationMessage = string.Empty;
            yield return false;
        }


        void LoadBlocks() {
            LoadList(GroupKey_AllPistons, PistonList);
            LoadList(GroupKey_AllWelders, WelderList);
        }
        void LoadList<T>(string groupName, List<T> blockList) where T : class {
            blockList.Clear();
            if (string.IsNullOrWhiteSpace(groupName)) return;
            var group = GridTerminalSystem.GetBlockGroupWithName(groupName);
            group.GetBlocksOfType(blockList);
        }

        bool IsExtended(IMyPistonBase piston) => Math.Round(piston.CurrentPosition, 3) >= Math.Round(piston.MaxLimit, 3);
        bool IsReteacted(IMyPistonBase piston) => Math.Round(piston.CurrentPosition, 3) <= Math.Round(piston.MinLimit, 3);


        const string SECTION_TAG = "Groups";
        readonly MyIniKey Key_AllWelders = new MyIniKey(SECTION_TAG, "Group - All Welders");
        readonly MyIniKey Key_AllPistons = new MyIniKey(SECTION_TAG, "Group - All Pistons");

        const string SECTION_TAG2 = "Speeds";
        readonly MyIniKey Key_SpeedOperation = new MyIniKey(SECTION_TAG2, "Pistons - Operation Speed");
        readonly MyIniKey Key_SpeedPosition = new MyIniKey(SECTION_TAG2, "Pistons - Position Speed");

        int _configHashCode = 0;
        void LoadConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;

            var ini = new MyIni();

            ini.TryParse(Me.CustomData);

            GroupKey_AllWelders = ini.Add(Key_AllWelders, GroupKey_AllWelders).ToString();
            GroupKey_AllPistons = ini.Add(Key_AllPistons, GroupKey_AllPistons).ToString();
            Speed_Operation = ini.Add(Key_SpeedOperation, Speed_Operation).ToSingle();
            Speed_MoveToPosition = ini.Add(Key_SpeedPosition, Speed_MoveToPosition).ToSingle();

            Me.CustomData = ini.ToString();
            _configHashCode = Me.CustomData.GetHashCode();
        }


    }
}
