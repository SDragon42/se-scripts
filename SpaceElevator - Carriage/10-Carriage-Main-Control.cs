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

        public void Main(string argument) {
            try {
                Echo("Carriage Control v1.8 " + _runSymbol.GetSymbol(Runtime));
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
                    if (_gravityGen != null)
                        _gravityGen.Enabled = (_gravVec.Length() < 9.81 / 2);
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
            //GridTerminalSystem.GetBlocksOfType(_displays, IsTaggedBlockOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_autoCloseDoors, IsTaggedBlockOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_boardingRamps, IsTaggedBlockOnThisGrid);

            _blocksLoaded = true;
        }
        void EchoBlockLists() {
            Echo($"Ascent Thrusters: {_ascentThrusters.Count}");
            Echo($"Descent Thrusters: {_descentThrusters.Count}");
            Echo($"Connectors: {_connectors.Count}");
            Echo($"Locking Gears: {_landingGears.Count}");
            Echo($"Ramp Rotors: {_boardingRamps.Count}");
            //Echo($"AirVents: {_airVents.Count}");
            //Echo($"O2 Tanks: {_o2Tanks.Count}");
            //Echo($"H2 Tanks: {_h2Tanks.Count}");
            //Echo($"Displays: {_displays.Count}");
        }

        void RunCommand(string argument) {
            CommMessage msg = null;
            if (CommMessage.TryParse(argument, out msg)) {
                // COMMs messages
                switch (msg.PayloadType) {
                    case StationResponseMessage.TYPE:
                        var responseMsg = StationResponseMessage.CreateFromPayload(msg.Payload);
                        if (responseMsg?.Response == StationResponses.DepartureOk)
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

            RaiseBoardingRamps();
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

    }
}
