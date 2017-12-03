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

        public void Main(string argument, UpdateType updateSource) {
            try {
                Echo("Carriage Control " + _runSymbol.GetSymbol(Runtime));
                //Echo("DEBUG " + (_debug.Enabled ? "enabled" : "disabled"));
                Echo($"Mode: {GetMode()}");

                _executionInterval.RecordTime(Runtime);
                //_connectorLockDelay.RecordTime(Runtime);
                _trasmitStatsDelay.RecordTime(Runtime);
                _updateDisplayDelay.RecordTime(Runtime);
                _blockRefreshInterval.RecordTime(Runtime);

                LoadConfigSettings();
                LoadBlockLists(_blockRefreshInterval.AtNextInterval);
                EchoBlockLists();
                if (GetMode() == CarriageMode.Init) SetMode(CarriageMode.Manual_Control);

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
                        _gravityGen.Enabled = (_gravVec.Length() < GRAV_Force_Earth / 2);
                }

                if (_updateDisplayDelay.AtNextInterval) {
                    UpdateDisplays();
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
            //_connectorLockDelay.SetInterval(_settings.ConnectorLockDelay);
            _doorManager.SecondsToLeaveOpen = _settings.DoorCloseDelay;
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
            if (dockedStation != null && destination != null && string.Compare(destination.Name, dockedStation.Name, true) == 0) return;

            _destination = destination;
            if (_destination == null) return;

            var bottom = _settings.GetBottomPoint();
            var myDist = Vector3D.Distance(bottom, _rc.GetPosition());
            var destDist = Vector3D.Distance(bottom, _destination.Location);
            _travelDirection = (myDist < destDist)
                ? TravelDirection.Ascent
                : TravelDirection.Descent;

            RaiseBoardingRamps();
            if (dockedStation == null || !dockedStation.NeedsClearance)
                SetMode(CarriageMode.Awaiting_CarriageReady2Depart);
            else {
                SetMode(CarriageMode.Awaiting_DepartureClearance);
                SendRequestDepartureClearance(dockedStation.Name);
            }
        }
        private GpsInfo GetDockedPoint(double range) {
            var loc = _rc.GetPosition();
            foreach (var gps in _settings.GpsPoints) {
                if (Vector3D.Distance(loc, gps.Location) < range)
                    return gps;
            }
            return null;
        }

    }
}
