// <mdk sortorder="1" />
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

        // const UpdateFrequency RUNNING_FREQUENCY = UpdateFrequency.Update100;
        // const UpdateFrequency INITIALIZATION_FREQUENCY = UpdateFrequency.Once;
        // const UpdateType RUNNING_SYMBOL_UPDATE_TYPE = UpdateType.Update100;

        // Operation block references
        readonly List<IMyLightingBlock> _warningLights = new List<IMyLightingBlock>();
        readonly List<IMyCargoContainer> _cargoContainers = new List<IMyCargoContainer>();
        readonly List<IMyPistonBase> _pistons = new List<IMyPistonBase>();
        readonly List<IMyShipDrill> _drills = new List<IMyShipDrill>();


        MyDrillState _currentState_internal = MyDrillState.Resetting;
        MyDrillState State {
            get { return _currentState_internal; }
            set {
                if (_currentState_internal != value) {
                    _currentState_internal = value;
                    foreach (var light in _warningLights) {
                        light.Enabled = _currentState_internal != MyDrillState.Ready;
                        switch (_currentState_internal) {
                            case MyDrillState.Holding: light.Color = Color.Yellow; break;
                            case MyDrillState.Ready: light.Color = Color.White; break;
                            default: light.Color = Color.Red; break;
                        }
                    }
                }
            }
        }

        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        readonly string _instructions;
        readonly Configuration _config = new Configuration();

        readonly RunningSymbol _runningSymbol = new RunningSymbol();
        readonly StateMachineQueue _operationQueue = new StateMachineQueue();
        readonly DebugLogging _debug;

        float _cargoInventoryPercentage;
        float _drillInventoryPercentage;
        float _extensionDistance;
        float _extensionPercentage;


        readonly MyIni _storageIni = new MyIni();
        const string STORAGE_SECTION = "DrillControl";

        public Program() {
            // Runtime.UpdateFrequency = UpdateFrequency.Once;
            _debug = new DebugLogging(this);
            _debug.MaxTextLinesToKeep = 10;

            _commands.Add("reset", CommandReset);
            _commands.Add("start", CommandStart);
            _commands.Add("stop", CommandStop);
            // _commands.Add("hold", CommandHold);
            _instructions = "Commands:\n   " + string.Join("\n   ", _commands.Keys);

            // Initialize the configuration
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();

            // Load the current state from storage
            if (_storageIni.TryParse(Storage)) {
                Debug($"Parsed storage");
                var version = _storageIni.Get(STORAGE_SECTION, "Version").ToString();
                Debug($"Storage version: {version}");
                switch (version) {
                    case "1":
                        // Handle version 1 if needed
                        var stateInt = _storageIni.Get(STORAGE_SECTION, "State").ToInt32();
                        Debug($"Loaded state int: {stateInt}");
                        State = (MyDrillState)stateInt;
                        Debug($"Loaded state: {State}");
                        break;
                    default: break;
                }
            }

            switch (State) {
                case MyDrillState.Resetting: CommandReset(); break;
                case MyDrillState.Drilling: CommandStart(); break;
                case MyDrillState.Holding: CommandHold(); break;
                case MyDrillState.Stopped: CommandStop(); break;
                default: break;
            }

            Runtime.UpdateFrequency = UpdateFrequency.Once;
        }

        public void Save() {
            // Save the current state to storage
            _storageIni.Clear();
            _storageIni.Add(STORAGE_SECTION, "Version", "1");
            _storageIni.Add(STORAGE_SECTION, "State", (int)State);
            Storage = _storageIni.ToString();
        }

        private void Debug(string message) => _debug?.AppendLine(message);

        public void Main(string argument, UpdateType updateSource) {
            Echo("$TITLE$ v$VERSION$ " + (updateSource == UpdateType.Update10 ? " " + _runningSymbol.GetSymbol() : ""));
            Echo(_instructions);
            Echo("");
            Echo($"Current State: {State}");

            try {
                _config.Load();

                if (argument.Length > 0 && _commands.ContainsKey(argument))
                    _commands[argument]?.Invoke();

                _cargoInventoryPercentage = GetInventoryFullPercentage(_cargoContainers);
                _drillInventoryPercentage = GetInventoryFullPercentage(_drills);

                _extensionDistance = 0f;
                var maxExtension = 0f;
                foreach (var p in _pistons) {
                    _extensionDistance += p.CurrentPosition;
                    maxExtension += p.MaxLimit;
                }
                _extensionPercentage = _pistons.Count > 0 ? _extensionDistance / maxExtension : 0;

                // Run the operation queue
                var result = _operationQueue.Run();
                Runtime.UpdateFrequency = result ? UpdateFrequency.Update10 : UpdateFrequency.None;

            } catch (Exception exc) {
                Debug($"Exception: {exc.Message}");
            } finally {
                _debug.UpdateDisplay();
                UpdateProgDisplay();
            }
        }

        float GetInventoryFullPercentage(IEnumerable<IMyTerminalBlock> blocks) {
            var totalVolume = 0.0;
            var currentVolume = 0.0;
            foreach (var c in blocks) {
                totalVolume += c.GetInventory().MaxVolume.RawValue;
                currentVolume += c.GetInventory().CurrentVolume.RawValue;
            }
            ;
            return totalVolume != 0 ? (float)(currentVolume / totalVolume) : 0;
        }

        void CommandReset() {
            Debug("CMD: Reset");
            LoadBlocks();
            State = MyDrillState.Resetting;

            _operationQueue.Clear();
            _operationQueue.Add(Action_StopMovement());
            // _operationQueue.Add(Action_RetractPistons());
            // _operationQueue.Add(Action_WaitForRetractionCompleted());
            _operationQueue.Add(Action_RetractPistonLoop(_config.NumberOfPistonsToRetract));
            _operationQueue.Add(Action_SetStatus(MyDrillState.Ready));
        }

        void CommandStart() {
            Debug("CMD: Start");
            LoadBlocks();
            State = MyDrillState.Drilling;

            _operationQueue.Clear();
            _operationQueue.Add(Action_StartAllDrills());
            _operationQueue.Add(Action_DrillingLoop());
            _operationQueue.Add(Action_StopMovement());
            // _operationQueue.Add(Action_RetractPistons(_config.NumberOfPistonsToRetract));
            // _operationQueue.Add(Action_WaitForRetractionCompleted(_config.NumberOfPistonsToRetract));
            _operationQueue.Add(Action_RetractPistonLoop(_config.NumberOfPistonsToRetract));
            _operationQueue.Add(Action_SetStatus(MyDrillState.Ready));
        }

        void CommandHold() {
            Debug("CMD: Hold");
            LoadBlocks();

            _operationQueue.Clear();
            _operationQueue.Add(Action_StopMovement());
            _operationQueue.Add(Action_SetStatus(MyDrillState.Holding));
            _operationQueue.Add(Action_WaitForHoldCondition());
            _operationQueue.Add(Action_RunCommand(CommandStart));
        }

        void CommandStop() {
            Debug("CMD: Stop");
            LoadBlocks();
            State = MyDrillState.Stopped;

            _operationQueue.Clear();
            _operationQueue.Add(Action_StopMovement());
        }

        StringBuilder _progDisplay = new StringBuilder();
        void UpdateProgDisplay() {
            _progDisplay.Clear();
            _progDisplay.AppendLine("$TITLE$\n");
            _progDisplay.AppendLine($"State: {State}");
            _progDisplay.AppendLine($"{BuildProgressBar(_extensionPercentage)} {_extensionDistance:N1} m");
            // _progDisplay.AppendLine($"{_cargoInventoryPercentage * 100:N1}");
            // _progDisplay.AppendLine($"Drill: {_drillInventoryPercentage * 100:N1}");
            _progDisplay.AppendLine($"{BuildProgressBar(_cargoInventoryPercentage, 40)} Cargo Capacity");
            _progDisplay.AppendLine($"{BuildProgressBar(_drillInventoryPercentage, 40)} Drill Capacity");

            var screen = Me.GetSurface(0);
            screen.ContentType = ContentType.TEXT_AND_IMAGE;
            screen.WriteText(_progDisplay.ToString());
        }

        static string BuildProgressBar(float percentage, int width = 50) {
            var bar = "".PadRight((int)(percentage * width), '|').PadRight(width, '\'');
            return $"[{bar}]";
        }

    }
}
