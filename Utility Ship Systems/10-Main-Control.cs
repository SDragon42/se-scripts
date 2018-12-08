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
        public void Main(string argument, UpdateType updateSource) {
            _timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            _timeLastCleared += Runtime.TimeSinceLastRun.TotalSeconds;
            var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - _timeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);
            Echo($"Utility Ship Systems 1.6.3 {_running.GetSymbol(Runtime)}");
            Echo($"Scanning for blocks in {timeTilUpdate:N0} seconds.");
            Echo("");
            Echo("Configure script in 'Custom Data'");

            Flag_SaveConfig = false;
            LoadConfig();

            _reloadBlocks = (_timeLastBlockLoad >= BLOCK_RELOAD_TIME);
            _dockSecure.Init(this, _reloadBlocks);
            if (_reloadBlocks) {
                LoadBlocks();
                _timeLastBlockLoad = 0;

                if (InventoryMultiplier <= 0) {
                    var b = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCargoContainer>(Collect.IsCargoContainer);
                    if (b != null) {
                        InventoryMultiplier = CargoHelper.GetInventoryMultiplier(b);
                        Flag_SaveConfig = true;
                    }
                }
            }
            if (!MaxOperationalCargoMass.HasValue || MaxOperationalCargoMass.Value == 0) {
                MaxOperationalCargoMass = LiftCapacity.GetMaxMass(_sc, LiftThrusters, MinimumTWR, InventoryMultiplier);
                Flag_SaveConfig = true;
            }

            SaveConfig();

            if (argument.Length > 0) {
                switch (argument.ToLower()) {
                    case CMD_DOCK: _dockSecure.Dock(); break;
                    case CMD_UNDOCK: _dockSecure.UnDock(); break;
                    case CMD_DOCK_TOGGLE: _dockSecure.ToggleDock(); break;
                    case CMD_TOOLS_OFF: TurnOffTools(); break;
                    case CMD_SCAN: ScanAhead(); break;
                    case CMD_TOOLS_TOGGLE: ToggleToolsOnOff(); break;
                }
            }

            if ((updateSource & UpdateType.Update10) > 0) {
                _dockSecure.AutoToggleDock();
                UpdateProximity();
            }

            if (_timeLastCleared >= ForwardDisplayClearTime && _scanRangeText.Length > 0) {
                _scanRangeText = string.Empty;
                _timeLastCleared = 0;
            }
            foreach (var d in _displayList) {
                var isRange = IsForwardRangeBlock(d);
                var isProx = IsProximityBlock(d);

                if (isRange && (!isProx || _scanRangeText.Length > 0)) {
                    Write2ForeDisplay(d, _scanRangeText);
                    continue;
                }
                if (isProx) {
                    Write2ProximityDisplay(d, _proximityText);
                }
            }
        }

        void UpdateProximity() {
            if (!_dockSecure.IsDocked) {
                _proximity.RunScan(this, _sc, _proxCameraList);
                CheckAlert();
                _proximityText = BuildProximityDisplayText();
            } else {
                _proximityText = $"\n     Docked";
                TurnOffProximityAlert();
            }
        }
        void CheckAlert() {
            var speed = _sc.GetShipSpeed();
            var alertValid = false;
            alertValid |= SetAlert(Direction.Up, speed);
            if (alertValid) return;
            alertValid |= SetAlert(Direction.Down, speed);
            if (alertValid) return;
            alertValid |= SetAlert(Direction.Left, speed);
            if (alertValid) return;
            alertValid |= SetAlert(Direction.Right, speed);
            if (alertValid) return;
            alertValid |= SetAlert(Direction.Backward, speed);
            if (alertValid) return;
            TurnOffProximityAlert();
        }
        bool SetAlert(Direction dir, double speed) {
            var range = _proximity.GetRange(dir);
            var diff = _proximity.GetRangeDiff(dir);
            if (diff < 0 && speed >= ProximityAlertSpeed && range <= ProximityAlertRange) {
                TurnOnProximityAlert();
                return true;
            }
            return false;
        }
        void TurnOnProximityAlert() {
            if (_alertSounding)
                return;
            if (!ProximityAlert)
                return;
            _proxSpeakerList.ForEach(s => s.Play());
            _alertSounding = true;
        }
        void TurnOffProximityAlert() {
            if (_alertSounding)
                _proxSpeakerList.ForEach(s => s.Stop());
            _alertSounding = false;
        }
        string BuildProximityDisplayText() {
            var txtUp = GetFormattedRange(Direction.Up);
            var txtDown = GetFormattedRange(Direction.Down);
            var txtLeft = GetFormattedRange(Direction.Left);
            var txtRight = GetFormattedRange(Direction.Right);
            var txtBack = GetFormattedRange(Direction.Backward);
            var txtForward = GetFormattedRange(Direction.Forward);
            return $" {txtForward} {txtUp}\n {txtLeft}<{txtBack}>{txtRight}\n      {txtDown}";
        }
        string GetFormattedRange(Direction dir) {
            var range = _proximity.GetRange(dir);
            if (!range.HasValue) return "----";
            return (range.Value < 100.0)
                ? $"{range,4:N1}"
                : $"{range,4:N0}";
        }
        void Write2ProximityDisplay(IMyTextPanel display, string text) {
            display.Font = LCDFonts.MONOSPACE;
            display.FontSize = 1.65f;
            display.WritePublicText(text);
            display.ShowPublicTextOnScreen();
        }

        void ScanAhead() {
            if (_foreRangeCamera == null) return;
            _foreRangeInfo = Ranger.GetDetailedRange(_foreRangeCamera, ForwardScanRange);
            BuildForwardDisplayText();
            _timeLastCleared = 0;
        }
        void BuildForwardDisplayText() {
            _scanRangeText =
                $"Entity: {_foreRangeInfo.DetectedEntity.Type}\n" +
                $"Name: {_foreRangeInfo.DetectedEntity.Name}\n" +
                $"Range: {_foreRangeInfo.Range:N1} m";
        }
        void Write2ForeDisplay(IMyTextPanel display, string text) {
            display.Font = LCDFonts.DEBUG;
            display.FontSize = 1.7f;
            display.WritePublicText(text);
            display.ShowPublicTextOnScreen();
        }

        void TurnOffTools() {
            _toolList.ForEach(b => b.Enabled = false);
        }
        void ToggleToolsOnOff() {
            _toolList.ForEach(b => b.Enabled = !b.Enabled);
        }
    }
}
