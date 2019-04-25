using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {

        const string TAG_BASE = "timcs:";
        const string SCRIPT_NAME = "TIM Config Switcher v1";

        readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        readonly char[] SPLITTER = new char[] { ' ' };

        public Program() {
            Echo(SCRIPT_NAME);
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo(SCRIPT_NAME);

            var argParts = argument.Split(SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            if (argParts.Length < 2) {
                Echo("Invalid Args >> " + argument);
                Echo("");
                Echo("Format expected:");
                Echo("<block tag> <config data tag>");
                return;
            }

            var targetTag = "[" + TAG_BASE + argParts[0] + "]";
            var configTag = "[" + argParts[1] + "]";

            GridTerminalSystem.GetBlocksOfType(blocks, b => Collect.IsTagged(b, targetTag));
            Echo($"Found: {blocks.Count:N0} block(s)");
            foreach (var b in blocks) {
                var timConfig = string.Empty;
                if (!GetTimConfig(b, configTag, out timConfig)) continue;
                ReplaceTimConfig(b, timConfig.Trim());
            }
        }

        bool GetTimConfig(IMyTerminalBlock block, string key, out string config) {
            config = string.Empty;

            var start = block.CustomData.IndexOf(key);
            if (start < 0) return false;

            start += key.Length;
            var end = block.CustomData.IndexOf('[', start);
            if (end < 0) end = block.CustomData.Length - 1;

            config = block.CustomData
                .Substring(start, end - start)
                .Replace('\n', ' ');
            return true;
        }

        void ReplaceTimConfig(IMyTerminalBlock b, string timConfig) {
            var name = b.CustomName.Trim();
            var start = name.IndexOf("[TIM");
            if (start >= 0) {
                var end = name.IndexOf(']', start);
                name = name.Remove(start, end - start + 1).Trim();
            }

            b.CustomName = name + " [TIM " + timConfig + "]";
        }

    }
}
