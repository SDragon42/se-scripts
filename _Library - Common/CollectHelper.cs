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
    static class CollectHelper {
        public static void GetblocksOfTypeWithFirst<T>(IMyGridTerminalSystem gts, List<IMyTerminalBlock> blockList, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class, IMyTerminalBlock {
            foreach (var collect in collectMethods) {
                gts.GetBlocksOfType<T>(blockList, collect);
                if (blockList.Count > 0) return;
            }
        }
        public static void GetblocksOfTypeWithFirst<T>(IMyGridTerminalSystem gts, List<T> blockList, params Func<IMyTerminalBlock, bool>[] collectMethods) where T : class, IMyTerminalBlock {
            foreach (var collect in collectMethods) {
                gts.GetBlocksOfType(blockList, collect);
                if (blockList.Count > 0) return;
            }
        }

    }
}
