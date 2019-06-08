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

        const string CMD_RETRACT = "retract";
        const string CMD_EXTEND = "extend";


        string GroupKey_AllWelders = string.Empty;
        string GroupKey_AllPistons = string.Empty;
        float Speed_Operation = 0.0F;
        float Speed_MoveToPosition = 0.0F;


        readonly RunningSymbol Running = new RunningSymbol();
        readonly List<IMyPistonBase> PistonList = new List<IMyPistonBase>();
        readonly List<IMyShipWelder> WelderList = new List<IMyShipWelder>();
        readonly StateMachine<bool> Operation = new StateMachine<bool>();

        string OperationMessage = string.Empty;


        public void Main(string argument, UpdateType updateSource) {
            LoadConfig();
            var autoRun = Runtime.UpdateFrequency.HasFlag(UpdateFrequency.Update10);
            if (autoRun)
                Echo("Running " + Running.GetSymbol(Runtime));

            argument = argument?.ToLower();
            switch (argument) {
                case "extend":
                    Operation.Clear();
                    Operation.Add("piston", SetFactoryState(true), true);
                    break;
                case "retract":
                    Operation.Clear();
                    Operation.Add("piston", SetFactoryState(false), true);
                    break;
            }

            Operation.RunAll();

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
            _configHashCode = tmpHashCode;

            var ini = new MyIni();

            MyIniParseResult result;
            ini.TryParse(Me.CustomData, out result);

            ini.Add(Key_AllWelders, "");
            ini.Add(Key_AllPistons, "");
            ini.Add(Key_SpeedOperation, 0.015f);
            ini.Add(Key_SpeedPosition, 1.0f);

            GroupKey_AllWelders = ini.Get(Key_AllWelders).ToString();
            GroupKey_AllPistons = ini.Get(Key_AllPistons).ToString();
            Speed_Operation = ini.Get(Key_SpeedOperation).ToSingle();
            Speed_MoveToPosition = ini.Get(Key_SpeedPosition).ToSingle();

            Me.CustomData = ini.ToString();
        }


    }
}
