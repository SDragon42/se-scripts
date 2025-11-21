using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        class ConfigCustom {
            readonly static string[] SepBlankLine = new string[] { "\n\n" };
            readonly static char[] SepNewLine = new char[] { '\n' };
            readonly static char[] SepEquals = new char[] { '=' };
            readonly Dictionary<string, ConfigItem> Items = new Dictionary<string, ConfigItem>();

            public void Clear() => Items.Clear();

            public bool ContainsKey(string key) => Items.ContainsKey(key);

            public void AddKey(string key, string description = "", string defaultValue = "") {
                if (!ContainsKey(key)) Items.Add(key, new ConfigItem(description, defaultValue));
            }

            public string GetValue(string key, string defVal = "") {
                return ContainsKey(key) ? Items[key].Value : defVal;
            }
            public void SetValue<T>(string key, T val) {
                if (ContainsKey(key)) Items[key].Value = (val != null) ? val.ToString() : string.Empty;
            }

            public void Load(IMyTerminalBlock b, bool addIfMissing = false) {
                if (b == null) return;
                var dataLines = b.CustomData.Split(SepNewLine, StringSplitOptions.None);
                foreach (var line in dataLines) {
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

                    Items[readKey].Value = parts[1].Trim();
                }
            }
            public void Save(IMyTerminalBlock b) {
                if (b == null) return;
                var sb = new StringBuilder();
                foreach (var sKey in Items.Keys) {
                    if (Items[sKey].Description.Length > 0)
                        sb.Append("\n# " + Items[sKey].Description.Replace("\n", "\n# ") + "\n");
                    sb.Append(sKey + " = " + Items[sKey].Value + "\n");
                }
                b.CustomData = sb.ToString().Trim();
            }

            class ConfigItem {
                public ConfigItem(string description, string val) {
                    Description = description ?? string.Empty;
                    Value = val;
                }

                public string Description { get; private set; }

                string _value;
                public string Value { 
                    get { return _value; } 
                    set { _value = value ?? string.Empty; } 
                }
            }
        }
    }
}
