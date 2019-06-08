﻿using Sandbox.Game.EntityComponents;
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

namespace IngameScript
{
    partial class Program {
        class DebugLogging : Logging {
            public const string DefaultDebugPanelName = "DEBUG";

            readonly List<IMyTextPanel> _debugDisplays = new List<IMyTextPanel>();

            readonly MyGridProgram _thisObj;
            readonly string _debugDisplayName;


            public DebugLogging(MyGridProgram thisObj, string debugDisplayName = null) {
                _thisObj = thisObj;
                _debugDisplayName = string.IsNullOrWhiteSpace(debugDisplayName) ? DefaultDebugPanelName : debugDisplayName;
                MaxTextLinesToKeep = -1;
            }


            public bool EchoMessages { get; set; }


            void Init() {
                _thisObj.GridTerminalSystem.GetBlocksOfType(_debugDisplays, IsValidDebugDisplay);
                foreach (var display in _debugDisplays) {
                    display.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                }
            }
            bool IsValidDebugDisplay(IMyTerminalBlock b) {
                if (b.CubeGrid != _thisObj.Me.CubeGrid) return false;
                return (string.Compare(b.CustomName, _debugDisplayName, true) == 0);
            }


            public override void Clear() {
                base.Clear();
                if (!Enabled) return;
                Init();
                WriteToDisplays(string.Empty);
            }
            public void UpdateDisplay() {
                if (!Enabled) return;
                var text = GetLogText();
                Init();
                if (EchoMessages) _thisObj.Echo(text);
                WriteToDisplays(text);
            }
            void WriteToDisplays(string text) {
                if (!Enabled) return;
                _debugDisplays.ForEach(d => d.WriteText(text));
            }
        }
    }
}
