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

        const string SCRIPT_TITLE = "SDragons Torpedo Guidance Launcher";

        enum TorpedoSelectionMode { Random = 0, Closest = 1, Furthest = 2 }

        Config _config = new Config();

        // Blocks
        readonly List<IMyRadioAntenna> _guidanceBlocks = new List<IMyRadioAntenna>();
        readonly List<IMyBeacon> _beaconBlocks = new List<IMyBeacon>();
        readonly List<IMyBatteryBlock> _powerCellBlocks = new List<IMyBatteryBlock>();
        IMyTerminalBlock _referenceBlock = null;

        // Command vars
        readonly IDictionary<TorpedoSelectionMode, Func<IMyRadioAntenna>> _torpedoSelection = new Dictionary<TorpedoSelectionMode, Func<IMyRadioAntenna>>();
        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>();
        readonly string _instructions;
        readonly Random _randomGenerator = new Random();

        string torpedoSecondaryTag = string.Empty;


        Action<string> Debug = (text) => { };

        public Program() {
            _commands.Add("lock", Command_LockOnAll);
            _commands.Add("off", Command_TurnOffAll);
            _commands.Add("launch", Command_Launch);
            _commands.Add("trdm-on", Command_TargetRandomBlockOnAll);
            _commands.Add("trdm-off", Command_TargetRandomBlockOffAll);

            _torpedoSelection.Add(TorpedoSelectionMode.Random, SelectRandomTorpedo);
            _torpedoSelection.Add(TorpedoSelectionMode.Closest, SelectClosestTorpedo);
            _torpedoSelection.Add(TorpedoSelectionMode.Furthest, SelectFurthestTorpedo);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in _commands.Keys) sb.AppendLine(c);
            _instructions = sb.ToString();

            Echo(SCRIPT_TITLE);
            Echo(_instructions);

            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();
        }

    }
}
