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
                Echo("Station Control 1.2a: " + _runSymbol.GetSymbol(Runtime));

                _executionInterval.RecordTime(Runtime);
                _blockRefreshInterval.RecordTime(Runtime);

                LoadConfigSettings();
                LoadBlockLists(_blockRefreshInterval.AtNextInterval);
                EchoBlockLists();

                if (!string.IsNullOrEmpty(argument))
                    RunCommand(argument);

                if (_executionInterval.AtNextInterval) {
                    _debug.Clear();
                    _comms.TransmitQueue(_antenna);
                    _doorManager.CloseOpenDoors(_executionInterval.Time, _autoCloseDoors);
                    UpdateDisplays();
                    RunCarriageDockDepartureActions(TAG_A1, _A1);
                    RunCarriageDockDepartureActions(TAG_A2, _A2);
                    RunCarriageDockDepartureActions(TAG_B1, _B1);
                    RunCarriageDockDepartureActions(TAG_B2, _B2);
                    RunCarriageDockDepartureActions(TAG_MAINTENANCE, _Maintenance);

                    _debug.AppendLine(_log.GetLogText());
                }
            } catch (Exception ex) {
                _debug.AppendLine(ex.Message);
                _debug.AppendLine(ex.StackTrace);
                throw ex;
            } finally {
                if (_debug != null)
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

        void LoadBlockLists(bool forceLoad = false) {
            if (_blocksLoaded && !forceLoad) return;

            _antenna = CollectHelper.GetFirstblockOfTypeWithFirst<IMyRadioAntenna>(GridTerminalSystem, _tempList, IsTaggedStationOnThisGrid, IsOnThisGrid);

            GridTerminalSystem.GetBlocksOfType(_h2Tanks, b => IsOnThisGrid(b) && Collect.IsHydrogenTank(b));
            GridTerminalSystem.GetBlocksOfType(_autoCloseDoors, b => IsTaggedStationOnThisGrid(b) && !IsTaggedTerminal(b));
            GridTerminalSystem.GetBlocksOfType(_displays, IsTaggedStationOnThisGrid);

            _blocksLoaded = true;
        }
        void EchoBlockLists() {
            Echo($"H2 Tanks: {_h2Tanks.Count}");
            Echo($"Doors: {_autoCloseDoors.Count}");
            Echo($"Displays: {_displays.Count}");
        }

        void RunCommand(string argument) {
            CommMessage msg = null;
            if (CommMessage.TryParse(argument, out msg)) {
                switch (msg.PayloadType) {
                    case CarriageStatusMessage.TYPE: CarriageStatusProcessing(msg.SenderGridName, msg.Payload); break;
                    case CarriageRequestMessage.TYPE: CarriageRequestProcessing(msg.SenderGridName, msg.Payload); break;
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
                }
            }
        }

        void CarriageRequestProcessing(string carriageName, string msgPayload) {
            var message = CarriageRequestMessage.CreateFromPayload(msgPayload);
            var carriage = GetCarriageVar(carriageName);
            if (message == null || carriage == null) return;
            _log.AppendLine("Request: " + message.Request);
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
            if (string.Compare(_Maintenance.GridName, carriageName, true) == 0) return _Maintenance;
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
