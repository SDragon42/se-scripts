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
    static class IMyGridTerminalSystemExtensions {
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
    }
}
