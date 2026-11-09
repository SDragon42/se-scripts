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
        readonly List<IMyFunctionalBlock> _OnOffBlocks = new List<IMyFunctionalBlock>();
        readonly List<IMyCargoContainer> _cargoContainers = new List<IMyCargoContainer>();
        readonly List<IMyPistonBase> _pistons = new List<IMyPistonBase>();
        readonly List<IMyShipDrill> _drills = new List<IMyShipDrill>();


        MyDrillState _currentState = MyDrillState.Initializing;
        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        readonly string _instructions;
        readonly Configuration _config = new Configuration();

        readonly RunningSymbol _runningSymbol = new RunningSymbol();
        readonly StateMachineQueue _operationQueue = new StateMachineQueue();
        readonly DebugLogging _debug;


        readonly MyIni _storageIni = new MyIni();
        const string STORAGE_SECTION = "DrillControl";

        public Program() {
            // Runtime.UpdateFrequency = UpdateFrequency.Once;
            _debug = new DebugLogging(this);
            _debug.MaxTextLinesToKeep = 10;

            _commands.Add("reset", CommandReset);
            _commands.Add("start", CommandStart);
            _commands.Add("stop", CommandStop);
            _commands.Add("hold", CommandHold);
            _instructions = "Commands:\n   " + string.Join("\n   ", _commands.Keys);

            // Initialize the configuration
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();

            // Load the current state from storage
            if (_storageIni.TryParse(Storage)) {
                var version = _storageIni.Get(STORAGE_SECTION, "Version").ToString();
                switch (version) {
                    case "1":
                        // Handle version 1 if needed
                        var stateString = _storageIni.Get(STORAGE_SECTION, "State").ToString();
                        break;
                    default:
                        // Unknown version, handle accordingly
                        break;
                }
            }
        }

        public void Save() {
            // Save the current state to storage
            _storageIni.Clear();
            _storageIni.Add(STORAGE_SECTION, "Version", "1");
            _storageIni.Add(STORAGE_SECTION, "State", _currentState);
            Storage = _storageIni.ToString();
        }

        private void Debug(string message) {
            if (_debug != null) _debug.AppendLine(message);
            // Echo(message);
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo("$TITLE$ v$VERSION$ " + (updateSource == UpdateType.Update100 ? " " + _runningSymbol.GetSymbol() : ""));
            Echo(_instructions);
            Echo("");
            Echo($"Current State: {_currentState}");

            try {
                _config.Load();

                if (argument.Length > 0 && _commands.ContainsKey(argument))
                    _commands[argument]?.Invoke();

                // Run the operation queue
                var result = _operationQueue.Run();
                Runtime.UpdateFrequency = result ? UpdateFrequency.Update10 : UpdateFrequency.Once;

            } catch (Exception exc) {
                Debug($"Exception: {exc.Message}");
            } finally {
                _debug.UpdateDisplay();
            }
        }

        void CommandReset() {
            Debug("CommandReset invoked.");
            LoadBlocks();
            _currentState = MyDrillState.Resetting;

            _operationQueue.Clear();
            _operationQueue.Add(Action_StopMovement());
            _operationQueue.Add(Action_RetractPistons());
            _operationQueue.Add(Action_WaitForRetractionCompleted());
            _operationQueue.Add(Action_SetStatus(MyDrillState.Ready));
        }

        void CommandStart() {
            // Start the drills.
            // Extend each piston sequentially, starting from the piston closest to the drill.
            // Monitor the progress and adjust pistons as needed.
            // Monitor the inventory levels of the cargo containers and drills. Issue the stop command if necessary.
            // Stop the drills and retract pistons once the operation is complete.
            Debug("CommandStart invoked.");
            LoadBlocks();
            _currentState = MyDrillState.Drilling;

            _operationQueue.Clear();
            _operationQueue.Add(Action_StartAllDrills());
            _operationQueue.Add(Action_DrillingLoop());
            _operationQueue.Add(Action_RunCommand(CommandStop));
        }

        void CommandHold() {
            // Stop all drills.
            // Stop all pistons.
            // Monitor the inventory levels of the cargo containers/drills,
            // and command start once sufficient capacity is available.
            Debug("CommandHold invoked.");
            LoadBlocks();

            _operationQueue.Clear();
            _operationQueue.Add(Action_StopMovement());
            _operationQueue.Add(Action_SetStatus(MyDrillState.Holding));
        }

        void CommandStop() {
            Debug("CommandStop invoked.");
            LoadBlocks();
            _currentState = MyDrillState.Stopped;

            _operationQueue.Clear();
            _operationQueue.Add(Action_StopMovement());
            _operationQueue.Add(Action_RetractPistons()); // TODO: Retract pistons back to a configured distance.
            _operationQueue.Add(Action_WaitForRetractionCompleted());
            _operationQueue.Add(Action_SetStatus(MyDrillState.Ready));
        }

    }
}
