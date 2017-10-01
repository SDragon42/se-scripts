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
    partial class Program : MyGridProgram {
        //-------------------------------------------------------------------------------
        //  SCRIPT COMMANDS
        //-------------------------------------------------------------------------------
        const string CMD_Reset = "reset";
        const string CMD_Goto = "goto";


        //-------------------------------------------------------------------------------
        //  CONSTANTS
        //-------------------------------------------------------------------------------
        const double SWITCH_TO_AUTOPILOT_RANGE = 1;
        const double DOCKED_AT_STATION_RANGE = 25.0;

        const float GRAV_RANGE_RampsDown = 42.5f;
        const float GRAV_RANGE_Rampsup = 12.5f;


        //-------------------------------------------------------------------------------
        //  VARIABLES
        //-------------------------------------------------------------------------------
        double _connectorLockDelayRemaining = 0.0;
        bool _activateSpeedLimiter = false;

        readonly DebugModule _debug;
        readonly RunningSymbolModule _runSymbol;
        readonly TimeIntervalModule _executionInterval;
        readonly TimeIntervalModule _connectorLockDelay;
        readonly TimeIntervalModule _trasmitStatsDelay;
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
        //readonly List<IMyTextPanel> _displays = new List<IMyTextPanel>();
        readonly List<IMyDoor> _autoCloseDoors = new List<IMyDoor>();
        readonly List<IMyMotorStator> _maintCarrageRamps = new List<IMyMotorStator>();
        IMyGravityGenerator _maintGravGen;

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
        bool _rampsClear = false;


        //-------------------------------------------------------------------------------
        //  MAIN CONTROL FLOW
        //-------------------------------------------------------------------------------
        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _debug = new DebugModule(this);
            //_debug.Enabled = false;
            _debug.EchoMessages = false;

            _custConfig = new CustomDataConfigModule();
            _settings = new ScriptSettingsModule();
            _settings.InitializeConfig(_custConfig);

            _lastCustomDataHash = -1;

            //_mode_SpecialUseOnly = (!string.IsNullOrWhiteSpace(Storage)) ? Storage : CarriageMode.Manual_Control;
            _mode_SpecialUseOnly = CarriageModeHelper.GetFromString(Storage);

            _runSymbol = new RunningSymbolModule();
            _executionInterval = new TimeIntervalModule(0.1);
            _connectorLockDelay = new TimeIntervalModule(0.1);
            _trasmitStatsDelay = new TimeIntervalModule(3.0);

            _doorManager = new AutoDoorCloserModule();

            _comms = new COMMsModule(Me);
        }

        public void Save() {
            Storage = GetMode().ToString();
        }

        public void Main(string argument) {
            try {
                Echo("Maint. Carriage Control v1.8 " + _runSymbol.GetSymbol(Runtime));
                Echo("DEBUG " + (_debug.Enabled ? "enabled" : "disabled"));

                _executionInterval.RecordTime(Runtime);
                _connectorLockDelay.RecordTime(Runtime);
                _trasmitStatsDelay.RecordTime(Runtime);

                LoadConfigSettings();
                LoadBlockLists();
                EchoBlockLists();

                if (!string.IsNullOrEmpty(argument)) {
                    RunCommand(argument);
                }

                if (_executionInterval.AtNextInterval) {
                    _debug.Clear();
                    LoadCalculations();
                    RunModeActions();
                    SaveLastValues();
                    _comms.TransmitQueue(_antenna);
                    _doorManager.CloseOpenDoors(_executionInterval.Time, _autoCloseDoors);
                    if (_maintGravGen != null)
                        _maintGravGen.Enabled = (_gravVec.Length() < 9.81 / 2);
                }

                if (_trasmitStatsDelay.AtNextInterval) {
                    SendStatsMessage();
                }

            } catch (Exception ex) {
                _debug.AppendLine(ex.Message);
                _debug.AppendLine(ex.StackTrace);
                throw ex;
            } finally {
                _debug.UpdateDisplay();
            }
        }


        void LoadBlockLists(bool forceLoad = false) {
            if (_blocksLoaded && !forceLoad) return;

            CollectHelper.GetblocksOfTypeWithFirst<IMyRemoteControl>(GridTerminalSystem, _tempList, IsTaggedBlockOnThisGrid, IsOnThisGrid);
            if (_tempList.Count > 0)
                _rc = (IMyRemoteControl)_tempList[0];

            CollectHelper.GetblocksOfTypeWithFirst<IMyRadioAntenna>(GridTerminalSystem, _tempList, IsTaggedBlockOnThisGrid, IsOnThisGrid);
            if (_tempList.Count > 0)
                _antenna = (IMyRadioAntenna)_tempList[0];

            _orientation.Init(_rc);
            GridTerminalSystem.GetBlocksOfType(_ascentThrusters, b => IsTaggedBlockOnThisGrid(b) && _orientation.IsDown(b));
            GridTerminalSystem.GetBlocksOfType(_descentThrusters, b => IsTaggedBlockOnThisGrid(b) && _orientation.IsUp(b));
            GridTerminalSystem.GetBlocksOfType(_allThrusters, IsOnThisGrid);

            CollectHelper.GetblocksOfTypeWithFirst(GridTerminalSystem, _connectors, IsTaggedBlockOnThisGrid, IsOnThisGrid);
            CollectHelper.GetblocksOfTypeWithFirst(GridTerminalSystem, _landingGears, IsTaggedBlockOnThisGrid, IsOnThisGrid);
            //GridTerminalSystem.GetBlocksOfType(_airVents, IsTaggedBlockOnThisGrid);
            //GridTerminalSystem.GetBlocksOfType(_o2Tanks, b => IsTaggedBlockOnThisGrid(b) && IsOxygenTank(b));

            GridTerminalSystem.GetBlocksOfType(_h2Tanks, b => IsOnThisGrid(b) && Collect.IsHydrogenTank(b));
            //GridTerminalSystem.GetBlocksOfType(_displays, IsTaggedBlockOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_autoCloseDoors, IsTaggedBlockOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_maintCarrageRamps, IsTaggedBlockOnThisGrid);

            CollectHelper.GetblocksOfTypeWithFirst<IMyGravityGenerator>(GridTerminalSystem, _tempList, IsTaggedBlockOnThisGrid, IsOnThisGrid);
            if (_tempList.Count > 0)
                _maintGravGen = (IMyGravityGenerator)_tempList[0];

            _blocksLoaded = true;
        }
        void EchoBlockLists() {
            Echo($"Ascent Thrusters: {_ascentThrusters.Count}");
            Echo($"Descent Thrusters: {_descentThrusters.Count}");
            Echo($"Connectors: {_connectors.Count}");
            Echo($"Locking Gears: {_landingGears.Count}");
            Echo($"Ramp Rotors: {_maintCarrageRamps.Count}");
            //Echo($"AirVents: {_airVents.Count}");
            //Echo($"O2 Tanks: {_o2Tanks.Count}");
            //Echo($"H2 Tanks: {_h2Tanks.Count}");
            //Echo($"Displays: {_displays.Count}");
        }

        //-------------------------------------------------------------------------------
        //  Flight calculations
        //-------------------------------------------------------------------------------
        void LoadCalculations() {
            _gravVec = _rc.GetNaturalGravity();
            //gravity in m/s^2
            _gravMS2 = Math.Sqrt(
                Math.Pow(_gravVec.X, 2) +
                Math.Pow(_gravVec.Y, 2) +
                Math.Pow(_gravVec.Z, 2));

            _inNaturalGravity = (_gravMS2 > 0.0);

            // carriage total mass including cargo mass
            var totalMass = _rc.CalculateShipMass().TotalMass;
            // mass of the carriage without cargo
            var baseMass = _rc.CalculateShipMass().BaseMass;
            _cargoMass = totalMass - baseMass;
            // the mass the game uses for physics calculation
            _actualMass = baseMass + (_cargoMass / _settings.InventoryMultiplier);
            // the gravity "thrust" applied to the carriage
            _gravityForceOnShip = _actualMass * _gravMS2;

            var pos = _rc.GetPosition();
            if (_destination != null)
                _rangeToDestination = Vector3D.Distance(pos, _destination.GetLocation());
            _rangeToGround = Vector3D.Distance(pos, _settings.GetBottomPoint());
            _rangeToSpace = Vector3D.Distance(pos, _settings.GetTopPoint());

            _verticalSpeed = ((_rangeToGround - _rangeToGroundLast) >= 0)
                ? _rc.GetShipSpeed()
                : _rc.GetShipSpeed() * -1;
            var totalMaxBreakingThrust = _ascentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));
            var brakeingRange = CalcBrakeDistance(totalMaxBreakingThrust, _gravityForceOnShip);

            _h2TankFilledPercent = GasTankHelper.GetTanksFillPercentage(_h2Tanks);

            var speed = Math.Round(_verticalSpeed, 1);
            var speedDir = "--";
            if (speed > 0) speedDir = "/\\";
            if (speed < 0) speedDir = "\\/";

            _debug.AppendLine("Speed: {1}  {0:N1}", Math.Abs(_verticalSpeed), speedDir);
            _debug.AppendLine("Lift T/W r: {0:N2}", totalMaxBreakingThrust / _gravityForceOnShip);
            _debug.AppendLine("Brake Dist: {0:N2}", brakeingRange);
            _debug.AppendLine("");
            if (_destination != null)
                _debug.AppendLine("Range to destination: {0:N2} m", _rangeToDestination);
            _debug.AppendLine("Range to Ground: {0:N2} m", _rangeToGround);
            _debug.AppendLine("MODE: {0}", GetMode());
        }
        void SaveLastValues() {
            _rangeToGroundLast = _rangeToGround;
        }

        //-------------------------------------------------------------------------------
        //  Custom Config
        //-------------------------------------------------------------------------------
        void LoadConfigSettings() {
            var hash = Me.CustomData.GetHashCode();
            if (hash == _lastCustomDataHash) return;
            _custConfig.ReadFromCustomData(Me);
            _settings.LoadFromSettingDict(_custConfig);
            _custConfig.SaveToCustomData(Me);
            _lastCustomDataHash = hash;
            _connectorLockDelay.SetInterval(_settings.ConnectorLockDelay);
            _doorManager.SecondsToLeaveOpen = _settings.DoorCloseDelay;
        }


        //-------------------------------------------------------------------------------
        //  COMMs
        //-------------------------------------------------------------------------------
        void SendStatsMessage() {
            if (!_settings.SendStatusMessages) return;
            if (_antenna == null) return;
            //if (!_trasmitStatsDelay.AtNextInterval()) return;

            var payload = new CarriageStatusMessage(
                GetMode().ToString(),
                _rc.GetPosition(),
                _verticalSpeed,
                _h2TankFilledPercent,
                _cargoMass,
                _rangeToGround,
                _rangeToSpace);
            _comms.AddMessageToQueue(payload);
        }
        void SendDockedMessage(string stationName) {
            var payload = new CarriageRequestMessage(Me.CubeGrid.CustomName, CarriageRequestMessage.REQUEST_DOCK);
            _comms.AddMessageToQueue(payload, stationName);
        }
        void SendRequestDepartureClearance(string stationName) {
            var payload = new CarriageRequestMessage(Me.CubeGrid.CustomName, CarriageRequestMessage.REQUEST_DEPART);
            _comms.AddMessageToQueue(payload, stationName);
        }

        //-------------------------------------------------------------------------------
        //  COMMAND OPERATIONS
        //-------------------------------------------------------------------------------
        void RunCommand(string argument) {
            CommMessage msg = null;
            if (CommMessage.TryParse(argument, out msg)) {
                // COMMs messages
                switch (msg.PayloadType) {
                    case StationResponseMessage.TYPE:
                        var responseMsg = StationResponseMessage.CreateFromPayload(msg.Payload);
                        if (responseMsg?.Response == StationResponseMessage.RESPONSE_DEPARTURE_OK)
                            SetMode(CarriageMode.Awaiting_CarriageReady2Depart);
                        break;
                    case SendCarriageToMessage.TYPE:
                        var sendToMsg = SendCarriageToMessage.CreateFromPayload(msg.Payload);
                        if (sendToMsg == null) break;
                        var destination = _settings.GetGpsInfo(sendToMsg.Destination);
                        SetDeparture(destination);
                        break;
                }
                return;
            }

            var cmdParts = argument.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            switch (cmdParts[0]) {
                case CMD_Reset:
                    LoadBlockLists(true);
                    SetMode(CarriageMode.Manual_Control);
                    break;

                case CMD_Goto:
                    if (cmdParts.Length != 2) return;
                    var destination = _settings.GetGpsInfo(cmdParts[1]);
                    SetDeparture(destination);
                    break;
            }
        }

        void SetDeparture(GpsInfo destination) {
            _destination = null;
            _travelDirection = TravelDirection.None;

            var dockedStation = GetDockedPoint(DOCKED_AT_STATION_RANGE);
            if (dockedStation != null && destination != null && string.Compare(destination.GetName(), dockedStation.GetName(), true) == 0) return;

            _destination = destination;
            if (_destination == null) return;

            var bottom = _settings.GetBottomPoint();
            var myDist = (bottom - _rc.GetPosition()).Length();
            var destDist = (bottom - _destination.GetLocation()).Length();
            _travelDirection = (myDist < destDist)
                ? TravelDirection.Ascent
                : TravelDirection.Descent;

            RaiseRamps();
            if (dockedStation == null || !dockedStation.GetNeedsClearance())
                SetMode(CarriageMode.Awaiting_CarriageReady2Depart);
            else {
                SetMode(CarriageMode.Awaiting_DepartureClearance);
                SendRequestDepartureClearance(dockedStation.GetName());
            }
        }
        private GpsInfo GetDockedPoint(double range) {
            var loc = _rc.GetPosition();
            foreach (var gps in _settings.GpsPoints) {
                if ((loc - gps.GetLocation()).Length() < range)
                    return gps;
            }
            return null;
        }

        //-------------------------------------------------------------------------------
        //  MODE OPERATIONS
        //-------------------------------------------------------------------------------
        CarriageMode _mode_SpecialUseOnly;
        CarriageMode GetMode() { return _mode_SpecialUseOnly; }
        void SetMode(CarriageMode value) {
            if (_mode_SpecialUseOnly == value && value != CarriageMode.Manual_Control) return;
            _mode_SpecialUseOnly = value;

            if (!CarriageModeHelper.IsValidModeValue(_mode_SpecialUseOnly))
                _mode_SpecialUseOnly = CarriageMode.Manual_Control;

            switch (_mode_SpecialUseOnly) {
                case CarriageMode.Manual_Control:
                    ClearAutopilot(true);
                    _activateSpeedLimiter = false;
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    _destination = null;
                    _travelDirection = TravelDirection.None;
                    break;

                case CarriageMode.Transit_Powered:
                    ClearAutopilot(false);
                    _activateSpeedLimiter = false;
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.Unlock();
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _connectors) b.Disconnect();
                    break;

                case CarriageMode.Transit_Coast:
                    ClearAutopilot(false);
                    _activateSpeedLimiter = false;
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    break;

                case CarriageMode.Transit_Slow2Approach:
                    ClearAutopilot(false);
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    break;

                case CarriageMode.Transit_Docking:
                    foreach (var b in _h2Tanks) b.Stockpile = false;
                    foreach (var b in _allThrusters) b.Enabled = true;
                    foreach (var b in _landingGears) b.Enabled = true;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    _connectorLockDelayRemaining = _settings.ConnectorLockDelay;
                    break;

                case CarriageMode.Docked:
                    ClearAutopilot(true);
                    foreach (var b in _h2Tanks) b.Stockpile = true;
                    foreach (var b in _allThrusters) b.Enabled = false;
                    foreach (var b in _landingGears) b.Enabled = false;
                    foreach (var b in _landingGears) b.AutoLock = false;
                    foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, 0f);
                    break;
            }
        }

        void ClearAutopilot(bool enableDampeners) {
            _rc.SetAutoPilotEnabled(false);
            _rc.DampenersOverride = enableDampeners;
            _rc.ClearWaypoints();
        }

        void ActivateAutopilot(Vector3D target) {
            _rc.DampenersOverride = true;
            _rc.ClearWaypoints();
            _rc.AddWaypoint(target, "Destination");
            _rc.SetValueBool("DockingMode", true); // Activate Precision mode.
            _rc.SetValue<long>("FlightMode", 2); // Sets Flight mode to "One way". 2 is index of "One way" in combobox. (0 = Patrol, 1 = Circle)
            _rc.SetValue<float>("SpeedLimit", Convert.ToSingle(_settings.DockSpeed));
            _rc.SetAutoPilotEnabled(true);
        }

        void RunModeActions() {
            CheckRampsAtLimits();
            Action travelMethod = null;
            if (_travelDirection == TravelDirection.Ascent && _rampsClear)
                travelMethod = AscentModeOps;
            else if (_travelDirection == TravelDirection.Descent && _rampsClear)
                travelMethod = DecentModeOps;

            switch (GetMode()) {
                case CarriageMode.Awaiting_CarriageReady2Depart:
                    if (_rampsClear && travelMethod != null)
                        SetMode(CarriageMode.Transit_Powered);
                    break;
                //case CarriageMode.Awaiting_DepartureClearance: goto case CarriageMode.Transit_Powered;
                case CarriageMode.Transit_Powered: travelMethod?.Invoke(); break;
                case CarriageMode.Transit_Coast: travelMethod?.Invoke(); break;
                case CarriageMode.Transit_Slow2Approach: travelMethod?.Invoke(); break;
                case CarriageMode.Transit_Docking: LockConnectorsWhenStopped(); break;
                case CarriageMode.Docked: LowerRamps(); break;
            }
        }

        void LockConnectorsWhenStopped() {
            if (_rc.GetShipSpeed() > 0.1) return;

            _landingGears.ForEach(b => b.Lock());

            var anyLocked = _landingGears.Any(Collect.IsLandingGearLocked);
            if (anyLocked) {
                SetMode(CarriageMode.Docked);
                SendDockedMessage(_destination.GetName());
                _travelDirection = TravelDirection.None;
                _destination = null;
            }
        }

        void RaiseRamps() {
            _rampsClear = true;
            if (_maintCarrageRamps.Count == 0) return;
            _maintCarrageRamps.ForEach(rotor => _rampsClear &= Rotate2Limit(rotor, false));
            if (_maintGravGen != null) _maintGravGen.FieldSize = new Vector3(GRAV_RANGE_Rampsup, _maintGravGen.FieldSize.Y, _maintGravGen.FieldSize.Z);
        }
        void LowerRamps() {
            if (_maintCarrageRamps.Count == 0) return;
            if (GetMode() != CarriageMode.Docked && GetMode() != CarriageMode.Manual_Control)
                return;
            _rampsClear = false;
            _maintCarrageRamps.ForEach(rotor => Rotate2Limit(rotor, true));
            if (_maintGravGen != null) _maintGravGen.FieldSize = new Vector3(GRAV_RANGE_RampsDown, _maintGravGen.FieldSize.Y, _maintGravGen.FieldSize.Z);
        }
        bool Rotate2Limit(IMyMotorStator rotor, bool rotateToMax) {
            if (IsRotated2Limit(rotor, rotateToMax)) return true;
            rotor.SafetyLock = false;
            var velocity = rotateToMax ? ElevatorConst.ROTOR_VELOCITY : ElevatorConst.ROTOR_VELOCITY * -1;
            rotor.SetValueFloat("Velocity", velocity);
            return false; // not in position yet
        }
        bool IsRotated2Limit(IMyMotorStator rotor, bool rotateToMax) {
            if (rotor == null) return true;
            var currAngle = Math.Round(rotor.Angle, ElevatorConst.RADIAN_ROUND_DIGITS);
            var angleLimiit = rotateToMax ? rotor.UpperLimit : rotor.LowerLimit;
            var targetAngle = Math.Round(angleLimiit, ElevatorConst.RADIAN_ROUND_DIGITS);
            var notAtTarget = rotateToMax ? (currAngle < targetAngle) : (currAngle > targetAngle);
            return !notAtTarget;
        }
        void CheckRampsAtLimits() {
            var allRaised = _maintCarrageRamps.All(rotor => IsRotated2Limit(rotor, false));
            var allLowered = _maintCarrageRamps.All(rotor => IsRotated2Limit(rotor, true));
            if (!(allRaised || allLowered)) return;
            _maintCarrageRamps.ForEach(rotor => rotor.SafetyLock = true);
            _rampsClear = allRaised;
        }

        void AscentModeOps() {
            _rc.DampenersOverride = false;
            // attempt to compensate for the changing gravity force on the ship
            var gravityForceChangeCompensation = (_gravityForceOnShip / 2) * -1;

            var totalMaxBreakingThrust = _descentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));
            var rangeToTarget = (_rc.GetPosition() - _destination.GetLocation()).Length();
            var brakeingRange = CalcBrakeDistance(totalMaxBreakingThrust, gravityForceChangeCompensation);
            var coastRange = CalcBrakeDistance(0.0, gravityForceChangeCompensation);

            _debug.AppendLine("Break Range: {0:N2}", brakeingRange);
            _debug.AppendLine("Coast Range: {0:N2}", coastRange);

            var inCoastRange = (rangeToTarget <= coastRange + _settings.ApproachDistance);
            var inBrakeRange = (rangeToTarget <= brakeingRange + _settings.ApproachDistance);
            var inDockRange = Math.Abs(rangeToTarget - brakeingRange) < SWITCH_TO_AUTOPILOT_RANGE;

            if (inDockRange) {
                SetMode(CarriageMode.Transit_Docking);
                ActivateAutopilot(_destination.GetLocation());
            } else if (!inCoastRange && !inBrakeRange && GetMode() != CarriageMode.Transit_Powered)
                SetMode(CarriageMode.Transit_Powered);
            else if (_settings.GravityDescelEnabled && inCoastRange && !inBrakeRange && GetMode() != CarriageMode.Transit_Coast)
                SetMode(CarriageMode.Transit_Coast);
            else if (inBrakeRange && GetMode() != CarriageMode.Transit_Slow2Approach)
                SetMode(CarriageMode.Transit_Slow2Approach);

            switch (GetMode()) {
                case CarriageMode.Transit_Powered:
                    MaintainSpeed(_settings.TravelSpeed);
                    break;
                case CarriageMode.Transit_Slow2Approach:
                    MaintainSpeed(_settings.DockSpeed);
                    break;
            }
        }
        void DecentModeOps() {
            _rc.DampenersOverride = false;
            var totalMaxBreakingThrust = _ascentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));
            var rangeToTarget = (_rc.GetPosition() - _destination.GetLocation()).Length();
            var brakeingRange = CalcBrakeDistance(totalMaxBreakingThrust, _gravityForceOnShip);

            _debug.AppendLine("Break Range: {0:N2}", brakeingRange);
            _debug.AppendLine("Target Break Diff: {0:N2}", rangeToTarget - brakeingRange);

            var inBrakeRange = (rangeToTarget <= brakeingRange + _settings.ApproachDistance);
            var inDockRange = Math.Abs(rangeToTarget - brakeingRange) < SWITCH_TO_AUTOPILOT_RANGE;
            var inCoastZone = (!inBrakeRange && _rc.GetShipSpeed() >= _settings.TravelSpeed - 5.0);

            if (inCoastZone)
                SetMode(CarriageMode.Transit_Coast);
            else if (inDockRange) {
                SetMode(CarriageMode.Transit_Docking);
                ActivateAutopilot(_destination.GetLocation());
            } else if (inBrakeRange && GetMode() != CarriageMode.Transit_Slow2Approach)
                SetMode(CarriageMode.Transit_Slow2Approach);

            switch (GetMode()) {
                case CarriageMode.Transit_Powered:
                    MaintainSpeed(_settings.TravelSpeed * -1);
                    break;
                case CarriageMode.Transit_Slow2Approach:
                    MaintainSpeed(_settings.DockSpeed * -1);
                    break;
                case CarriageMode.Transit_Coast:
                    if (rangeToTarget - brakeingRange < 300
                        && !_activateSpeedLimiter
                        && _inNaturalGravity)
                        ThrusterHelper.SetThrusterOverride(_ascentThrusters[0], 1.001);
                    break;
            }
        }
        void MaintainSpeed(double targetVertSpeed) {
            var ascentMaxEffectiveThrust = _ascentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));
            var decentMaxEffectiveThrust = _descentThrusters.Sum(b => ThrusterHelper.GetMaxEffectiveThrust(b));

            var hoverOverridePower = _inNaturalGravity
                ? Convert.ToSingle((_gravityForceOnShip / ascentMaxEffectiveThrust) * 100)
                : 0f;

            var ascentOverridePower = 0f;
            var decentOverridePower = 0f;

            var speedDiff = targetVertSpeed - _verticalSpeed;
            _debug.AppendLine("");
            _debug.AppendLine("S.Diff: {0:N1}", speedDiff);

            if (speedDiff > -0.5f && speedDiff < 0.5f) {
                ascentOverridePower = hoverOverridePower;
            } else if (speedDiff > 2) {
                ascentOverridePower = 100f;
            } else if (speedDiff > 0) {
                ascentOverridePower = hoverOverridePower + ((100f - hoverOverridePower) / 2);
            } else if (speedDiff < -2) {
                decentOverridePower = 100f;
            } else if (speedDiff < 0) {
                ascentOverridePower = hoverOverridePower;
                decentOverridePower = 2f;
            }

            _debug.AppendLine("Ascent Override %: {0:N1}", ascentOverridePower);
            _debug.AppendLine("Decent Override %: {0:N1}", decentOverridePower);

            foreach (var b in _ascentThrusters) ThrusterHelper.SetThrusterOverride(b, ascentOverridePower);
            foreach (var b in _descentThrusters) ThrusterHelper.SetThrusterOverride(b, decentOverridePower);
        }
        double CalcBrakeDistance(double maxthrust, double gravForceOnShip) {
            var brakeForce = maxthrust - gravForceOnShip;
            if (brakeForce < 0.0) brakeForce = 0.0;
            var deceleration = brakeForce / _actualMass;
            return Math.Pow(_rc.GetShipSpeed(), 2) / (2 * deceleration);
        }

        //-------------------------------------------------------------------------------
        //  Collection Methods
        //-------------------------------------------------------------------------------
        bool IsOnThisGrid(IMyTerminalBlock b) { return Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }
        bool IsTaggedBlock(IMyTerminalBlock b) {
            if (string.IsNullOrWhiteSpace(_settings.BlockTag))
                return true;
            return (b.CustomName.Contains(_settings.BlockTag));
        }
        bool IsTaggedBlockOnThisGrid(IMyTerminalBlock b) { return (IsOnThisGrid(b) && IsTaggedBlock(b)); }

    }
}
