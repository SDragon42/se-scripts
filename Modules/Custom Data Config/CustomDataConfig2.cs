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
        class CustomDataConfig2 {
            readonly static string[] SepBlankLine = new string[] { "\n\n" };
            readonly static char[] SepNewLine = new char[] { '\n' };
            readonly static char[] SepEquals = new char[] { '=' };

            readonly string _section;
            readonly Dictionary<string, string> _items = new Dictionary<string, string>();

            public CustomDataConfig2(string section) {
                _section = $"[{section}]";
            }

            public void AddKey(string key, string defaultValue = "") {
                if (!_items.ContainsKey(key)) _items.Add(key, defaultValue);
            }

            public void Load(IMyTerminalBlock b, bool addIfMissing = false) {
                if (b == null) return;
                var sec = GetSections(b.CustomData)
                    .Where(d => d.StartsWith(_section))
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(sec)) return;

                var lines = sec.Split(SepNewLine)
                    .Select(i => i.Split(SepEquals, 2))
                    .Where(p => p.Length == 2);
                foreach (var cfgItem in lines) {
                    if (!_items.ContainsKey(cfgItem[0])) {
                        if (addIfMissing)
                            AddKey(cfgItem[0], cfgItem[1]);
                    } else
                        _items[cfgItem[0]] = cfgItem[1];
                }
            }
            public void Save(IMyTerminalBlock b) {
                if (b == null) return;
                var written = false;
                var sb = new StringBuilder();
                var sections = GetSections(b.CustomData);
                var sec = "";
                Action addSection = () => {
                    if (sec.Length == 0) return;
                    if (sb.Length > 0) sb.Append("\n");
                    sb.Append(sec + "\n");
                };
                for (var i = 0; i < sections.Length; i++) {
                    sec = sections[i].Trim();
                    if (sec.StartsWith(_section)) {
                        sec = BuildSection();
                        written = true;
                    }
                    addSection();
                }

                if (!written) {
                    sec = BuildSection();
                    addSection();
                }

                b.CustomData = sb.ToString().Trim();
            }

            public void SetValue<T>(string key, T val) {
                if (!_items.ContainsKey(key)) return;
                _items[key] = (val != null) ? val.ToString() : string.Empty;
            }
            public string GetValue(string key, string defVal = "") {
                return _items.ContainsKey(key) ? _items[key] : defVal;
            }

            string[] GetSections(string data) => data.Split(SepBlankLine, StringSplitOptions.RemoveEmptyEntries);
            string BuildSection() {
                if (_items.Count == 0) return string.Empty;
                var sb = new StringBuilder();
                sb.Append(_section + "\n");
                foreach (var sKey in _items.Keys)
                    sb.Append(sKey + "=" + _items[sKey] + "\n");
                return sb.ToString().Trim();
            }
        }
    }
}
