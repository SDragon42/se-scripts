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
        public static bool IsTextPanel(IMyTerminalBlock b) { return b is IMyTextPanel; }

        public static bool IsSmBlockTextPanel(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_SmBlock_Text); }
        public static bool IsLgBlockTextPanel(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_LgBlock_Text); }

        public static bool IsLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (IsSmBlockLcd(b) || IsLgBlockLcd(b))); }
        public static bool IsSmBlockLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_SmBlock_Lcd); }
        public static bool IsLgBlockLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_LgBlock_Lcd); }

        public static bool IsWideLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (IsSmBlockWideLcd(b) || IsLgBlockWideLcd(b))); }
        public static bool IsSmBlockWideLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_SmBlock_WideLcd); }
        public static bool IsLgBlockWideLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_LgBlock_WideLcd); }

        public static bool IsCornerLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (IsSmBlockCornerLcd(b) || IsLgBlockCornerLcd(b))); }
        public static bool IsSmBlockCornerLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_SmBlock_CornerLcd1 || b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_SmBlock_CornerLcd2)); }
        public static bool IsLgBlockCornerLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_LgBlock_CornerLcd1 || b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_LgBlock_CornerLcd2)); }

        public static bool IsCornerFlatLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (IsSmBlockCornerFlatLcd(b) || IsLgBlockCornerFlatLcd(b))); }
        public static bool IsSmBlockCornerFlatLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_SmBlock_CornerFlatLcd1 || b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_SmBlock_CornerFlatLcd2)); }
        public static bool IsLgBlockCornerFlatLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_LgBlock_CornerFlatLcd1 || b.BlockDefinition.SubtypeId == LCDHelper.SUBTYPE_LgBlock_CornerFlatLcd2)); }

    }
}
