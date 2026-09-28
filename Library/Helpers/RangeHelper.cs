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

        static class RangeHelper {
            public static bool TryGetDetailedRange(IMyCameraBlock camera, double maxScanRange, out MyDetectedEntityInfo detectedInfo) {
                camera.EnableRaycast = true;
                detectedInfo = default(MyDetectedEntityInfo);
                if (!camera.CanScan(maxScanRange)) return false;
                detectedInfo = camera.Raycast(maxScanRange, 0, 0);
                return !detectedInfo.IsEmpty();
            }
        }

    }
}
