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
    static class LandingGearHelper
    {
        public static bool IsLandingGear(IMyTerminalBlock b) { return b is IMyLandingGear; }
        public static bool IsLandingGearUnlocked(IMyTerminalBlock b) { return IsLandingGearUnlocked(b as IMyLandingGear); }
        public static bool IsLandingGearUnlocked(IMyLandingGear b)
        {
            if (b == null) return false;
            return ((int)b.LockMode == 0); //TODO: Unlocked - Workaround this this is fixed
        }
        public static bool IsLandingGearReadyToLock(IMyTerminalBlock b) { return IsLandingGearReadyToLock(b as IMyLandingGear); }
        public static bool IsLandingGearReadyToLock(IMyLandingGear b)
        {
            if (b == null) return false;
            return ((int)b.LockMode == 1); //TODO: ReadyToLock - Workaround this this is fixed
        }
        public static bool IsLandingGearLocked(IMyTerminalBlock b) { return IsLandingGearLocked(b as IMyLandingGear); }
        public static bool IsLandingGearLocked(IMyLandingGear b)
        {
            if (b == null) return false;
            return ((int)b.LockMode == 2); //TODO: Locked - Workaround this this is fixed
        }
    }
}
