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

        const string ScriptTitle = "SDragons Torpedo Guidance Launcher";

        enum TorpedoSelectionMode { Random = 0, Closest = 1, Furthest = 2 }

        // Blocks
        readonly List<IMyRadioAntenna> guidanceBlocks = new List<IMyRadioAntenna>();
        readonly List<IMyBeacon> beaconBlocks = new List<IMyBeacon>();
        readonly List<IMyBatteryBlock> powerCellBlocks = new List<IMyBatteryBlock>();
        IMyTerminalBlock referenceBlock = null;

        // Command vars
        readonly IDictionary<TorpedoSelectionMode, Func<IMyRadioAntenna>> TorpedoSelection = new Dictionary<TorpedoSelectionMode, Func<IMyRadioAntenna>>();
        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();
        readonly string Instructions;
        readonly Random randomGenerator = new Random();

        // Config Settings
        string referenceTag = "Cockpit";
        string torpedoPrimaryTag = "Torpedo Payload Guidance";
        string torpedoSecondaryTag = string.Empty;
        string torpedoBeaconTag = "Torpedo Payload Beacon";
        string torpedoPowerCellTag = "Torp Power Cell";
        TorpedoSelectionMode selectionMode = TorpedoSelectionMode.Random;


        Action<string> Debug = (text) => { };

        public Program() {
            //Debug = Echo;

            Commands.Add("lock", Command_LockOnAll);
            Commands.Add("off", Command_TurnOffAll);
            Commands.Add("launch", Command_Launch);
            Commands.Add("trdm-on", Command_TargetRandomBlockOnAll);
            Commands.Add("trdm-off", Command_TargetRandomBlockOffAll);

            TorpedoSelection.Add(TorpedoSelectionMode.Random, SelectRandomTorpedo);
            TorpedoSelection.Add(TorpedoSelectionMode.Closest, SelectClosestTorpedo);
            TorpedoSelection.Add(TorpedoSelectionMode.Furthest, SelectFurthestTorpedo);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            Echo(ScriptTitle);
            Echo(Instructions);

            ProcessConfig();
        }

    }
}
