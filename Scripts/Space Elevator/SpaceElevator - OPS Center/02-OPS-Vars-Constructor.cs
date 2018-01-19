﻿using Sandbox.Game.EntityComponents;
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
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        readonly CustomDataConfig _custConfig;
        readonly ScriptSettings _settings;
        readonly DebugLogging _debug;
        readonly Logging _log;
        readonly COMMsModule _comms;
        readonly RunningSymbol _runSymbol;



        bool _blocksLoaded = false;
        int _lastCustomDataHash;
        double _timeLast;
        double _timeDisplayLast;

        IMyRadioAntenna _antenna;
        readonly List<IMyTextPanel> _displaysAllCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllCarriagesWide = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllPassengerCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllPassengerCarriagesWide = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysSingleCarriages = new List<IMyTextPanel>();
        readonly List<IMyTerminalBlock> _tempList = new List<IMyTerminalBlock>();

        readonly Dictionary<string, string> _displayText = new Dictionary<string, string>();
        readonly Dictionary<string, CarriageStatusMessage> _carriageStatuses = new Dictionary<string, CarriageStatusMessage>();

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _debug = new DebugLogging(this);
            //_debug.Enabled = false;
            _debug.EchoMessages = true;

            _log = new Logging(20);

            _custConfig = new CustomDataConfig();
            _settings = new ScriptSettings();
            _settings.InitConfig(_custConfig);

            _lastCustomDataHash = -1;

            _runSymbol = new RunningSymbol();
            _comms = new COMMsModule(Me);

            _displayText[DisplayKeys.ALL_CARRIAGES] = "";
            _displayText[DisplayKeys.ALL_CARRIAGES_WIDE] = "";
            _displayText[DisplayKeys.ALL_PASSENGER_CARRIAGES] = "";
            _displayText[DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE] = "";
            _displayText[DisplayKeys.SINGLE_CARRIAGE] = "";
            _displayText[DisplayKeys.SINGLE_CARRIAGE_DETAIL] = "";

            GridNameConstants.AllCarriages.ForEach(c => _carriageStatuses[c] = null);

            Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;
        }

        public void Save() {
        }

        void LoadBlockLists(bool forceLoad = false) {
            if (_blocksLoaded && !forceLoad) return;

            _antenna = CollectHelper.GetFirstblockOfTypeWithFirst<IMyRadioAntenna>(GridTerminalSystem, _tempList,
                b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsCommRadioAntenna(b),
                b => IsOnThisGrid(b) && Collect.IsCommRadioAntenna(b));

            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_CARRIAGES));
            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriagesWide, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_CARRIAGES_WIDE));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_PASSENGER_CARRIAGES));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriagesWide, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE));
            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.SINGLE_CARRIAGE_DETAIL));

            _blocksLoaded = true;
        }
        void EchoBlockLists() {
            //Echo($"Displays (All Carr): {_displaysAllCarriages.Count}");
            //Echo($"Displays (W All Carr): {_displaysAllCarriagesWide.Count}");
            //Echo($"Displays (Pass Carr): {_displaysAllPassengerCarriages.Count}");
            //Echo($"Displays (W Pass Carr): {_displaysAllPassengerCarriagesWide.Count}");
            //Echo($"Displays (Single Carr): {_displaysSingleCarriages.Count}");
        }


        string GetDisplayText(string displayKey) {
            return _displayText.ContainsKey(displayKey) ? _displayText[displayKey] : string.Empty;
        }
        void SetDisplayText(string displayKey, string text) {
            var currText = GetDisplayText(displayKey);
            if (string.Compare(currText, text) == 0) return;
            _displayText[displayKey] = text;
        }

    }
}
