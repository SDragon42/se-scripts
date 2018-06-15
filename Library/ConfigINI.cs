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
            protected readonly static char[] SepColon = new char[] { ':' };

            readonly string _section;

            public ConfigINI(string section) {
                _section = $"<{section}>";
            }

            public void AddKey(string key) => AddKey(key, string.Empty);
            public void AddKey<T>(string key, T initalValue) {
                if (!ContainsKey(key)) Items.Add(key, initalValue?.ToString() ?? string.Empty);
            }

            public string GetValue(string key, string defaultValue = "") {
                return ContainsKey(key) ? Items[key] : defaultValue;
            }
            public void SetValue<T>(string key, T value) {
                if (ContainsKey(key)) Items[key] = value?.ToString() ?? string.Empty;
            }

            public override void Load(IMyTerminalBlock block, bool addIfMissing = false) {
                if (block == null) return;
                var mySection = GetSections(block.CustomData)
                    .Where(IsMySection)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(mySection)) return;

                var lines = mySection.Split(SepNewLine)
                    .Select(i => i.Split(SepColon, 2))
                    .Where(p => p.Length == 2);
                foreach (var cfgItem in lines) {
                    if (!ContainsKey(cfgItem[0])) {
                        if (addIfMissing)
                            AddKey(cfgItem[0], cfgItem[1].Trim());
                    } else
                        Items[cfgItem[0]] = cfgItem[1].Trim();
                }
            }
            public override void Save(IMyTerminalBlock block) {
                if (block == null) return;
                var allSections = GetSections(block.CustomData);
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

                block.CustomData = sb.ToString().Trim();
            }

            bool IsMySection(string text) => text.StartsWith(_section);

            string[] GetSections(string data) => data.Split(SepBlankLine, StringSplitOptions.RemoveEmptyEntries);

            string BuildSection() {
                if (Items.Count == 0) return string.Empty;
                var sb = new StringBuilder();
                sb.Append(_section + "\n");
                foreach (var sKey in Items.Keys)
                    sb.Append(sKey + ": " + Items[sKey] + "\n");
                return sb.ToString().Trim();
            }
        }
    }
}
