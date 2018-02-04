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
        static partial class Collect {
            public static bool IsOrientedForward(IMyTerminalBlock b) => (b.Orientation.TransformDirectionInverse(b.Orientation.Forward) == Base6Directions.Direction.Forward);
            public static bool IsTagged(IMyTerminalBlock b, string tag) {
                if (tag == null || tag.Length == 0) return false;
                return b.CustomName.ToLower().Contains(tag.ToLower());
            }
        }
    }
}
