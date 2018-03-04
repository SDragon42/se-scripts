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
        class ConfigCustom : ConfigBase<ConfigItem> {
            public void AddKey(string key, string description = "", string defaultValue = "") {
                if (!ContainsKey(key)) _items.Add(key, new ConfigItem(description, defaultValue));
            }

            public string GetValue(string key, string defVal = "") {
                return ContainsKey(key) ? _items[key].Value : defVal;
            }
            public void SetValue<T>(string key, T val) {
                if (ContainsKey(key)) _items[key].Value = (val != null) ? val.ToString() : string.Empty;
            }

            public override void Load(IMyTerminalBlock b, bool addIfMissing = false) {
                if (b == null) return;
                var datalines = b.CustomData.Split(SepNewLine, StringSplitOptions.None);
                foreach (var line in datalines) {
                    if (line.Length <= 0) continue;
                    if (line.StartsWith("# ")) continue;

                    var parts = line.Split(SepEquals, 2);
                    if (parts == null) continue;
                    if (parts.Length != 2) continue;

                    var readKey = parts[0].Trim();
                    if (!ContainsKey(readKey)) {
                        if (!addIfMissing) continue;
                        AddKey(readKey);
                    }

                    _items[readKey].Value = parts[1].Trim();
                }
            }
            public override void Save(IMyTerminalBlock b) {
                if (b == null) return;
                var sb = new StringBuilder();
                foreach (var sKey in _items.Keys) {
                    if (_items[sKey].Description.Length > 0)
                        sb.Append("\n# " + _items[sKey].Description.Replace("\n", "\n# ") + "\n");
                    sb.Append(sKey + " = " + _items[sKey].Value + "\n");
                }
                b.CustomData = sb.ToString().Trim();
            }
        }
    }
}
