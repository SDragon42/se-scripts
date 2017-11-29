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
    static partial class Collect {
        public static bool IsDoor(IMyTerminalBlock b) => b is IMyDoor;
        public static bool IsBasicDoor(IMyTerminalBlock b) => !(IsSlidingDoor(b) || IsHangarDoor(b));
        public static bool IsHangarDoor(IMyTerminalBlock b) => (b is IMyAirtightHangarDoor);
        public static bool IsSlidingDoor(IMyTerminalBlock b) => (b is IMyAirtightSlideDoor);
        public static bool IsHumanDoor(IMyTerminalBlock b) => (IsDoor(b) && !IsHangarDoor(b));
    }
}
