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

        readonly RunningSymbol _running = new RunningSymbol();
        readonly List<IMyPistonBase> _pistons = new List<IMyPistonBase>();
        readonly List<IMyShipWelder> _welders = new List<IMyShipWelder>();
        readonly StateMachineQueue _operation = new StateMachineQueue();

        readonly Config _config = new Config();

        string _operationMessage = string.Empty;


        public Program() {
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();
        }


        public void Main(string argument, UpdateType updateSource) {
            _config.Load();
            var autoRun = (updateSource & UpdateType.Update10) == UpdateType.Update10;
            if (autoRun) Echo("Running " + _running.GetSymbol());

            argument = argument?.ToLower();
            switch (argument) {
                case "extend":
                    _operation.Clear();
                    _operation.Add(SetFactoryState(true));
                    break;
                case "retract":
                    _operation.Clear();
                    _operation.Add(SetFactoryState(false));
                    break;
            }

            _operation.Run();

            if (_operation.HasTasks)
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            else if (autoRun)
                Runtime.UpdateFrequency = UpdateFrequency.Once;
            else
                Runtime.UpdateFrequency = UpdateFrequency.None;

            Echo(_operationMessage);
        }

        IEnumerator<bool> SetFactoryState(bool extend) {
            _operationMessage = extend
                ? "Moving welders to start position"
                : "Moving welders to retracted position";

            Action<IMyPistonBase> MovePistonAction;
            Func<IMyPistonBase, bool> PositionCheckFunc;

            if (extend) {
                _operationMessage = "Moving welders to start position";
                MovePistonAction = (p) => p.Extend();
                PositionCheckFunc = IsExtended;
            } else {
                _operationMessage = "Moving welders to retracted position";
                MovePistonAction = (p) => p.Retract();
                PositionCheckFunc = IsRetracted;
            }

            LoadBlocks();
            yield return true;

            _welders.ForEach(w => w.Enabled = false);
            yield return true;

            _pistons.ForEach(p => p.Velocity = _config.Speed_MoveToPosition);
            _pistons.ForEach(MovePistonAction);

            var allAtEnd = false;
            do {
                yield return true;
                allAtEnd = _pistons.All(PositionCheckFunc);
            } while (!allAtEnd);

            _pistons.ForEach(p => p.Velocity = _config.Speed_Operation);
            _pistons.ForEach(MovePistonAction);
            yield return true;
            _operationMessage = string.Empty;
            yield return false;
        }


        void LoadBlocks() {
            LoadList(_config.GroupKey_AllPistons, _pistons);
            LoadList(_config.GroupKey_AllWelders, _welders);
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
