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

        readonly CustomDataConfigModule _custConfig;
        readonly ScriptSettingsModule _settings;
        readonly DebugModule _debug;
        readonly LogModule _log;
        readonly COMMsModule _comms;
        readonly RunningSymbolModule _runSymbol;
        readonly TimeIntervalModule _executionInterval;
        readonly TimeIntervalModule _blockRefreshInterval;
        readonly AutoDoorCloserModule _doorManager;



        bool _blocksLoaded = false;
        int _lastCustomDataHash;

        IMyRadioAntenna _antenna;
        readonly List<IMyTextPanel> _displaysAllCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllCarriagesWide = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllPassengerCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllPassengerCarriagesWide = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysSingleCarriages = new List<IMyTextPanel>();
        readonly List<IMyDoor> _autoCloseDoors = new List<IMyDoor>();
        readonly List<IMyTerminalBlock> _tempList = new List<IMyTerminalBlock>();

        readonly Dictionary<string, string> _displayText = new Dictionary<string, string>();
        readonly Dictionary<string, CarriageStatusMessage> _carriageStatuses = new Dictionary<string, CarriageStatusMessage>();

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _debug = new DebugModule(this);
            //_debug.Enabled = false;
            _debug.EchoMessages = true;

            _log = new LogModule(20);

            _custConfig = new CustomDataConfigModule();
            _settings = new ScriptSettingsModule();
            _settings.InitConfig(_custConfig);

            _lastCustomDataHash = -1;

            _runSymbol = new RunningSymbolModule();
            _executionInterval = new TimeIntervalModule(0.1);
            _blockRefreshInterval = new TimeIntervalModule(1);

            _doorManager = new AutoDoorCloserModule();

            _comms = new COMMsModule(Me);

            _displayText[Displays.DISPLAY_KEY_ALL_CARRIAGES] = "";
            _displayText[Displays.DISPLAY_KEY_ALL_CARRIAGES_WIDE] = "";
            _displayText[Displays.DISPLAY_KEY_ALL_PASSENGER_CARRIAGES] = "";
            _displayText[Displays.DISPLAY_KEY_ALL_PASSENGER_CARRIAGES_WIDE] = "";
            _displayText[Displays.DISPLAY_KEY_SINGLE_CARRIAGE] = "";
            _displayText[Displays.DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL] = "";

            _carriageStatuses[CARRIAGE_A1] = null;
            _carriageStatuses[CARRIAGE_A2] = null;
            _carriageStatuses[CARRIAGE_B1] = null;
            _carriageStatuses[CARRIAGE_B2] = null;
            _carriageStatuses[CARRIAGE_MAINT] = null;
        }

        public void Save() {
        }

        void LoadBlockLists(bool forceLoad = false) {
            if (_blocksLoaded && !forceLoad) return;

            _antenna = CollectHelper.GetFirstblockOfTypeWithFirst<IMyRadioAntenna>(GridTerminalSystem, _tempList, b => IsOnThisGrid(b) && IsTaggedStation(b), IsOnThisGrid);
            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Displays.IsAllCarriagesDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriagesWide, b => IsOnThisGrid(b) && IsTaggedStation(b) && Displays.IsAllCarriagesWideDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Displays.IsAllPassengerCarriagesDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriagesWide, b => IsOnThisGrid(b) && IsTaggedStation(b) && Displays.IsAllPassengerCarriagesWideDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Displays.IsSingleCarriageDetailDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_autoCloseDoors, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsHumanDoor(b));

            _blocksLoaded = true;
        }
        void EchoBlockLists() {
            Echo($"Doors: {_autoCloseDoors.Count}");
            Echo($"Displays (All Carr): {_displaysAllCarriages.Count}");
            Echo($"Displays (W All Carr): {_displaysAllCarriagesWide.Count}");
            Echo($"Displays (Pass Carr): {_displaysAllPassengerCarriages.Count}");
            Echo($"Displays (W Pass Carr): {_displaysAllPassengerCarriagesWide.Count}");
            Echo($"Displays (Single Carr): {_displaysSingleCarriages.Count}");
        }


        static string MakeDisplayKey(string carriageKey, string displayKey) { return $"{carriageKey}|{displayKey}"; }

        string GetDisplayText(string carriageKey, string displayKey) {
            var cKey = MakeDisplayKey(carriageKey, displayKey);
            return _displayText.ContainsKey(cKey) ? _displayText[cKey] : string.Empty;
        }
        void SetDisplayText(string carriageKey, string displayKey, string text) {
            var currText = GetDisplayText(carriageKey, displayKey);
            if (string.Compare(currText, text) == 0) return;
            var cKey = MakeDisplayKey(carriageKey, displayKey);
            _displayText[cKey] = text;
            SendCOMMs_DisplayUpdate(carriageKey, displayKey, _displayText[cKey]);
        }

    }
}
