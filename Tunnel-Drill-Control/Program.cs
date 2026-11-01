using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
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
    public partial class Program : MyGridProgram {
        // const double BLOCK_RELOAD_TIME = 10;

        // double _timeLastBlockLoad = BLOCK_RELOAD_TIME * 2;
        // double _timeLastCleared = 0;

        // readonly RunningSymbol _runningSymbol = new RunningSymbol();

        enum MyDrillState {
            initializing = 0,
            Stopped = 1,
            Drilling = 2,
            OnHold = 3
        }

        MyDrillState _drillState = MyDrillState.initializing;
        private MyDrillState DrillState {
            get { return _drillState; }
            set { 
                if (_drillState == value) {
                    return;
                }
                _drillState = value; 
                switch (_drillState) {
                    case MyDrillState.Drilling:
                        Runtime.UpdateFrequency = UpdateFrequency.Update100;
                        StartDrilling();
                        break;
                    case MyDrillState.OnHold:
                        Runtime.UpdateFrequency = UpdateFrequency.Update100;
                        HoldDrilling();
                        break;
                    case MyDrillState.Stopped:
                        Runtime.UpdateFrequency = UpdateFrequency.None;
                        StopDrilling();
                        break;
                }
            }
        }

        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        readonly string _instructions;
        readonly Configuration _config = new Configuration();

        readonly List<IMyShipDrill> _drills = new List<IMyShipDrill>();
        readonly List<IMyFunctionalBlock> _OnOffBlocks = new List<IMyFunctionalBlock>();
        readonly List<IMyPistonBase> _pistons = new List<IMyPistonBase>();
        IMyCargoContainer _cargoContainer;
        // readonly List<IMyPistonBase> _pistonsTemp = new List<IMyPistonBase>();

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            _commands.Add("start", StartDrilling);
            _commands.Add("stop", StopDrilling);
            _commands.Add("hold", HoldDrilling);

            _instructions = "$TITLE$ v$VERSION$\nScript Commands:\n" + string.Join("\n", _commands.Keys);

            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();

            Load();
        }

        public void Load() {
            var cfg = StateConfig.Load(Storage);
            _drillState = cfg.State;
        }

        public void Save() {
            var cfg = new StateConfig();
            cfg.State = _drillState;

            Storage = cfg.BuildStorageString();
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo(_instructions);
            Echo("\n\nState: " + _drillState);
            Echo("Configure script in 'Custom Data'");

            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();

            if (_commands.ContainsKey(argument)) {
                _commands[argument]?.Invoke();
                return;
            }            

            LoadBlocks();

            var cargoPercentFull = _cargoContainer.GetInventory().CurrentVolume.RawValue / (float)_cargoContainer.GetInventory().MaxVolume.RawValue;
            switch (_drillState) {
                case MyDrillState.Drilling:
                    if (cargoPercentFull >= _config.MaxCargoPercentage) {
                        DrillState = MyDrillState.OnHold;
                        break;
                    }
                    
                    var isFullyExtended = _pistons.All(p => p.CurrentPosition >= p.HighestPosition);
                    if (isFullyExtended)
                        DrillState = MyDrillState.Stopped;
                    break;
                case MyDrillState.OnHold:
                    if (cargoPercentFull < _config.MinCargoPercentage)
                        DrillState = MyDrillState.Drilling;
                    break;
            }
        }

        void LoadBlocks() {
            _pistons.Clear();
            GridTerminalSystem.GetBlocksOfType(_OnOffBlocks, b => Me.IsSameConstructAs(b) && IsTagged(b, _config.TagName) && !(b is IMyPistonBase));
            var piston = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyPistonBase>(b => IsSameGrid(b, Me) && IsTagged(b, _config.TagName));
            var lastPiston = piston;
            while (piston != null) {
                _pistons.Add(piston);
                // if (!piston.IsAttached) break;
                lastPiston = piston;
                piston = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyPistonBase>(p => p.CubeGrid == piston.TopGrid);
                if (piston != null) _pistons.Add(piston);
            }

            GridTerminalSystem.GetBlocksOfType(_drills, b => b.CubeGrid == lastPiston.TopGrid);

            _cargoContainer = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCargoContainer>(b => IsSameGrid(b, Me) && IsTagged(b, _config.TagName));
        }

        void StartDrilling() {
            LoadBlocks();
            var velocity = _config.MaxDrillVelocity / _pistons.Count;
            _pistons.ForEach(piston => piston.Velocity = velocity);
            _drills.ForEach(drill => drill.Enabled = true);
            _OnOffBlocks.ForEach(b => b.Enabled = true);
            _drillState = MyDrillState.Drilling;
        }

        void StopDrilling() {
            LoadBlocks();
            _pistons.ForEach(piston => piston.Velocity = -_config.RetractionVelocity);
            _drills.ForEach(drill => drill.Enabled = false);
            _OnOffBlocks.ForEach(b => b.Enabled = false);
            _drillState = MyDrillState.Stopped;
        }
        void HoldDrilling() {
            LoadBlocks();
            _pistons.ForEach(piston => piston.Velocity = 0);
            _drills.ForEach(drill => drill.Enabled = false);
            _OnOffBlocks.ForEach(b => b.Enabled = false);
            _drillState = MyDrillState.OnHold;
        }


        class Configuration : ConfigBase {
            public string TagName { get; private set; } = "Tunnel-Drill";
            public float MaxDrillVelocity { get; private set; } = 0.08f;
            public float RetractionVelocity { get; private set; } = 0.5f;
            public float MaxCargoPercentage { get; private set; } = 0.95f;
            public float MinCargoPercentage { get; private set; } = 0.5f;

            public void Load() {
                if (!LoadIni()) return;

                _ini.AddSection("Tunnel-Drill");
                _ini.SetSectionComment("Tunnel-Drill", "Configuration for the tunnel drill");
                TagName = _ini.Add("Tunnel-Drill", "TagName", TagName).ToString();
                MaxDrillVelocity = _ini.Add("Tunnel-Drill", "MaxVelocity", MaxDrillVelocity).ToSingle();
                RetractionVelocity = _ini.Add("Tunnel-Drill", "RetractionVelocity", RetractionVelocity).ToSingle();
                MaxCargoPercentage = _ini.Add("Tunnel-Drill", "MaxCargoPercentage", MaxCargoPercentage).ToSingle();
                MinCargoPercentage = _ini.Add("Tunnel-Drill", "MinCargoPercentage", MinCargoPercentage).ToSingle();

                Save();
            }
        }

        class StateConfig {
            public static StateConfig Load(string storage) {
                var config = new StateConfig();

                var ini = new MyIni();
                if (!ini.TryParse(storage)) return config;

                config.State = (MyDrillState)ini.Add(StateKey, config.State).ToInt32();

                return config;
            }

            public string BuildStorageString() {
                var ini = new MyIni();

                ini.Add(StateKey, (int)State);

                return ini.ToString();
            }


            static MyIniKey StateKey = new MyIniKey("Drill-Control", "State");
            public MyDrillState State { get; set; } = MyDrillState.initializing;

        }
    }
}
