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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    class TimBlockConfigData {
        public Action<string> Echo = (text) => { };
        public Action<string> Debug = (text) => { };

        bool GetIndexes(IMyTerminalBlock b, string key, out int start, out int end) {
            start = b.CustomData.IndexOf(key);
            if (start < 0) {
                start = -1;
                end = -1;
                return false;
            }

            start += key.Length;
            end = b.CustomData.IndexOf('[', start) - 1;
            if (end < 0) end = b.CustomData.Length - 1;
            return true;
        }
        public bool Get(IMyTerminalBlock b, string key, out string timConfig) {
            Debug("ConfigData.Get()");
            timConfig = string.Empty;

            int start, end;
            if (!GetIndexes(b, key, out start, out end)) return false;

            timConfig = b.CustomData
                .Substring(start, end - start + 1)
                .Replace('\n', ' ')
                .Trim();
            return true;
        }
        public void Set(IMyTerminalBlock b, string key, string timConfig) {
            try {
                Debug("ConfigData.Set()");
                Debug(">> " + timConfig);
                int start, end;
                if (!GetIndexes(b, key, out start, out end)) {
                    b.CustomData += $"\n\n{key}";
                    if (!GetIndexes(b, key, out start, out end)) return;
                }

                var data = b.CustomData.Remove(start, end - start + 1);
                timConfig = timConfig.Replace(' ', '\n') + "\n\n";
                b.CustomData = data.Insert(start, timConfig);
            } finally {
                b.CustomData = b.CustomData.Trim();
            }
        }
    }
}
