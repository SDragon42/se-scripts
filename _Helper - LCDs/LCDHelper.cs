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
    /********************************************************************************
    **  Module: LCDs
    **  3rd Party Credits:
    *********************************************************************************
    alex-thatradarguy: for the xbox character codes
    http://steamcommunity.com/sharedfiles/filedetails/?id=627416824
    ********************************************************************************/
    static class LCDHelper
    {

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



        public static IMyTextPanel InitPanel(IMyGridTerminalSystem gts, string textPanelName, float fontSize = 1.0f)
        {
            var textPanel = gts.GetBlockWithName(textPanelName) as IMyTextPanel;

            if (textPanel != null)
            {
                textPanel.WritePublicText("");
                textPanel.SetValue("FontSize", fontSize);
                textPanel.ShowTextureOnScreen();
                textPanel.ShowPublicTextOnScreen();
            }

            return textPanel;
        }



        public static void WriteToTextPanel(IMyGridTerminalSystem gts, string textPanelName, string text, params object[] args)
        {
            var listPanels = new List<IMyTerminalBlock>();
            gts.SearchBlocksOfName(textPanelName, listPanels, b => b is IMyTextPanel);
            if (args != null && args.Length > 0)
                text = string.Format(text, args);
            for (int i = 0; i < listPanels.Count; i++)
            {
                var textPanel = listPanels[i] as IMyTextPanel;
                WriteToTextPanel(textPanel, text);
            }
        }
        public static void WriteToTextPanel(IMyTextPanel textPanel, string text, params object[] args)
        {
            if (textPanel == null) return;
            if (args != null && args.Length > 0)
                text = string.Format(text, args);
            textPanel.WritePublicText(text);
            textPanel.ShowTextureOnScreen();
            textPanel.ShowPublicTextOnScreen();
        }



        public static void AppendToTextPanel(IMyGridTerminalSystem gts, string textPanelName, string text, params object[] args)
        {
            var listPanels = new List<IMyTerminalBlock>();
            gts.SearchBlocksOfName(textPanelName, listPanels, b => b is IMyTextPanel);
            if (args != null && args.Length > 0)
                text = string.Format(text, args);
            for (int i = 0; i < listPanels.Count; i++)
            {
                var textPanel = listPanels[i] as IMyTextPanel;
                AppendToTextPanel(textPanel, text);
            }
        }
        public static void AppendToTextPanel(IMyTextPanel textPanel, string text, params object[] args)
        {
            if (textPanel == null) return;
            if (args != null && args.Length > 0)
                text = string.Format(text, args);
            textPanel.WritePublicText(text, true);
            textPanel.ShowTextureOnScreen();
            textPanel.ShowPublicTextOnScreen();
        }

        public static long GetFont(IMyTextPanel panel) { return panel.GetValue<long>("Font"); }
        public static void SetFont_Debug(IMyTextPanel panel) { panel.SetValue("Font", FONT_Debug); }
        public static void SetFont_Monospaced(IMyTextPanel panel) { panel.SetValue("Font", FONT_MonoSpace); }

        public static float GetFontSize(IMyTextPanel panel) { return panel.GetValue<float>("FontSize"); }
        public static void SetFontSize(IMyTextPanel panel, float size) { panel.SetValue("FontSize", size); }


        public static char ColorChar(int r, int g, int b)
        {
            return (char)(0xE100 + (MathHelper.Clamp(r, 0, 7) << 6) + (MathHelper.Clamp(g, 0, 7) << 3) + MathHelper.Clamp(b, 0, 7));
        }
    }
}
