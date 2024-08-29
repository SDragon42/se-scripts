using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {
        delegate int RenameMethodSig(List<IMyTerminalBlock> blocks, string content);
        readonly Dictionary<string, RenameMethodSig> GroupPrefixes = new Dictionary<string, RenameMethodSig>();

        readonly List<IMyBlockGroup> groups = new List<IMyBlockGroup>();

        readonly string instructions;

        public Program() {
            GroupPrefixes.Add("rename to:", RenameMethods.RenameTo);
            GroupPrefixes.Add("num rename to:", RenameMethods.NumberRenameTo);
            GroupPrefixes.Add("prefix with:", RenameMethods.PrefixWith);
            GroupPrefixes.Add("suffix with:", RenameMethods.SuffixWith);
            GroupPrefixes.Add("remove:", RenameMethods.Remove);
            GroupPrefixes.Add("remove prefix:", RenameMethods.RemovePrefix);
            GroupPrefixes.Add("remove suffix:", RenameMethods.RemoveSuffix);
            GroupPrefixes.Add("replace:", RenameMethods.Replace);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Group Renamer");
            sb.AppendLine("== prefixes ==");
            foreach (var c in GroupPrefixes.Keys) sb.AppendLine(c);
            instructions = sb.ToString();

            Echo(instructions);
        }


        public void Main(string argument, UpdateType updateSource) {
            Echo(instructions);
            GridTerminalSystem.GetBlockGroups(groups, IsRenameGroup);

            var log = new StringBuilder();

            foreach (var currentGroup in groups) {
                if (!IsRenameGroup(currentGroup)) continue;

                currentGroup.GetBlocks(TmpBlocks);
                var methodKeyPair = GetMethodKeyPair(currentGroup);
                var content = currentGroup.Name.Substring(methodKeyPair.Key.Length);
                var count = methodKeyPair.Value.Invoke(TmpBlocks, content);
                log.AppendLine(currentGroup.Name);
                log.AppendLine($"# Blocks : {count:N0}");
            }

            Echo(string.Empty);
            Echo(log.ToString());
        }



        public bool IsRenameGroup(IMyBlockGroup g) {
            foreach (var dic in GroupPrefixes)
                if (g.Name.StartsWith(dic.Key, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        KeyValuePair<string, RenameMethodSig> GetMethodKeyPair(IMyBlockGroup currentGroup) {
            foreach (var kp in GroupPrefixes) {
                if (currentGroup.Name.StartsWith(kp.Key, StringComparison.CurrentCultureIgnoreCase))
                    return kp;
            }
            return new KeyValuePair<string, RenameMethodSig>("", (l, c) => 0);
        }

    }
}
