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
                _timeDisplayLast += Runtime.TimeSinceLastRun.TotalSeconds;
                _timeBlockReloadLast += Runtime.TimeSinceLastRun.TotalSeconds;
                Echo("OPS Center " + _runSymbol.GetSymbol(Runtime));

                var runInterval = ((updateSource & UpdateType.Update10) == UpdateType.Update10);

                LoadConfigSettings();
                LoadBlockLists();

                if (!string.IsNullOrEmpty(argument))
                    RunCommand(argument);

                if (runInterval) {
                    _comms.TransmitQueue(_antenna);

                    BuildDisplays();
                    UpdateDisplays();
                }

                if (_timeDisplayLast >= TIME_TransmitStatusDelay) {
                    COMMs_UpdateAllDisplaysMessage();
                    _timeDisplayLast = 0;
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
            if (hash == _lastCustomDataHash) return;
            _custConfig.Load(Me);
            _settings.LoadFromSettingDict(_custConfig);
            _custConfig.Save(Me);
            _lastCustomDataHash = hash;
            _log.MaxTextLinesToKeep = _settings.LogLines2Show;
        }

        void RunCommand(string argument) {
            CommMessage msg = null;
            if (CommMessage.TryParse(argument, out msg)) {
                switch (msg.PayloadType) {
                    case CarriageStatusMessage.TYPE: CarriageStatusProcessing(msg.SenderGridName, msg.Payload); break;
                    case StationRequestMessage.TYPE: CarriageRequestedProcessing(msg.SenderGridName, msg.Payload); break;
                }
            } else {
                if (argument.StartsWith(CMD_SendCarriage)) {
                    var parts = argument.Remove(0, CMD_SendCarriage.Length).Trim().Split(new char[] { ' ' }, 2);
                    if (parts.Length >= 2) {
                        SendCarriageToStation(parts[0].Trim(), parts[1].Trim());
                    }
                }
            }
        }

        void CarriageStatusProcessing(string carriageName, string msgPayload) {
            var status = CarriageStatusMessage.CreateFromPayload(msgPayload);
            _carriageStatuses[carriageName] = status;
        }



    }
}
