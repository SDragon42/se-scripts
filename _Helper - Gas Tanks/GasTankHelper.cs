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
    static class GasTankHelper
    {
        public static bool IsGasTank(IMyTerminalBlock b) { return b is IMyGasTank; }
        public static bool IsOxygenTank(IMyTerminalBlock b) { return (IsGasTank(b) && !b.BlockDefinition.SubtypeId.Contains("Hydro")); }
        public static bool IsHydrogenTank(IMyTerminalBlock b) { return (IsGasTank(b) && b.BlockDefinition.SubtypeId.Contains("Hydro")); }

        public static float GetTanksFillPercentage(IList<IMyTerminalBlock> tankList)
        {
            var totalPercent = 0f;
            var tankCount = 0;
            for (var i = 0; i < tankList.Count; i++)
            {
                var tank = tankList[i] as IMyGasTank;
                if (tank != null)
                {
                    totalPercent += tank.FilledRatio;
                    tankCount++;
                }
            }

            return (tankCount > 0)
                ? totalPercent / tankCount
                : 0f;
        }
    }
}
