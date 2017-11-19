using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript {
    static class StringExtensions {
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
