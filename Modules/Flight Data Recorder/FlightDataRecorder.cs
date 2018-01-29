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
        class FlightDataRecorder {
            readonly Logging _log;
            readonly Dictionary<string, string> _items;
            readonly string[] _keyList;

            public FlightDataRecorder(string[] keyList, int maxNumLogEntries = 100) {
                _keyList = keyList;
                if (maxNumLogEntries < 1) maxNumLogEntries = 1;
                _log = new Logging(maxNumLogEntries);
                _items = new Dictionary<string, string>();
                foreach (var key in _keyList) _items.Add(key, string.Empty);
                ClearLog();
            }

            public bool Enabled { get; set; }


            public void ClearEntry() {
                if (!Enabled) return;
                foreach (var key in _keyList) _items[key] = string.Empty;
            }
            public void RecordEntry() {
                if (!Enabled) return;
                _log.Append(DateTime.Now.ToString("HH:mm:ss.f"));
                foreach (var key in _keyList) _log.Append("\t" + _items[key]);
                _log.Append("\n");
            }
            public void SetEntry(object key, string val) {
                SetEntry(key.ToString(), val);
            }
            public void SetEntry(string key, string val) {
                if (!Enabled) return;
                if (_items.ContainsKey(key))
                    _items[key] = val;
            }

            public void ClearLog() {
                if (!Enabled) return;
                _log.Clear();
                // Create Log Header
                _log.Append($"Time\t{string.Join("\t", _keyList)}\n");
            }
            public string GetLog() {
                if (!Enabled) return string.Empty;
                return _log.ToString();
            }
        }
    }
}
