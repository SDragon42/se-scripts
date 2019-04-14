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
            argument = argument.ToLower();
            TimeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            TimeLastCleared += Runtime.TimeSinceLastRun.TotalSeconds;
            DisplayInstructions();

            Flag_SaveConfig = false;
            LoadConfig();

            var reloadBlocks = (TimeLastBlockLoad >= BLOCK_RELOAD_TIME);
            DockSecureModule.Init(this, reloadBlocks);
            if (reloadBlocks) {
                LoadBlocks();
                TimeLastBlockLoad = 0;

                if (InventoryMultiplier <= 0) {
                    var b = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCargoContainer>(Collect.IsCargoContainer);
                    if (b != null) {
                        InventoryMultiplier = CargoHelper.GetInventoryMultiplier(b);
                        Flag_SaveConfig = true;
                    }
                }
            }
            if (!MaxOperationalCargoMass.HasValue || MaxOperationalCargoMass.Value == 0) {
                MaxOperationalCargoMass = LiftCapacity.GetMaxMass(Sc, LiftThrusters, MinimumTWR, InventoryMultiplier);
                Flag_SaveConfig = true;
            }

            SaveConfig();

            if (Commands.ContainsKey(argument)) Commands[argument]?.Invoke();

            if ((updateSource & UpdateType.Update10) > 0) {
                DockSecureModule.AutoToggleDock();
                UpdateProximity();
            }

            if (TimeLastCleared >= ForwardDisplayClearTime && ScanRangeText.Length > 0) {
                ScanRangeText = string.Empty;
                TimeLastCleared = 0;
            }
            foreach (var d in DisplayList) {
                var isRange = IsForwardRangeBlock(d);
                var isProx = IsProximityBlock(d);

                if (isRange && (!isProx || ScanRangeText.Length > 0)) {
                    Write2Display(d, ScanRangeText, fontSize: 1.7f);
                    continue;
                }
                if (isProx) {
                    Write2Display(d, ProximityText, fontName: LCDFonts.MONOSPACE, fontSize: 1.65f);
                }
            }
        }
        void DisplayInstructions() {
            var text = new StringBuilder();
            Action<string> Write = (t) => text.AppendLine(t);
            var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - TimeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);

            Write($"Utility Ship Systems 1.6.4 {RunningModule.GetSymbol(Runtime)}");
            Write($"Scanning for blocks in {timeTilUpdate:N0} seconds.\n");
            Write("Configure script in 'Custom Data'\n");
            Write("Script Commands");
            foreach (var c in Commands.Keys) Write(c);

            Echo(text.ToString());
        }

        void UpdateProximity() {
            if (!DockSecureModule.IsDocked) {
                ProximityModule.RunScan(this, Sc, ProxCameraList);
                CheckAlert();
                ProximityText = BuildProximityDisplayText();
            } else {
                ProximityText = $"\n     Docked";
                TurnOffProximityAlert();
            }
        }
        void CheckAlert() {
            var speed = Sc.GetShipSpeed();
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
            var range = ProximityModule.GetRange(dir);
            var diff = ProximityModule.GetRangeDiff(dir);
            if (diff < 0 && speed >= ProximityAlertSpeed && range <= ProximityAlertRange) {
                TurnOnProximityAlert();
                return true;
            }
            return false;
        }
        void TurnOnProximityAlert() {
            if (AlertSounding)
                return;
            if (!ProximityAlert)
                return;
            ProxSpeakerList.ForEach(s => s.Play());
            AlertSounding = true;
        }
        void TurnOffProximityAlert() {
            if (AlertSounding)
                ProxSpeakerList.ForEach(s => s.Stop());
            AlertSounding = false;
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
            var range = ProximityModule.GetRange(dir);
            if (!range.HasValue) return "----";
            return (range.Value < 100.0)
                ? $"{range,4:N1}"
                : $"{range,4:N0}";
        }

        void ScanAhead() {
            if (ForeRangeCamera == null) return;
            ForeRangeInfo = Ranger.GetDetailedRange(ForeRangeCamera, ForwardScanRange);
            BuildForwardDisplayText();
            TimeLastCleared = 0;
        }
        void BuildForwardDisplayText() {
            ScanRangeText =
                $"Entity: {ForeRangeInfo.DetectedEntity.Type}\n" +
                $"Name: {ForeRangeInfo.DetectedEntity.Name}\n" +
                $"Range: {ForeRangeInfo.Range:N1} m";
        }
        void Write2Display(IMyTextSurface display, string text, string fontName = LCDFonts.DEBUG, float fontSize = 1f) {
            display.Font = fontName;
            display.FontSize = fontSize;
            display.WriteText(text);
            display.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        }

        void TurnOffTools() {
            ToolList.ForEach(b => b.Enabled = false);
        }
        void ToggleToolsOnOff() {
            ToolList.ForEach(b => b.Enabled = !b.Enabled);
        }
    }
}
