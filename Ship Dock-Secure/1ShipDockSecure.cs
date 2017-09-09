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

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        const string CMD_DOCK = "dock";
        const string CMD_UNDOCK = "undock";

        readonly List<IMyFunctionalBlock> _buffer = new List<IMyFunctionalBlock>();
        readonly List<IMyTerminalBlock> _landingGearAndConnectors = new List<IMyTerminalBlock>();
        readonly RunningSymbolModule _runSymbol;
        readonly TimeIntervalModule _executionInterval;
        readonly DockSecureModule _dockSecure;
        readonly ScriptSettings _settings = new ScriptSettings();

        bool _wasLockedLastRun = false;

        public Program()
        {
            Echo = (t) => { }; // Disable Echo

            _runSymbol = new RunningSymbolModule();
            _executionInterval = new TimeIntervalModule();
            _dockSecure = new DockSecureModule();

            _settings.InitConfig(Me, SetExecutionInterval);
        }

        public void Main(string argument)
        {
            Echo("Dock-Secure v1.2 " + _runSymbol.GetSymbol(this.Runtime));
            _executionInterval.RecordTime(this.Runtime);
            if (!_executionInterval.AtNextInterval() && argument?.Length == 0) return;

            _settings.LoadConfig(Me, SetExecutionInterval);

            GridTerminalSystem.GetBlocksOfType(_landingGearAndConnectors, IsConnectorOrLandingGear);

            if (argument?.Length > 0)
            {
                switch (argument.ToLower())
                {
                    case CMD_DOCK: Dock(); return;
                    case CMD_UNDOCK: UnDock(); return;
                }
            }

            var isDockedNow = IsDocked();
            if (_wasLockedLastRun == isDockedNow) return;
            _wasLockedLastRun = isDockedNow;

            if (isDockedNow && _settings.Auto_Off)
                TurnOffSystems();
            else if (!isDockedNow && _settings.Auto_On)
                TurnOnSystems();
        }

        void SetExecutionInterval()
        {
            _executionInterval.SetNumIntervalsPerSecond(_settings.RunInterval);
        }

        void Dock()
        {
            if (IsDocked()) return;
            if (!IsReadyToDock()) return;
            TurnOffSystems();
            _landingGearAndConnectors.ForEach(b => b.ApplyAction("Lock"));
        }
        void UnDock()
        {
            if (!IsDocked()) return;
            TurnOnSystems();
            _landingGearAndConnectors.ForEach(b => b.ApplyAction("Unlock"));
        }

        void TurnOffSystems()
        {
            GridTerminalSystem.GetBlocksOfType(_buffer, IsBlock2TurnOFF);
            _buffer.ForEach(b => b.Enabled = false);
        }
        void TurnOnSystems()
        {
            GridTerminalSystem.GetBlocksOfType(_buffer, IsBlock2TurnON);
            _buffer.ForEach(b => b.Enabled = true);
        }

        bool IsConnectorOrLandingGear(IMyTerminalBlock b)
        {
            if (!IsOnThisGrid(b)) return false;
            if (b is IMyLandingGear) return true;
            if (b is IMyShipConnector) return true;
            return false;
        }
        bool IsDocked()
        {
            return _landingGearAndConnectors
                .Where(b => IsLandingGearLocked(b) || IsConnectorConnected(b))
                .Any();
        }
        bool IsReadyToDock()
        {
            return _landingGearAndConnectors
                .Where(b => IsLandingGearReadyToLock(b) || IsConnectorConnectable(b))
                .Any();
        }
        bool IsBlock2TurnON(IMyTerminalBlock b)
        {
            if (!IsOnThisGrid(b)) return false;
            if (_settings.Thrusters_OnOff && b is IMyThrust) return true;
            if (_settings.Gyros_OnOff && b is IMyGyro) return true;
            if (_settings.Lights_OnOff && b is IMyInteriorLight) return true;
            if (_settings.Beacons_OnOff && b is IMyBeacon) return true;
            if (_settings.RadioAntennas_OnOff && b is IMyRadioAntenna) return true;
            if (_settings.Sensors_OnOff && b is IMySensorBlock) return true;
            if (_settings.OreDetectors_OnOff && b is IMyOreDetector) return true;
            return false;
        }
        bool IsBlock2TurnOFF(IMyTerminalBlock b)
        {
            if (!IsOnThisGrid(b)) return false;
            if (IsBlock2TurnON(b)) return true;
            if (_settings.Spotlights_Off && (b is IMyReflectorLight)) return true;
            return false;
        }
        bool IsOnThisGrid(IMyTerminalBlock b) { return Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }
        bool IsConnectorConnectable(IMyTerminalBlock b)
        {
            var c = b as IMyShipConnector;
            if (c == null) return false;
            return (c.Status == MyShipConnectorStatus.Connectable);
        }
        bool IsConnectorConnected(IMyTerminalBlock b)
        {
            var c = b as IMyShipConnector;
            if (c == null) return false;
            return (c.Status == MyShipConnectorStatus.Connected);
        }
        bool IsLandingGearReadyToLock(IMyTerminalBlock b)
        {
            var lg = b as IMyLandingGear;
            if (lg == null) return false;
            return ((int)lg.LockMode == 1); //TODO: ReadyToLock - Workaround this this is fixed
        }
        bool IsLandingGearLocked(IMyTerminalBlock b)
        {
            var lg = b as IMyLandingGear;
            if (lg == null) return false;
            return ((int)lg.LockMode == 2); //TODO: Locked - Workaround this this is fixed
        }
    }
}