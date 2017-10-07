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

        double _connectorLockDelayRemaining = 0.0;
        bool _activateSpeedLimiter = false;

        readonly DebugModule _debug;
        readonly RunningSymbolModule _runSymbol;
        readonly TimeIntervalModule _executionInterval;
        readonly TimeIntervalModule _connectorLockDelay;
        readonly TimeIntervalModule _trasmitStatsDelay;
        readonly TimeIntervalModule _updateDisplayDelay;
        readonly AutoDoorCloserModule _doorManager;
        readonly COMMsModule _comms;
        readonly CustomDataConfigModule _custConfig;
        readonly ScriptSettingsModule _settings;
        BlocksByOrientation _orientation = new BlocksByOrientation();
        int _lastCustomDataHash;

        // Block Lists
        bool _blocksLoaded = false;
        IMyRemoteControl _rc;
        IMyRadioAntenna _antenna;
        readonly List<IMyTerminalBlock> _tempList = new List<IMyTerminalBlock>();
        readonly List<IMyThrust> _ascentThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> _descentThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> _allThrusters = new List<IMyThrust>();
        readonly List<IMyShipConnector> _connectors = new List<IMyShipConnector>();
        readonly List<IMyLandingGear> _landingGears = new List<IMyLandingGear>();
        //readonly List<IMyAirVent> _airVents = new List<IMyAirVent>();
        //readonly List<IMyGasTank> _o2Tanks = new List<IMyGasTank>();
        readonly List<IMyGasTank> _h2Tanks = new List<IMyGasTank>();
        readonly List<IMyDoor> _autoCloseDoors = new List<IMyDoor>();
        readonly List<IMyMotorStator> _boardingRamps = new List<IMyMotorStator>();
        readonly List<IMyTextPanel> _displaysSingleCarriages = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displaySpeed = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displayDestination = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displayCargo = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> _displayFuel = new List<IMyTextPanel>();
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
        float _h2TankFilledPercent;

        GpsInfo _destination;
        TravelDirection _travelDirection = TravelDirection.None;
        bool _boardingRampsClear = false;


        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _debug = new DebugModule(this);
            //_debug.Enabled = false;
            _debug.EchoMessages = false;

            _custConfig = new CustomDataConfigModule();
            _settings = new ScriptSettingsModule();
            _settings.InitializeConfig(_custConfig);

            _lastCustomDataHash = -1;

            _mode_SpecialUseOnly = Storage.ToEnum(defValue: CarriageMode.Manual_Control);

            _runSymbol = new RunningSymbolModule();
            _executionInterval = new TimeIntervalModule(0.1);
            _connectorLockDelay = new TimeIntervalModule(0.1);
            _trasmitStatsDelay = new TimeIntervalModule(3.0);
            _updateDisplayDelay = new TimeIntervalModule(1.0);

            _doorManager = new AutoDoorCloserModule();

            _comms = new COMMsModule(Me);

            _status = new CarriageStatusMessage(GetMode(), Vector3D.Zero, 0, 0, 0, 0, 0);
        }
        public void Save() {
            Storage = GetMode().ToString();
        }


        void LoadBlockLists(bool forceLoad = false) {
            if (_blocksLoaded && !forceLoad) return;

            _rc = CollectHelper.GetFirstblockOfTypeWithFirst<IMyRemoteControl>(GridTerminalSystem, _tempList, IsTaggedBlockOnThisGrid, IsOnThisGrid);
            _antenna = CollectHelper.GetFirstblockOfTypeWithFirst<IMyRadioAntenna>(GridTerminalSystem, _tempList, IsTaggedBlockOnThisGrid, IsOnThisGrid);
            _gravityGen = CollectHelper.GetFirstblockOfTypeWithFirst<IMyGravityGenerator>(GridTerminalSystem, _tempList, IsTaggedBlockOnThisGrid, IsOnThisGrid);

            _orientation.Init(_rc);
            GridTerminalSystem.GetBlocksOfType(_ascentThrusters, b => IsTaggedBlockOnThisGrid(b) && _orientation.IsDown(b));
            GridTerminalSystem.GetBlocksOfType(_descentThrusters, b => IsTaggedBlockOnThisGrid(b) && _orientation.IsUp(b));
            GridTerminalSystem.GetBlocksOfType(_allThrusters, IsOnThisGrid);

            CollectHelper.GetblocksOfTypeWithFirst(GridTerminalSystem, _connectors, IsTaggedBlockOnThisGrid, IsOnThisGrid);
            CollectHelper.GetblocksOfTypeWithFirst(GridTerminalSystem, _landingGears, IsTaggedBlockOnThisGrid, IsOnThisGrid);
            //GridTerminalSystem.GetBlocksOfType(_airVents, IsTaggedBlockOnThisGrid);
            //GridTerminalSystem.GetBlocksOfType(_o2Tanks, b => IsTaggedBlockOnThisGrid(b) && IsOxygenTank(b));

            GridTerminalSystem.GetBlocksOfType(_h2Tanks, b => IsOnThisGrid(b) && Collect.IsHydrogenTank(b));
            GridTerminalSystem.GetBlocksOfType(_cargo, IsOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_displaysSingleCarriages, b => IsTaggedBlockOnThisGrid(b) && Displays.IsSingleCarriageDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displaySpeed, b => IsTaggedBlockOnThisGrid(b) && Displays.IsSlimSpeedDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displayDestination, b => IsTaggedBlockOnThisGrid(b) && Displays.IsSlimDestDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displayCargo, b => IsTaggedBlockOnThisGrid(b) && Displays.IsSlimCargoDisplay(b));
            GridTerminalSystem.GetBlocksOfType(_displayFuel, b => IsTaggedBlockOnThisGrid(b) && Displays.IsSlimFuelDisplay(b));

            GridTerminalSystem.GetBlocksOfType(_autoCloseDoors, IsTaggedBlockOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_boardingRamps, IsTaggedBlockOnThisGrid);

            _blocksLoaded = true;
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
            Echo($"Cargo: {_cargo.Count}");
        }


    }
}
