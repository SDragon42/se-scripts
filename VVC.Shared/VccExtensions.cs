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
    static class VccExtensions {
        public static string ToRaceTimeString(this TimeSpan time) {
            if (time.TotalSeconds < 0) return "00:00.000";
            //var hours = (int)time.TotalHours;
            var minutes = (int)time.TotalMinutes;
            var seconds = time.Seconds;
            var milliseconds = time.Milliseconds;
            return $"{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
        }

        public static void AppendLines(this StringBuilder sb, IEnumerable<string> lines) {
            if (lines == null) return;
            foreach (var line in lines) {
                if (line != null) sb.AppendLine(line);
            }
        }
    }
}
