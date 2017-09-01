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
    class LogModule
    {
        public LogModule(int maxLines2Keep = 10)
        {
            SetMaxTextLinesToKeep(maxLines2Keep);
        }

        protected readonly List<string> _lines = new List<string>();

        string _lineBuffer = string.Empty;

        bool _enabled = true;
        public bool GetEnabled() { return _enabled; }
        public void SetEnabled(bool value) { _enabled = value; }

        int _maxTextLinesToKeep;
        public int GetMaxTextLinesToKeep() { return _maxTextLinesToKeep; }
        public void SetMaxTextLinesToKeep(int value) { _maxTextLinesToKeep = value; }


        public virtual void Clear()
        {
            _lines.Clear();
            _lineBuffer = string.Empty;
        }

        public void Append(string text, params object[] args)
        {
            if (!_enabled) return;
            _lineBuffer += string.Format(text, args);
        }

        public void AppendLine()
        {
            AppendLine(string.Empty);
        }
        public void AppendLine(string text, params object[] args)
        {
            if (!_enabled) return;
            Append(text, args);
            _lines.Add(_lineBuffer);
            _lineBuffer = string.Empty;
        }

        public string GetLogText()
        {
            if (!_enabled) return string.Empty;
            if (_maxTextLinesToKeep > 0)
            {
                while (_lines.Count > _maxTextLinesToKeep)
                    _lines.RemoveAt(0);
            }
            var sb = new StringBuilder();
            for (var i = 0; i < _lines.Count; i++)
                sb.AppendLine(_lines[i]);
            if (!string.IsNullOrWhiteSpace(_lineBuffer))
                sb.AppendLine(_lineBuffer);
            return sb.ToString();
        }
    }
}
