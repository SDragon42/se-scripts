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
        class CustomDataConfig {
            readonly char[] SepNewLine = new char[] { '\n' };
            readonly char[] SepEquals = new char[] { '=' };
            readonly Dictionary<string, ConfigItem> _items = new Dictionary<string, ConfigItem>();


            public void Clear() {
                _items.Clear();
            }

            public void AddKey(string key, string description = "", string defaultValue = "") {
                if (_items.ContainsKey(key)) return;
                _items.Add(key, new ConfigItem(description, defaultValue));
            }
            public bool ContainsKey(string key) {
                return _items.ContainsKey(key);
            }


            public void ReadFromCustomData(IMyTerminalBlock b, bool addIfMissing = false) {
                if (b == null) return;
                var datalines = b.CustomData.Split(SepNewLine, StringSplitOptions.None);
                foreach (var line in datalines) {
                    if (line.Length <= 0) continue;
                    if (line.StartsWith("# ")) continue;

                    var parts = line.Split(SepEquals, 2);
                    if (parts == null) continue;
                    if (parts.Length != 2) continue;

                    var readKey = parts[0].Trim();
                    if (!_items.ContainsKey(readKey)) {
                        if (!addIfMissing) continue;
                        AddKey(readKey);
                    }

                    _items[readKey].Value = parts[1].Trim();
                }
            }
            public void SaveToCustomData(IMyTerminalBlock b) {
                if (b == null) return;
                var sb = new StringBuilder();
                foreach (var sKey in _items.Keys) {
                    if (_items[sKey].Description.Length > 0)
                        sb.Append("\n# " + _items[sKey].Description.Replace("\n", "\n# ") + "\n");
                    sb.Append(sKey + " = " + _items[sKey].Value + "\n");
                }
                b.CustomData = sb.ToString().Trim();
            }


            public void SetValue<T>(string key, T val) {
                if (!_items.ContainsKey(key)) return;
                _items[key].Value = (val != null) ? val.ToString() : string.Empty;
            }
            public string GetValue(string key, string defVal = "") {
                if (!_items.ContainsKey(key)) return defVal;
                return _items[key].Value;
            }
        }
    }
}
