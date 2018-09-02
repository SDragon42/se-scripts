﻿using Sandbox.Game.EntityComponents;
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
            readonly Logging Log;
            readonly Dictionary<string, string> Items;

            public FlightDataRecorder(IList<string> keyList, int maxNumLogEntries = 100) {
                if (maxNumLogEntries < 1) maxNumLogEntries = 1;
                Log = new Logging(maxNumLogEntries);
                Items = new Dictionary<string, string>();
                foreach (var key in keyList) Items.Add(key, string.Empty);
                ClearLog();
            }

            public bool Enabled {
                get { return Log.Enabled; }
                set { Log.Enabled = value; }
            }


            public void ClearEntry() {
                if (!Enabled) return;
                foreach (var key in Items.Keys) Items[key] = string.Empty;
            }
            public void RecordEntry() {
                if (!Enabled) return;
                Log.Append(DateTime.Now.ToString("HH:mm:ss.f"));
                foreach (var key in Items.Keys) Log.Append("\t" + Items[key]);
                Log.Append("\n");
            }
            public void SetEntry(string key, string val) {
                if (!Enabled) return;
                if (Items.ContainsKey(key))
                    Items[key] = val;
            }

            public void ClearLog() {
                if (!Enabled) return;
                Log.Clear();
                Log.Append($"Time\t{string.Join("\t", Items.Keys)}\n");
            }
            public string GetLog() => Log.GetLogText();
        }
    }
}