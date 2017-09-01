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
            SetMaxTextLinesToKeep(-1);
        }


        IMyTextPanel _display;

        bool _echoMessages = true;
        public bool GetEchoMessages() { return _echoMessages; }
        public void SetEchoMessages(bool value) { _echoMessages = value; }


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
            if (!GetEnabled()) return;
            Init();
            if (_display == null) return;
            _display.WritePublicText(string.Empty);
        }


        public void UpdateDisplay()
        {
            if (!GetEnabled()) return;

            var text = GetLogText();

            Init();
            if (_echoMessages) _thisObj.Echo(text);
            if (_display == null) return;
            _display.WritePublicText(text, false);
        }
    }
}
