// <mdk sortorder="1000" />
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        class GravMap
        {
            static GravMap() {
                double[] elevations = new double[] { 7258.96, 7358.11, 7458.42, 7559.92, 7662.68, 7766.67, 7871.99, 7978.66, 8086.60, 8196.02, 8306.80, 8419.07, 8532.81, 8648.08, 8764.96, 8883.40, 9003.53, 9125.31, 9248.86, 9374.20, 9501.40, 9630.45, 9761.43, 9894.48, 10029.52, 10166.72, 10306.05, 10447.69, 10591.59, 10737.90, 10886.64, 11037.99, 11191.94, 11348.54, 11507.97, 11670.31, 11835.68, 12004.08, 12175.77, 12350.74, 12529.15, 12711.15, 12896.86, 13086.48, 13280.11, 13477.91, 13680.04, 13886.71, 14098.12, 14314.49, 14535.98, 14762.89, 14995.48, 15233.90, 15478.58, 15729.79, 15987.81, 16253.04, 16525.91, 16806.73, 17096.04, 17394.31, 17702.03, 18019.90, 18348.38, 18688.32, 19040.43, 19405.57, 19784.61, 20178.63, 20588.79, 21016.38, 21462.79, 21929.72, 22418.87, 22932.48, 23472.86, 24042.77, 24645.35, 25284.29, 25964.03, 26689.69, 27467.40, 28304.74, 29210.79, 30197.03, 31277.83, 32471.84, 33803.48, 35305.84, 37025.29, 39029.18, 41420.90 };
                double gStart = 9.8;
                foreach (var e in elevations) {
                    var gNext = gStart - 0.1;
                    map.Add(new GravMapItem(e, gStart, gNext));
                    gStart = gNext;
                }
            }
            static readonly List<GravMapItem> map = new List<GravMapItem>();
        }

        class GravMapItem {
            public GravMapItem(double elev, double gbefore, double gafter) {
                SeaLevelElevation = elev;
                GravBefore = gbefore;
                GravAfter = gafter;
            }
            public double SeaLevelElevation { get; private set; }
            public double GravBefore { get; private set; }
            public double GravAfter { get; private set; }
        }
    }
}
