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

        int configHashCode = 0;

        const string SECTION_SANDBAG = "Sandbag";

        readonly MyIniKey Key_CommGroupName = new MyIniKey(SECTION_SANDBAG, "COMM Group Name");
        readonly MyIniKey Key_StealthMode = new MyIniKey(SECTION_SANDBAG, "Stealth Mode Enabled");
        readonly MyIniKey Key_StatusLights = new MyIniKey(SECTION_SANDBAG, "Use Status Lights");
        readonly MyIniKey Key_StatusAntenna = new MyIniKey(SECTION_SANDBAG, "Use Status Antenna");
        readonly MyIniKey Key_StatusComms = new MyIniKey(SECTION_SANDBAG, "Use Status COMMs");


        void LoadConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (configHashCode == tmpHashCode) return;
            configHashCode = tmpHashCode;

            MyIniParseResult result;
            var ini = new MyIni();
            ini.Clear();
            if (!ini.TryParse(Me.CustomData, out result)) {
                ini.EndContent = Me.CustomData;
            }

            ini.Add(Key_CommGroupName, "sandbag");
            ini.Add(Key_StealthMode, false);
            ini.Add(Key_StatusLights, true);
            ini.Add(Key_StatusAntenna, true);
            ini.Add(Key_StatusComms, true);

            Me.CustomData = ini.ToString();

            CommGroupName = ini.Get(Key_CommGroupName).ToString();
            StealthMode = ini.Get(Key_StealthMode).ToBoolean();
            if (!StealthMode) {
                ShowStatusLights = ini.Get(Key_StatusLights).ToBoolean();
                ShowStatusAntenna = ini.Get(Key_StatusAntenna).ToBoolean();
                //ReportStatusCOMMs = ini.Get(Key_StatusComms).ToBoolean();
            } else {
                ShowStatusLights = false;
                ShowStatusAntenna = false;
                //ReportStatusCOMMs = false;
            }
        }
    }
}
