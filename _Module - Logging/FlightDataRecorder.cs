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
    class FlightDataRecorder
    {
        //private readonly StringBuilder _log;
        readonly LogModule _log;
        readonly Dictionary<string, string> _items;
        readonly string[] _keyList;

        public FlightDataRecorder(string[] keyList, int maxNumLogEntries = 100)
        {
            _keyList = keyList;
            if (maxNumLogEntries < 1) maxNumLogEntries = 1;
            _log = new LogModule(maxNumLogEntries);
            _items = new Dictionary<string, string>();
            foreach (var key in _keyList)
                _items.Add(key, string.Empty);
            ClearLog();
        }

        bool _enabled = true;
        public bool GetEnabled() { return _enabled; }
        public void SetEnabled(bool value) { _enabled = value; }


        public void ClearEntry()
        {
            if (!_enabled) return;
            foreach (var key in _keyList)
                _items[key] = string.Empty;
        }
        public void RecordEntry()
        {
            if (!_enabled) return;
            _log.Append(DateTime.Now.ToString("HH:mm:ss.f"));
            foreach (var key in _keyList)
                _log.Append("\t" + _items[key]);
            _log.Append("\n");
        }
        public void SetEntry(object key, string val)
        {
            SetEntry(key.ToString(), val);
        }
        public void SetEntry(string key, string val)
        {
            if (!_enabled) return;
            if (_items.ContainsKey(key))
                _items[key] = val;
        }

        public void ClearLog()
        {
            if (!_enabled) return;
            _log.Clear();
            // Create Log Header
            _log.Append("Time");
            _log.Append("\t" + string.Join("\t", _keyList));
            //foreach (var key in _keyList)
            //    _log.Append("\t" + key);
            _log.Append("\n");
        }
        public string GetLog()
        {
            if (!_enabled) return string.Empty;
            return _log.ToString();
        }
    }
}
