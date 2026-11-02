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
        const float PISTON_BASE_LENGTH = 5.16f;

        enum MyDrillState {
            Initializing = 0,
            Stopped = 1,
            Drilling = 2,
            OnHold = 3
        }

        MyDrillState _operatingMode_InternalUseOnly = MyDrillState.Initializing;
        private MyDrillState OperatingMode {
            get { return _operatingMode_InternalUseOnly; }
            set {
                if (_operatingMode_InternalUseOnly == value) {
                    return;
                }
                _operatingMode_InternalUseOnly = value;
                Log($"Mode -> {value}");
                switch (value) {
                    case MyDrillState.Drilling:
                        Runtime.UpdateFrequency = UpdateFrequency.Update100;
                        StartDrilling();
                        break;
                    case MyDrillState.OnHold:
                        Runtime.UpdateFrequency = UpdateFrequency.Update100;
                        HoldDrilling();
                        break;
                    case MyDrillState.Stopped:
                        Runtime.UpdateFrequency = UpdateFrequency.Once;
                        StopDrilling();
                        break;
                }
            }
        }

        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        readonly string _instructions;
        readonly Configuration _config = new Configuration();

        readonly List<IMyFunctionalBlock> _OnOffBlocks = new List<IMyFunctionalBlock>();
        readonly List<IMyCargoContainer> _cargoContainers = new List<IMyCargoContainer>();
        readonly List<IMyPistonBase> _pistons = new List<IMyPistonBase>();
        readonly List<IMyShipDrill> _drills = new List<IMyShipDrill>();


        StringBuilder _output = new StringBuilder();
        Logging _logging = new Logging();


        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            _commands.Add("start", StartDrilling);
            _commands.Add("stop", StopDrilling);
            _commands.Add("hold", HoldDrilling);
            _commands.Add("show", ShowPistons);

            _instructions = "$TITLE$ v$VERSION$\nScript Commands:\n" + string.Join("\n", _commands.Keys);

            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();

            Load();
        }

        public void Load() {
            var cfg = StateConfig.Load(Storage);
            OperatingMode = cfg.State;
        }

        public void Save() {
            var cfg = new StateConfig();
            cfg.State = OperatingMode;

            Storage = cfg.BuildStorageString();
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo(_instructions);
            Echo("\n\nState: " + OperatingMode);
            Echo("Configure script in 'Custom Data'");


            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();

            _output.Clear();

            try {
                if (_commands.ContainsKey(argument)) {
                    _commands[argument]?.Invoke();
                }

                _output.AppendLine($"Operation: {OperatingMode}");

                var cargoPercentFull = GetInventoryFullPercentage(_cargoContainers);
                switch (OperatingMode) {
                    case MyDrillState.Drilling:
                        var numPistonsToExtend = 0;
                        var currentExtension = 0.0f;
                        var totalExtension = 0.0f;
                        foreach (var piston in _pistons) {
                            if (piston.CurrentPosition < piston.HighestPosition)
                                numPistonsToExtend++;
                            currentExtension += piston.CurrentPosition + PISTON_BASE_LENGTH;
                            totalExtension += piston.HighestPosition + PISTON_BASE_LENGTH;
                        }

                        // var numPistonsToExtend = _pistons.Count(p => p.CurrentPosition < p.HighestPosition);
                        if (numPistonsToExtend == 0) {
                            OperatingMode = MyDrillState.Stopped;
                            Log("No pistons to extend, stopping drill.");
                            break;
                        }

                        var velocity = _config.MaxDrillVelocity / numPistonsToExtend;
                        _pistons.ForEach(piston => piston.Velocity = velocity);
                        if (cargoPercentFull >= _config.MaxCargoPercentage) {
                            Log("Cargo full, putting drill on hold.");
                            OperatingMode = MyDrillState.OnHold;
                            break;
                        }

                        _output.AppendLine($"Cargo: {cargoPercentFull * 100:N1} %");
                        _output.AppendLine($"Pistons to Extend: {numPistonsToExtend}");
                        _output.AppendLine($"Drill Velocity: {velocity:N4}");
                        _output.AppendLine($"Extension: {currentExtension:N3}m -> {totalExtension:N3}m");
                        // var isFullyExtended = _pistons.All(p => p.CurrentPosition >= p.HighestPosition);
                        break;
                    case MyDrillState.OnHold:
                        if (cargoPercentFull < _config.MinCargoPercentage)
                            OperatingMode = MyDrillState.Drilling;
                        break;
                }
            } catch (Exception e) {
                _output.AppendLine("Error: " + e.Message);
                _output.AppendLine("StackTrace: " + e.StackTrace);
            } finally {
                Echo(_output.ToString() + "\n" + _logging.ToString());
                if (Me.SurfaceCount > 0) {
                    var surface = Me.GetSurface(0);
                    surface.ContentType = ContentType.TEXT_AND_IMAGE;
                    surface.WriteText(_output.ToString() + "\n" + _logging.ToString());
                }
            }
        }

        void Log(string message) => _logging.AppendLine(DateTime.Now.ToString("HH:mm:ss") + " - " + message);

        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(_OnOffBlocks, b => Me.IsSameConstructAs(b) && IsTagged(b, _config.TagName) && !(b is IMyPistonBase));

            _pistons.Clear();
            IMyPistonBase lastPiston = null;
            var piston = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyPistonBase>(b => IsSameGrid(b, Me) && IsTagged(b, _config.TagName));
            if (piston != null) {
                _pistons.Add(piston);
                var rootPistonName = piston.CustomName.Replace(_config.TagName, "").Trim();
                var i = 0;
                while (piston != null) {
                    // if (!piston.IsAttached) break;
                    lastPiston = piston;
                    piston = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyPistonBase>(p => p.CubeGrid == piston.TopGrid);
                    if (piston != null) {
                        i++;
                        piston.CustomName = $"{rootPistonName} {i:00}";
                        _pistons.Add(piston);
                    }
                }
            }

            _drills.Clear();
            if (lastPiston != null)
                GridTerminalSystem.GetBlocksOfType(_drills, b => b.CubeGrid == lastPiston.TopGrid);

            GridTerminalSystem.GetBlocksOfType(_cargoContainers, b => IsSameGrid(b, Me) && IsTagged(b, _config.TagName));
        }

        float GetInventoryFullPercentage(IEnumerable<IMyTerminalBlock> blocks) {
            var totalVolume = 0.0;
            var currentVolume = 0.0;
            foreach (var c in blocks) {
                totalVolume += c.GetInventory().MaxVolume.RawValue;
                currentVolume += c.GetInventory().CurrentVolume.RawValue;
            };
            return (float)(currentVolume / totalVolume);
        }

        void StartDrilling() {
            LoadBlocks();
            _pistons.ForEach(piston => piston.Velocity = 0f);
            _drills.ForEach(drill => drill.Enabled = true);
            _OnOffBlocks.ForEach(b => b.Enabled = true);
            OperatingMode = MyDrillState.Drilling;
        }

        void StopDrilling() {
            LoadBlocks();
            _pistons.ForEach(piston => piston.Velocity = _config.RetractionVelocity * (_config.RetractionVelocity > 0 ? -1 : 1));
            _drills.ForEach(drill => drill.Enabled = false);
            _OnOffBlocks.ForEach(b => b.Enabled = false);
            OperatingMode = MyDrillState.Stopped;
        }
        void HoldDrilling() {
            LoadBlocks();
            _pistons.ForEach(piston => piston.Velocity = 0);
            _drills.ForEach(drill => drill.Enabled = false);
            _OnOffBlocks.ForEach(b => b.Enabled = false);
            OperatingMode = MyDrillState.OnHold;
        }

        void ShowPistons() {
            LoadBlocks();
            for (var i = 0; i < _pistons.Count; i++) {
                var piston = _pistons[i];
                _output.AppendLine($"Piston[{i}]: {piston.CustomName} / {piston.EntityId}");
            }
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
            public MyDrillState State { get; set; } = MyDrillState.Initializing;

        }
    }
}
