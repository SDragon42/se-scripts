﻿using Sandbox.Game.EntityComponents;
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
    static class CargoHelper
    {
        public const string SUBTYPE_SmBlock_SmContainer = "SmallBlockSmallContainer";
        public const string SUBTYPE_SmBlock_MdContainer = "SmallBlockMediumContainer";
        public const string SUBTYPE_SmBlock_LgContainer = "SmallBlockLargeContainer";
        public const string SUBTYPE_LgBlock_SmContainer = "LargeBlockSmallContainer";
        public const string SUBTYPE_LgBlock_LgContainer = "LargeBlockLargeContainer";

        public const long MaxVolume_SmBlock_SmContainer = 125000;
        public const long MaxVolume_SmBlock_MdContainer = 3375000;
        public const long MaxVolume_SmBlock_LgContainer = 15625000;
        public const long MaxVolume_LgBlock_SmContainer = 15625000;
        public const long MaxVolume_LgBlock_LgContainer = 421000000;

        public static int GetInventoryMultiplier(List<IMyCargoContainer> blockList)
        {
            if (blockList == null || blockList.Count <= 0) return 0;
            foreach (var b in blockList)
            {
                var result = GetInventoryMultiplier(b);
                if (result > 0) return result;
            }
            return 0;
        }
        public static int GetInventoryMultiplier(IMyCargoContainer b)
        {
            var defMaxVolume = GetDefaultMaxVolume(b);
            if (defMaxVolume <= 0) return 0;
            var maxVolume = b.GetInventory().MaxVolume.RawValue;
            if (maxVolume == long.MaxValue) return 0; // infinite volume
            return Convert.ToInt32(maxVolume / defMaxVolume);
        }
        public static long GetDefaultMaxVolume(IMyCargoContainer b)
        {
            if (b != null) return 0;
            switch (b.BlockDefinition.SubtypeId)
            {
                case SUBTYPE_SmBlock_SmContainer: return MaxVolume_SmBlock_SmContainer;
                case SUBTYPE_SmBlock_MdContainer: return MaxVolume_SmBlock_MdContainer;
                case SUBTYPE_SmBlock_LgContainer: return MaxVolume_SmBlock_LgContainer;
                case SUBTYPE_LgBlock_SmContainer: return MaxVolume_LgBlock_SmContainer;
                case SUBTYPE_LgBlock_LgContainer: return MaxVolume_LgBlock_LgContainer;
            }
            return 0;
        }

        public static long GetInventoryTotals(IMyTerminalBlock b, Func<IMyInventory, long> propMethod)
        {
            if (b == null) return 0;
            if (propMethod == null) return 0;
            var val = 0L;
            if (b.HasInventory)
            {
                for (var i = 0; i < b.InventoryCount; i++)
                    val += propMethod(b.GetInventory(i));
            }
            return val;
        }
        public static long GetInventoryMaxVolume(IMyInventory inv)
        {
            if (inv == null) return 0;
            return inv.MaxVolume.RawValue;
        }
        public static long GetInventoryCurrentVolume(IMyInventory inv)
        {
            if (inv == null) return 0;
            return inv.CurrentVolume.RawValue;
        }
        public static long GetInventoryCurrentMass(IMyInventory inv)
        {
            if (inv == null) return 0;
            return inv.CurrentMass.RawValue;
        }

    }
}
