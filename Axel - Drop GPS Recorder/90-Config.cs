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

        class ScriptConfig {

            const string SECTION = "Drop GPS Recorder";

            const string DefaultTag = "[drop-gps]";
            const string DefaultGpsLabel = "Probe Dropped";

            int hash = -1;

            public string LcdTag { get; private set; } = DefaultTag;
            public string MergeTag { get; private set; } = string.Empty;
            public string GpsLabel { get; private set; } = DefaultGpsLabel;

            public void Load(IMyTerminalBlock b) {
                var currHash = b.CustomData.GetHashCode();
                if (hash == currHash) return;

                var ini = new MyIni();
                ini.TryParse(b.CustomData);

                LcdTag = ini.Add(SECTION, "LCD Tag", LcdTag).ToString();
                MergeTag = ini.Add(SECTION, "Merge Tag", MergeTag).ToString();
                GpsLabel = ini.Add(SECTION, "GPS Label", GpsLabel).ToString();

                b.CustomData = ini.ToString();
                hash = b.CustomData.GetHashCode();
            }

        }

    }
}
