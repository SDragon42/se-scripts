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

namespace IngameScript {
    partial class Program {
        class Settings {

            public void Load(ConfigINI config) {
                CommGroupName = config.GetValue(ConfigKeys.COMM_GROUP_NAME);
                StealthMode = config.GetValue(ConfigKeys.STEALTH_MODE).ToBoolean();
                if (!StealthMode) {
                    ShowStatusLights = config.GetValue(ConfigKeys.STATUS_LIGHTS).ToBoolean();
                    ShowStatusAntenna = config.GetValue(ConfigKeys.STATUS_ANTENNA).ToBoolean();
                    ReportStatusCOMMs = config.GetValue(ConfigKeys.STATUS_COMMS).ToBoolean();
                }
            }

            public string CommGroupName { get; private set; } = "";
            public bool StealthMode { get; private set; } = false;
            public bool ShowStatusLights { get; private set; } = false;
            public bool ShowStatusAntenna { get; private set; } = false;
            public bool ReportStatusCOMMs { get; private set; } = false;
        }
    }
}
