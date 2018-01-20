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

        bool _activateSpeedLimiter = false;

        readonly DebugLogging _debug;
        readonly Logging _log;
        readonly RunningSymbol _runSymbol;
        readonly COMMsModule _comms;
        readonly CustomDataConfig _custConfig;
        readonly ScriptSettings _settings;
        readonly BlocksByOrientation _orientation = new BlocksByOrientation();
        int _lastCustomDataHash;
        double _timeTransmitLast;

        // Block Lists
        bool _blocksLoaded = false;
        double _timeBlockReloadLast = 0;
        IMyRemoteControl _rc;
        IMyRadioAntenna _antenna;
        readonly List<IMyTerminalBlock> _tempList = new List<IMyTerminalBlock>();
        readonly List<IMyThrust> _ascentThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> _descentThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> _allThrusters = new List<IMyThrust>();
        readonly List<IMyShipConnector> _connectors = new List<IMyShipConnector>();
        readonly List<IMyLandingGear> _landingGears = new List<IMyLandingGear>();
        readonly List<IMyGasTank> _h2Tanks = new List<IMyGasTank>();
        readonly List<IMyDoor> _autoCloseDoors = new List<IMyDoor>();
        readonly List<IMyMotorStator> _boardingRamps = new List<IMyMotorStator>();
        readonly List<IMyTextPanel> _displaysAllCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllCarriagesWide = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllPassengerCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysAllPassengerCarriagesWide = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysSingleCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaysSingleCarriagesDetailed = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaySpeed = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displayDestination = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displayCargo = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displayFuel = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displayLog = new List<IMyTextPanel>();
        readonly List<IMyCargoContainer> _cargo = new List<IMyCargoContainer>();
        IMyGravityGenerator _gravityGen;
        readonly CarriageStatusMessage _status;
        bool _doCalcStatus = true;

        //  Flight calculations
        Vector3D _gravVec;
        double _gravMS2;
        bool _inNaturalGravity;
        double _actualMass;
        double _cargoMass;
        double _gravityForceOnShip;
        double _rangeToGround, _rangeToGroundLast, _rangeToSpace, _rangeToDestination;
        double _verticalSpeed;
        double _h2TankFilledPercent;

        GpsInfo _destination;
        TravelDirection _travelDirection = TravelDirection.None;
        bool _boardingRampsClear = false;


        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _debug = new DebugLogging(this);
            //_debug.Enabled = false;
            _debug.EchoMessages = true;

            _log = new Logging(20);

            _custConfig = new CustomDataConfig();
            _settings = new ScriptSettings();
            _settings.InitializeConfig(_custConfig);

            _lastCustomDataHash = -1;

            _runSymbol = new RunningSymbol();
            _comms = new COMMsModule(Me);
            _status = new CarriageStatusMessage();

            _mode_SpecialUseOnly = CarriageMode.Init;
            LoadState();

            Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;
        }
        void LoadState() {
            var states = Storage.Split('\t');
            if (states == null || states.Length != 3) return;
            _mode_SpecialUseOnly = states[0].ToEnum(defValue: CarriageMode.Init);
            _destination = (string.IsNullOrWhiteSpace(states[1])) ? null : new GpsInfo(states[1]);
            _travelDirection = states[2].ToEnum(defValue: TravelDirection.None);
        }
        public void Save() {
            Storage = $"{GetMode()}\t{_destination?.RawGPS}\t{_travelDirection}";
        }


        void LoadBlockLists(bool forceLoad = false) {
            if (_blocksLoaded && !forceLoad && _timeBlockReloadLast <= TIME_ReloadBlockDelay) return;

            _rc = CollectHelper.GetFirstblockOfTypeWithFirst<IMyRemoteControl>(GridTerminalSystem, _tempList,
                b => IsOnThisGrid(b) && IsTaggedCarriage(b),
                IsOnThisGrid);
            _antenna = CollectHelper.GetFirstblockOfTypeWithFirst<IMyRadioAntenna>(GridTerminalSystem, _tempList,
                b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsCommRadioAntenna(b),
                b => IsOnThisGrid(b) && Collect.IsCommRadioAntenna(b));
            _gravityGen = CollectHelper.GetFirstblockOfTypeWithFirst<IMyGravityGenerator>(GridTerminalSystem, _tempList,
                b => IsOnThisGrid(b) && IsTaggedCarriage(b),
                IsOnThisGrid);

            _orientation.Init(_rc);
            GridTerminalSystem.GetBlocksOfType(_ascentThrusters, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && _orientation.IsDown(b));
            GridTerminalSystem.GetBlocksOfType(_descentThrusters, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && _orientation.IsUp(b));
            GridTerminalSystem.GetBlocksOfType(_allThrusters, b => IsOnThisGrid(b) && IsTaggedCarriage(b));

            CollectHelper.GetblocksOfTypeWithFirst(GridTerminalSystem, _connectors,
                b => IsOnThisGrid(b) && IsTaggedCarriage(b),
                IsOnThisGrid);
            CollectHelper.GetblocksOfTypeWithFirst(GridTerminalSystem, _landingGears,
                b => IsOnThisGrid(b) && IsTaggedCarriage(b),
                IsOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_h2Tanks, b => IsOnThisGrid(b) && Collect.IsHydrogenTank(b));
            GridTerminalSystem.GetBlocksOfType(_cargo, IsOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriages, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.ALL_CARRIAGES));
            GridTerminalSystem.GetBlocksOfType(_displaysAllCarriagesWide, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.ALL_CARRIAGES_WIDE));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriages, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.ALL_PASSENGER_CARRIAGES));
            GridTerminalSystem.GetBlocksOfType(_displaysAllPassengerCarriagesWide, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE));

            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriages, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.SINGLE_CARRIAGE));
            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriagesDetailed, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.SINGLE_CARRIAGE_DETAIL));

            GridTerminalSystem.GetBlocksOfType(_displaySpeed, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.FLAT_SPEED) && Collect.IsCornerLcd(b));
            GridTerminalSystem.GetBlocksOfType(_displayDestination, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.FLAT_DESTINATION) && Collect.IsCornerLcd(b));
            GridTerminalSystem.GetBlocksOfType(_displayCargo, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.FLAT_CARGO) && Collect.IsCornerLcd(b));
            GridTerminalSystem.GetBlocksOfType(_displayFuel, b => IsOnThisGrid(b) && IsTaggedCarriage(b) && Collect.IsTagged(b, DisplayKeys.FLAT_FUEL) && Collect.IsCornerLcd(b));
            GridTerminalSystem.GetBlocksOfType(_displayLog, b => IsOnThisGrid(b) && b.CustomName == "LCD Panel - Control Log");

            GridTerminalSystem.GetBlocksOfType(_autoCloseDoors, b => IsOnThisGrid(b) && IsTaggedCarriage(b));

            GridTerminalSystem.GetBlocksOfType(_boardingRamps, b => IsOnThisGrid(b) && IsTaggedCarriage(b));

            _blocksLoaded = true;
            _timeBlockReloadLast = 0;
        }
        void EchoBlockLists() {
            //Echo($"Ascent Thrusters: {_ascentThrusters.Count}");
            //Echo($"Descent Thrusters: {_descentThrusters.Count}");
            //Echo($"Connectors: {_connectors.Count}");
            //Echo($"Locking Gears: {_landingGears.Count}");
            //Echo($"Ramp Rotors: {_boardingRamps.Count}");
            //Echo($"AirVents: {_airVents.Count}");
            //Echo($"O2 Tanks: {_o2Tanks.Count}");
            //Echo($"H2 Tanks: {_h2Tanks.Count}");
            //Echo($"Displays: {_displaysSingleCarriages.Count}");
            //Echo($"Cargo: {_cargo.Count}");
        }


    }
}
