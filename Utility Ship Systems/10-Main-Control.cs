using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {

        public void Main(string argument, UpdateType updateSource) {
            // Initialize the script and load configuration if necessary
            _timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            _timeLastCleared += Runtime.TimeSinceLastRun.TotalSeconds;
            var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - _timeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);

            Echo("Utility Ship Systems $SCRIPT_VERSION$ " + RunningModule.GetSymbol());
            Echo($"Scanning for blocks in {timeTilUpdate:N0} seconds.\n");
            Echo("Configure script in 'Custom Data'\n");
            Echo(_instructions);

            // Load the configuration
            //Flag_SaveConfig = false;
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load(DockSecureModule, ProximityModule);

            // Load blocks if necessary
            LoadBlocks();

            //if (!MaxOperationalCargoMass.HasValue || MaxOperationalCargoMass.Value == 0) {
            //    MaxOperationalCargoMass = ThrusterHelper.CalculateMaxLiftableCargoMass(_sc, _liftThrusters, InventoryMultiplier, MinimumTWR);
            //    Flag_SaveConfig = true;
            //}

            //if (Flag_SaveConfig) _config.Save();

            // Handle Script Commands
            if (_commands.ContainsKey(argument)) _commands[argument]?.Invoke();

            // Automatic Operations
            DockSecureModule.AutoToggleDock();
            UpdateProximity();
            Runtime.UpdateFrequency = DockSecureModule.IsDocked ? FREQ_DOCKED : FREQ_NORMAL;

            if (_timeLastCleared >= _config.ForwardScanRangeDisplayTime && _scanRangeText.Length > 0) {
                _scanRangeText = string.Empty;
                _timeLastCleared = 0;
            }

            UpdateScreens();
        }

        void UpdateProximity() {
            if (!DockSecureModule.IsDocked) {
                ProximityModule.RunScan(this, _sc, _proximityCameraList);
                CheckAlert();
                _proximityText = BuildProximityDisplayText();
            } else {
                _proximityText = "\nDocked\n";
                TurnOffProximityAlert();
            }
        }
        void CheckAlert() {
            var speed = _sc.GetShipSpeed();
            foreach (var dir in Base6Directions.EnumDirections) {
                if (SetAlert(dir, speed)) return;
            }
            TurnOffProximityAlert();
        }
        bool SetAlert(Base6Directions.Direction dir, double speed) {
            var range = ProximityModule.GetRange(dir);
            var diff = ProximityModule.GetRangeDiff(dir);
            if (diff < 0 && speed >= _config.ProximityAlertSpeed && range <= _config.ProximityAlertRange) {
                TurnOnProximityAlert();
                return true;
            }
            return false;
        }
        void TurnOnProximityAlert() {
            if (_alertSounding)
                return;
            if (!_config.ProximityAlert)
                return;
            _proximitySpeakerList.ForEach(s => s.Play());
            _alertSounding = true;
        }
        void TurnOffProximityAlert() {
            if (_alertSounding)
                _proximitySpeakerList.ForEach(s => s.Stop());
            _alertSounding = false;
        }

        void ScanAhead() {
            if (_foreRangeCamera == null) return;
            MyDetectedEntityInfo _foreRangeInfo;
            Ranger.TryGetDetailedRange(_foreRangeCamera, _config.ForwardScanRange, out _foreRangeInfo);
            _scanRangeText = BuildForwardDisplayText(_foreRangeInfo, _foreRangeCamera);
            _timeLastCleared = 0;
        }

        void TurnOffTools() => _toolList.ForEach(b => b.Enabled = false);
        void ToggleToolsOnOff() => _toolList.ForEach(b => b.Enabled = !b.Enabled);

    }
}
