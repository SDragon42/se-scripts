using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    class ScriptSettingsModule
    {
        const string DEFAULT_StationTag = "[station]";
        const string DEFAULT_TerminalTag = "[Terminal]";
        const string DEFAULT_TransferTag = "[Transfer Arm]";
        const double DEFAULT_DoorCloseDelay = 4.0;

        const string KEY_StationTag = "Station Tag";
        const string KEY_TerminalTag = "Terminal Tag";
        const string KEY_TransferTag = "Transfer Arm Tag";
        const string KEY_TimeToLeaveDoorOpen = "Time to Leave Doors Open";

        public void InitConfig(CustomDataConfigModule config)
        {
            config.AddKey(KEY_StationTag,
                description: "This is the name tag to add to the blocks so that the script can\ncontrol them.",
                defaultValue: DEFAULT_StationTag);
            config.AddKey(KEY_TerminalTag,
                defaultValue: DEFAULT_TerminalTag);
            config.AddKey(KEY_TransferTag,
                defaultValue: DEFAULT_TransferTag);
        }
        public void LoadFromSettingDict(CustomDataConfigModule config)
        {
            _StationTag = config.GetValue(KEY_StationTag, DEFAULT_StationTag);
            _TerminalTag = config.GetValue(KEY_TerminalTag, DEFAULT_TerminalTag);
            _TransferTag = config.GetValue(KEY_TransferTag, DEFAULT_TransferTag);
            _DoorCloseDelay = config.GetValue(KEY_TimeToLeaveDoorOpen).ToDouble(DEFAULT_DoorCloseDelay);
        }
        public void BuidSettingDict(CustomDataConfigModule config)
        {
            config.SetValue(KEY_StationTag, _StationTag);
            config.SetValue(KEY_TerminalTag, _TerminalTag);
            config.SetValue(KEY_TransferTag, _TransferTag);
            config.SetValue(KEY_TimeToLeaveDoorOpen, _DoorCloseDelay.ToString());
        }

        string _StationTag;
        public string GetStationTag() { return _StationTag; }
        public void SetStationTag(string value) { _StationTag = value ?? String.Empty; }

        string _TerminalTag;
        public string GetTerminalTag() { return _TerminalTag; }
        public void SetTerminalTag(string value) { _TerminalTag = value ?? String.Empty; }

        string _TransferTag;
        public string GetTransferTag() { return _TransferTag; }
        public void SetTransferTag(string value) { _TransferTag = value ?? String.Empty; }

        double _DoorCloseDelay;
        public double GetDoorCloseDelay() { return _DoorCloseDelay; }
        public void SetDoorCloseDelay(double value) { _DoorCloseDelay = value; }

    }
}
