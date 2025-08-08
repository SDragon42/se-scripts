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
            if (argument != string.Empty) argument = argument.ToLower();
            TimeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            TimeLastCleared += Runtime.TimeSinceLastRun.TotalSeconds;
            var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - TimeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);

            Echo($"Utility Ship Systems 1.6.9 {RunningModule.GetSymbol()}");
            Echo($"Scanning for blocks in {timeTilUpdate:N0} seconds.\n");
            Echo("Configure script in 'Custom Data'\n");
            Echo(Instructions);

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
                MaxOperationalCargoMass = ThrusterHelper.GetMaxMass(Sc, LiftThrusters, MinimumTWR, InventoryMultiplier);
                Flag_SaveConfig = true;
            }

            SaveConfig();

            if (Commands.ContainsKey(argument)) Commands[argument]?.Invoke();

            DockSecureModule.AutoToggleDock();
            UpdateProximity();
            Runtime.UpdateFrequency = DockSecureModule.IsDocked ? UpdateFrequency.Update100 : UpdateFrequency.Update10;

            if (TimeLastCleared >= ForwardDisplayClearTime && ScanRangeText.Length > 0) {
                ScanRangeText = string.Empty;
                TimeLastCleared = 0;
            }

            foreach (var sc in ScreenList) {
                if (sc.IsRange && (!sc.IsProx || ScanRangeText.Length > 0)) {
                    InitDisplay(sc.Screen, fontName: LCDFonts.DEBUG, fontSize: DISPLAY_RANGE_FONT_SIZE, alignment: TextAlignment.CENTER);
                    sc.Screen.WriteText(ScanRangeText);
                    continue;
                }
                if (sc.IsProx) {
                    InitDisplay(sc.Screen, fontName: LCDFonts.MONOSPACE, fontSize: DISPLAY_PROX_FONT_SIZE, alignment: TextAlignment.CENTER);
                    sc.Screen.WriteText(ProximityText);
                }
            }
        }

        void UpdateProximity() {
            if (!DockSecureModule.IsDocked) {
                ProximityModule.RunScan(this, Sc, ProxCameraList);
                CheckAlert();
                ProximityText = BuildProximityDisplayText();
            } else {
                ProximityText = "\nDocked\n";
                TurnOffProximityAlert();
            }
        }
        void CheckAlert() {
            var speed = Sc.GetShipSpeed();
            foreach (var dir in Base6Directions.EnumDirections) {
                if (SetAlert(dir, speed)) return;
            }
            TurnOffProximityAlert();
        }
        bool SetAlert(Base6Directions.Direction dir, double speed) {
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
            var txtUp = GetFormattedRange(Base6Directions.Direction.Up);
            var txtDown = GetFormattedRange(Base6Directions.Direction.Down);
            var txtLeft = GetFormattedRange(Base6Directions.Direction.Left);
            var txtRight = GetFormattedRange(Base6Directions.Direction.Right);
            var txtBack = GetFormattedRange(Base6Directions.Direction.Backward);
            var txtForward = GetFormattedRange(Base6Directions.Direction.Forward);
            var txtForward2 = string.Empty.PadRight(txtForward.Length, ' ');
            return $"{txtForward} {txtUp} {txtForward2}\n{txtLeft}<{txtBack}>{txtRight}\n{txtDown}";
        }
        string GetFormattedRange(Base6Directions.Direction dir) {
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
        void InitDisplay(IMyTextSurface display, string fontName = LCDFonts.DEBUG, float fontSize = 1f, TextAlignment alignment = TextAlignment.LEFT, float padding = 0f) {
            display.Font = fontName;
            display.TextPadding = padding;
            display.Alignment = alignment;
            display.ContentType = ContentType.TEXT_AND_IMAGE;

            if (display.TextureSize.X < DEFAULT_SCREEN_WIDTH) fontSize /= 2;
            display.FontSize = fontSize;
        }

        void TurnOffTools() {
            ToolList.ForEach(b => b.Enabled = false);
        }
        void ToggleToolsOnOff() {
            ToolList.ForEach(b => b.Enabled = !b.Enabled);
        }
    }
}
