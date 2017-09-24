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
        public static bool IsGasTank(IMyTerminalBlock b) { return b is IMyGasTank; }
        public static bool IsOxygenTank(IMyTerminalBlock b) { return (IsGasTank(b) && !b.BlockDefinition.SubtypeId.Contains("Hydro")); }
        public static bool IsHydrogenTank(IMyTerminalBlock b) { return (IsGasTank(b) && b.BlockDefinition.SubtypeId.Contains("Hydro")); }
    }
}
