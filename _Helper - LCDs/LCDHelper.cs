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
    /********************************************************************************
    **  Module: LCDs
    **  3rd Party Credits:
    *********************************************************************************
    alex-thatradarguy: for the xbox character codes
    http://steamcommunity.com/sharedfiles/filedetails/?id=627416824
    ********************************************************************************/
    static class LCDHelper {
        public const string SUBTYPE_SmBlock_Text = "SmallTextPanel";
        public const string SUBTYPE_SmBlock_Lcd = "SmallLCDPanel";
        public const string SUBTYPE_SmBlock_WideLcd = "SmallLCDPanelWide";
        public const string SUBTYPE_SmBlock_CornerLcd1 = "SmallBlockCorner_LCD_1";
        public const string SUBTYPE_SmBlock_CornerLcd2 = "SmallBlockCorner_LCD_2";
        public const string SUBTYPE_SmBlock_CornerFlatLcd1 = "SmallBlockCorner_LCD_Flat_1";
        public const string SUBTYPE_SmBlock_CornerFlatLcd2 = "SmallBlockCorner_LCD_Flat_2";

        public const string SUBTYPE_LgBlock_Text = "LargeTextPanel";
        public const string SUBTYPE_LgBlock_Lcd = "LargeLCDPanel";
        public const string SUBTYPE_LgBlock_WideLcd = "LargeLCDPanelWide";
        public const string SUBTYPE_LgBlock_CornerLcd1 = "LargeBlockCorner_LCD_1";
        public const string SUBTYPE_LgBlock_CornerLcd2 = "LargeBlockCorner_LCD_2";
        public const string SUBTYPE_LgBlock_CornerFlatLcd1 = "LargeBlockCorner_LCD_Flat_1";
        public const string SUBTYPE_LgBlock_CornerFlatLcd2 = "LargeBlockCorner_LCD_Flat_2";

        //COLOR CONSTANTS: Do not change!!! 
        public const string LCD_green = "\uE001";
        public const string LCD_blue = "\uE002";
        public const string LCD_red = "\uE003";
        public const string LCD_yellow = "\uE004";
        public const string LCD_lightGray = "\uE00E";
        public const string LCD_mediumGray = "\uE00D";
        public const string LCD_darkGray = "\uE00F";

        // Wider Color Constants
        public const string LCD_white = "\uE006";

        public const long FONT_Debug = 151057691;
        public const long FONT_MonoSpace = 1147350002;

        public const string SUBTYPE_LargeLCDPanel = "LargeLCDPanel";

        public static long GetFont(IMyTextPanel panel) { return panel.GetValue<long>("Font"); }
        public static void SetFont_Debug(IMyTextPanel panel) { panel.SetValue("Font", FONT_Debug); }
        public static void SetFont_Monospaced(IMyTextPanel panel) { panel.SetValue("Font", FONT_MonoSpace); }

        public static float GetFontSize(IMyTextPanel panel) { return panel.GetValue<float>("FontSize"); }
        public static void SetFontSize(IMyTextPanel panel, float size) { panel.SetValue("FontSize", size); }


        public static char ColorChar(int r, int g, int b) {
            return (char)(0xE100 + (MathHelper.Clamp(r, 0, 7) << 6) + (MathHelper.Clamp(g, 0, 7) << 3) + MathHelper.Clamp(b, 0, 7));
        }
    }
}
