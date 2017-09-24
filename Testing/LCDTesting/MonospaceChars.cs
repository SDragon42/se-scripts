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
    class MonospaceChars : TestingBase, ITestingBase
    {
        public MonospaceChars(MyGridProgram thisObj) : base(thisObj) { }

        public void Main(string argument)
        {
            var text = BuildAllCarrigesDisplay();
            //var text = BuildCharMap();
            UpdateDisplays(text);
        }

        readonly List<IMyTextPanel> _lcds = new List<IMyTextPanel>();
        void UpdateDisplays(string text)
        {
            GridTerminalSystem.GetBlocksOfType(_lcds, b => b.CustomName == "All Chars");
            foreach (var display in _lcds)
            {
                LCDHelper.SetFontSize(display, 1f);
                LCDHelper.SetFont_Monospaced(display);
                display.WritePublicText(text);
                display.ShowPublicTextOnScreen();
            }
        }

        string BuildCharMap()
        {
            var sb = new StringBuilder();
            var maxLen = 70;

            //DrawRange(sb, 1, 2048, 100);
            //DrawRange(sb, 6000, 12000, 100);
            //DrawRange(sb, '\u2190', '\u2195', maxLen);

            //// 1-4 button symbols (1-4 squares) at codes \uE030-\uE033
            //DrawRange(sb, '\uE030', '\uE033');

            //// Colored bullets at codes \uE034-\uE055
            DrawRange(sb, '\uE034', '\uE055', maxLen);

            //// Warning symbol /!\ at \uE056
            //DrawRange(sb, '\uE056');

            //// Info symbol (i) at \uE057
            //DrawRange(sb, '\uE057');

            //// Error symbol (x) at \uE058
            //DrawRange(sb, '\uE058');

            //// Spacers with 2^n step (1,2,4,..,256) at \uE070-\uE078
            //DrawRange(sb, '\uE070', '\uE078', maxLen);

            //// palette in range \uE100-\uE2FF
            //DrawRange(sb, '\uE100', '\uE2FF', maxLen);

            //DrawRange(sb, '\uD000', '\uE2FF', lineLength);


            //var display = _lcds[0];

            return sb.ToString();
        }
        void DrawRange(StringBuilder sb, int from, int? to = null, int lineLength = -1)
        {
            if (!to.HasValue) to = from;
            //var max = to.Value;

            while (from <= to.Value)
            {
                sb.Append(Convert.ToChar(from));
                from++;
                if (lineLength > 0 && from % lineLength == 0)
                    sb.AppendLine();
            }

            sb.AppendLine();
        }

        const string CHR_UP = "↑↑"; //"\u2191\u2191";
        const string CHR_DOWN = "↓↓"; //"\u2193\u2193";
        const string CHRS_Carriage_Red = "\uE050\uE03C";
        const string CHRS_Carriage_Green = "\uE051\uE03D";
        const string CHRS_Carriage_Blue = "\uE052\uE03E";
        string BuildAllCarrigesDisplay()
        {
            var sb = new StringBuilder();
            sb.AppendLine(" A1 A2     Maint    B1 B2");
            sb.AppendLine();

            var a1Dir = "  ";
            var a2Dir = "  ";
            var b1Dir = CHR_UP;
            var b2Dir = CHR_DOWN;
            var maintDir = CHR_UP;
            sb.AppendLine($" {a1Dir} {a2Dir}      {maintDir}      {b1Dir} {b2Dir}");

            //for (var i = 0; i < 14; i++)
            var max = 13;
            for (var i = max; i >= 0; i--)
            {
                var a1 = (i == 0) ? CHRS_Carriage_Green : "  ";
                var a2 = (i == 13) ? CHRS_Carriage_Green : "  ";
                var b1 = (i == 11) ? CHRS_Carriage_Blue : "  ";
                var b2 = (i == 6) ? CHRS_Carriage_Red : "  ";
                var maint = (i == 4) ? CHRS_Carriage_Blue : "  ";
                // ┌┬┐   250c 252c 2510
                // ├┼┤│  251c 253c 2524 2502
                // └┴┘   2514 2534 2518
                if (i == max)
                    sb.AppendLine($"┌{a1}┬{a2}┐    ┌{maint}┐    ┌{b1}┬{b2}┐");
                else if (i == 0)
                    sb.AppendLine($"└{a1}┴{a2}┘    └{maint}┘    └{b1}┴{b2}┘");
                else
                    sb.AppendLine($"│{a1}│{a2}│    │{maint}│    │{b1}│{b2}│");

                //sb.AppendLine($"|{a1}|{a2}|    |{maint}|    |{b1}|{b2}|");
            }


            return sb.ToString();
        }
    }
}
