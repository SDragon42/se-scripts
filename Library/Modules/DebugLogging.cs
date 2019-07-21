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

            readonly MyGridProgram gridProg;
            readonly string displayName;
            int displayIndex;


            public DebugLogging(MyGridProgram thisObj, string displayName = null, int displayIndex = 0) {
                gridProg = thisObj;
                this.displayName = string.IsNullOrWhiteSpace(displayName) ? DefaultDebugPanelName : displayName;
                this.displayIndex = displayIndex >= 0 ? displayIndex : 0;
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
                var b = gridProg.GridTerminalSystem.GetBlockWithName(displayName);
                if (b == null) return;
                if (!b.IsSameConstructAs(gridProg.Me)) return;
                var d = b as IMyTextSurfaceProvider;
                if (d == null) return;
                if (displayIndex > d.SurfaceCount - 1)
                    displayIndex = d.SurfaceCount - 1;
                var surface = d.GetSurface(displayIndex);
                surface.ContentType = ContentType.TEXT_AND_IMAGE;
                surface.WriteText(text);
            }
        }
    }
}
