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

            string LineBuffer = string.Empty;

            public bool Enabled { get; set; }

            int maxLines;
            public int MaxTextLinesToKeep {
                get { return maxLines; }
                set { maxLines = value > 0 ? value : 0; }
            }


            public virtual void Clear() {
                if (!Enabled) return;
                Lines.Clear();
                LineBuffer = string.Empty;
            }

            public void Append(string text, params object[] args) {
                if (!Enabled) return;
                LineBuffer += string.Format(text, args);
            }

            public void AppendLine() {
                AppendLine(string.Empty);
            }
            public void AppendLine(string text, params object[] args) {
                if (!Enabled) return;
                Append(text, args);
                Lines.Add(LineBuffer);
                LineBuffer = string.Empty;
                TrimLines();
            }

            void TrimLines() {
                if (maxLines <= 0) return;
                while (Lines.Count > maxLines)
                    Lines.RemoveAt(0);
            }

            public string GetLogText() => ToString();

            public override string ToString() {
                var sb = new StringBuilder();
                foreach (var line in Lines) sb.AppendLine(line);
                if (!string.IsNullOrWhiteSpace(LineBuffer))
                    sb.AppendLine(LineBuffer);
                return sb.ToString();
            }
        }
    }
}
