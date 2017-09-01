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

namespace IngameScript
{
    static class Common
    {
        public static bool IsLessThen<T>(T value, T other) where T : IComparable { return value.CompareTo(other) < 0; }
        public static bool IsGreaterThen<T>(T value, T other) where T : IComparable { return value.CompareTo(other) > 0; }

        // Applies an action to all blocks. If the blocks are null or does not have the action specified, no action is taken and no error is thrown.
        //----------------------------------------
        public static void ApplyAction2All(List<IMyTerminalBlock> blockList, string actionName)
        {
            for (var i = 0; i < blockList.Count; i++)
                blockList[i].ApplyAction(actionName);
        }
        public static void ExecuteForAll(List<IMyTerminalBlock> blockList, Action<IMyTerminalBlock> method)
        {
            if (method == null) return;
            foreach (var b in blockList)
                method(b);
        }


        public static bool IsAny<T>(List<T> blockList, Func<T, bool> collect) where T : IMyTerminalBlock
        {
            if (blockList == null || blockList.Count <= 0 || collect == null) return false;
            foreach (var b in blockList)
                if (collect(b)) return true;
            return false;
        }
        public static bool IsAll<T>(List<T> blockList, Func<T, bool> collect) where T : IMyTerminalBlock
        {
            if (blockList == null || blockList.Count <= 0 || collect == null) return false;
            foreach (var b in blockList)
                if (!collect(b)) return false;
            return true;
        }

        public static double SumPropertyFloatToDouble<T>(List<IMyTerminalBlock> blockList, Func<IMyTerminalBlock, float> getValueMeathod)
        {
            var val = 0.0;
            foreach (var b in blockList)
            {
                if (b is T)
                    val += getValueMeathod(b);
            }
            return val;
        }
        public static double SumPropertyFloatToDouble<T>(List<T> blockList, Func<T, float> getValueMeathod)
        {
            var val = 0.0;
            foreach (var b in blockList)
                val += getValueMeathod(b);
            return val;
        }
        public static double SumPropertyAsDouble<T>(List<IMyTerminalBlock> blockList, Func<IMyTerminalBlock, double> getValueMeathod)
        {
            var val = 0.0;
            foreach (var b in blockList)
            {
                if (!(b is T)) continue;
                val += getValueMeathod(b);
            }
            return val;
        }
        public static double SumPropertyAsDouble<T>(List<T> blockList, Func<T, double> getValueMeathod)
        {
            var val = 0.0;
            foreach (var b in blockList)
                val += getValueMeathod(b);
            return val;
        }
        public static double AvgPropertyAsDouble<T>(List<IMyTerminalBlock> blockList, Func<IMyTerminalBlock, double> getValueMeathod)
        {
            var val = 0.0;
            var cnt = 0;
            foreach (var b in blockList)
            {
                if (!(b is T)) continue;
                cnt++;
                val += getValueMeathod(b);
            }
            return (cnt > 0) ? val / cnt : 0.0;
        }
        public static double AvgPropertyAsDouble<T>(List<T> blockList, Func<T, double> getValueMeathod)
        {
            var val = 0.0;
            var cnt = 0;
            foreach (var b in blockList)
            {
                cnt++;
                val += getValueMeathod(b);
            }
            return (cnt > 0) ? val / cnt : 0.0;
        }
    }
}
