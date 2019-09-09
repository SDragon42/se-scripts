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
    partial class Program : MyGridProgram {

        public void Main(string argument, UpdateType updateSource) {
            var lcdList = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType(lcdList, Me.IsSameConstructAs);

            Echo($"Found {lcdList.Count:N0} LCDs");
            Echo("==========");
            foreach (var panel in lcdList) {
                var text = panel.GetPrivateText();
                if (!string.IsNullOrWhiteSpace(text)) {
                    Echo(panel.CustomName);
                    if (!string.IsNullOrWhiteSpace(panel.CustomData)) {
                        Echo("    * Has CD. Skipped.");
                        continue;
                    }
                    panel.CustomData = text;
                    panel.WritePrivateText("");
                    Echo("    - Text copied.");
                }
            }
            Echo("==========");
            Echo("FINISHED");
        }

    }
}
