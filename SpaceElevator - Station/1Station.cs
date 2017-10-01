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
        const string CMD_DockCarriage = "dock";
        const string CMD_UndockCarriage = "undock";


        //-------------------------------------------------------------------------------
        //  CONSTANTS
        //-------------------------------------------------------------------------------
        const string TAG_A1 = "[A1]";
        const string TAG_A2 = "[A2]";
        const string TAG_B1 = "[B1]";
        const string TAG_B2 = "[B2]";
        const string TAG_MAINTENANCE = "[Maint]";
        const string TAG_TRANSFER_ARM = "[Transfer Arm]";
        const string TAG_TERMINAL = "[Terminal]";


        readonly CustomDataConfigModule _custConfig;
        readonly ScriptSettingsModule _settings;

        readonly CarriageVars _A1;
        readonly CarriageVars _A2;
        readonly CarriageVars _B1;
        readonly CarriageVars _B2;
        readonly CarriageVars _Maintenance;

        readonly DebugModule _debug;
        readonly LogModule _log;
        readonly COMMsModule _comms;
        readonly RunningSymbolModule _runSymbol;
        readonly TimeIntervalModule _executionInterval;
        readonly AutoDoorCloserModule _doorManager;
        readonly Queue<CommMessage> _messageQueue = new Queue<CommMessage>();

        bool _blocksLoaded = false;
        IMyRadioAntenna _antenna;
        readonly List<IMyTerminalBlock> _tempList = new List<IMyTerminalBlock>();
        readonly List<IMyGasTank> _h2Tanks = new List<IMyGasTank>();
        readonly List<IMyTextPanel> _displays = new List<IMyTextPanel>();
        readonly List<IMyDoor> _autoCloseDoors = new List<IMyDoor>();

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _debug = new DebugModule(this);
            //_debug.Enabled = false;
            _debug.EchoMessages = false;

            _log = new LogModule();

            _custConfig = new CustomDataConfigModule();
            _settings = new ScriptSettingsModule();
            _settings.InitConfig(_custConfig);

            _A1 = new CarriageVars("Carriage A1");
            _A2 = new CarriageVars("Carriage A2");
            _B1 = new CarriageVars("Carriage B1");
            _B2 = new CarriageVars("Carriage B2");
            _Maintenance = new CarriageVars("Maint Carriage");
            if (!string.IsNullOrWhiteSpace(Storage)) {
                var gateStates = Storage.Split('\n');
                if (gateStates.Length == 5) {
                    _A1.FromString(gateStates[0]);
                    _A2.FromString(gateStates[1]);
                    _B1.FromString(gateStates[2]);
                    _B2.FromString(gateStates[3]);
                    _Maintenance.FromString(gateStates[4]);
                }
            }

            _lastCustomDataHash = -1;

            _runSymbol = new RunningSymbolModule();
            _executionInterval = new TimeIntervalModule(10);

            _doorManager = new AutoDoorCloserModule();

            _comms = new COMMsModule(Me);
        }

        public void Save() {
            Storage = _A1.ToString() + "\n" +
                _A2.ToString() + "\n" +
                _B1.ToString() + "\n" +
                _B2.ToString() + "\n" +
                _Maintenance.ToString();
        }

        public void Main(string argument) {
            try {
                Echo("Station Control 1.2a: " + _runSymbol.GetSymbol(Runtime));

                _executionInterval.RecordTime(Runtime);

                LoadConfigSettings();
                LoadBlockLists();
                EchoBlockLists();

                if (!string.IsNullOrEmpty(argument))
                    RunCommand(argument);

                if (_executionInterval.AtNextInterval()) {
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

        int _lastCustomDataHash;
        void LoadConfigSettings() {
            var hash = Me.CustomData.GetHashCode();
            if (hash == _lastCustomDataHash) return;
            _custConfig.ReadFromCustomData(Me);
            _settings.LoadFromSettingDict(_custConfig);
            _custConfig.SaveToCustomData(Me);
            _lastCustomDataHash = hash;
            _doorManager.SecondsToLeaveOpen = _settings.GetDoorCloseDelay();
        }

        void LoadBlockLists(bool forceLoad = false) {
            if (_blocksLoaded && !forceLoad) return;

            GetblocksOfTypeWithFirst<IMyRadioAntenna>(_tempList, IsTaggedStationOnThisGrid, IsOnThisGrid);
            if (_tempList.Count > 0)
                _antenna = (IMyRadioAntenna)_tempList[0];

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

        bool IsTaggedStation(IMyTerminalBlock b) {
            return (b.CustomName.Contains(_settings.GetStationTag()));
        }
        bool IsTaggedStationOnThisGrid(IMyTerminalBlock b) {
            return (IsOnThisGrid(b) && IsTaggedStation(b));
        }
        bool IsTaggedTerminal(IMyTerminalBlock b) {
            return (b.CustomName.Contains(_settings.GetTerminalTag()));
        }
        bool IsTaggedTransfer(IMyTerminalBlock b) {
            return (b.CustomName.Contains(_settings.GetTransferTag()));
        }

        //-------------------------------------------------------------------------------
        //  COMMs
        //-------------------------------------------------------------------------------
        void SendDockingCompleteMessage(string carriageName) {
            if (_antenna == null) return;
            var msgPayload = new StationResponseMessage(StationResponseMessage.RESPONSE_DOCKING_COMPLETE);
            _comms.AddMessageToQueue(msgPayload, carriageName);
        }
        void SendGoForDepartureMessage(string carriageName) {
            if (_antenna == null) return;
            var msgPayload = new StationResponseMessage(StationResponseMessage.RESPONSE_DEPARTURE_OK);
            _comms.AddMessageToQueue(msgPayload, carriageName);
        }

        //-------------------------------------------------------------------------------
        //  COMMAND OPERATIONS
        //-------------------------------------------------------------------------------
        void RunCommand(string argument) {
            CommMessage msg = null;
            if (CommMessage.TryParse(argument, out msg)) {
                //_log.AppendLine("MSG: " + msg.PayloadType);
                switch (msg.PayloadType) {
                    case CarriageStatusMessage.TYPE: CarriageStatusProcessing(msg.SenderGridName, msg.Payload); break;
                    case CarriageRequestMessage.TYPE: CarriageRequestProcessing(msg.SenderGridName, msg.Payload); break;
                }
            } else {
                //_log.AppendLine("CMD: " + argument);
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
                case CarriageRequestMessage.REQUEST_DOCK:
                    carriage.Connect = true;
                    carriage.SendResponseMsg = true;
                    break;
                case CarriageRequestMessage.REQUEST_DEPART:
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


        //-------------------------------------------------------------------------------
        //  CARRIAGE DOCK OPERATIONS
        //-------------------------------------------------------------------------------
        readonly List<IMyTerminalBlock> _gateBlocks = new List<IMyTerminalBlock>();
        readonly List<IMyTerminalBlock> _armLights = new List<IMyTerminalBlock>();
        readonly List<IMyTerminalBlock> _terminalDoors = new List<IMyTerminalBlock>();

        void RunCarriageDockDepartureActions(string gateTag, CarriageVars carriage) {

            GridTerminalSystem.SearchBlocksOfName(gateTag, _gateBlocks, IsTaggedStation);
            GridTerminalSystem.SearchBlocksOfName(gateTag, _armLights, IsLightOnTransferArm);
            var _armRotor = GetFirstBlockInList<IMyMotorAdvancedStator>(_gateBlocks, IsOnTransferArm);
            var _armPiston = GetFirstBlockInList<IMyPistonBase>(_gateBlocks, IsOnTransferArm);
            var _armConnector = GetFirstBlockInList<IMyShipConnector>(_gateBlocks, IsOnTransferArm);
            var _terminalPiston = GetFirstBlockInList<IMyPistonBase>(_gateBlocks, IsOnTerminal);
            GridTerminalSystem.SearchBlocksOfName(gateTag, _terminalDoors, IsDoorOnTerminal);

            var CanSendConnectedMessage = false;
            var CanSendDisconnectedMessage = false;

            var newState = HookupState.Disconnecting;
            if (carriage.Connect) {
                var completed = ConnectArm(_armRotor, _armPiston, _armConnector, _terminalPiston) & ExtendRamp(_armRotor, _armPiston, _armConnector, _terminalPiston);
                newState = completed ? HookupState.Connected : HookupState.Connecting;
            } else {
                var completed = DisconnectArm(_armRotor, _armPiston, _armConnector, _terminalPiston) & RetractRamp(_armRotor, _armPiston, _armConnector, _terminalPiston);
                newState = completed ? HookupState.Disconnected : HookupState.Disconnecting;
            }

            if (newState == HookupState.Connected && (carriage.GateState == HookupState.Connecting || carriage.SendResponseMsg))
                CanSendConnectedMessage = true;
            if (newState == HookupState.Disconnected && (carriage.GateState == HookupState.Disconnecting || carriage.SendResponseMsg))
                CanSendDisconnectedMessage = true;
            carriage.GateState = newState;

            if (CanSendConnectedMessage && carriage.SendResponseMsg) {
                SendDockingCompleteMessage(carriage.GridName);
                carriage.SendResponseMsg = false;
            }
            if (CanSendDisconnectedMessage && carriage.SendResponseMsg) {
                SendGoForDepartureMessage(carriage.GridName);
                carriage.SendResponseMsg = false;
            }
        }

        bool IsOnTransferArm(IMyTerminalBlock b) { return IsTaggedStation(b) && IsTaggedTransfer(b); }
        bool IsLightOnTransferArm(IMyTerminalBlock b) { return IsTaggedStation(b) && IsTaggedTransfer(b) && (b is IMyInteriorLight || b is IMyReflectorLight); }

        bool IsOnTerminal(IMyTerminalBlock b) { return IsTaggedStation(b) && IsTaggedTerminal(b); }
        bool IsDoorOnTerminal(IMyTerminalBlock b) { return IsTaggedStation(b) && IsTaggedTerminal(b) && Collect.IsHumanDoor(b); }


        bool ConnectArm(IMyMotorAdvancedStator _armRotor, IMyPistonBase _armPiston, IMyShipConnector _armConnector, IMyPistonBase _terminalPiston) {
            // turn on lights
            foreach (var light in _armLights) {
                ((IMyFunctionalBlock)light).Enabled = true;
            }

            // rotate arm
            if (_armRotor != null) {
                var currAngle = Math.Round(_armRotor.Angle, ElevatorConst.RADIAN_ROUND_DIGITS);
                var maxAngle = Math.Round(_armRotor.UpperLimit, ElevatorConst.RADIAN_ROUND_DIGITS);
                if (currAngle < maxAngle) {
                    _armRotor.SafetyLock = false;
                    _armRotor.SetValueFloat("Velocity", ElevatorConst.ROTOR_VELOCITY);
                    return false; // not in position yet
                }
                _armRotor.SafetyLock = true;
            }

            if (_armConnector == null) return false;
            // extend pistion - extends till the piston can connect
            if (_armConnector.Status == MyShipConnectorStatus.Unconnected) {
                if (_armPiston != null) {
                    _armPiston.SafetyLock = false;
                    _armPiston.Extend();
                }
            } else {
                if (_armPiston != null)
                    _armPiston.SafetyLock = true;
                _armConnector.Connect();
            }

            return (_armConnector.Status == MyShipConnectorStatus.Connected);
        }
        bool DisconnectArm(IMyMotorAdvancedStator _armRotor, IMyPistonBase _armPiston, IMyShipConnector _armConnector, IMyPistonBase _terminalPiston) {
            // retract piston
            if (_armConnector == null) return false;
            _armConnector.Disconnect();
            if (_armPiston != null) {
                if (_armPiston.CurrentPosition > _armPiston.MinLimit) {
                    _armPiston.SafetyLock = false;
                    _armPiston.Retract();
                    return false; // not fully retracted
                }
                _armPiston.SafetyLock = true;
            }

            // rotate arm
            if (_armRotor != null) {
                var currAngle = Math.Round(_armRotor.Angle, ElevatorConst.RADIAN_ROUND_DIGITS);
                var minAngle = Math.Round(_armRotor.LowerLimit, ElevatorConst.RADIAN_ROUND_DIGITS);
                if (currAngle > minAngle) {
                    _armRotor.SafetyLock = false;
                    _armRotor.SetValueFloat("Velocity", ElevatorConst.ROTOR_VELOCITY * -1);
                    return false; // not fully retracted
                }
                _armRotor.SafetyLock = true;
            }

            // turn off lights
            foreach (var light in _armLights) {
                ((IMyFunctionalBlock)light).Enabled = false;
            }

            return true;
        }

        bool ExtendRamp(IMyMotorAdvancedStator _armRotor, IMyPistonBase _armPiston, IMyShipConnector _armConnector, IMyPistonBase _terminalPiston) {
            // extend piston
            if (_terminalPiston != null) {
                _terminalPiston.Extend();
                if (_terminalPiston.CurrentPosition < _terminalPiston.MaxLimit) return false;
            }

            // open doors
            if (_terminalDoors != null) {
                foreach (var b in _terminalDoors) {
                    var door = (IMyDoor)b;
                    if (door.Status != DoorStatus.Open && door.Status != DoorStatus.Opening) {
                        door.Enabled = true;
                        door.OpenDoor();
                    }
                }
            }

            return true;
        }
        bool RetractRamp(IMyMotorAdvancedStator _armRotor, IMyPistonBase _armPiston, IMyShipConnector _armConnector, IMyPistonBase _terminalPiston) {
            // Close doors
            if (_terminalDoors != null) {
                foreach (var b in _terminalDoors) {
                    var door = (IMyDoor)b;
                    if (door.Status != DoorStatus.Closing && door.Status != DoorStatus.Closed) {
                        door.Enabled = true;
                        door.CloseDoor();
                    }
                }
                foreach (var b in _terminalDoors) {
                    var door = (IMyDoor)b;
                    if (door.Status != DoorStatus.Closed)
                        return false;
                }

                // lock doors
                foreach (var b in _terminalDoors) {
                    var door = (IMyDoor)b;
                    if (door.Status == DoorStatus.Closed)
                        door.Enabled = false;
                }
            }

            // retract piston
            if (_terminalPiston != null) {
                _terminalPiston.Retract();
                return !(_terminalPiston.CurrentPosition > _terminalPiston.MinLimit);
            }

            return true;
        }


        public void GetblocksOfTypeWithFirst<T>(List<IMyTerminalBlock> blockList, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class {
            blockList.Clear();
            foreach (var collect in collectMethods) {
                GridTerminalSystem.GetBlocksOfType<T>(blockList, collect);
                if (blockList.Count > 0) return;
            }
        }

        public static T GetFirstBlockInList<T>(List<IMyTerminalBlock> blockList, Func<IMyTerminalBlock, bool> collect) where T : class {
            foreach (var b in blockList) {
                if (!(b is T)) continue;
                if (collect(b)) return (T)b;
            }
            return null;
        }


        bool IsOnThisGrid(IMyTerminalBlock b) { return Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }


        //-------------------------------------------------------------------------------
        //  CARRIAGE DOCK OPERATIONS
        //-------------------------------------------------------------------------------
        void UpdateDisplays() {
            _log.AppendLine($"   A2: {_A1?.Status?.Range2Bottom:N2} -- {_A1?.Status?.Range2Top:N2}");
            _log.AppendLine($"Maint: {_Maintenance?.Status?.Range2Bottom:N2} -- {_Maintenance?.Status?.Range2Top:N2}");
            var allCarriagesDisplay = Displays.BuildAllCarriagePositionSummary(_A1.Status, _A2.Status, _B1.Status, _B2.Status, _Maintenance.Status);
            var allCarriagesWideDisplay = Displays.BuildAllCarriagePositionSummaryWide(_A1.Status, _A2.Status, _B1.Status, _B2.Status, _Maintenance.Status);

            foreach (var d in _displays.Where(Displays.IsAllCarriagesDisplay)) { Write2MonospaceDisplay(d, allCarriagesDisplay, 1f); }
            foreach (var d in _displays.Where(Displays.IsAllCarriagesWideDisplay)) { Write2MonospaceDisplay(d, allCarriagesWideDisplay, 1f); }
        }
        void Write2MonospaceDisplay(IMyTextPanel display, string text, float fontSize) {
            LCDHelper.SetFont_Monospaced(display);
            LCDHelper.SetFontSize(display, fontSize);
            display.WritePublicText(text);
        }
    }
}
