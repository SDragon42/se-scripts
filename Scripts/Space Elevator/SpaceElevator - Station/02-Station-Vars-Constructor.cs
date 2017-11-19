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

        readonly CarriageVars _A1;
        readonly CarriageVars _A2;
        readonly CarriageVars _B1;
        readonly CarriageVars _B2;
        readonly CarriageVars _Maintenance;

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
        readonly List<IMyTerminalBlock> _tempList = new List<IMyTerminalBlock>();
        readonly List<IMyGasTank> _h2Tanks = new List<IMyGasTank>();
        readonly List<IMyTextPanel> _displaysAllCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllCarriagesWide = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllPassengerCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllPassengerCarriagesWide = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysSingleCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysSingleCarriagesDetailed = new List<IMyTextPanel>();

        readonly List<IMyDoor> _autoCloseDoors = new List<IMyDoor>();

        readonly List<IMyTerminalBlock> _gateBlocks = new List<IMyTerminalBlock>();
        readonly List<IMyTerminalBlock> _armLights = new List<IMyTerminalBlock>();
        readonly List<IMyTerminalBlock> _terminalDoors = new List<IMyTerminalBlock>();


        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _debug = new DebugModule(this);
            //_debug.Enabled = false;
            _debug.EchoMessages = true;

            _log = new LogModule(50);
            //_log.Enabled = false;

            _custConfig = new CustomDataConfigModule();
            _settings = new ScriptSettingsModule();
            _settings.InitConfig(_custConfig);

            _A1 = new CarriageVars("Carriage A1");
            _A2 = new CarriageVars("Carriage A2");
            _B1 = new CarriageVars("Carriage B1");
            _B2 = new CarriageVars("Carriage B2");
            _Maintenance = new CarriageVars("Maint Carriage");
            if (!string.IsNullOrWhiteSpace(Storage)) {
                var gateStates = Storage.Split('\n');
                if (gateStates.Length == 5) {
                    _A1.FromString(gateStates[0]);
                    _A2.FromString(gateStates[1]);
                    _B1.FromString(gateStates[2]);
                    _B2.FromString(gateStates[3]);
                    _Maintenance.FromString(gateStates[4]);
                }
            }

            _lastCustomDataHash = -1;

            _runSymbol = new RunningSymbolModule();
            _executionInterval = new TimeIntervalModule(0.1);
            _blockRefreshInterval = new TimeIntervalModule(1);

            _doorManager = new AutoDoorCloserModule();

            _comms = new COMMsModule(Me);
        }
        public void Save() {
            Storage = _A1.ToString() + "\n" +
                _A2.ToString() + "\n" +
                _B1.ToString() + "\n" +
                _B2.ToString() + "\n" +
                _Maintenance.ToString();
        }

        void LoadBlockLists(bool forceLoad = false) {
            if (_blocksLoaded && !forceLoad) return;

            _antenna = CollectHelper.GetFirstblockOfTypeWithFirst<IMyRadioAntenna>(GridTerminalSystem, _tempList, IsTaggedStationOnThisGrid, IsOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_h2Tanks, b => IsOnThisGrid(b) && Collect.IsHydrogenTank(b));
            GridTerminalSystem.GetBlocksOfType(_autoCloseDoors, b => IsTaggedStationOnThisGrid(b) && IsDoorOnStationOnly(b));
            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriages, b => IsTaggedStationOnThisGrid(b) && Displays.IsAllCarriagesDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriagesWide, b => IsTaggedStationOnThisGrid(b) && Displays.IsAllCarriagesWideDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriages, b => IsTaggedStationOnThisGrid(b) && Displays.IsAllPassengerCarriagesDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriagesWide, b => IsTaggedStationOnThisGrid(b) && Displays.IsAllPassengerCarriagesWideDisplay(b));

            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriages, b => IsTaggedStationOnThisGrid(b) && Displays.IsSingleCarriageDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriagesDetailed, b => IsTaggedStationOnThisGrid(b) && Displays.IsSingleCarriageDetailDisplay(b));

            _blocksLoaded = true;
        }
        void EchoBlockLists() {
            //Echo($"H2 Tanks: {_h2Tanks.Count}");
            //Echo($"Doors: {_autoCloseDoors.Count}");
            //Echo($"Displays (All Carr): {_displaysAllCarriages.Count}");
            //Echo($"Displays (W All Carr): {_displaysAllCarriagesWide.Count}");
            //Echo($"Displays (Pass Carr): {_displaysAllPassengerCarriages.Count}");
            //Echo($"Displays (W Pass Carr): {_displaysAllPassengerCarriagesWide.Count}");
            //Echo($"Displays (Single Carr): {_displaysSingleCarriages.Count}");
            //Echo($"Displays (Single Carr D): {_displaysSingleCarriagesDetailed.Count}");
            //Echo("");
        }

    }
}
