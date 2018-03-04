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
        class ConfigINI : ConfigBase<string> {
            readonly string _section;

            public ConfigINI(string section) {
                _section = $"[{section}]";
            }

            public void AddKey(string key, string defaultValue = "") {
                if (!ContainsKey(key)) _items.Add(key, defaultValue);
            }

            public string GetValue(string key, string defVal = "") {
                return ContainsKey(key) ? _items[key] : defVal;
            }
            public void SetValue<T>(string key, T val) {
                if (ContainsKey(key)) _items[key] = (val != null) ? val.ToString() : string.Empty;
            }

            public override void Load(IMyTerminalBlock b, bool addIfMissing = false) {
                if (b == null) return;
                var mySection = GetSections(b.CustomData)
                    .Where(IsMySection)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(mySection)) return;

                var lines = mySection.Split(SepNewLine)
                    .Select(i => i.Split(SepEquals, 2))
                    .Where(p => p.Length == 2);
                foreach (var cfgItem in lines) {
                    if (!ContainsKey(cfgItem[0])) {
                        if (addIfMissing)
                            AddKey(cfgItem[0], cfgItem[1]);
                    } else
                        _items[cfgItem[0]] = cfgItem[1];
                }
            }
            public override void Save(IMyTerminalBlock b) {
                if (b == null) return;
                var allSections = GetSections(b.CustomData);
                var written = false;
                var sb = new StringBuilder();
                Action<string> addSection = (sec) => {
                    if (sec.Length == 0) return;
                    if (sb.Length > 0) sb.Append("\n");
                    sb.Append(sec + "\n");
                };
                for (var i = 0; i < allSections.Length; i++) {
                    var sec = allSections[i].Trim();
                    if (IsMySection(sec)) {
                        sec = BuildSection();
                        written = true;
                    }
                    addSection(sec);
                }

                if (!written) addSection(BuildSection());

                b.CustomData = sb.ToString().Trim();
            }

            bool IsMySection(string t) => t.StartsWith(_section);

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
