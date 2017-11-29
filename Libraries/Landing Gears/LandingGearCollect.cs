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
        public static bool IsLandingGear(IMyTerminalBlock b) => b is IMyLandingGear;

        public static bool IsLandingGearUnlocked(IMyTerminalBlock b) => IsLandingGearUnlocked(b as IMyLandingGear);
        public static bool IsLandingGearUnlocked(IMyLandingGear b) => ((int)b?.LockMode == 0); //TODO: Unlocked - Workaround this this is fixed

        public static bool IsLandingGearReadyToLock(IMyTerminalBlock b) => IsLandingGearReadyToLock(b as IMyLandingGear);
        public static bool IsLandingGearReadyToLock(IMyLandingGear b) => ((int)b?.LockMode == 1); //TODO: ReadyToLock - Workaround this this is fixed

        public static bool IsLandingGearLocked(IMyTerminalBlock b) => IsLandingGearLocked(b as IMyLandingGear);
        public static bool IsLandingGearLocked(IMyLandingGear b) => ((int)b?.LockMode == 2); //TODO: Locked - Workaround this this is fixed
    }
}
