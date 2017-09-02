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
    class GroupRenamer
    {
        const string GROUP_NAME_RenameTo = "rename to:";
        const string GROUP_NAME_NumberRenameTo = "num rename to:";
        const string GROUP_NAME_PrefixWith = "prefix with:";
        const string GROUP_NAME_RemovePrefix = "remove prefix:";
        const string GROUP_NAME_SuffixWith = "suffix with:";
        const string GROUP_NAME_RemoveSuffix = "remove suffix:";

        readonly List<IMyBlockGroup> _allGroups = new List<IMyBlockGroup>();
        readonly List<IMyTerminalBlock> _groupBlocks = new List<IMyTerminalBlock>();

        public void RenameAllBlocksInGroups(MyGridProgram thisObj)
        {
            thisObj.GridTerminalSystem.GetBlockGroups(_allGroups);

            foreach (var currentGroup in _allGroups)
            {
                currentGroup.GetBlocks(_groupBlocks);
                var groupName = currentGroup.Name;
                var loweredGroupName = groupName.ToLower();
                var cnt = 0;

                if (loweredGroupName.StartsWith(GROUP_NAME_RenameTo))
                {
                    thisObj.Echo(groupName);
                    cnt = RenameTo(_groupBlocks, groupName);
                }

                else if (loweredGroupName.StartsWith(GROUP_NAME_NumberRenameTo))
                {
                    thisObj.Echo(groupName);
                    cnt = NumberRenameTo(_groupBlocks, groupName);
                }

                else if (loweredGroupName.StartsWith(GROUP_NAME_PrefixWith))
                {
                    thisObj.Echo(groupName);
                    cnt = PrefixWith(_groupBlocks, groupName);
                }

                else if (loweredGroupName.StartsWith(GROUP_NAME_SuffixWith))
                {
                    thisObj.Echo(groupName);
                    cnt = SuffixWith(_groupBlocks, groupName);
                }

                else if (loweredGroupName.StartsWith(GROUP_NAME_RemovePrefix))
                {
                    thisObj.Echo(groupName);
                    cnt = RemovePrefix(_groupBlocks, groupName);
                }

                else if (loweredGroupName.StartsWith(GROUP_NAME_RemoveSuffix))
                {
                    thisObj.Echo(groupName);
                    cnt = RemoveSuffix(_groupBlocks, groupName);
                }
                else continue;

                thisObj.Echo("# Blocks :" + cnt.ToString());
            }
        }

        private int RenameTo(IList<IMyTerminalBlock> groupBlocks, string groupName)
        {
            var cnt = 0;
            var baseName = groupName.Substring(GROUP_NAME_RenameTo.Length).Trim();
            for (var x = 0; x < groupBlocks.Count; x++)
            {
                groupBlocks[x].CustomName = baseName;
                cnt++;
            }

            return cnt;
        }
        private int NumberRenameTo(IList<IMyTerminalBlock> groupBlocks, string groupName)
        {
            var cnt = 0;
            var baseName = groupName.Substring(GROUP_NAME_NumberRenameTo.Length).Trim();
            var numDigits = groupBlocks.Count.ToString().Length;
            for (var x = 0; x < groupBlocks.Count; x++)
            {
                var num = (x + 1).ToString();
                groupBlocks[x].CustomName = baseName + " " + num.PadLeft(numDigits, '0');
                cnt++;
            }

            return cnt;
        }
        private int PrefixWith(IList<IMyTerminalBlock> groupBlocks, string groupName)
        {
            var cnt = 0;
            var prefix = groupName.Substring(GROUP_NAME_PrefixWith.Length);
            for (var x = 0; x < groupBlocks.Count; x++)
            {
                if (!groupBlocks[x].CustomName.ToLower().StartsWith(prefix.ToLower()))
                {
                    groupBlocks[x].CustomName = (prefix + groupBlocks[x].CustomName).Trim();
                    cnt++;
                }
            }

            return cnt;
        }
        private int SuffixWith(IList<IMyTerminalBlock> groupBlocks, string groupName)
        {
            var cnt = 0;
            var suffix = groupName.Substring(GROUP_NAME_SuffixWith.Length);
            for (var x = 0; x < groupBlocks.Count; x++)
            {
                if (!groupBlocks[x].CustomName.ToLower().EndsWith(suffix.ToLower()))
                {
                    groupBlocks[x].CustomName = (groupBlocks[x].CustomName + suffix).Trim();
                    cnt++;
                }
            }

            return cnt;
        }
        private int RemovePrefix(IList<IMyTerminalBlock> groupBlocks, string groupName)
        {
            var cnt = 0;
            var prefix = groupName.Substring(GROUP_NAME_RemovePrefix.Length);
            for (var x = 0; x < groupBlocks.Count; x++)
            {
                if (groupBlocks[x].CustomName.ToLower().StartsWith(prefix.ToLower()))
                {
                    groupBlocks[x].CustomName = groupBlocks[x].CustomName.Substring(prefix.Length).Trim();
                    cnt++;
                }
            }

            return cnt;
        }
        private int RemoveSuffix(IList<IMyTerminalBlock> groupBlocks, string groupName)
        {
            var cnt = 0;
            var suffix = groupName.Substring(GROUP_NAME_RemoveSuffix.Length);
            for (var x = 0; x < groupBlocks.Count; x++)
            {
                if (groupBlocks[x].CustomName.ToLower().EndsWith(suffix.ToLower()))
                {
                    var tmp = groupBlocks[x].CustomName.Substring(0, groupBlocks[x].CustomName.Length - suffix.Length);
                    groupBlocks[x].CustomName = tmp.Trim();
                    cnt++;
                }
            }
            return cnt;
        }

    }
}
