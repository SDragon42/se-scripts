﻿using Sandbox.Game.EntityComponents;
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
                _timeBlockReloadLast += Runtime.TimeSinceLastRun.TotalSeconds;
                Echo("Station Control " + _runSymbol.GetSymbol(Runtime));

                var runInterval = ((updateSource & UpdateType.Update10) == UpdateType.Update10);

                LoadConfigSettings();
                LoadBlockLists();

                if (!string.IsNullOrEmpty(argument))
                    RunCommand(argument);

                if (runInterval) {
                    _comms.TransmitQueue(_antenna);
                    RunCarriageDockDepartureActions(TAG_A1, _A1);
                    RunCarriageDockDepartureActions(TAG_A2, _A2);
                    RunCarriageDockDepartureActions(TAG_B1, _B1);
                    RunCarriageDockDepartureActions(TAG_B2, _B2);
                    RunCarriageDockDepartureActions(TAG_MAINT, _Maint);
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
            _custConfig.ReadFromCustomData(Me);
            _settings.LoadFromSettingDict(_custConfig);
            _custConfig.SaveToCustomData(Me);
            _lastCustomDataHash = hash;
        }

        void RunCommand(string argument) {
            //_log.AppendLine(argument);
            CommMessage msg = null;
            if (CommMessage.TryParse(argument, out msg)) {
                switch (msg.PayloadType) {
                    case CarriageStatusMessage.TYPE: CarriageStatusProcessing(msg.SenderGridName, msg.Payload); break;
                    case CarriageRequestMessage.TYPE: CarriageRequestProcessing(msg.SenderGridName, msg.Payload); break;
                    case UpdateAllDisplaysMessage.TYPE: DisplayProcessing(msg.Payload); break;
                }
            } else {
                if (argument.StartsWith(CMD_DockCarriage)) {
                    argument = argument.Remove(0, CMD_DockCarriage.Length).Trim();
                    var carriage = GetCarriageVar(argument);
                    if (carriage != null)
                        carriage.Connect = true;
                } else if (argument.StartsWith(CMD_UndockCarriage)) {
                    argument = argument.Remove(0, CMD_UndockCarriage.Length).Trim();
                    var carriage = GetCarriageVar(argument);
                    if (carriage != null)
                        carriage.Connect = false;
                } else if (argument.StartsWith(CMD_RequestCarriage)) {
                    argument = argument.Remove(0, CMD_RequestCarriage.Length).Trim();
                    SendCarriageRequestMessage(argument);
                }
            }
        }

        void CarriageRequestProcessing(string carriageName, string msgPayload) {
            _log.AppendLine($"C.Request - Carriage: {carriageName}");
            var message = CarriageRequestMessage.CreateFromPayload(msgPayload);
            var carriage = GetCarriageVar(carriageName);
            if (message == null || carriage == null) return;
            _log.AppendLine($"{DateTime.Now.ToLongTimeString()} From {carriageName} | Request: {message.Request}");
            switch (message.Request) {
                case CarriageRequests.Dock:
                    carriage.Connect = true;
                    carriage.SendResponseMsg = true;
                    break;
                case CarriageRequests.Depart:
                    carriage.Connect = false;
                    carriage.SendResponseMsg = true;
                    break;
            }
        }
        void CarriageStatusProcessing(string carriageName, string msgPayload) {
            _log.AppendLine($"C.Status - Carriage: {carriageName}");
            var message = CarriageStatusMessage.CreateFromPayload(msgPayload);
            var carriage = GetCarriageVar(carriageName);
            if (message == null || carriage == null) return;
            carriage.Status = message;
        }
        CarriageVars GetCarriageVar(string carriageName) {
            if (string.Compare(_A1.GridName, carriageName, true) == 0) return _A1;
            if (string.Compare(_A2.GridName, carriageName, true) == 0) return _A2;
            if (string.Compare(_B1.GridName, carriageName, true) == 0) return _B1;
            if (string.Compare(_B2.GridName, carriageName, true) == 0) return _B2;
            if (string.Compare(_Maint.GridName, carriageName, true) == 0) return _Maint;
            return null;
        }

        public static T GetFirstBlockInList<T>(List<IMyTerminalBlock> blockList, Func<IMyTerminalBlock, bool> collect) where T : class {
            foreach (var b in blockList) {
                if (!(b is T)) continue;
                if (collect(b)) return (T)b;
            }
            return null;
        }

    }
}