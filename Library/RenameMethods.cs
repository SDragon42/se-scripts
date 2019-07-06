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
        static class RenameMethods {
            public static int RenameTo(List<IMyTerminalBlock> blocks, string newName) {
                blocks.ForEach(b => b.CustomName = newName.Trim());
                return blocks.Count();
            }

            public static int NumberRenameTo(List<IMyTerminalBlock> blocks, string newName) => NumberRenameTo<IMyTerminalBlock>(blocks, newName);
            public static int NumberRenameTo<T>(List<T> blocks, string newName, Action<T> customAction = null) where T: IMyTerminalBlock {
                var num = 1;
                var numDigits = blocks.Count.ToString().Length;
                foreach (var b in blocks) {
                    b.CustomName = (newName + " " + (num++).ToString().PadLeft(numDigits, '0')).Trim();
                    customAction?.Invoke(b);
                }
                return blocks.Count();
            }

            public static int PrefixWith(List<IMyTerminalBlock> blocks, string prefix) {
                blocks = blocks.Where(b => !b.CustomName.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)).ToList();
                blocks.ForEach(b => b.CustomName = (prefix + b.CustomName).Trim());
                return blocks.Count;
            }

            public static int SuffixWith(List<IMyTerminalBlock> blocks, string suffix) {
                blocks = blocks.Where(b => !b.CustomName.EndsWith(suffix, StringComparison.CurrentCultureIgnoreCase)).ToList();
                blocks.ForEach(b => b.CustomName = (b.CustomName + suffix).Trim());
                return blocks.Count;
            }

            public static int RemovePrefix(List<IMyTerminalBlock> blocks, string prefix) {
                blocks = blocks.Where(b => b.CustomName.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)).ToList();
                blocks.ForEach(b => b.CustomName = b.CustomName.Substring(prefix.Length).Trim());
                return blocks.Count;
            }

            public static int RemoveSuffix(List<IMyTerminalBlock> blocks, string suffix) {
                blocks = blocks.Where(b => b.CustomName.EndsWith(suffix, StringComparison.CurrentCultureIgnoreCase)).ToList();
                blocks.ForEach(b => b.CustomName = (b.CustomName.Substring(0, b.CustomName.Length - suffix.Length)).Trim());
                return blocks.Count;
            }

            public static int Remove(List<IMyTerminalBlock> blocks, string text) {
                var lowerText = text.ToLower();
                var textLength = lowerText.Length;
                var blockPairs = blocks.Select(b => new { b, searchText = b.CustomName.ToLower() })
                    .Select(pair => new { pair.b, startIdx = pair.searchText.IndexOf(lowerText) })
                    .Where(pair => pair.startIdx >= 0)
                    .ToList();
                blockPairs.ForEach(pair => pair.b.CustomName = pair.b.CustomName.Remove(pair.startIdx, textLength).Trim());
                return blockPairs.Count;
            }
        }
    }
}
