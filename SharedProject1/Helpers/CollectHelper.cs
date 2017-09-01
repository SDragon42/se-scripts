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
    static class CollectHelper
    {
        public static void GetblocksOfTypeWithFirst<T>(IMyGridTerminalSystem gts, List<IMyTerminalBlock> blockList, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class
        {
            blockList.Clear();
            foreach (var collect in collectMethods)
            {
                gts.GetBlocksOfType<T>(blockList, collect);
                if (blockList.Count > 0) return;
            }
        }

        public static IEnumerable<IMyTerminalBlock> GetBlocksInList(List<IMyTerminalBlock> blockList, Func<IMyTerminalBlock, bool> collect)
        {
            foreach (var b in blockList)
                if (collect(b)) yield return b;
        }
        public static IEnumerable<T> GetBlocksInList<T>(List<IMyTerminalBlock> blockList, Func<IMyTerminalBlock, bool> collect) where T : class
        {
            foreach (var b in blockList)
            {
                if (!(b is T)) continue;
                if (collect(b)) yield return (T)b;
            }
        }

        public static IMyTerminalBlock GetFirstBlockInList(List<IMyTerminalBlock> blockList, Func<IMyTerminalBlock, bool> collect)
        {
            foreach (var b in blockList)
                if (collect(b)) return b;
            return null;
        }
        public static T GetFirstBlockInList<T>(List<IMyTerminalBlock> blockList, Func<IMyTerminalBlock, bool> collect) where T : class
        {
            foreach (var b in blockList)
            {
                if (!(b is T)) continue;
                if (collect(b)) return (T)b;
            }
            return null;
        }

    }
}
