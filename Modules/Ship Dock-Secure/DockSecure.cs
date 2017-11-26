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
    class DockSecure {
        readonly List<IMyFunctionalBlock> _toggleBlocks = new List<IMyFunctionalBlock>();
        readonly List<IMyLandingGear> _landingGears = new List<IMyLandingGear>();
        readonly List<IMyShipConnector> _connectors = new List<IMyShipConnector>();

        bool _wasLockedLastRun = false;
        MyGridProgram thisObj;

        public bool Auto_On { get; set; }
        public bool Auto_Off { get; set; }
        public bool Thrusters_OnOff { get; set; }
        public bool Gyros_OnOff { get; set; }
        public bool Lights_OnOff { get; set; }
        public bool Beacons_OnOff { get; set; }
        public bool RadioAntennas_OnOff { get; set; }
        public bool Sensors_OnOff { get; set; }
        public bool OreDetectors_OnOff { get; set; }
        public bool Sorters_Off { get; set; }
        public bool Spotlights_Off { get; set; }



        public void Init(MyGridProgram thisObj) {
            this.thisObj = thisObj;
            thisObj.GridTerminalSystem.GetBlocksOfType(_landingGears, IsOnThisGrid);
            thisObj.GridTerminalSystem.GetBlocksOfType(_connectors, IsOnThisGrid);
        }
        public void AutoDockUndock() {
            var isDockedNow = IsDocked();
            if (_wasLockedLastRun == isDockedNow) return;
            _wasLockedLastRun = isDockedNow;

            if (isDockedNow && Auto_Off)
                TurnOffSystems();
            else if (!isDockedNow && Auto_On)
                TurnOnSystems();
        }
        public void DockUndock() {
            if (IsDocked())
                UnDock();
            else
                Dock();
        }
        public void Dock() {
            var good2Go = IsDocked() || IsReadyToDock();
            if (!good2Go) return;
            TurnOffSystems();
            _landingGears.ForEach(b => b.Lock());
            _connectors.ForEach(b => b.Connect());
        }
        public void UnDock() {
            TurnOnSystems();
            _landingGears.ForEach(b => b.Unlock());
            _connectors.ForEach(b => b.Disconnect());
        }


        void TurnOffSystems() {
            thisObj.GridTerminalSystem.GetBlocksOfType(_toggleBlocks, IsBlock2TurnOFF);
            _toggleBlocks.ForEach(b => b.Enabled = false);
        }
        void TurnOnSystems() {
            thisObj.GridTerminalSystem.GetBlocksOfType(_toggleBlocks, IsBlock2TurnON);
            _toggleBlocks.ForEach(b => b.Enabled = true);
        }


        bool IsDocked() {
            var docked = _landingGears.Where(Collect.IsLandingGearLocked).Any();
            docked |= _connectors.Where(Collect.IsConnectorConnected).Any();
            return docked;
        }
        bool IsReadyToDock() {
            var ready = _landingGears.Where(Collect.IsLandingGearReadyToLock).Any();
            ready |= _connectors.Where(Collect.IsConnectorConnectable).Any();
            return ready;
        }

        bool IsBlock2TurnON(IMyTerminalBlock b) {
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
        bool IsBlock2TurnOFF(IMyTerminalBlock b) {
            if (!IsOnThisGrid(b)) return false;
            if (IsBlock2TurnON(b)) return true;
            if (Spotlights_Off && (b is IMyReflectorLight)) return true;
            if (Sorters_Off && (b is IMyConveyorSorter)) return true;
            return false;
        }

        bool IsOnThisGrid(IMyTerminalBlock b) { return thisObj.Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }

    }
}
