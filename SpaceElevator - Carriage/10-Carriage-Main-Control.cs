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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {

        public void Main(string argument, UpdateType updateSource) {
            try {
                _timeTransmitStatusLast += Runtime.TimeSinceLastRun.TotalSeconds;
                _timeBlockReloadLast += Runtime.TimeSinceLastRun.TotalSeconds;

                Echo("Carriage Control " + _runSymbol.GetSymbol(Runtime));

                var runInterval = ((updateSource & UpdateType.Update10) == UpdateType.Update10);
                var updateDisplayInterval = ((updateSource & UpdateType.Update100) == UpdateType.Update100);

                LoadConfigSettings();
                LoadBlockLists();

                if (GetMode() == CarriageMode.Init)
                    SetMode(CarriageMode.Manual_Control);

                if (argument.Length > 0)
                    RunCommand(argument);

                if (runInterval) {
                    LoadCalculations();
                    RunModeActions();
                    SaveLastValues();
                    _comms.TransmitQueue(_antenna);
                    if (_gravityGen != null)
                        _gravityGen.Enabled = (_gravVec.Length() < GRAV_Force_Earth / 2);
                }

                if (updateDisplayInterval)
                    UpdateDisplays();

                if (_timeTransmitStatusLast >= TIME_TransmitStatusDelay) {
                    Add2Comms_Status();
                    _timeTransmitStatusLast = 0;
                }

                if (_log.Enabled) {
                    var logText = _log.GetLogText();
                    Echo(logText);
                    foreach (var d in _displayLog) {
                        d.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                        d.TextPadding = 0f;
                        d.WriteText(logText);
                    }
                }
            } catch (Exception ex) {
                Echo("##########");
                Echo(ex.Message);
                Echo(ex.StackTrace);
                Echo("##########");
                throw ex;
            } finally {
                Echo(_log.GetLogText());
            }
        }

        void LoadConfigSettings() {
            var hash = Me.CustomData.GetHashCode();
            if (hash == _lastCustomDataHash)
                return;
            _custConfig.Load(Me);
            _settings.LoadFromSettingDict(_custConfig);
            _custConfig.Save(Me);
            _lastCustomDataHash = hash;
            _log.MaxTextLinesToKeep = _settings.LogLines2Show;
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

            var cmdParts = argument.Split(SepSpace, 2, StringSplitOptions.RemoveEmptyEntries);
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
                Add2Comms_Request(dockedStation.Name, CarriageRequests.Depart);
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
