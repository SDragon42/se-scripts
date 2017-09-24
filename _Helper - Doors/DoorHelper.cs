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
    class DoorHelper
    {
        public static bool IsDoor(IMyTerminalBlock b) { return b is IMyDoor; }
        public static bool IsBasicDoor(IMyTerminalBlock b) { return !(IsSlidingDoor(b) || IsHangarDoor(b)); }
        public static bool IsHangarDoor(IMyTerminalBlock b) { return (b is IMyAirtightHangarDoor); }
        public static bool IsSlidingDoor(IMyTerminalBlock b) { return (b is IMyAirtightSlideDoor); }
        public static bool IsHumanDoor(IMyTerminalBlock b) { return (IsDoor(b) && !IsHangarDoor(b)); }
    }
}
