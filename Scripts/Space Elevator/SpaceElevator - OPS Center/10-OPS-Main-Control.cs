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
                Echo("OPS Center " + _runSymbol.GetSymbol(Runtime));

                _executionInterval.RecordTime(Runtime);
                _blockRefreshInterval.RecordTime(Runtime);
                _displayRefreshInterval.RecordTime(Runtime);

                LoadConfigSettings();
                LoadBlockLists(_blockRefreshInterval.AtNextInterval);
                EchoBlockLists();

                if (!string.IsNullOrEmpty(argument))
                    RunCommand(argument);

                if (_executionInterval.AtNextInterval) {
                    _debug.Clear();
                    _comms.TransmitQueue(_antenna);
                    _doorManager.CloseOpenDoors(_executionInterval.Time, _autoCloseDoors);

                    BuildDisplays();
                    UpdateDisplays();

                    _debug.AppendLine(_log.GetLogText());
                }

                if (_displayRefreshInterval.AtNextInterval) {
                    SendAllCOMMsDisplays();
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
            _doorManager.SecondsToLeaveOpen = _settings.DoorCloseDelay;
        }

        void RunCommand(string argument) {
            CommMessage msg = null;
            if (CommMessage.TryParse(argument, out msg)) {
                switch (msg.PayloadType) {
                    case CarriageStatusMessage.TYPE: CarriageStatusProcessing(msg.SenderGridName, msg.Payload); break;
                        //case CarriageRequestMessage.TYPE: CarriageRequestProcessing(msg.SenderGridName, msg.Payload); break;
                }
            } else {
                if (argument.StartsWith(CMD_SendCarriage)) {
                    //argument = argument.Remove(0, CMD_DockCarriage.Length).Trim();
                    //var carriage = GetCarriageVar(argument);
                    //if (carriage != null)
                    //    carriage.Connect = true;
                }
            }
        }

        void CarriageStatusProcessing(string carriageName, string msgPayload) {
            var status = CarriageStatusMessage.CreateFromPayload(msgPayload);
            //var carriage = GetCarriageVar(carriageName);
            //if (status == null || carriage == null) return;
            //carriage.Status = status;
            _carriageStatuses[carriageName] = status;
            _log.AppendLine($"{DateTime.Now.ToLongTimeString()}|{carriageName}");
        }

    }
}
