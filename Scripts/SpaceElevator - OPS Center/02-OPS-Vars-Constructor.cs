using Sandbox.Game.EntityComponents;
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

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            //_debug = new DebugLogging(this);
            //_debug.Enabled = false;
            //_debug.EchoMessages = true;

            _log.Enabled = false;

            _settings.InitConfig(_custConfig);

            _lastCustomDataHash = -1;

            _comms = new COMMsModule(Me);

            _displayText[DisplayKeys.ALL_CARRIAGES] = "";
            _displayText[DisplayKeys.ALL_CARRIAGES_WIDE] = "";
            _displayText[DisplayKeys.ALL_PASSENGER_CARRIAGES] = "";
            _displayText[DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE] = "";
            _displayText[DisplayKeys.SINGLE_CARRIAGE] = "";
            _displayText[DisplayKeys.SINGLE_CARRIAGE_DETAIL] = "";

            GridNameConstants.AllCarriages.ForEach(c => _carriageStatuses[c] = null);

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }


        readonly CustomDataConfig _custConfig = new CustomDataConfig();
        readonly ScriptSettings _settings = new ScriptSettings();
        //readonly DebugLogging _debug;
        readonly Logging _log = new Logging(ScriptSettings.DEF_NumLogLines);
        readonly COMMsModule _comms;
        readonly RunningSymbol _runSymbol = new RunningSymbol();



        double _timeBlockReloadLast = TIME_ReloadBlockDelay * 2;
        int _lastCustomDataHash;
        //double _timeLast;
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



        public void Save() {
        }

        void LoadBlockLists(bool forceLoad = false) {
            if (!forceLoad && _timeBlockReloadLast < TIME_ReloadBlockDelay) return;

            _antenna = GridTerminalSystem.GetFirstblockOfTypeWithFirst<IMyRadioAntenna>(_tempList,
                b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsCommRadioAntenna(b),
                b => IsOnThisGrid(b) && Collect.IsCommRadioAntenna(b));

            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_CARRIAGES));
            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriagesWide, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_CARRIAGES_WIDE));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_PASSENGER_CARRIAGES));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriagesWide, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE));
            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.SINGLE_CARRIAGE_DETAIL));
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
