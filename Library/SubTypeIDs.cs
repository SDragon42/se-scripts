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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {
        static class SubTypeIDs {
            public const string TextPanelSM = "SmallTextPanel";
            public const string TextPanelLG = "LargeTextPanel";
            public const string LcdPanelSM = "SmallLCDPanel";
            public const string LcdPanelLG = "LargeLCDPanel";
            public const string WideLcdPanelSM = "SmallLCDPanelWide";
            public const string WideLcdPanelLG = "LargeLCDPanelWide";

            public const string CornerLcdPanel1SM = "SmallBlockCorner_LCD_1";
            public const string CornerLcdPanel2SM = "SmallBlockCorner_LCD_2";
            public const string CornerLcdPanel1LG = "LargeBlockCorner_LCD_1";
            public const string CornerLcdPanel2LG = "LargeBlockCorner_LCD_2";

            public const string FlatCornerLcdPanel1SM = "SmallBlockCorner_LCD_Flat_1";
            public const string FlatCornerLcdPanel2SM = "SmallBlockCorner_LCD_Flat_2";
            public const string FlatCornerLcdPanel1LG = "LargeBlockCorner_LCD_Flat_1";
            public const string FlatCornerLcdPanel2LG = "LargeBlockCorner_LCD_Flat_2";

            public const string SmBlock_SmContainer = "SmallBlockSmallContainer";
            public const string SmBlock_MdContainer = "SmallBlockMediumContainer";
            public const string SmBlock_LgContainer = "SmallBlockLargeContainer";
            public const string LgBlock_SmContainer = "LargeBlockSmallContainer";
            public const string LgBlock_LgContainer = "LargeBlockLargeContainer";
        }
    }
}
