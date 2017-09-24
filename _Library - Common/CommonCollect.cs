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
    static partial class Collect
    {
        public static bool IsOrientedForward(IMyTerminalBlock b)
        {
            if (b == null)
                return false;
            var i = b.Orientation.TransformDirectionInverse(b.Orientation.Forward);
            var j = VRageMath.Base6Directions.Direction.Forward;
            return (i == j);
        }
    }
}
