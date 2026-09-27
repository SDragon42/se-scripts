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
        // Whip's Monospace TextHelper Class v2
        // Taken from his Compass Script
        static class TextHelper {
            const float ADJUSTED_PIXEL_WIDTH = 512f / 0.778378367f; //adjusted for the font size of 0.778378367f to make the text fit the screen width of 512 pixels
            const int MONOSPACE_CHAR_WIDTH = 24 + 1; //accounting for spacer

            static StringBuilder _textBuilder = null;
            static StringBuilder InitStringBuilder() {
                if (_textBuilder == null) _textBuilder = new StringBuilder();
                _textBuilder.Clear();
                return _textBuilder;
            }

            public static float GetMinimumFontSizeMonospace(int textCharacters) {
                var pixelWidth = textCharacters * MONOSPACE_CHAR_WIDTH;
                return ADJUSTED_PIXEL_WIDTH / pixelWidth;
            }

            public static string WrapTextMonospace(string text, float fontSize) {
                var sb = InitStringBuilder();
                var words = text.Split(' ');
                var screenWidth = ADJUSTED_PIXEL_WIDTH / fontSize;
                var currentLineWidth = 0;
                foreach (var word in words) {
                    if (currentLineWidth == 0) {
                        sb.Append($"{word}");
                        currentLineWidth += word.Length * MONOSPACE_CHAR_WIDTH;
                        continue;
                    }

                    currentLineWidth += (1 + word.Length) * MONOSPACE_CHAR_WIDTH;
                    if (currentLineWidth > screenWidth) //new line
                    {
                        currentLineWidth = word.Length * MONOSPACE_CHAR_WIDTH;
                        sb.Append($"\n{word}");
                    } else {
                        sb.Append($" {word}");
                    }

                }
                return sb.ToString();
            }

            public static string CenterTextMonospace(string wrappedText, float fontSize) {
                var sb = InitStringBuilder();
                var lines = wrappedText.Split('\n');
                var screenWidth = ADJUSTED_PIXEL_WIDTH / fontSize;
                var maxCharsPerLine = Math.Floor(screenWidth / MONOSPACE_CHAR_WIDTH);

                foreach (var line in lines) {
                    var trimmedLine = line.Trim();
                    var charCount = trimmedLine.Length;
                    var diff = maxCharsPerLine - charCount;
                    var halfDiff = (int)Math.Max(diff / 2, 0);
                    sb.Append(new string(' ', halfDiff)).Append(trimmedLine).Append("\n");
                }
                return sb.ToString();
            }

            public static string RightJustifyMonospace(string wrappedText, float fontSize) {
                var sb = InitStringBuilder();
                var lines = wrappedText.Split('\n');
                var screenWidth = ADJUSTED_PIXEL_WIDTH / fontSize;
                var maxCharsPerLine = (int)Math.Floor(screenWidth / MONOSPACE_CHAR_WIDTH);

                foreach (var line in lines) {
                    var trimmedLine = line.Trim();
                    var charCount = trimmedLine.Length;
                    var diff = maxCharsPerLine - charCount;
                    diff = (int)Math.Max(0, diff);
                    sb.Append(new string(' ', diff)).Append(trimmedLine).Append("\n");
                }
                return sb.ToString();
            }


            public static string FormatMass(float? mass) => FormatMass((double?)mass);
            public static string FormatMass(double? mass) {
                if (!mass.HasValue || double.IsNaN(mass.Value)) return string.Empty;
                if (mass < 1000) return $"{mass:N2} kg";
                if (mass < 1000000) return $"{mass / 1000:N2} t";
                return $"{mass / 1000000:N2} kt";
            }

            public static string FormatForce(float? force) => FormatForce((double?)force);
            public static string FormatForce(double? force) {
                if (!force.HasValue || double.IsNaN(force.Value)) return string.Empty;
                if (force < 1000) return $"{force:N2} N";
                if (force < 1000000) return $"{force / 1000:N2} kN";
                return $"{force / 1000000:N2} MN";
            }

            public static string FormatDistance(float? range) => FormatDistance((double?)range);
            public static string FormatDistance(double? range) {
                if (!range.HasValue || double.IsNaN(range.Value)) return string.Empty;
                if (range < 1000) return $"{range:N1} m";
                if (range < 1000000) return $"{range / 1000:N1} km";
                return $"{range / 1000000:N1} Mm";
            }
        }
    }
}
