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
        public const string DISPLAY_KEY_ALL_CARRIAGES = "[all-carriages]";
        public const string DISPLAY_KEY_ALL_CARRIAGES_WIDE = "[all-carriages-wide]";
        public const string DISPLAY_KEY_ALL_PASSENGER_CARRIAGES = "[all-passenger-carriages]";
        public const string DISPLAY_KEY_ALL_PASSENGER_CARRIAGES_WIDE = "[all-passenger-carriages-wide]";
        public const string DISPLAY_KEY_SINGLE_CARRIAGE = "[single-carriage]";
        public const string DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL = "[single-carriage-detail]";


        public static bool IsAllCarriagesDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains(DISPLAY_KEY_ALL_CARRIAGES); }
        public static bool IsAllCarriagesWideDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains(DISPLAY_KEY_ALL_CARRIAGES_WIDE); }
        public static string BuildAllCarriageDisplayText(CarriageStatusMessage a1, CarriageStatusMessage a2, CarriageStatusMessage b1, CarriageStatusMessage b2, CarriageStatusMessage maint, bool wide = false) {
            const int max = 15;

            var a1Info = GetGraphInfo(a1, max);
            var a2Info = GetGraphInfo(a2, max);
            var b1Info = GetGraphInfo(b1, max);
            var b2Info = GetGraphInfo(b2, max);
            var maintInfo = GetGraphInfo(maint, max);

            IEnumerator<string> detailLines = (wide) ? GetAllCarriagesDetailLines(a1, a2, b1, b2, maint).GetEnumerator() : null;
            Func<string> GetNext;
            if (wide)
                GetNext = () => detailLines?.MoveNext() ?? false ? detailLines.Current : " │ ";
            else
                GetNext = () => string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine(" A2   A1   Maint  B1   B2 " + GetNext());
            sb.AppendLine($"{a2Info.Altitude} {a1Info.Altitude}  {maintInfo.Altitude}  {b1Info.Altitude} {b2Info.Altitude}" + GetNext());
            sb.AppendLine($" {a2Info.DirText}   {a1Info.DirText}    {maintInfo.DirText}    {b1Info.DirText}   {b2Info.DirText} " + GetNext());
            for (var i = max - 1; i >= 0; i--) {
                var a1Text = (i == a1Info.VertPosNum) ? a1Info.Icon : "  ";
                var a2Text = (i == a2Info.VertPosNum) ? a2Info.Icon : "  ";
                var b1Text = (i == b1Info.VertPosNum) ? b1Info.Icon : "  ";
                var b2Text = (i == b2Info.VertPosNum) ? b2Info.Icon : "  ";
                var maintText = (i == maintInfo.VertPosNum) ? maintInfo.Icon : "  ";

                if (i == max - 1)
                    sb.AppendLine($"┌{a2Text}┐ ┌{a1Text}┐  ┌{maintText}┐  ┌{b1Text}┐ ┌{b2Text}┐" + GetNext());
                else if (i == 0)
                    sb.AppendLine($"└{a2Text}┘ └{a1Text}┘  └{maintText}┘  └{b1Text}┘ └{b2Text}┘" + GetNext());
                else if (i == max / 2)
                    sb.AppendLine($"│{a2Text}│ │{a1Text}│  ├{maintText}┤  │{b1Text}│ │{b2Text}│" + GetNext());
                else
                    sb.AppendLine($"│{a2Text}│ │{a1Text}│  │{maintText}│  │{b1Text}│ │{b2Text}│" + GetNext());
            }
            return sb.ToString();
        }
        static IEnumerable<string> GetAllCarriagesDetailLines(CarriageStatusMessage a1, CarriageStatusMessage a2, CarriageStatusMessage b1, CarriageStatusMessage b2, CarriageStatusMessage maint) {
            yield return " │ ";
            yield return " │ ";
            yield return " │ ";
            foreach (var txt in GetCarriageDetails("Carriage A2", a2)) yield return " │ " + txt;
            foreach (var txt in GetCarriageDetails("Carriage A1", a1)) yield return " │ " + txt;
            foreach (var txt in GetCarriageDetails("Maintenance Carriage", maint)) yield return " │ " + txt;
            foreach (var txt in GetCarriageDetails("Carriage B1", b1)) yield return " │ " + txt;
            foreach (var txt in GetCarriageDetails("Carriage B2", b2)) yield return " │ " + txt;
        }


        public static bool IsAllPassengerCarriagesDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains(DISPLAY_KEY_ALL_PASSENGER_CARRIAGES); }
        public static bool IsAllPassengerCarriagesWideDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains(DISPLAY_KEY_ALL_PASSENGER_CARRIAGES_WIDE); }
        public static string BuildAllPassengerCarriageDisplayText(CarriageStatusMessage a1, CarriageStatusMessage a2, CarriageStatusMessage b1, CarriageStatusMessage b2, bool wide = false) {
            const int max = 15;

            var a1Info = GetGraphInfo(a1, max);
            var a2Info = GetGraphInfo(a2, max);
            var b1Info = GetGraphInfo(b1, max);
            var b2Info = GetGraphInfo(b2, max);

            IEnumerator<string> detailLines = (wide) ? GetPassengerCarriagesDetailLines(a1, a2, b1, b2).GetEnumerator() : null;
            Func<string> GetNext;
            if (wide)
                GetNext = () => detailLines?.MoveNext() ?? false ? detailLines.Current : " │ ";
            else
                GetNext = () => string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine(" A2     A1      B1     B2 " + GetNext());
            sb.AppendLine($"{a2Info.Altitude}   {a1Info.Altitude}    {b1Info.Altitude}   {b2Info.Altitude}" + GetNext());
            sb.AppendLine($" {a2Info.DirText}     {a1Info.DirText}      {b1Info.DirText}     {b2Info.DirText} " + GetNext());
            for (var i = max - 1; i >= 0; i--) {
                var a1Text = (i == a1Info.VertPosNum) ? a1Info.Icon : "  ";
                var a2Text = (i == a2Info.VertPosNum) ? a2Info.Icon : "  ";
                var b1Text = (i == b1Info.VertPosNum) ? b1Info.Icon : "  ";
                var b2Text = (i == b2Info.VertPosNum) ? b2Info.Icon : "  ";

                if (i == max - 1)
                    sb.AppendLine($"┌{a2Text}┐   ┌{a1Text}┐    ┌{b1Text}┐   ┌{b2Text}┐" + GetNext());
                else if (i == 0)
                    sb.AppendLine($"└{a2Text}┘   └{a1Text}┘    └{b1Text}┘   └{b2Text}┘" + GetNext());
                else
                    sb.AppendLine($"│{a2Text}│   │{a1Text}│    │{b1Text}│   │{b2Text}│" + GetNext());
            }
            return sb.ToString();
        }
        static IEnumerable<string> GetPassengerCarriagesDetailLines(CarriageStatusMessage a1, CarriageStatusMessage a2, CarriageStatusMessage b1, CarriageStatusMessage b2) {
            yield return " │ ";
            yield return " │ ";
            yield return " │ ";
            foreach (var txt in GetCarriageDetails("Carriage A2", a2)) yield return " │ " + txt;
            yield return " │ ";
            foreach (var txt in GetCarriageDetails("Carriage A1", a1)) yield return " │ " + txt;
            yield return " │ ";
            foreach (var txt in GetCarriageDetails("Carriage B1", b1)) yield return " │ " + txt;
            yield return " │ ";
            foreach (var txt in GetCarriageDetails("Carriage B2", b2)) yield return " │ " + txt;
        }


        public static bool IsSingleCarriageDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains(DISPLAY_KEY_SINGLE_CARRIAGE); }
        public static bool IsSingleCarriageDetailDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains(DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL); }
        public static string BuildOneCarriageDisplay(string carriageName, CarriageStatusMessage carriageStatus, bool opsDetail = false, bool retransRingMarker = false) {
            const int max = 17;

            var statusInfo = GetGraphInfo(carriageStatus, max);

            var detailLines = GetOneCarriagesDetails(carriageName, carriageStatus, opsDetail).GetEnumerator();
            Func<string> GetNext = () => detailLines?.MoveNext() ?? false ? detailLines.Current : string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine($"{statusInfo.DirText,25}");
            for (var i = max - 1; i >= 0; i--) {
                var icon = (i == statusInfo.VertPosNum) ? statusInfo.Icon : "  ";

                if (i == max - 1)
                    sb.AppendLine($"     Space Terminal ──┬{icon}┐");
                else if (i == 0)
                    sb.AppendLine($"    Ground Terminal ──┴{icon}┘");
                else if (retransRingMarker && i == max / 2)
                    sb.AppendLine($"{GetNext(),-22}├{icon}┤");
                else
                    sb.AppendLine($"{GetNext(),-22}│{icon}│");
            }
            return sb.ToString();
        }
        static IEnumerable<string> GetOneCarriagesDetails(string carriageName, CarriageStatusMessage status, bool opsDetail = false) {
            var velocityText = status != null
                ? $"{GetDirectionArrows(status.VerticalSpeed)} {Math.Abs(status.VerticalSpeed):N1}"
                : "---.-";
            var altitudeText = status != null ? $"{status?.Range2Bottom:N1}" : "--,---";

            if (!opsDetail) {
                yield return "";
                yield return "";
            }
            yield return "";
            yield return "";
            yield return "";
            yield return carriageName;
            yield return "";
            yield return $"Speed: {velocityText,8} m/s";
            yield return $" Alt.: {altitudeText,8} m";
            if (opsDetail) {
                var fuelLevelText = status != null ? $"{status.FuelLevel * 100:N0}" : "---";
                var cargoMassText = status != null ? $"{status.CargoMass:N1}" : "---,---,---";
                yield return "";
                yield return "Systems";
                yield return $"Hydrogen: {fuelLevelText,6} %";
                yield return $"Cargo Mass";
                yield return $"{cargoMassText,16} kg";
            }
        }


        static string GetDirectionArrows(double? vertSpeed) {
            if (!vertSpeed.HasValue) return " ";
            var vspeed = Math.Round(vertSpeed.Value, 1);
            return vspeed > 0 ? "↑" : vspeed < 0 ? "↓" : " "; // "\u2191"  "\u2193"
        }
        static string GetCarriageIcon(CarriageStatusMessage carriage) {
            switch (carriage.Mode) {
                case CarriageMode.Manual_Control: return ""; // magenta <>
                case CarriageMode.Awaiting_DepartureClearance: return ""; // yellow <>
                case CarriageMode.Awaiting_CarriageReady2Depart: return ""; // yellow <>
                case CarriageMode.Transit_Powered: return ""; // yellow <>
                case CarriageMode.Transit_Coast: return ""; // yellow <>
                case CarriageMode.Transit_Slow2Approach: return ""; // yellow <>
                case CarriageMode.Transit_Docking: return ""; // yellow <>
                case CarriageMode.Docked: return ""; // green <> "\uE051\uE03D"
                default: return ""; // white <>
            }
        }
        static int GetCarriagePositionIndex(CarriageStatusMessage carriage, int numLines) {
            var totalDist = carriage.Range2Bottom + carriage.Range2Top;
            var percent = carriage.Range2Bottom / totalDist;
            var idx = (numLines - 1) * percent;
            return Convert.ToInt32(Math.Round(idx, 0));
        }
        static CarriageGraphInfo GetGraphInfo(CarriageStatusMessage carriage, int numLines) {
            if (carriage == null) return new CarriageGraphInfo();
            var dir = GetDirectionArrows(carriage.VerticalSpeed);
            return new CarriageGraphInfo() {
                DirText = dir + dir,
                Altitude = $"{carriage.Range2Bottom / 1000.0,4:N1}",
                Icon = GetCarriageIcon(carriage),
                VertPosNum = GetCarriagePositionIndex(carriage, numLines)
            };

        }

        public static bool IsSlimSpeedDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains("[lcd-speed]") && Collect.IsCornerFlatLcd(b); }
        public static bool IsSlimDestDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains("[lcd-destination]") && Collect.IsCornerFlatLcd(b); }
        public static bool IsSlimCargoDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains("[lcd-cargo]") && Collect.IsCornerFlatLcd(b); }
        public static bool IsSlimFuelDisplay(IMyTerminalBlock b) { return b.CustomName.ToLower().Contains("[lcd-fuel]") && Collect.IsCornerFlatLcd(b); }

        public static string BuildSpeedDisplayText(double vertSpeed, double range) {
            var speedText = $"{GetDirectionArrows(vertSpeed)} {Math.Abs(vertSpeed):N1}";
            return $"Speed: {speedText,8} m/s\nRange: {range,8:N1} m";
        }

        public static string BuildDestinationDisplayText(string destination) {
            var padding = (15 - destination?.Length) / 2.0 ?? 0.0;
            var iPadding = Convert.ToInt32(Math.Round(padding, 0));
            return "".PadLeft(iPadding, ' ') + destination;
        }

        public static string BuildCargoDisplayText(double? cargoMass) {
            return $"Cargo Mass\n{cargoMass,17:N1} kg";
        }

        public static string BuildFuelDisplayText(float? h2Percentage) {
            var idx = (12) * (h2Percentage ?? 0f);
            var cnt = Convert.ToInt32(Math.Round(idx, 0));
            var bar = "".PadRight(cnt, '▒');
            return $"Hydrogen Fuel\n▐{bar,-12}▌{h2Percentage * 100,4:N1}%";
        }


        static IEnumerable<string> GetCarriageDetails(string carriageName, CarriageStatusMessage status) {
            var velocityText = status != null
                ? $"{GetDirectionArrows(status.VerticalSpeed)} {Math.Abs(status.VerticalSpeed):N1}"
                : "---.-";
            var altitudeText = status != null ? $"{status.Range2Bottom:N1}" : "--,---.-";
            yield return carriageName;
            yield return $"   Velocity: {velocityText,8} m/s";
            yield return $"   Altitude: {altitudeText,8} m";
        }

        public static void Write2MonospaceDisplay(IMyTextPanel display, string text, float fontSize) {
            LCDHelper.SetFont_Monospaced(display);
            LCDHelper.SetFontSize(display, fontSize);
            display.WritePublicText(text);
            display.ShowPublicTextOnScreen();
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
