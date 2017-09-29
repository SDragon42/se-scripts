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

namespace IngameScript {
    static partial class Collect {
        public static bool IsCargoContainer(IMyTerminalBlock b) { return b is IMyCargoContainer; }
        public static bool IsSmallBlockSmallCargoContainer(IMyTerminalBlock b) { return (IsCargoContainer(b) && b.BlockDefinition.SubtypeId == CargoHelper.SUBTYPE_SmBlock_SmContainer); }
        public static bool IsSmallBlockMediumCargoContainer(IMyTerminalBlock b) { return (IsCargoContainer(b) && b.BlockDefinition.SubtypeId == CargoHelper.SUBTYPE_SmBlock_MdContainer); }
        public static bool IsSmallBlockLargeCargoContainer(IMyTerminalBlock b) { return (IsCargoContainer(b) && b.BlockDefinition.SubtypeId == CargoHelper.SUBTYPE_SmBlock_LgContainer); }
        public static bool IsLargeBlockSmallCargoContainer(IMyTerminalBlock b) { return (IsCargoContainer(b) && b.BlockDefinition.SubtypeId == CargoHelper.SUBTYPE_LgBlock_SmContainer); }
        public static bool IsLargeBlockLargeCargoContainer(IMyTerminalBlock b) { return (IsCargoContainer(b) && b.BlockDefinition.SubtypeId == CargoHelper.SUBTYPE_LgBlock_LgContainer); }
    }
}
