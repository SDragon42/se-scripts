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
    class DockSecureModule
    {
        public const string CMD_DOCK = "dock";
        public const string CMD_UNDOCK = "undock";

        public bool Auto_On { get; private set; }
        public bool Auto_Off { get; private set; }

        public bool Thrusters_OnOff { get; private set; }
        public bool Gyros_OnOff { get; private set; }
        public bool Lights_OnOff { get; private set; }
        public bool Beacons_OnOff { get; private set; }
        public bool RadioAntennas_OnOff { get; private set; }
        public bool Sensors_OnOff { get; private set; }
        public bool OreDetectors_OnOff { get; private set; }
        public bool Spotlights_Off { get; private set; }

        readonly List<IMyFunctionalBlock> _buffer = new List<IMyFunctionalBlock>();
        readonly List<IMyTerminalBlock> _landingGearAndConnectors = new List<IMyTerminalBlock>();

        bool _wasLockedLastRun = false;

        public DockSecureModule()
        {
        }

        MyGridProgram thisObj;

        public void Init(MyGridProgram thisObj)
        {
            this.thisObj = thisObj;
            thisObj.GridTerminalSystem.GetBlocksOfType(_landingGearAndConnectors, IsConnectorOrLandingGear);
        }

        public void AutoDockUndock()
        {
            var isDockedNow = IsDocked();
            if (_wasLockedLastRun == isDockedNow) return;
            _wasLockedLastRun = isDockedNow;

            if (isDockedNow && Auto_Off)
                TurnOffSystems();
            else if (!isDockedNow && Auto_On)
                TurnOnSystems();
        }

        public void Dock()
        {
            if (IsDocked()) return;
            if (!IsReadyToDock()) return;
            TurnOffSystems();
            _landingGearAndConnectors.ForEach(b => b.ApplyAction("Lock"));
        }
        public void UnDock()
        {
            if (!IsDocked()) return;
            TurnOnSystems();
            _landingGearAndConnectors.ForEach(b => b.ApplyAction("Unlock"));
        }

        void TurnOffSystems()
        {
            thisObj.GridTerminalSystem.GetBlocksOfType(_buffer, IsBlock2TurnOFF);
            _buffer.ForEach(b => b.Enabled = false);
        }
        void TurnOnSystems()
        {
            thisObj.GridTerminalSystem.GetBlocksOfType(_buffer, IsBlock2TurnON);
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
            if (Thrusters_OnOff && b is IMyThrust) return true;
            if (Gyros_OnOff && b is IMyGyro) return true;
            if (Lights_OnOff && b is IMyInteriorLight) return true;
            if (Beacons_OnOff && b is IMyBeacon) return true;
            if (RadioAntennas_OnOff && b is IMyRadioAntenna) return true;
            if (Sensors_OnOff && b is IMySensorBlock) return true;
            if (OreDetectors_OnOff && b is IMyOreDetector) return true;
            return false;
        }
        bool IsBlock2TurnOFF(IMyTerminalBlock b)
        {
            if (!IsOnThisGrid(b)) return false;
            if (IsBlock2TurnON(b)) return true;
            if (Spotlights_Off && (b is IMyReflectorLight)) return true;
            return false;
        }
        bool IsOnThisGrid(IMyTerminalBlock b) { return thisObj.Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }
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
