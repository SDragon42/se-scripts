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

        readonly CustomDataConfig _custConfig;
        readonly ScriptSettings _settings;

        readonly CarriageVars _A1;
        readonly CarriageVars _A2;
        readonly CarriageVars _B1;
        readonly CarriageVars _B2;
        readonly CarriageVars _Maint;

        readonly DebugLogging _debug;
        readonly Logging _log;
        readonly COMMsModule _comms;
        readonly RunningSymbol _runSymbol;
        readonly AutoDoorCloser _doorManager;

        bool _blocksLoaded = false;
        int _lastCustomDataHash;
        double _timeLast;

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
        readonly List<IMyDoor> _terminalDoors = new List<IMyDoor>();


        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _debug = new DebugLogging(this);
            //_debug.Enabled = false;
            _debug.EchoMessages = true;

            _log = new Logging(50);
            //_log.Enabled = false;

            _custConfig = new CustomDataConfig();
            _settings = new ScriptSettings();
            _settings.InitConfig(_custConfig);

            _A1 = new CarriageVars(GridNameConstants.A1);
            _A2 = new CarriageVars(GridNameConstants.A2);
            _B1 = new CarriageVars(GridNameConstants.B1);
            _B2 = new CarriageVars(GridNameConstants.B2);
            _Maint = new CarriageVars(GridNameConstants.MAINT);
            if (!string.IsNullOrWhiteSpace(Storage)) {
                var gateStates = Storage.Split('\n');
                if (gateStates.Length == 5) {
                    _A1.FromString(gateStates[0]);
                    _A2.FromString(gateStates[1]);
                    _B1.FromString(gateStates[2]);
                    _B2.FromString(gateStates[3]);
                    _Maint.FromString(gateStates[4]);
                }
            }

            _lastCustomDataHash = -1;

            _runSymbol = new RunningSymbol();
            _doorManager = new AutoDoorCloser();
            _comms = new COMMsModule(Me);

            Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;
        }
        public void Save() {
            Storage = _A1.ToString() + "\n" +
                _A2.ToString() + "\n" +
                _B1.ToString() + "\n" +
                _B2.ToString() + "\n" +
                _Maint.ToString();
        }

        void LoadBlockLists(bool forceLoad = false) {
            if (_blocksLoaded && !forceLoad) return;

            _antenna = CollectHelper.GetFirstblockOfTypeWithFirst<IMyRadioAntenna>(GridTerminalSystem, _tempList,
                b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsCommRadioAntenna(b),
                b => IsOnThisGrid(b) && Collect.IsCommRadioAntenna(b));

            GridTerminalSystem.GetBlocksOfType(_h2Tanks, b => IsOnThisGrid(b) && Collect.IsHydrogenTank(b));
            GridTerminalSystem.GetBlocksOfType(_autoCloseDoors, b => IsOnThisGrid(b) && IsDoorOnStationOnly(b));
            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_CARRIAGES));
            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriagesWide, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_CARRIAGES_WIDE));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_PASSENGER_CARRIAGES));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriagesWide, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE));

            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriages, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.SINGLE_CARRIAGE));
            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriagesDetailed, b => IsOnThisGrid(b) && IsTaggedStation(b) && Collect.IsTagged(b, DisplayKeys.SINGLE_CARRIAGE_DETAIL));

            _blocksLoaded = true;
        }
        void EchoBlockLists() {
            //Echo($"Antenna: {_antenna?.CustomName} | E:{_antenna?.Enabled}  B:{_antenna?.EnableBroadcasting}");
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
