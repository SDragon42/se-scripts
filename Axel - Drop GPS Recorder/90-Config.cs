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

        int configHash = -1;
        MyIniKey keyLcdTag = new MyIniKey("Drop GPS Recorder", "Tag");
        MyIniKey keyGpsLabel = new MyIniKey("Drop GPS Recorder", "GPS Label");
        void LoadConfig() {
            var hash = Me.CustomData.GetHashCode();
            if (configHash == hash) return;

            var ini = new MyIni();
            ini.TryParse(Me.CustomData);

            Tag = ini.Add(keyLcdTag, Tag).ToString();
            GpsLabel = ini.Add(keyGpsLabel, GpsLabel).ToString();

            Me.CustomData = ini.ToString();
            configHash = Me.CustomData.GetHashCode();
        }

    }
}
