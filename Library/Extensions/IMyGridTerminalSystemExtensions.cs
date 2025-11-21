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
        /// <summary>
        /// Get the Blocks with the first collect method that finds any blocks. If the collectMethods array is null, then all blocks of type T are loaded into the list.
        /// </summary>
        /// <typeparam name="T">The base type of the blocks to find.</typeparam>
        /// <param name="gts">GridTerminalSystem.</param>
        /// <param name="blockList">The List to hold the blocks found.</param>
        /// <param name="collectMethods">Array of collection methods to execute.</param>
        public static void GetBlocksOfTypeWithFirst<T>(this IMyGridTerminalSystem gts, List<IMyTerminalBlock> blockList, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class, IMyTerminalBlock {
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
        /// <summary>
        /// Get the Blocks with the first collect method that finds any blocks. If the collectMethods array is null, then all blocks of type T are loaded into the list.
        /// </summary>
        /// <typeparam name="T">The base type of the blocks to find.</typeparam>
        /// <param name="gts">GridTerminalSystem.</param>
        /// <param name="blockList">The List (typed) to hold the blocks found.</param>
        /// <param name="collectMethods">Array of collection methods to execute.</param>
        public static void GetBlocksOfTypeWithFirst<T>(this IMyGridTerminalSystem gts, List<T> blockList, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class, IMyTerminalBlock {
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
        /// <summary>
        /// Gets the first block found with the first collect method that finds any blocks. If the collectMethods array is null, then the first block of type T that is found is returned.
        /// </summary>
        /// <typeparam name="T">The base type of the block to find.</typeparam>
        /// <param name="gts">GridTerminalSystem.</param>
        /// <param name="collectMethods">Array of collection methods to execute.</param>
        /// <returns></returns>
        public static T GetBlockOfTypeWithFirst<T>(this IMyGridTerminalSystem gts, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class, IMyTerminalBlock {
            gts.GetBlocksOfTypeWithFirst<T>(temp, collectMethods);
            return (temp.Count > 0) ? (T)temp[0] : null;
        }
    }
}
