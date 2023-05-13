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

        const string SECTION_REMOTE_TURRET = "Remote Turret";

        readonly MyIniKey Key_CommGroupName = new MyIniKey(SECTION_REMOTE_TURRET, "COMM Group Name");
        readonly MyIniKey Key_TurretId = new MyIniKey(SECTION_REMOTE_TURRET, "ID");
        readonly MyIniKey Key_StealthMode = new MyIniKey(SECTION_REMOTE_TURRET, "Stealth Mode Enabled");
        readonly MyIniKey Key_ShowStatusOnAntenna = new MyIniKey(SECTION_REMOTE_TURRET, "Show Status on Antenna");
        readonly MyIniKey Key_ReportStatusOnComms = new MyIniKey(SECTION_REMOTE_TURRET, "Report Status on COMMs");


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

            ini.Add(Key_CommGroupName, "Deployed-Defense");
            ini.Add(Key_TurretId, string.Empty);
            ini.Add(Key_StealthMode, false);
            ini.Add(Key_ShowStatusOnAntenna, true);
            ini.Add(Key_ReportStatusOnComms, true);

            Me.CustomData = ini.ToString();

            CommGroupName = ini.Get(Key_CommGroupName).ToString();
            TurretId = ini.Get(Key_TurretId).ToString();
            StealthMode = ini.Get(Key_StealthMode).ToBoolean();
            ShowStatusOnAntenna = ini.Get(Key_ShowStatusOnAntenna).ToBoolean();
            ReportStatusOnCOMMs = ini.Get(Key_ReportStatusOnComms).ToBoolean();

            if (string.IsNullOrEmpty(TurretId))
                TurretId = Me.CubeGrid.EntityId.ToString();
        }
    }
}
