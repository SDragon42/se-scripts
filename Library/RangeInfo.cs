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
        class RangeInfo {
            public static readonly RangeInfo Empty = new RangeInfo(new MyDetectedEntityInfo(), null);

            public RangeInfo(MyDetectedEntityInfo info, double? range) {
                DetectedEntity = info;
                Range = range;
            }

            public double? Range { get; private set; }
            public MyDetectedEntityInfo DetectedEntity { get; private set; }
        }
    }
}
