// <mdk sortorder="900" />
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
        static class LCDChars {
            // alex-thatradarguy: for the xbox character codes
            // http://steamcommunity.com/sharedfiles/filedetails/?id=627416824
            // TODO: Change Names to be X-BOX button names
            public const string XBox_A = "\uE001";
            public const string XBox_X = "\uE002";
            public const string XBox_B = "\uE003";
            public const string XBox_Y = "\uE004";
            public const string XBox_Menu = "\uE00E";
            public const string XBox_Back = "\uE00D";
            public const string XBox_DPad = "\uE00F";
            public const string XBox_RB = "\uE006"; // Wider Color Constants

            public static char ColorChar(int r, int g, int b) => (char)(0xE100 + (MathHelper.Clamp(r, 0, 7) << 6) + (MathHelper.Clamp(g, 0, 7) << 3) + MathHelper.Clamp(b, 0, 7));
        }
    }
}
