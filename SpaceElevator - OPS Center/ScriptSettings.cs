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
        class ScriptSettings {
            const string DEFAULT_StationTag = "[ops]";
            public const int DEF_NumLogLines = 20;

            const string KEY_StationTag = "Station Tag";
            const string KEY_LogDisplayName = "Log LCD Name";
            const string KEY_LogLinesToShow = "Lines to Show";

            public void InitConfig(ConfigCustom config) {
                config.AddKey(KEY_StationTag,
                    description: "This is the name tag to add to the blocks so that the script can\ncontrol them.",
                    defaultValue: DEFAULT_StationTag);

                config.AddKey(KEY_LogDisplayName,
                    description: "The LCD to display the log on. (OPTIONAL)");
                config.AddKey(KEY_LogLinesToShow,
                    defaultValue: DEF_NumLogLines.ToString());
            }
            public void LoadFromSettingDict(ConfigCustom config) {
                StationTag = config.GetValue(KEY_StationTag, DEFAULT_StationTag);
                LogLcdName = config.GetValue(KEY_LogDisplayName);
                LogLines2Show = config.GetValue(KEY_LogLinesToShow).ToInt(DEF_NumLogLines);
            }
            public void BuidSettingDict(ConfigCustom config) {
                config.SetValue(KEY_StationTag, StationTag);
                config.SetValue(KEY_LogDisplayName, LogLcdName);
                config.SetValue(KEY_LogLinesToShow, LogLines2Show.ToString());
            }

            public string StationTag { get; private set; }
            public string LogLcdName { get; private set; }
            public int LogLines2Show { get; private set; }
        }
    }
}
