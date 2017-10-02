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
    class DebugModule : LogModule {
        public const string DefaultDebugPanelName = "DEBUG";

        readonly List<IMyTextPanel> _debugDisplays = new List<IMyTextPanel>();

        readonly MyGridProgram _thisObj;
        readonly string _debugDisplayName;


        public DebugModule(MyGridProgram thisObj, string debugDisplayName = null) {
            _thisObj = thisObj;
            _debugDisplayName = string.IsNullOrWhiteSpace(debugDisplayName) ? DefaultDebugPanelName : debugDisplayName;
            MaxTextLinesToKeep = -1;
        }


        public bool EchoMessages { get; set; }


        void Init() {
            _thisObj.GridTerminalSystem.GetBlocksOfType(_debugDisplays, IsValidDebugDisplay);
            foreach (var display in _debugDisplays) {
                display.ShowTextureOnScreen();
                display.ShowPublicTextOnScreen();
            }
        }
        bool IsValidDebugDisplay(IMyTerminalBlock b) {
            if (b.CubeGrid != _thisObj.Me.CubeGrid) return false;
            return (b.CustomName.ToLower() == _debugDisplayName);
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
            _debugDisplays.ForEach(d => d.WritePublicText(text));
        }
    }
}
