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
    class LCDTesting : TestingBase, ITestingBase {

        readonly List<IMyTextPanel> _displays = new List<IMyTextPanel>();

        public LCDTesting(MyGridProgram thisObj) : base(thisObj) { }

        public void Main(string argument) {
            GridTerminalSystem.GetBlocksOfType(_displays);

            foreach (var d in _displays) {
                //d.WritePublicText(d.Font);
                d.WritePublicTitle(string.Empty);
                d.WritePublicText(
                    $"{LCDChars.XBox_X} - Blue\n" +
                    $"{LCDChars.XBox_DPad} - LCD_darkGray\n" +
                    $"{LCDChars.XBox_A} - LCD_green\n" +
                    $"{LCDChars.XBox_Menu} - LCD_lightGray\n" +
                    $"{LCDChars.XBox_Back} - LCD_mediumGray\n" +
                    $"{LCDChars.XBox_B} - LCD_red\n" +
                    $"{LCDChars.XBox_RB} - LCD_white\n" +
                    $"{LCDChars.XBox_Y} - LCD_yellow\n"
                    );
                d.ShowPublicTextOnScreen();
            }
        }

    }
}
