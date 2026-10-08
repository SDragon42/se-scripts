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

        static class RenameHelper {
            public static void RenameTo(List<IMyTerminalBlock> blocks, string newName) {
                foreach (var b in blocks)
                    b.CustomName = newName.Trim();
            }

            public static void NumberRenameTo(List<IMyTerminalBlock> blocks, string newName) {
                var num = 1;
                var numDigits = (int)Math.Floor(Math.Log10(blocks.Count)) + 1;
                foreach (var b in blocks)
                    b.CustomName = (newName + " " + num++.ToString().PadLeft(numDigits, '0')).Trim();
            }

            public static void PrefixWith(List<IMyTerminalBlock> blocks, string prefix) {
                foreach (var b in blocks)
                    if (!b.CustomName.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                        b.CustomName = (prefix + b.CustomName).Trim();
            }

            public static void SuffixWith(List<IMyTerminalBlock> blocks, string suffix) {
                foreach (var b in blocks)
                    if (!b.CustomName.EndsWith(suffix, StringComparison.CurrentCultureIgnoreCase))
                        b.CustomName = (b.CustomName + suffix).Trim();
            }

            public static void RemovePrefix(List<IMyTerminalBlock> blocks, string prefix) {
                foreach (var b in blocks)
                    if (b.CustomName.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                        b.CustomName = b.CustomName.Substring(prefix.Length).Trim();
            }

            public static void RemoveSuffix(List<IMyTerminalBlock> blocks, string suffix) {
                foreach (var b in blocks)
                    if (b.CustomName.EndsWith(suffix, StringComparison.CurrentCultureIgnoreCase))
                        b.CustomName = b.CustomName.Substring(0, b.CustomName.Length - suffix.Length).Trim();
            }

            public static void Remove(List<IMyTerminalBlock> blocks, string text) {
                foreach (var b in blocks) {
                    var startIndex = b.CustomName.IndexOf(text, StringComparison.CurrentCultureIgnoreCase);
                    if (startIndex < 0) continue;
                    b.CustomName = b.CustomName.Remove(startIndex, text.Length).Trim();
                }
            }
            public static void Replace(List<IMyTerminalBlock> blocks, string text) {
                var parts = text.Split('|');
                var removeText = parts[0];
                if (removeText.Length == 0) return;
                var addText = parts.Length > 1 ? parts[1] : string.Empty;
                foreach (var b in blocks) {
                    var startIndex = b.CustomName.IndexOf(removeText, StringComparison.CurrentCultureIgnoreCase);
                    if (startIndex < 0) continue;
                    b.CustomName = b.CustomName.Remove(startIndex, removeText.Length).Insert(startIndex, addText).Trim();
                }
            }
        }

    }
}
