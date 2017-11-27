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
    struct RangeInfo {
        public static RangeInfo Empty => new RangeInfo(new MyDetectedEntityInfo(), null);

        public RangeInfo(MyDetectedEntityInfo info, double? range) {
            _info = info;
            _range = range;
        }

        readonly double? _range;
        public double? Range => _range;

        readonly MyDetectedEntityInfo _info;
        public MyDetectedEntityInfo DetectedEntity => _info;
    }
}
