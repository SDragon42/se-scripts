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
    partial class Program {

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

        string BuildForwardDisplayText(MyDetectedEntityInfo detectedInfo, IMyCameraBlock camera) {
            if (detectedInfo.IsEmpty()) {
                return "\nNo Entity Detected";
            }
            var range = Vector3D.Distance(camera.GetPosition(), detectedInfo.HitPosition ?? detectedInfo.Position);
            return $"Entity: {detectedInfo.Type}\n" +
                $"Name: {detectedInfo.Name}\n" +
                $"Range: {TextHelper.FormatDistance(range)}";
        }

        string GetFormattedRange(Base6Directions.Direction dir) {
            var range = ProximityModule.GetRange(dir);
            if (!range.HasValue) return "----";
            return (range.Value < 100.0)
                ? $"{range,4:N1}"
                : $"{range,4:N0}";
        }

        void UpdateScreens() {
            foreach (var sc in _screenList) {
                if (sc.IsRange && (!sc.IsProx || _scanRangeText.Length > 0)) {
                    InitDisplay(sc.Screen, fontName: LCDFonts.DEBUG, fontSize: DISPLAY_RANGE_FONT_SIZE, alignment: TextAlignment.CENTER);
                    sc.Screen.WriteText(_scanRangeText);
                    continue;
                }
                if (sc.IsProx) {
                    InitDisplay(sc.Screen, fontName: LCDFonts.MONOSPACE, fontSize: DISPLAY_PROX_FONT_SIZE, alignment: TextAlignment.CENTER);
                    sc.Screen.WriteText(_proximityText);
                }
            }
        }

        void InitDisplay(IMyTextSurface display, string fontName = LCDFonts.DEBUG, float fontSize = 1f, TextAlignment alignment = TextAlignment.LEFT, float padding = 0f) {
            display.Font = fontName;
            display.TextPadding = padding;
            display.Alignment = alignment;
            display.ContentType = ContentType.TEXT_AND_IMAGE;

            if (display.TextureSize.X < DEFAULT_SCREEN_WIDTH) fontSize /= 2;
            display.FontSize = fontSize;
        }

        class ScreenConfig {
            public ScreenConfig(IMyTextSurface screen, bool isProx, bool isRange) {
                Screen = screen;
                IsProx = isProx;
                IsRange = isRange;
            }
            public IMyTextSurface Screen { get; private set; }
            public bool IsProx { get; private set; }
            public bool IsRange { get; private set; }
        }

    }
}
