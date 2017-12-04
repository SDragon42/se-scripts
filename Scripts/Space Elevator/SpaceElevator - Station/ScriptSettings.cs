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
        const double DEFAULT_DoorCloseDelay = 4.0;

        const string KEY_StationTag = "Station Tag";
        const string KEY_TerminalTag = "Terminal Tag";
        const string KEY_TransferTag = "Transfer Arm Tag";
        const string KEY_TimeToLeaveDoorOpen = "Time to Leave Doors Open";

        public void InitConfig(CustomDataConfig config) {
            config.AddKey(KEY_StationTag,
                description: "This is the name tag to add to the blocks so that the script can\ncontrol them.",
                defaultValue: DEFAULT_StationTag);
            config.AddKey(KEY_TerminalTag,
                defaultValue: DEFAULT_TerminalTag);
            config.AddKey(KEY_TransferTag,
                defaultValue: DEFAULT_TransferTag);

            config.AddKey(KEY_TimeToLeaveDoorOpen,
                description: "This the the amount of time (in seconds) to leave\ninternal doors open before closing them.",
                defaultValue: DEFAULT_DoorCloseDelay.ToString());
        }
        public void LoadFromSettingDict(CustomDataConfig config) {
            StationTag = config.GetValue(KEY_StationTag, DEFAULT_StationTag);
            TerminalTag = config.GetValue(KEY_TerminalTag, DEFAULT_TerminalTag);
            TransferTag = config.GetValue(KEY_TransferTag, DEFAULT_TransferTag);
            DoorCloseDelay = config.GetValue(KEY_TimeToLeaveDoorOpen).ToDouble(DEFAULT_DoorCloseDelay);
        }
        public void BuidSettingDict(CustomDataConfig config) {
            config.SetValue(KEY_StationTag, StationTag);
            config.SetValue(KEY_TerminalTag, TerminalTag);
            config.SetValue(KEY_TransferTag, TransferTag);
            config.SetValue(KEY_TimeToLeaveDoorOpen, DoorCloseDelay.ToString());
        }

        public string StationTag { get; private set; }
        public string TerminalTag { get; private set; }
        public string TransferTag { get; private set; }
        public double DoorCloseDelay { get; private set; }

    }
}
