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

        readonly RunningSymbol Running = new RunningSymbol();
        readonly List<IMyPistonBase> PistonList = new List<IMyPistonBase>();
        readonly List<IMyShipWelder> WelderList = new List<IMyShipWelder>();
        readonly StateMachineQueue Operation = new StateMachineQueue();

        readonly Config _config = new Config();

        string OperationMessage = string.Empty;


        public Program() {
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();
        }


        public void Main(string argument, UpdateType updateSource) {
            _config.Load();
            var autoRun = (updateSource & UpdateType.Update10) == UpdateType.Update10;
            if (autoRun)
                Echo("Running " + Running.GetSymbol());

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
                PositionCheckFunc = IsRetracted;
            }

            LoadBlocks();
            yield return true;

            WelderList.ForEach(w => w.Enabled = false);
            yield return true;

            PistonList.ForEach(p => p.Velocity = _config.Speed_MoveToPosition);
            PistonList.ForEach(MovePistonAction);

            var allAtEnd = false;
            do {
                yield return true;
                allAtEnd = PistonList.All(PositionCheckFunc);
            } while (!allAtEnd);

            PistonList.ForEach(p => p.Velocity = _config.Speed_Operation);
            PistonList.ForEach(MovePistonAction);
            yield return true;
            OperationMessage = string.Empty;
            yield return false;
        }


        void LoadBlocks() {
            LoadList(_config.GroupKey_AllPistons, PistonList);
            LoadList(_config.GroupKey_AllWelders, WelderList);
        }
        void LoadList<T>(string groupName, List<T> blockList) where T : class {
            blockList.Clear();
            if (string.IsNullOrWhiteSpace(groupName)) return;
            var group = GridTerminalSystem.GetBlockGroupWithName(groupName);
            group.GetBlocksOfType(blockList);
        }

        bool IsExtended(IMyPistonBase piston) => Math.Round(piston.CurrentPosition, 3) >= Math.Round(piston.MaxLimit, 3);
        bool IsRetracted(IMyPistonBase piston) => Math.Round(piston.CurrentPosition, 3) <= Math.Round(piston.MinLimit, 3);


        class Config : ConfigBase {
            public string GroupKey_AllWelders { get; private set; } = string.Empty;
            public string GroupKey_AllPistons { get; private set; } = string.Empty;
            public float Speed_Operation { get; private set; } = 0.015f;
            public float Speed_MoveToPosition { get; private set; } = 1.0F;
            
            const string SECTION_TAG = "Groups";
            const string SECTION_TAG2 = "Speeds";

            public void Load() {
                if (!LoadIni()) return;

                GroupKey_AllWelders = _ini.Add(SECTION_TAG, "Group - All Welders", GroupKey_AllWelders).ToString();
                GroupKey_AllPistons = _ini.Add(SECTION_TAG, "Group - All Pistons", GroupKey_AllPistons).ToString();

                Speed_Operation = _ini.Add(SECTION_TAG2, "Pistons - Operation Speed", Speed_Operation).ToSingle();
                Speed_MoveToPosition = _ini.Add(SECTION_TAG2, "Pistons - Position Speed", Speed_MoveToPosition).ToSingle();

                Save();
            }
        }

    }
}
