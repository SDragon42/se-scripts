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
        class DebugLogging : Logging {
            public const string DefaultDebugPanelName = "DEBUG";

            readonly List<IMyTextPanel> Displays = new List<IMyTextPanel>();

            readonly MyGridProgram gridProg;
            readonly string debugDisplayName;


            public DebugLogging(MyGridProgram thisObj, string debugDisplayName = null) {
                gridProg = thisObj;
                this.debugDisplayName = string.IsNullOrWhiteSpace(debugDisplayName) ? DefaultDebugPanelName : debugDisplayName;
                MaxTextLinesToKeep = -1;
            }


            public bool EchoMessages { get; set; } = false;


            public override void Clear() {
                base.Clear();
                UpdateDisplay();
            }
            public void UpdateDisplay() {
                if (!Enabled) return;
                var text = GetLogText();
                if (EchoMessages && text.Length > 0) gridProg.Echo(text);
                WriteToDisplays(text);
            }
            void WriteToDisplays(string text) {
                if (!Enabled) return;
                gridProg.GridTerminalSystem.GetBlocksOfType(Displays, b => b.IsSameConstructAs(gridProg.Me) && b.CustomName.Equals(debugDisplayName));
                foreach (var d in Displays) {
                    d.ContentType = ContentType.TEXT_AND_IMAGE;
                    d.TextPadding = 0f;
                    d.WriteText(text);
                }
            }
        }
    }
}
