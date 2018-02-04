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
        class DockSecure {
            readonly List<IMyFunctionalBlock> _toggleBlocks = new List<IMyFunctionalBlock>();
            readonly List<IMyLandingGear> _landingGears = new List<IMyLandingGear>();
            readonly List<IMyShipConnector> _connectors = new List<IMyShipConnector>();

            MyGridProgram thisObj;
            bool _wasLockedLastRun = false;
            bool _isLocked = false;

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

            public bool IsDocked { get; private set; }


            public void Init(MyGridProgram thisObj, bool findBlocks = true) {
                this.thisObj = thisObj;
                if (!findBlocks) return;
                thisObj.GridTerminalSystem.GetBlocksOfType(_landingGears, IsOnThisGrid);
                thisObj.GridTerminalSystem.GetBlocksOfType(_connectors, IsOnThisGrid);
            }
            public void AutoToggleDock() {
                CheckIfLocked();
                if (_wasLockedLastRun == _isLocked) return;
                _wasLockedLastRun = _isLocked;

                if (_isLocked) {
                    if (Auto_Off) {
                        TurnOffSystems();
                        IsDocked = true;
                    }
                } else  {
                    if (Auto_On)
                        TurnOnSystems();
                    IsDocked = false;
                }
            }
            public void ToggleDock() {
                CheckIfLocked();
                if (_isLocked)
                    UnDock();
                else
                    Dock();
            }
            public void Dock() {
                _landingGears.ForEach(b => b.Lock());
                _connectors.ForEach(b => b.Connect());
                CheckIfLocked();
                if (_isLocked) {
                    TurnOffSystems();
                    IsDocked = true;
                }
            }
            public void UnDock() {
                TurnOnSystems();
                _landingGears.ForEach(b => b.Unlock());
                _connectors.ForEach(b => b.Disconnect());
                _isLocked = false;
                IsDocked = false;
            }


            void TurnOffSystems() {
                thisObj.GridTerminalSystem.GetBlocksOfType(_toggleBlocks, IsBlock2TurnOFF);
                _toggleBlocks.ForEach(b => b.Enabled = false);
            }
            void TurnOnSystems() {
                thisObj.GridTerminalSystem.GetBlocksOfType(_toggleBlocks, IsBlock2TurnON);
                _toggleBlocks.ForEach(b => b.Enabled = true);
            }

            void CheckIfLocked() {
                _isLocked = _connectors.Where(Collect.IsConnectorConnected).Any();
                if (_isLocked) {
                    IsDocked = true;
                    return;
                }
                _isLocked = _landingGears.Where(Collect.IsLandingGearLocked).Any();
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

            bool IsOnThisGrid(IMyTerminalBlock b) => thisObj.Me.CubeGrid == b.CubeGrid;

        }
    }
}
