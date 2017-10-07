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

        public static bool IsSmBlockTextPanel(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "SmallTextPanel"); }
        public static bool IsLgBlockTextPanel(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "LargeTextPanel"); }

        public static bool IsLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (IsSmBlockLcd(b) || IsLgBlockLcd(b))); }
        public static bool IsSmBlockLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "SmallLCDPanel"); }
        public static bool IsLgBlockLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "LargeLCDPanel"); }

        public static bool IsWideLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (IsSmBlockWideLcd(b) || IsLgBlockWideLcd(b))); }
        public static bool IsSmBlockWideLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "SmallLCDPanelWide"); }
        public static bool IsLgBlockWideLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "LargeLCDPanelWide"); }

        public static bool IsCornerLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (IsSmBlockCornerLcd(b) || IsLgBlockCornerLcd(b))); }
        public static bool IsSmBlockCornerLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == "SmallBlockCorner_LCD_1" || b.BlockDefinition.SubtypeId == "SmallBlockCorner_LCD_2")); }
        public static bool IsLgBlockCornerLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == "LargeBlockCorner_LCD_1" || b.BlockDefinition.SubtypeId == "LargeBlockCorner_LCD_2")); }

        public static bool IsCornerFlatLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (IsSmBlockCornerFlatLcd(b) || IsLgBlockCornerFlatLcd(b))); }
        public static bool IsSmBlockCornerFlatLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == "SmallBlockCorner_LCD_Flat_1" || b.BlockDefinition.SubtypeId == "SmallBlockCorner_LCD_Flat_2")); }
        public static bool IsLgBlockCornerFlatLcd(IMyTerminalBlock b) { return (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == "LargeBlockCorner_LCD_Flat_1" || b.BlockDefinition.SubtypeId == "LargeBlockCorner_LCD_Flat_2")); }
    }
}
