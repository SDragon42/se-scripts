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

namespace IngameScript
{
    class CustomDataConfigModule
    {
        readonly char[] SepNewLine = new char[] { '\n' };
        readonly char[] SepEquals = new char[] { '=' };

        private readonly Dictionary<string, CustomDataConfigItem> _items;

        public CustomDataConfigModule()
        {
            _items = new Dictionary<string, CustomDataConfigItem>();
        }


        public void Clear()
        {
            _items.Clear();
        }

        public void AddKey(object key, string description = "", string defaultValue = "")
        {
            var sKey = key.ToString();
            if (_items.ContainsKey(sKey)) return;
            _items.Add(sKey, new CustomDataConfigItem(description, defaultValue));
        }
        public bool ContainsKey(object key)
        {
            var sKey = key.ToString();
            return _items.ContainsKey(sKey);
        }


        public void ReadFromCustomData(IMyTerminalBlock block, bool addIfMissing = false)
        {
            if (block == null) return;
            var datalines = block.CustomData.Split(SepNewLine, StringSplitOptions.None);
            foreach (var line in datalines)
            {
                if (line.Length <= 0) continue;
                if (line.StartsWith("# ")) continue;

                var settingParts = line.Split(SepEquals, 2);
                if (settingParts == null) continue;
                if (settingParts.Length != 2) continue;

                var readKey = settingParts[0].Trim();
                if (!_items.ContainsKey(readKey))
                {
                    if (!addIfMissing) continue;
                    AddKey(readKey);
                }

                _items[readKey].SetValue(settingParts[1].Trim());
            }
        }
        public void SaveToCustomData(IMyTerminalBlock block)
        {
            if (block == null) return;
            var sb = new StringBuilder();
            foreach (var sKey in _items.Keys)
            {
                if (_items[sKey].GetDescription().Length > 0)
                    sb.Append("\n# " + _items[sKey].GetDescription().Replace("\n", "\n# ") + "\n");
                sb.Append(sKey + " = " + _items[sKey].GetValue() + "\n");
            }
            block.CustomData = sb.ToString().Trim();
        }


        public void SetValue<T>(object key, T val)
        {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return;
            _items[sKey].SetValue((val != null) ? val.ToString() : string.Empty);
        }
        public string GetString(object key, string defVal = "")
        {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return defVal;
            return _items[sKey].GetValue();
        }
        public int GetInt(object key, int defVal = 0)
        {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return defVal;
            int val;
            if (!int.TryParse(_items[sKey].GetValue(), out val))
                val = defVal;
            return val;
        }
        public float GetFloat(object key, float defVal = 0f)
        {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return defVal;
            float val;
            if (!float.TryParse(_items[sKey].GetValue(), out val))
                val = defVal;
            return val;
        }
        public double GetDouble(object key, double defVal = 0.0)
        {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return defVal;
            double val;
            if (!double.TryParse(_items[sKey].GetValue(), out val))
                val = defVal;
            return val;
        }
        public bool GetBoolean(object key, bool defVal = false)
        {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return defVal;
            bool val;
            if (!bool.TryParse(_items[sKey].GetValue(), out val))
                val = defVal;
            return val;
        }
        public bool GetBoolean2(object key, bool defVal = false)
        {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return defVal;
            var sVal = _items[sKey].GetValue().ToLower();
            bool val;
            if (bool.TryParse(_items[sKey].GetValue(), out val)) return val;
            if (sVal == "t") return true;
            if (sVal == "y") return true;
            if (sVal == "yes") return true;
            if (sVal == "1") return true;
            return false;
        }
        public T GetEnum<T>(object key, T defVal = default(T)) where T : struct
        {
            var sKey = key.ToString();
            if (!_items.ContainsKey(sKey)) return defVal;
            T val;
            if (!Enum.TryParse(_items[sKey].GetValue(), out val)) return defVal;
            return val;
        }

    }
}
