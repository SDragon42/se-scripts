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
    class ScriptSettingsModule {
        const string DEFAULT_StationTag = "[ops]";
        const double DEFAULT_DoorCloseDelay = 4.0;

        const string KEY_StationTag = "Station Tag";
        const string KEY_TimeToLeaveDoorOpen = "Time to Leave Doors Open";

        public void InitConfig(CustomDataConfigModule config) {
            config.AddKey(KEY_StationTag,
                description: "This is the name tag to add to the blocks so that the script can\ncontrol them.",
                defaultValue: DEFAULT_StationTag);

            config.AddKey(KEY_TimeToLeaveDoorOpen,
                description: "This the the amount of time (in seconds) to leave\ninternal doors open before closing them.",
                defaultValue: DEFAULT_DoorCloseDelay.ToString());
        }
        public void LoadFromSettingDict(CustomDataConfigModule config) {
            StationTag = config.GetValue(KEY_StationTag, DEFAULT_StationTag);
            DoorCloseDelay = config.GetValue(KEY_TimeToLeaveDoorOpen).ToDouble(DEFAULT_DoorCloseDelay);
        }
        public void BuidSettingDict(CustomDataConfigModule config) {
            config.SetValue(KEY_StationTag, StationTag);
            config.SetValue(KEY_TimeToLeaveDoorOpen, DoorCloseDelay.ToString());
        }

        public string StationTag { get; private set; }
        public double DoorCloseDelay { get; private set; }
    }
}
