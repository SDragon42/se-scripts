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
        StateMachineQueue _operationQueue = new StateMachineQueue();

        readonly MyIni _storageIni = new MyIni();

        public Program() {
            _commands.Add("reset", null);
            _commands.Add("start", null);
            _commands.Add("stop", null);

            // Initialize the configuration
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();

            // Load the current state from storage
            // var state = new StateConfig(Storage);
            if (_storageIni.TryParse(Storage)) {
                var version = _storageIni.Get("", "Version").ToString();
                switch (version) {
                    case "1":
                        // Handle version 1 if needed
                        var stateString = _storageIni.Get("", "State").ToString();
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
            _storageIni.Add("", "Version", "1");
            _storageIni.Add("", "State", _currentState);
            Storage = _storageIni.ToString();
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo("Tunnel Drill Control" + (updateSource == UpdateType.Update100 ? " " + _runningSymbol.GetSymbol() : ""));
            Echo($"Current State: {_currentState}");

            _config.Load();

            if (argument.Length > 0 && _commands.ContainsKey(argument)) 
                _commands[argument]?.Invoke();

            // Run the operation queue
            _operationQueue.Run();
        }


        void LoadBlocks() {
            // In Operations On/Off blocks
            GridTerminalSystem.GetBlocksOfType(_OnOffBlocks, b => IsMine(b) && !(b is IMyPistonBase));

            // Piston Blocks
            _pistons.Clear();
            IMyPistonBase previousPiston = null;
            var piston = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyPistonBase>(IsMine);
            if (piston != null) {
                _pistons.Add(piston);
                var rootPistonName = piston.CustomName.Replace(_config.TagName, "").Trim();
                var i = 0;
                // Traverse the piston chain and rename them sequentially
                while (piston != null) {
                    previousPiston = piston;
                    piston = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyPistonBase>(b => piston.TopGrid == b.CubeGrid);
                    if (piston != null) {
                        i++;
                        piston.CustomName = $"{rootPistonName} {i:00}";
                        _pistons.Add(piston);
                    }
                }
            }

            // Drill Blocks
            _drills.Clear();
            if (previousPiston != null)
                GridTerminalSystem.GetBlocksOfType(_drills, b => b.CubeGrid == previousPiston.TopGrid);

            // Cargo Container Blocks
            GridTerminalSystem.GetBlocksOfType(_cargoContainers, IsMine);
        }

        bool IsMine(IMyTerminalBlock b) => b.IsSameConstructAs(Me) && IsTagged(b, _config.TagName);


        void CommandReset() {
            // Implement reset logic here
        }

        void CommandStart() {
            // Implement start logic here
        }

        void CommandStop() {
            // Implement stop logic here
        }

    }
}
