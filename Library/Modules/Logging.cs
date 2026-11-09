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

        class Logging {
            readonly List<string> Lines = new List<string>();

            public Logging(int maxLines2Keep = 10) {
                Enabled = true;
                MaxTextLinesToKeep = maxLines2Keep;
            }

            string _lineBuffer = string.Empty;

            public bool Enabled { get; set; }

            int _maxTextLinesToKeep;
            public int MaxTextLinesToKeep {
                get { return _maxTextLinesToKeep; }
                set { _maxTextLinesToKeep = Math.Max(value, 0); }
            }


            public virtual void Clear() {
                if (!Enabled) return;
                Lines.Clear();
                _lineBuffer = string.Empty;
            }

            public void Append(string text, params object[] args) {
                if (!Enabled) return;
                _lineBuffer += string.Format(text, args);
            }

            public void AppendLine() {
                AppendLine(string.Empty);
            }
            public void AppendLine(string text, params object[] args) {
                if (!Enabled) return;
                Append(text, args);
                Lines.Add(_lineBuffer);
                _lineBuffer = string.Empty;
                TrimLines();
            }

            void TrimLines() {
                if (MaxTextLinesToKeep <= 0) return;
                while (Lines.Count > MaxTextLinesToKeep)
                    Lines.RemoveAt(0);
            }

            public string GetLogText() => ToString();

            public override string ToString() {
                var sb = new StringBuilder();
                foreach (var line in Lines) sb.AppendLine(line);
                if (!string.IsNullOrWhiteSpace(_lineBuffer))
                    sb.AppendLine(_lineBuffer);
                return sb.ToString();
            }
        }

    }
}
