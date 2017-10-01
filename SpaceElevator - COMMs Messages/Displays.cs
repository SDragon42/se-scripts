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
    static class Displays {

        // ┌┬┐   250c 252c 2510
        // ├┼┤│  251c 253c 2524 2502
        // └┴┘   2514 2534 2518

        const string CHR_UP = "↑↑"; //"\u2191\u2191";
        const string CHR_DOWN = "↓↓"; //"\u2193\u2193";
        const string CHRS_Carriage_White = "";
        const string CHRS_Carriage_Red = ""; //"\uE050\uE03C";
        const string CHRS_Carriage_Green = ""; //"\uE051\uE03D";
        const string CHRS_Carriage_Blue = ""; //"\uE052\uE03E";
        const string CHRS_Carriage_Yellow = "";
        const string CHRS_Carriage_Magenta = "";
        const string CHRS_Carriage_Cyan = "";


        public static bool IsAllCarriagesDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains("[all-carriages]"); }
        //public static bool IsAllCarriagesWideDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains("[all-carriages-wide]"); }

        public static string BuildAllCarriagePositionSummary(CarriageStatusMessage a1, CarriageStatusMessage a2, CarriageStatusMessage b1, CarriageStatusMessage b2, CarriageStatusMessage maint) {
            const int max = 15;
            // Monospace, fontsize 1.0

            var a1Info = GetGraphInfo(a1, max);
            var a2Info = GetGraphInfo(a2, max);
            var b1Info = GetGraphInfo(b1, max);
            var b2Info = GetGraphInfo(b2, max);
            var maintInfo = GetGraphInfo(maint, max);

            var sb = new StringBuilder();
            sb.AppendLine(" A2   A1   Maint  B1   B2");
            sb.AppendLine($"{a2Info.Altitude} {a1Info.Altitude}  {maintInfo.Altitude}  {b1Info.Altitude} {b2Info.Altitude}");
            sb.AppendLine($" {a2Info.DirText}   {a1Info.DirText}    {maintInfo.DirText}    {b1Info.DirText}   {b2Info.DirText}");
            for (var i = max - 1; i >= 0; i--) {
                var a1Text = (i == a1Info.VertPosNum) ? a1Info.Icon : "  ";
                var a2Text = (i == a2Info.VertPosNum) ? a2Info.Icon : "  ";
                var b1Text = (i == b1Info.VertPosNum) ? b1Info.Icon : "  ";
                var b2Text = (i == b2Info.VertPosNum) ? b2Info.Icon : "  ";
                var maintText = (i == maintInfo.VertPosNum) ? maintInfo.Icon : "  ";

                if (i == max - 1)
                    sb.AppendLine($"┌{a2Text}┐ ┌{a1Text}┐  ┌{maintText}┐  ┌{b1Text}┐ ┌{b2Text}┐");
                else if (i == 0)
                    sb.AppendLine($"└{a2Text}┘ └{a1Text}┘  └{maintText}┘  └{b1Text}┘ └{b2Text}┘");
                else if (i == max / 2)
                    sb.AppendLine($"│{a2Text}│ │{a1Text}│  ├{maintText}┤  │{b1Text}│ │{b2Text}│");
                else
                    sb.AppendLine($"│{a2Text}│ │{a1Text}│  │{maintText}│  │{b1Text}│ │{b2Text}│");
            }

            return sb.ToString();
        }

        static string GetDirectionArrows(CarriageStatusMessage carriage) {
            var vspeed = Math.Round(carriage.VerticalSpeed, 1);
            return vspeed > 0 ? CHR_UP : vspeed < 0 ? CHR_DOWN : "  ";
        }
        static string GetCarriageIcon(CarriageStatusMessage carriage) {
            switch (carriage.Mode) {
                case CarriageMode.Manual_Control: return CHRS_Carriage_Magenta;
                case CarriageMode.Awaiting_DepartureClearance: return CHRS_Carriage_Yellow;
                case CarriageMode.Awaiting_CarriageReady2Depart: return CHRS_Carriage_Yellow;
                case CarriageMode.Transit_Powered: return CHRS_Carriage_Yellow;
                case CarriageMode.Transit_Coast: return CHRS_Carriage_Yellow;
                case CarriageMode.Transit_Slow2Approach: return CHRS_Carriage_Yellow;
                case CarriageMode.Transit_Docking: return CHRS_Carriage_Yellow;
                case CarriageMode.Docked: return CHRS_Carriage_Green;
                default: return CHRS_Carriage_White;
            }
        }
        static int GetCarriagePositionIndex(CarriageStatusMessage carriage, int numLines) {
            var totalDist = carriage.Range2Bottom + carriage.Range2Top;
            var percent = carriage.Range2Bottom / totalDist;
            return Convert.ToInt32(Math.Round((numLines - 1) * percent, 0));
        }
        static CarriageGraphInfo GetGraphInfo(CarriageStatusMessage carriage, int numLines) {
            if (carriage == null) return new CarriageGraphInfo();
            return new CarriageGraphInfo() {
                DirText = GetDirectionArrows(carriage),
                Altitude = $"{carriage.Range2Bottom / 1000.0,4:N1}",
                Icon = GetCarriageIcon(carriage),
                VertPosNum = GetCarriagePositionIndex(carriage, numLines)
            };

        }

        class CarriageGraphInfo {
            public CarriageGraphInfo() {
                DirText = "  ";
                Altitude = "--.-";
                Icon = "  ";
                VertPosNum = -1;
            }

            public string DirText { get; set; }
            public string Altitude { get; set; }
            public string Icon { get; set; }
            public int VertPosNum { get; set; }
        }
    }
}
