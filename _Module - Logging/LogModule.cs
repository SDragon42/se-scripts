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
            MaxTextLinesToKeep = maxLines2Keep;
        }

        protected readonly List<string> _lines = new List<string>();

        string _lineBuffer = string.Empty;

        public bool Enabled { get; set; }
        public int MaxTextLinesToKeep { get; set; }


        public virtual void Clear()
        {
            _lines.Clear();
            _lineBuffer = string.Empty;
        }

        public void Append(string text, params object[] args)
        {
            if (!Enabled) return;
            _lineBuffer += string.Format(text, args);
        }

        public void AppendLine()
        {
            AppendLine(string.Empty);
        }
        public void AppendLine(string text, params object[] args)
        {
            if (!Enabled) return;
            Append(text, args);
            _lines.Add(_lineBuffer);
            _lineBuffer = string.Empty;
        }

        public string GetLogText()
        {
            if (!Enabled) return string.Empty;
            if (MaxTextLinesToKeep > 0)
            {
                while (_lines.Count > MaxTextLinesToKeep)
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
