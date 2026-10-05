// <mdk sortorder="1000" />
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

        class DockSecure {
            readonly IMyProgrammableBlock Me;
            readonly IMyGridTerminalSystem GridTerminalSystem;
            readonly List<IMyFunctionalBlock> _toggleBlocks = new List<IMyFunctionalBlock>();
            readonly List<IMyLandingGear> _landingGears = new List<IMyLandingGear>();
            readonly List<IMyShipConnector> _connectors = new List<IMyShipConnector>();
            readonly StateMachineQueue _operation = new StateMachineQueue();

            public DockSecure(IMyProgrammableBlock me, IMyGridTerminalSystem gridTerminalSystem) {
                Me = me;
                GridTerminalSystem = gridTerminalSystem;
            }


            bool _wasLockedLastRun = false;

            public string Tag { get; set; } = string.Empty;
            public string IgnoreTag { get; set; } = string.Empty;
            public bool EnableAutoOnOff { get; set; } = false;

            public bool Thrusters_OnOff { get; set; } = true;
            public bool Gyros_OnOff { get; set; } = true;
            public bool Lights_OnOff { get; set; } = true;
            public bool Beacons_OnOff { get; set; } = true;
            public bool RadioAntennas_OnOff { get; set; } = true;
            public bool Sensors_OnOff { get; set; } = true;
            public bool OreDetectors_OnOff { get; set; } = true;
            public bool Sorters_Off { get; set; } = true;
            public bool Spotlights_OnOff { get; set; } = true;

            public bool IsDocked { get; private set; }


            public void LoadBlocks() {
                GridTerminalSystem.GetBlocksOfType(_landingGears, IsValidBlock);
                GridTerminalSystem.GetBlocksOfType(_connectors, IsValidBlock);
            }

            public void RunUpdate() {
                _operation.Run();

                if (!EnableAutoOnOff) return;
                var isLocked = IsLocked();
                if (_wasLockedLastRun == isLocked) return;
                _wasLockedLastRun = isLocked;

                if (isLocked)
                    Dock(true);
                else
                    UnDock();
            }
            public void ToggleDock() {
                if (IsLocked())
                    UnDock();
                else
                    Dock();
            }
            public void Dock() => Dock(false);
            void Dock(bool forceTurnOff) {
                _operation.Clear();
                _operation.Add(DockOperations(forceTurnOff));
            }
            public void UnDock() {
                _operation.Clear();
                _operation.Add(UnDockOperations());
            }


            IEnumerator<bool> DockOperations(bool forceTurnOff = false) {
                _landingGears.ForEach(b => b.Lock());
                _connectors.ForEach(b => b.Connect());
                if (!IsLocked() && !forceTurnOff) yield break;

                IsDocked = true;

                if (Thrusters_OnOff) ToggleThrusters(false);
                if (Gyros_OnOff) ToggleGyros(false);
                if (Lights_OnOff) ToggleLights(false);
                if (Beacons_OnOff) ToggleBeacons(false);
                if (RadioAntennas_OnOff) ToggleRadioAntennas(false);
                if (Gyros_OnOff) ToggleGyros(false);
                if (Sensors_OnOff) ToggleSensors(false);
                if (OreDetectors_OnOff) ToggleOreDetectors(false);
                if (Spotlights_OnOff) ToggleSpotlights(false);
            }

            IEnumerator<bool> UnDockOperations() {
                if (Thrusters_OnOff) ToggleThrusters(true);
                if (Gyros_OnOff) ToggleGyros(true);
                if (Lights_OnOff) ToggleLights(true);
                if (Beacons_OnOff) ToggleBeacons(true);
                if (RadioAntennas_OnOff) ToggleRadioAntennas(true);
                if (Gyros_OnOff) ToggleGyros(true);
                if (Sensors_OnOff) ToggleSensors(true);
                if (OreDetectors_OnOff) ToggleOreDetectors(true);
                if (Spotlights_OnOff) ToggleSpotlights(true);
                yield return true;

                _landingGears.ForEach(b => b.Unlock());
                _connectors.ForEach(b => b.Disconnect());
                IsDocked = false;
            }

            bool IsLocked() => _connectors.Any(IsConnectorConnected) || _landingGears.Any(IsLandingGearLocked);

            bool IsValidBlock(IMyTerminalBlock b) {
                var sc = Me.IsSameConstructAs(b);
                var tagged = IsTagged(b, Tag);
                var ignored = !string.IsNullOrEmpty(IgnoreTag) && IsTagged(b, IgnoreTag);
                return (sc || tagged) && !ignored;
            }

            void ToggleThrusters(bool enabled) {
                GridTerminalSystem.GetBlocksOfType(_toggleBlocks, b => IsValidBlock(b) && b is IMyThrust);
                _toggleBlocks.ForEach(b => b.Enabled = enabled);
            }

            void ToggleGyros(bool enabled) {
                GridTerminalSystem.GetBlocksOfType(_toggleBlocks, b => IsValidBlock(b) && b is IMyGyro);
                _toggleBlocks.ForEach(b => b.Enabled = enabled);
            }

            void ToggleLights(bool enabled) {
                GridTerminalSystem.GetBlocksOfType(_toggleBlocks, b => IsValidBlock(b) && b is IMyInteriorLight);
                _toggleBlocks.ForEach(b => b.Enabled = enabled);
            }

            void ToggleBeacons(bool enabled) {
                GridTerminalSystem.GetBlocksOfType(_toggleBlocks, b => IsValidBlock(b) && b is IMyBeacon);
                _toggleBlocks.ForEach(b => b.Enabled = enabled);
            }

            void ToggleRadioAntennas(bool enabled) {
                GridTerminalSystem.GetBlocksOfType(_toggleBlocks, b => IsValidBlock(b) && b is IMyRadioAntenna);
                _toggleBlocks.ForEach(b => b.Enabled = enabled);
            }

            void ToggleSensors(bool enabled) {
                GridTerminalSystem.GetBlocksOfType(_toggleBlocks, b => IsValidBlock(b) && b is IMySensorBlock);
                _toggleBlocks.ForEach(b => b.Enabled = enabled);
            }

            void ToggleOreDetectors(bool enabled) {
                GridTerminalSystem.GetBlocksOfType(_toggleBlocks, b => IsValidBlock(b) && b is IMyOreDetector);
                _toggleBlocks.ForEach(b => b.Enabled = enabled);
            }

            void ToggleSpotlights(bool enabled) {
                GridTerminalSystem.GetBlocksOfType(_toggleBlocks, b => IsValidBlock(b) && b is IMyReflectorLight);
                _toggleBlocks.ForEach(b => b.Enabled = enabled);
            }

        }

    }
}
