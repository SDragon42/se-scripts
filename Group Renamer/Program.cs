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

        delegate void RenameMethodSig(List<IMyTerminalBlock> blocks, string content);

        readonly Dictionary<string, RenameMethodSig> _groupPrefixes = new Dictionary<string, RenameMethodSig>();

        readonly List<IMyBlockGroup> _groups = new List<IMyBlockGroup>();
        readonly List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();

        readonly string _instructions;

        public Program() {
            _groupPrefixes.Add("rename to:", RenameHelper.RenameTo);
            _groupPrefixes.Add("num rename to:", RenameHelper.NumberRenameTo);
            _groupPrefixes.Add("prefix with:", RenameHelper.PrefixWith);
            _groupPrefixes.Add("suffix with:", RenameHelper.SuffixWith);
            _groupPrefixes.Add("remove:", RenameHelper.Remove);
            _groupPrefixes.Add("remove prefix:", RenameHelper.RemovePrefix);
            _groupPrefixes.Add("remove suffix:", RenameHelper.RemoveSuffix);
            _groupPrefixes.Add("replace:", RenameHelper.Replace);

            // Instructions
            _instructions = "Group Renamer\n== prefixes ==\n" + string.Join("\n", _groupPrefixes.Keys);
            Echo(_instructions);
        }


        public void Main(string argument, UpdateType updateSource) {
            Echo(_instructions);
            GridTerminalSystem.GetBlockGroups(_groups, IsRenameGroup);

            var log = new StringBuilder();

            foreach (var currentGroup in _groups) {
                if (!IsRenameGroup(currentGroup)) continue;

                currentGroup.GetBlocks(_blocks);
                var methodKeyPair = GetMethodKeyPair(currentGroup);
                var content = currentGroup.Name.Substring(methodKeyPair.Key.Length);
                methodKeyPair.Value.Invoke(_blocks, content);
                log.AppendLine(currentGroup.Name);
                log.AppendLine($"# Blocks : {_blocks.Count:N0}");
            }

            Echo(string.Empty);
            Echo(log.ToString());
        }

        public bool IsRenameGroup(IMyBlockGroup g) {
            foreach (var dic in _groupPrefixes)
                if (g.Name.StartsWith(dic.Key, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        KeyValuePair<string, RenameMethodSig> GetMethodKeyPair(IMyBlockGroup currentGroup) {
            foreach (var kp in _groupPrefixes) {
                if (currentGroup.Name.StartsWith(kp.Key, StringComparison.CurrentCultureIgnoreCase))
                    return kp;
            }
            return new KeyValuePair<string, RenameMethodSig>("", (l, c) => {});
        }

    }
}
