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
    class LCDTesting : TestingBase
    {
        readonly char blk = LCDHelper.ColorChar(0, 0, 0);
        readonly char grn = LCDHelper.ColorChar(1, 2, 1);
        readonly char blu = LCDHelper.ColorChar(1, 1, 2);
        readonly char red = LCDHelper.ColorChar(4, 0, 0);

        readonly List<IMyTextPanel> _displays = new List<IMyTextPanel>();
        //readonly DebugModule _debug;

        public LCDTesting() : base()
        {
        }

        public override void Main(string argument)
        {
            //_debug.Clear();
            GridTerminalSystem.GetBlocksOfType(_displays, b => b.CustomName != "DEBUG");
            if (_displays.Count == 0) return;

            var sb = new StringBuilder();


            var num = 1;
            foreach (var panel in _displays)
            {
                DrawDots(panel, 52, 52);
                //_debug.AppendLine($"{num,2}: {panel.CustomName}\n{panel.BlockDefinition.TypeIdString}\n");
                Echo($"{num,2}: {panel.CustomName} | {panel.BlockDefinition.TypeIdString}");
                num++;
            }

            //_debug.UpdateDisplay();
        }

        void UpdateDisplay(IMyTextPanel panel, string text)
        {
            LCDHelper.SetFont_Monospaced(panel);
            panel.ShowPublicTextOnScreen();
            panel.WritePublicText(text);
        }


        void DrawDots(IMyTextPanel panel, int maxX, int maxY)
        {
            var sb = new StringBuilder();
            for (var y = 0; y < maxY; y++)
            {
                for (var x = 0; x < maxX; x++)
                {
                    var c = ((x % 2 == 0) && (y % 2 == 0)) ? red : blk;
                    if (x == y) c = grn;
                    sb.Append(c);
                }
                sb.AppendLine();
            }

            UpdateDisplay(panel, sb.ToString());
        }
    }
}
