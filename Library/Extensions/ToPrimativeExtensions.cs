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
    static class ToPrimativeExtensions {
        public static int ToInt(this string text, int defValue = 0) {
            if (text == null) return defValue;
            int val;
            if (!int.TryParse(text, out val)) return defValue;
            return val;
        }
        public static float ToFloat(this string text, float defValue = 0f) {
            if (text == null) return defValue;
            float val;
            if (!float.TryParse(text, out val)) return defValue;
            return val;
        }
        public static double ToDouble(this string text, double defValue = 0.0) {
            if (text == null) return defValue;
            double val;
            if (!double.TryParse(text, out val)) return defValue;
            return val;
        }
        public static bool ToBoolean(this string text, bool defValue = false) {
            if (text == null) return defValue;
            bool val;
            if (!bool.TryParse(text, out val)) return defValue;
            return val;
        }
        public static T ToEnum<T>(this string text, T defValue = default(T)) where T : struct {
            if (text == null) return defValue;
            T val;
            if (!Enum.TryParse(text, out val)) return defValue;
            return val;
        }
    }
}
