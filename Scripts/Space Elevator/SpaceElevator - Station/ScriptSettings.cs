using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript {
    class ScriptSettings {
        const string DEFAULT_StationTag = "[station]";
        const string DEFAULT_TerminalTag = "[Terminal]";
        const string DEFAULT_TransferTag = "[Transfer Arm]";
        public const int DEF_NumLogLines = 20;

        const string KEY_StationTag = "Station Tag";
        const string KEY_TerminalTag = "Terminal Tag";
        const string KEY_TransferTag = "Transfer Arm Tag";

        const string KEY_LogDisplayName = "Log LCD Name";
        const string KEY_LogLinesToShow = "Lines to Show";

        public void InitConfig(CustomDataConfig config) {
            config.AddKey(KEY_StationTag,
                description: "This is the name tag to add to the blocks so that the script can\ncontrol them.",
                defaultValue: DEFAULT_StationTag);
            config.AddKey(KEY_TerminalTag,
                defaultValue: DEFAULT_TerminalTag);
            config.AddKey(KEY_TransferTag,
                defaultValue: DEFAULT_TransferTag);

            config.AddKey(KEY_LogDisplayName,
                description: "The LCD to display the log on. (OPTIONAL)");
            config.AddKey(KEY_LogLinesToShow,
                defaultValue: DEF_NumLogLines.ToString());
        }
        public void LoadFromSettingDict(CustomDataConfig config) {
            StationTag = config.GetValue(KEY_StationTag, DEFAULT_StationTag);
            TerminalTag = config.GetValue(KEY_TerminalTag, DEFAULT_TerminalTag);
            TransferTag = config.GetValue(KEY_TransferTag, DEFAULT_TransferTag);
            LogLcdName = config.GetValue(KEY_LogDisplayName);
            LogLines2Show = config.GetValue(KEY_LogLinesToShow).ToInt(DEF_NumLogLines);
        }
        public void BuidSettingDict(CustomDataConfig config) {
            config.SetValue(KEY_StationTag, StationTag);
            config.SetValue(KEY_TerminalTag, TerminalTag);
            config.SetValue(KEY_TransferTag, TransferTag);
            config.SetValue(KEY_LogDisplayName, LogLcdName);
            config.SetValue(KEY_LogLinesToShow, LogLines2Show.ToString());
        }

        public string StationTag { get; private set; }
        public string TerminalTag { get; private set; }
        public string TransferTag { get; private set; }

        public string LogLcdName { get; private set; }
        public int LogLines2Show { get; private set; }

    }
}
