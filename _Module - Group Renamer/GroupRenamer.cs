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
    class GroupRenamer {
        delegate int RenameMethodSig(List<IMyTerminalBlock> blocks, string content);
        readonly Dictionary<string, RenameMethodSig> _renameMethods = new Dictionary<string, RenameMethodSig>();
        public GroupRenamer() {
            _renameMethods.Add("rename to:", RenameMethods.RenameTo);
            _renameMethods.Add("num rename to:", RenameMethods.NumberRenameTo);
            _renameMethods.Add("prefix with:", RenameMethods.PrefixWith);
            _renameMethods.Add("remove prefix:", RenameMethods.RemovePrefix);
            _renameMethods.Add("suffix with:", RenameMethods.SuffixWith);
            _renameMethods.Add("remove suffix:", RenameMethods.RemoveSuffix);
        }

        readonly List<IMyTerminalBlock> _groupBlocks = new List<IMyTerminalBlock>();

        public bool IsRenameGroup(IMyBlockGroup g) {
            return _renameMethods
                .Where(b => g.Name.StartsWith(b.Key, StringComparison.CurrentCultureIgnoreCase))
                .Any();
        }

        public string RenameAllBlocksInGroups(List<IMyBlockGroup> _allGroups) {
            var log = new StringBuilder();

            foreach (var currentGroup in _allGroups.Where(IsRenameGroup)) {
                currentGroup.GetBlocks(_groupBlocks);
                var methodKeyPair = _renameMethods
                    .Where(b => currentGroup.Name.StartsWith(b.Key, StringComparison.CurrentCultureIgnoreCase))
                    .FirstOrDefault();
                var content = currentGroup.Name.Substring(methodKeyPair.Key.Length);
                var count = methodKeyPair.Value.Invoke(_groupBlocks, content);
                log.AppendLine(currentGroup.Name);
                log.AppendLine($"# Blocks : {count:N0}");
            }

            return log.ToString();
        }

    }
}
