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
    class DebugModule : LogModule
    {
        public const string DefaultDebugPanelName = "DEBUG";

        readonly List<IMyTerminalBlock> tmp = new List<IMyTerminalBlock>();

        readonly MyGridProgram _thisObj;
        readonly string _debugDisplayName;


        public DebugModule(MyGridProgram thisObj, string debugDisplayName = null)
        {
            _thisObj = thisObj;
            _debugDisplayName = string.IsNullOrWhiteSpace(debugDisplayName) ? DefaultDebugPanelName : debugDisplayName;
            MaxTextLinesToKeep = -1;
        }


        IMyTextPanel _display;

        public bool EchoMessages { get; set; }


        private void Init()
        {
            _thisObj.GridTerminalSystem.SearchBlocksOfName(_debugDisplayName, tmp, b => b.CubeGrid.EntityId == _thisObj.Me.CubeGrid.EntityId);
            if (tmp.Count <= 0) return;
            _display = tmp[0] as IMyTextPanel;
            if (_display == null) return;
            _display.ShowTextureOnScreen();
            _display.ShowPublicTextOnScreen();
        }

        public override void Clear()
        {
            base.Clear();
            if (!Enabled) return;
            Init();
            if (_display == null) return;
            _display.WritePublicText(string.Empty);
        }


        public void UpdateDisplay()
        {
            if (!Enabled) return;

            var text = GetLogText();

            Init();
            if (EchoMessages) _thisObj.Echo(text);
            if (_display == null) return;
            _display.WritePublicText(text, false);
        }
    }
}
