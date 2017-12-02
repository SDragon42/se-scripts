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
    class CustomDataConfig {
        readonly char[] SepNewLine = new char[] { '\n' };
        readonly char[] SepEquals = new char[] { '=' };
        readonly Dictionary<string, ConfigItem> _items = new Dictionary<string, ConfigItem>();


        public void Clear() {
            _items.Clear();
        }

        public void AddKey(object key, string description = "", string defaultValue = "") {
            var sKey = key.ToString();
            if (_items.ContainsKey(sKey)) return;
            _items.Add(sKey, new ConfigItem(description, defaultValue));
        }
        public bool ContainsKey(object key) {
            var sKey = key.ToString();
            return _items.ContainsKey(sKey);
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


        public void SetValue<T>(object key, T val) {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return;
            _items[sKey].Value = (val != null) ? val.ToString() : string.Empty;
        }
        public string GetValue(object key, string defVal = "") {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return defVal;
            return _items[sKey].Value;
        }


        class ConfigItem {
            public ConfigItem(string description, string val) {
                Description = description ?? string.Empty;
                Value = val;
            }

            public string Description { get; private set; }

            string _value;
            public string Value { get { return _value; } set { _value = value ?? string.Empty; } }
        }

    }
}
