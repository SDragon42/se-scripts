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
        class ScriptSettings {
            const string DEF_ProgName = "";
            const string DEF_LogLcdName = "";
            const int DEF_NumLogLines = 20;

            const string KEY_ProgramBlockName = "Program Block";
            const string KEY_LogDisplayName = "Log LCD Name";
            const string KEY_LogLinesToShow = "Lines to Show";

            public void InitConfig(CustomDataConfig config) {
                config.AddKey(KEY_ProgramBlockName,
                    description: "The is the name of the program block forward messages to.",
                    defaultValue: DEF_ProgName);
                config.AddKey(KEY_LogDisplayName,
                    description: "The LCD to display the log on. (OPTIONAL)",
                    defaultValue: DEF_LogLcdName);
                config.AddKey(KEY_LogLinesToShow,
                    defaultValue: DEF_NumLogLines.ToString());
            }
            public void LoadFromSettingDict(CustomDataConfig config) {
                ProgramBlockName = config.GetValue(KEY_ProgramBlockName, DEF_ProgName);
                LogLcdName = config.GetValue(KEY_LogDisplayName, DEF_LogLcdName);
                LogLines2Show = config.GetValue(KEY_LogLinesToShow).ToInt(DEF_NumLogLines);
            }
            public void BuidSettingDict(CustomDataConfig config) {
                config.SetValue(KEY_ProgramBlockName, ProgramBlockName);
                config.SetValue(KEY_LogDisplayName, LogLcdName);
                config.SetValue(KEY_LogLinesToShow, LogLines2Show.ToString());
            }

            public string ProgramBlockName { get; private set; }
            public string LogLcdName { get; private set; }
            public int LogLines2Show { get; private set; }
        }
    }
}
