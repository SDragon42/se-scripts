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

        class ScriptConfig : ConfigBase {

            const string SECTION = "Drop GPS Recorder";
            const string DEFAULT_TAG = "[drop-gps]";
            const string DEFAULT_GPS_LABEL = "Probe Dropped";

            public string LcdTag { get; private set; } = DEFAULT_TAG;
            public string MergeTag { get; private set; } = string.Empty;
            public string GpsLabel { get; private set; } = DEFAULT_GPS_LABEL;

            public void Load() {
                if (!LoadIni()) return;

                LcdTag = _ini.Add(SECTION, "LCD Tag", LcdTag).ToString();
                MergeTag = _ini.Add(SECTION, "Merge Tag", MergeTag).ToString();
                GpsLabel = _ini.Add(SECTION, "GPS Label", GpsLabel).ToString();

                Save();
            }

        }

    }
}
