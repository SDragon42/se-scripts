// <mdk sortorder="2000" />
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
        /// <summary>
        /// Whip's Monospace TextHelper Class v2
        /// Taken from his Compass Script
        /// </summary>
        static class TextHelper {
            static StringBuilder textSB = new StringBuilder();
            const float adjustedPixelWidth = (512f / 0.778378367f);
            const int monospaceCharWidth = 24 + 1; //accounting for spacer

            public static float GetMinimumFontSizeMonospace(int textCharacters) {
                var pixelWidth = textCharacters * monospaceCharWidth;
                return adjustedPixelWidth / pixelWidth;
            }

            public static string WrapTextMonospace(string text, float fontSize) {
                textSB.Clear();
                var words = text.Split(' ');
                var screenWidth = (adjustedPixelWidth / fontSize);
                int currentLineWidth = 0;
                foreach (var word in words) {
                    if (currentLineWidth == 0) {
                        textSB.Append($"{word}");
                        currentLineWidth += word.Length * monospaceCharWidth;
                        continue;
                    }

                    currentLineWidth += (1 + word.Length) * monospaceCharWidth;
                    if (currentLineWidth > screenWidth) //new line
                    {
                        currentLineWidth = word.Length * monospaceCharWidth;
                        textSB.Append($"\n{word}");
                    } else {
                        textSB.Append($" {word}");
                    }

                }
                return textSB.ToString();
            }

            public static string CenterTextMonospace(string wrappedText, float fontSize) {
                textSB.Clear();
                var lines = wrappedText.Split('\n');
                var screenWidth = (adjustedPixelWidth / fontSize);
                var maxCharsPerLine = Math.Floor(screenWidth / monospaceCharWidth);

                foreach (var line in lines) {
                    var trimmedLine = line.Trim();
                    var charCount = trimmedLine.Length;
                    var diff = maxCharsPerLine - charCount;
                    var halfDiff = (int)Math.Max(diff / 2, 0);
                    textSB.Append(new string(' ', halfDiff)).Append(trimmedLine).Append("\n");
                }
                return textSB.ToString();
            }

            public static string RightJustifyMonospace(string wrappedText, float fontSize) {
                textSB.Clear();
                var lines = wrappedText.Split('\n');
                var screenWidth = (adjustedPixelWidth / fontSize);
                var maxCharsPerLine = (int)Math.Floor(screenWidth / monospaceCharWidth);

                foreach (var line in lines) {
                    var trimmedLine = line.Trim();
                    var charCount = trimmedLine.Length;
                    var diff = maxCharsPerLine - charCount;
                    diff = (int)Math.Max(0, diff);
                    textSB.Append(new string(' ', diff)).Append(trimmedLine).Append("\n");
                }
                return textSB.ToString();
            }
        }
    }
}
