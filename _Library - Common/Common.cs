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
    static class Common {
        public static bool IsLessThen<T>(T value, T other) where T : IComparable { return value.CompareTo(other) < 0; }
        public static bool IsGreaterThen<T>(T value, T other) where T : IComparable { return value.CompareTo(other) > 0; }
    }
}
