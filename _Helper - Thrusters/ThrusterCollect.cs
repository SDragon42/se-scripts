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

namespace IngameScript
{
    static partial class Collect
    {
        public static bool IsThruster(IMyTerminalBlock b) { return b is IMyThrust; }
        public static bool IsThrusterIon(IMyTerminalBlock b) { return (IsThruster(b) && !IsThrusterHydrogen(b) && !IsThrusterAtmospheric(b)); }
        public static bool IsThrusterHydrogen(IMyTerminalBlock b) { return (IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Hydro")); }
        public static bool IsThrusterAtmospheric(IMyTerminalBlock b) { return (IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Atmo")); }
    }
}
