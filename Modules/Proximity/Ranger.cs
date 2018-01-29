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


namespace IngameScript {
    partial class Program {
        static class Ranger {

            public static RangeInfo GetDetailedRange(IMyCameraBlock camera, double maxScanRange) {
                camera.EnableRaycast = true;
                if (camera.CanScan(maxScanRange)) {
                    var info = camera.Raycast(maxScanRange, 0, 0);
                    var range = (info.HitPosition.HasValue)
                        ? Vector3D.Distance(camera.GetPosition(), info.HitPosition.Value)
                        : (double?)null;
                    return new RangeInfo(info, range);
                }
                return RangeInfo.Empty;
            }
        }
    }
}
