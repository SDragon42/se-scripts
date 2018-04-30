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
    partial class Program {
        static class GasTankHelper {
            public static double GetTanksFillPercentage(List<IMyTerminalBlock> tankList) {
                var totalPercent = 0.0;
                var tankCount = 0;
                for (var i = 0; i < tankList.Count; i++) {
                    var tank = tankList[i] as IMyGasTank;
                    if (tank != null) {
                        totalPercent += tank.FilledRatio;
                        tankCount++;
                    }
                }

                return (tankCount > 0) ? totalPercent / tankCount : 0f;
            }
            public static double GetTanksFillPercentage(List<IMyGasTank> tankList) => tankList.Average(t => t.FilledRatio);
            
        }
    }
}
