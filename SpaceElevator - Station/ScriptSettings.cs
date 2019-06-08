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
            const string DEFAULT_StationTag = "[station]";
            const string DEFAULT_TerminalTag = "[Terminal]";
            const string DEFAULT_TransferTag = "[Transfer Arm]";

            const string KEY_StationTag = "Station Tag";
            const string KEY_TerminalTag = "Terminal Tag";
            const string KEY_TransferTag = "Transfer Arm Tag";

            public void InitConfig(ConfigCustom config) {
                config.AddKey(KEY_StationTag,
                    description: "This is the name tag to add to the blocks so that the script can\ncontrol them.",
                    defaultValue: DEFAULT_StationTag);
                config.AddKey(KEY_TerminalTag,
                    defaultValue: DEFAULT_TerminalTag);
                config.AddKey(KEY_TransferTag,
                    defaultValue: DEFAULT_TransferTag);
            }
            public void LoadFromSettingDict(ConfigCustom config) {
                StationTag = config.GetValue(KEY_StationTag, DEFAULT_StationTag);
                TerminalTag = config.GetValue(KEY_TerminalTag, DEFAULT_TerminalTag);
                TransferTag = config.GetValue(KEY_TransferTag, DEFAULT_TransferTag);
            }
            public void BuidSettingDict(ConfigCustom config) {
                config.SetValue(KEY_StationTag, StationTag);
                config.SetValue(KEY_TerminalTag, TerminalTag);
                config.SetValue(KEY_TransferTag, TransferTag);
            }

            public string StationTag { get; private set; }
            public string TerminalTag { get; private set; }
            public string TransferTag { get; private set; }
        }
    }
}
