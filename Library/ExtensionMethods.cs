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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    #region mdk preserve
    static class ExtensionMethods {
        public static void GetblocksOfTypeWithFirst<T>(this IMyGridTerminalSystem gts, List<IMyTerminalBlock> blockList, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class, IMyTerminalBlock {
            if (collectMethods == null || collectMethods.Length == 0) {
                gts.GetBlocksOfType<T>(blockList);
                if (blockList.Count > 0) return;
            } else {
                foreach (var collect in collectMethods) {
                    gts.GetBlocksOfType<T>(blockList, collect);
                    if (blockList.Count > 0) return;
                }
            }
        }
        public static void GetblocksOfTypeWithFirst<T>(this IMyGridTerminalSystem gts, List<T> blockList, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class, IMyTerminalBlock {
            if (collectMethods == null || collectMethods.Length == 0) {
                gts.GetBlocksOfType<T>(blockList);
                if (blockList.Count > 0) return;
            } else {
                foreach (var collect in collectMethods) {
                    gts.GetBlocksOfType(blockList, collect);
                    if (blockList.Count > 0) return;
                }
            }
        }

        static readonly List<IMyTerminalBlock> temp = new List<IMyTerminalBlock>();
        public static T GetBlockOfTypeWithFirst<T>(this IMyGridTerminalSystem gts, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class, IMyTerminalBlock {
            gts.GetblocksOfTypeWithFirst<T>(temp, collectMethods);
            return (temp.Count > 0) ? (T)temp[0] : null;
        }


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


        public static void Add(this MyIni ini, MyIniKey k, bool v, string comment = null) {
            if (ini.ContainsKey(k)) return;
            ini.Set(k, v);
            ini.SetComment(k, comment);
        }
        public static void Add(this MyIni ini, MyIniKey k, double v, string comment = null) {
            if (!ini.ContainsKey(k)) return;
            ini.Set(k, v);
            ini.SetComment(k, comment);
        }
        public static void Add(this MyIni ini, MyIniKey k, string v, string comment = null) {
            if (!ini.ContainsKey(k)) return;
            ini.Set(k, v);
            ini.SetComment(k, comment);
        }
    }
    #endregion
}
