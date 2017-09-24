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
        public static float GetTanksFillPercentage(List<IMyTerminalBlock> tankList)
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
        public static float GetTanksFillPercentage(List<IMyGasTank> tankList)
        {
            var totalPercent = tankList.Sum(t => t.FilledRatio);
            return (tankList.Count > 0)
                ? totalPercent / tankList.Count
                : 0f;
        }
    }
}
