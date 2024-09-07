// <mdk sortorder="2000" />
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
    partial class Program {
        /// <summary>
        /// 
        /// </summary>
        static class CargoHelper {

            public const long MaxVolume_SmBlock_SmContainer = 125000;
            public const long MaxVolume_SmBlock_MdContainer = 3375000;
            public const long MaxVolume_SmBlock_LgContainer = 15625000;
            public const long MaxVolume_LgBlock_SmContainer = 15625000;
            public const long MaxVolume_LgBlock_LgContainer = 421000000;

            /// <summary>
            /// Gets the Inventory Multiplier amount from the list of cargo containers.
            /// </summary>
            /// <param name="blockList"></param>
            /// <returns></returns>
            public static int GetInventoryMultiplier(List<IMyCargoContainer> blockList) {
                if (blockList == null) return 0;
                foreach (var b in blockList) {
                    var mult = GetInventoryMultiplier(b);
                    if (mult > 0) return mult;
                }
                return 0;
            }
            /// <summary>
            /// Gets the Inventory Multiplier amount from the cargo container block.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static int GetInventoryMultiplier(IMyCargoContainer b) {
                var defMaxVolume = GetDefaultMaxVolume(b);
                if (defMaxVolume <= 0) return 0;
                var maxVolume = b.GetInventory().MaxVolume.RawValue;
                if (maxVolume == long.MaxValue) return 0; // infinite volume
                return Convert.ToInt32(maxVolume / defMaxVolume);
            }

            /// <summary>
            /// Gets the maximum volume amount from the vanilla cargo container blocks.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static long GetDefaultMaxVolume(IMyCargoContainer b) {
                if (b != null)
                    switch (b.BlockDefinition.SubtypeId) {
                        case SubTypeIDs.SmBlock_SmContainer: return MaxVolume_SmBlock_SmContainer;
                        case SubTypeIDs.SmBlock_MdContainer: return MaxVolume_SmBlock_MdContainer;
                        case SubTypeIDs.SmBlock_LgContainer: return MaxVolume_SmBlock_LgContainer;
                        case SubTypeIDs.LgBlock_SmContainer: return MaxVolume_LgBlock_SmContainer;
                        case SubTypeIDs.LgBlock_LgContainer: return MaxVolume_LgBlock_LgContainer;
                    }
                return 0;
            }

            public static long GetInventoryTotals(IMyTerminalBlock b, Func<IMyInventory, long> propMethod) {
                var val = 0L;
                if (b != null && b.HasInventory && propMethod != null)
                    for (var i = 0; i < b.InventoryCount; i++)
                        val += propMethod(b.GetInventory(i));
                return val;
            }
            /// <summary>
            /// Gets the maximum volume of the inventory in the block.
            /// </summary>
            /// <param name="inv"></param>
            /// <returns></returns>
            public static long GetInventoryMaxVolume(IMyInventory inv) { return inv?.MaxVolume.RawValue ?? 0; }
            /// <summary>
            /// Gets the currently used volume of the inventory in the block.
            /// </summary>
            /// <param name="inv"></param>
            /// <returns></returns>
            public static long GetInventoryCurrentVolume(IMyInventory inv) { return inv?.CurrentVolume.RawValue ?? 0; }
            /// <summary>
            /// Gets the currently used mass of the inventory in the block.
            /// </summary>
            /// <param name="inv"></param>
            /// <returns></returns>
            public static long GetInventoryCurrentMass(IMyInventory inv) { return inv?.CurrentMass.RawValue ?? 0; }

        }
    }
}
