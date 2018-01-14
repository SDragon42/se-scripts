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
    class ScriptSettings {
        const string DEFAULT_StationTag = "[ops]";

        const string KEY_StationTag = "Station Tag";

        public void InitConfig(CustomDataConfig config) {
            config.AddKey(KEY_StationTag,
                description: "This is the name tag to add to the blocks so that the script can\ncontrol them.",
                defaultValue: DEFAULT_StationTag);
        }
        public void LoadFromSettingDict(CustomDataConfig config) {
            StationTag = config.GetValue(KEY_StationTag, DEFAULT_StationTag);
        }
        public void BuidSettingDict(CustomDataConfig config) {
            config.SetValue(KEY_StationTag, StationTag);
        }

        public string StationTag { get; private set; }
    }
}
