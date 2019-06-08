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
        static class Font_X {
            static Font_X() {
                dic = new Dictionary<char, FontChar>();
                dic.Add('0', new FontChar(4, new int[] { 0, 1, 2, 4, 6, 8, 10, 12, 14, 16, 17, 18 }));
                dic.Add('1', new FontChar(4, new int[] { 1, 4, 5, 9, 13, 16, 17, 18 }));
                dic.Add('2', new FontChar(4, new int[] { 0, 1, 2, 6, 8, 9, 10, 12, 16, 17, 18 }));

                dic.Add(':', new FontChar(2, new int[] { 2, 6 }));
            }

            readonly static Dictionary<char, FontChar> dic;
            public static Dictionary<char, FontChar> GetChars() { return dic; }
        }
    }
}
