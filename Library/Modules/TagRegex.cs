// <mdk sortorder="1000" />
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
        class TagRegex {

            readonly List<IMyProgrammableBlock> programBlocks = new List<IMyProgrammableBlock>();
            System.Text.RegularExpressions.Regex tagRegex = null;
            public string TagText { get; private set; }

            public void SetTagRegex(string tag) {
                tagRegex = null;
                TagText = tag;
                if (string.IsNullOrEmpty(tag)) return;
                tagRegex = new System.Text.RegularExpressions.Regex(
                    $"\\[ *{System.Text.RegularExpressions.Regex.Escape(tag)}(|[ ,]+[^\\]]*)\\]",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            public bool IsOtherProgramOnDuty(IMyGridTerminalSystem gts, IMyProgrammableBlock Me, Func<IMyProgrammableBlock, bool> preferedCriteria = null) {
                if (tagRegex == null) return false;
                gts.GetBlocksOfType(programBlocks, pb => (pb == Me) | tagRegex.IsMatch(pb.CustomName));

                var i = programBlocks.IndexOf(Me);
                var j = -1;
                if (preferedCriteria != null) j = programBlocks.FindIndex(pb => pb.IsFunctional & pb.IsWorking & preferedCriteria(pb));
                if (j < 0) j = programBlocks.FindIndex(pb => pb.IsFunctional & pb.IsWorking);

                var blkTag = "[" + TagText + (programBlocks.Count > 1 ? " #" + (i + 1) : "") + "]";

                Me.CustomName = tagRegex.IsMatch(Me.CustomName)
                    ? tagRegex.Replace(Me.CustomName, blkTag, 1)
                    : Me.CustomName + " " + blkTag;

                if (i == j) return false;
                //Echo($"{tagPrefix} #{j + 1} is on duty. Standing by.");
                //if (("" + ((IMyProgrammableBlock)TmpBlocks[j]).TerminalRunArgument).Trim() != ("" + Me.TerminalRunArgument).Trim())
                //    Echo($"WARNING: Script arguments do not match {tagPrefix} #{j + 1}.");
                return true;
            }
        }
    }
}
