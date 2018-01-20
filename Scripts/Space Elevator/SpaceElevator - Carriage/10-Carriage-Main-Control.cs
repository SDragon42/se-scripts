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
                _timeTransmitLast += Runtime.TimeSinceLastRun.TotalSeconds;
                _timeBlockReloadLast += Runtime.TimeSinceLastRun.TotalSeconds;

                Echo("Carriage Control " + _runSymbol.GetSymbol(Runtime));

                var runInterval = ((updateSource & UpdateType.Update10) == UpdateType.Update10);
                var forceBlockReload = ((updateSource & UpdateType.Update100) == UpdateType.Update100);

                Echo($"Mode: {GetMode()}");

                LoadConfigSettings();
                LoadBlockLists();
                EchoBlockLists();
                if (GetMode() == CarriageMode.Init) SetMode(CarriageMode.Manual_Control);

                if (argument.Length > 0)
                    RunCommand(argument);

                if (runInterval) {
                    _debug.Clear();
                    LoadCalculations();
                    RunModeActions();
                    SaveLastValues();
                    _comms.TransmitQueue(_antenna);
                    if (_gravityGen != null)
                        _gravityGen.Enabled = (_gravVec.Length() < GRAV_Force_Earth / 2);

                    _debug.AppendLine(_log.GetLogText());
                }

                if (forceBlockReload)
                    UpdateDisplays();

                if (_timeTransmitLast >= TIME_TransmitDelay) {
                    SendStatsMessage();
                    _timeTransmitLast = 0;
                }

                if (_displayLog.Count > 0) {
                    var txt = _log.GetLogText();
                    foreach (var d in _displayLog) {
                        d.WritePublicText(txt);
                    }
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
        }

        void RunCommand(string argument) {
            CommMessage msg = null;
            if (CommMessage.TryParse(argument, out msg)) {
                // COMMs messages

                if (string.Compare(msg.TargetGridName, Me.CubeGrid.CustomName, true) == 0)
                    _log.AppendLine($"{DateTime.Now.ToLongTimeString()} From: {msg.SenderGridName} | To: {msg.TargetGridName} | Type: {msg.PayloadType}");
                switch (msg.PayloadType) {
                    case StationResponseMessage.TYPE: StationResponseProcessing(msg.Payload); break;
                    case SendCarriageToMessage.TYPE: SendCarriageToProcessing(msg.Payload); break;
                    case UpdateAllDisplaysMessage.TYPE: DisplayProcessing(msg.Payload); break;
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
        void StationResponseProcessing(string msgPayload) {
            var responseMsg = StationResponseMessage.CreateFromPayload(msgPayload);
            if (responseMsg?.Response == StationResponses.DepartureOk)
                SetMode(CarriageMode.Awaiting_CarriageReady2Depart);
        }
        void SendCarriageToProcessing(string msgPayload) {
            var sendToMsg = SendCarriageToMessage.CreateFromPayload(msgPayload);
            if (sendToMsg == null) return;
            var destination = _settings.GetGpsInfo(sendToMsg.Destination);
            SetDeparture(destination);
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
