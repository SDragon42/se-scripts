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
    partial class Program {
        class Logging {

            readonly List<string> Lines = new List<string>();

            public Logging(int maxLines2Keep = 10) {
                Enabled = true;
                MaxTextLinesToKeep = maxLines2Keep;
            }

            string LineBuffer = string.Empty;

            public bool Enabled { get; set; }
            public int MaxTextLinesToKeep { get; set; }


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
            }

            public string GetLogText() {
                if (!Enabled) return string.Empty;
                if (MaxTextLinesToKeep > 0) {
                    while (Lines.Count > MaxTextLinesToKeep)
                        Lines.RemoveAt(0);
                }
                var sb = new StringBuilder();
                for (var i = 0; i < Lines.Count; i++)
                    sb.AppendLine(Lines[i]);
                if (!string.IsNullOrWhiteSpace(LineBuffer))
                    sb.AppendLine(LineBuffer);
                return sb.ToString();
            }
        }
    }
}
