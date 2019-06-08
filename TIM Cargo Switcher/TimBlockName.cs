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
    partial class Program {
        class TimBlockName {
            public Action<string> Echo = (text) => { };
            public Action<string> Debug = (text) => { };

            public string TimTag { get; set; }

            public string Get(IMyTerminalBlock b) {
                Debug("TimBlockConfig.Get()");
                var start = b.CustomName.IndexOf("[" + TimTag);
                if (start < 0) return string.Empty;

                start += TimTag.Length + 1;
                var end = b.CustomName.IndexOf(']', start);

                var timConfig = b.CustomName.Substring(start, end - start);
                Debug(">> " + timConfig);
                return timConfig;
            }

            public void Replace(IMyTerminalBlock b, string timConfig) {
                Debug("TimBlockConfig.Replace()");
                Remove(b);
                b.CustomName = $"{b.CustomName} [{TimTag} {timConfig}]";
            }

            void Remove(IMyTerminalBlock b) {
                var start = b.CustomName.IndexOf("[" + TimTag);
                if (start < 0) return;
                var end = b.CustomName.IndexOf(']', start);

                b.CustomName = b.CustomName.Remove(start, end - start + 1).Trim();
            }
        }
    }
}
