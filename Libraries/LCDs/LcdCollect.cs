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
        public static bool IsTextPanel(IMyTerminalBlock b) => b is IMyTextPanel;

        public static bool IsSmTextPanel(IMyTerminalBlock b) => (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "SmallTextPanel");
        public static bool IsLgTextPanel(IMyTerminalBlock b) => (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "LargeTextPanel");

        public static bool IsLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (IsSmLcd(b) || IsLgLcd(b)));
        public static bool IsSmLcd(IMyTerminalBlock b) => (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "SmallLCDPanel");
        public static bool IsLgLcd(IMyTerminalBlock b) => (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "LargeLCDPanel");

        public static bool IsWideLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (IsSmWideLcd(b) || IsLgWideLcd(b)));
        public static bool IsSmWideLcd(IMyTerminalBlock b) => (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "SmallLCDPanelWide");
        public static bool IsLgWideLcd(IMyTerminalBlock b) => (IsTextPanel(b) && b.BlockDefinition.SubtypeId == "LargeLCDPanelWide");

        public static bool IsAngledCornerLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (IsSmAngledCornerLcd(b) || IsLgAngledCornerLcd(b)));
        public static bool IsSmAngledCornerLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == "SmallBlockCorner_LCD_1" || b.BlockDefinition.SubtypeId == "SmallBlockCorner_LCD_2"));
        public static bool IsLgAngledCornerLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == "LargeBlockCorner_LCD_1" || b.BlockDefinition.SubtypeId == "LargeBlockCorner_LCD_2"));

        public static bool IsFlatCornerLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (IsSmFlatCornerLcd(b) || IsLgFlatCornerLcd(b)));
        public static bool IsSmFlatCornerLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == "SmallBlockCorner_LCD_Flat_1" || b.BlockDefinition.SubtypeId == "SmallBlockCorner_LCD_Flat_2"));
        public static bool IsLgFlatCornerLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (b.BlockDefinition.SubtypeId == "LargeBlockCorner_LCD_Flat_1" || b.BlockDefinition.SubtypeId == "LargeBlockCorner_LCD_Flat_2"));

        public static bool IsCornerLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (IsAngledCornerLcd(b) || IsFlatCornerLcd(b)));
        public static bool IsSmCornerLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (IsSmAngledCornerLcd(b) || IsSmFlatCornerLcd(b)));
        public static bool IsLgCornerLcd(IMyTerminalBlock b) => (IsTextPanel(b) && (IsLgAngledCornerLcd(b) || IsLgFlatCornerLcd(b)));

    }
}
