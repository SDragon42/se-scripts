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
    partial class Program {
        class GroupRenamer {
            delegate int RenameMethodSig(List<IMyTerminalBlock> blocks, string content);

            readonly Dictionary<string, RenameMethodSig> _renameMethods = new Dictionary<string, RenameMethodSig>();
            readonly List<IMyTerminalBlock> _groupBlocks = new List<IMyTerminalBlock>();

            public GroupRenamer() {
                _renameMethods.Add("rename to:", RenameMethods.RenameTo);
                _renameMethods.Add("num rename to:", RenameMethods.NumberRenameTo);
                _renameMethods.Add("prefix with:", RenameMethods.PrefixWith);
                _renameMethods.Add("remove prefix:", RenameMethods.RemovePrefix);
                _renameMethods.Add("suffix with:", RenameMethods.SuffixWith);
                _renameMethods.Add("remove suffix:", RenameMethods.RemoveSuffix);
            }
            
            public bool IsRenameGroup(IMyBlockGroup g) {
                foreach (var dic in _renameMethods)
                    if (g.Name.StartsWith(dic.Key, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                return false;
            }

            public string RenameAllBlocksInGroups(List<IMyBlockGroup> _allGroups) {
                var log = new StringBuilder();

                foreach (var currentGroup in _allGroups) {
                    if (!IsRenameGroup(currentGroup)) continue;
                    currentGroup.GetBlocks(_groupBlocks);
                    var methodKeyPair = GetMethodKeyPair(currentGroup);
                    var content = currentGroup.Name.Substring(methodKeyPair.Key.Length);
                    var count = methodKeyPair.Value.Invoke(_groupBlocks, content);
                    log.AppendLine(currentGroup.Name);
                    log.AppendLine($"# Blocks : {count:N0}");
                }

                return log.ToString();
            }

            KeyValuePair<string, RenameMethodSig> GetMethodKeyPair(IMyBlockGroup currentGroup) {
                foreach (var kp in _renameMethods) {
                    if (currentGroup.Name.StartsWith(kp.Key, StringComparison.CurrentCultureIgnoreCase))
                        return kp;
                }
                return new KeyValuePair<string, RenameMethodSig>("", (l, c) => 0);
            }

        }
    }
}
