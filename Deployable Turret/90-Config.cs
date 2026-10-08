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

        class Config : ConfigBase {
            public string CommGroupName { get; set; } = "Deployed-Defense";
            public string Id { get; set; } = string.Empty;
            public bool StealthMode { get; set; } = false;
            public bool ShowStatusOnAntenna { get; set; } = true;
            public bool ReportStatusOnCOMMs { get; set; } = true;
            // public bool ShowStatusLights { get; set; } = false;
            // public bool ShowStatusAntenna { get; set; } = false;

            const string SECTION_REMOTE_TURRET = "Remote Turret";

            public void Load() {
                if (!LoadIni()) return;

                CommGroupName = _ini.Add(SECTION_REMOTE_TURRET, "COMM Group Name", CommGroupName).ToString();
                Id = _ini.Add(SECTION_REMOTE_TURRET, "ID", Id).ToString();
                StealthMode = _ini.Add(SECTION_REMOTE_TURRET, "Stealth Mode Enabled", false).ToBoolean();
                ShowStatusOnAntenna = _ini.Add(SECTION_REMOTE_TURRET, "Show Status on Antenna", true).ToBoolean();
                ReportStatusOnCOMMs = _ini.Add(SECTION_REMOTE_TURRET, "Report Status on COMMs", true).ToBoolean();

                if (string.IsNullOrEmpty(Id)) {
                    Id = Block.CubeGrid.EntityId.ToString();
                    _ini.Set(SECTION_REMOTE_TURRET, "ID", Id);
                }

                Save();
            }
        }

    }
}
