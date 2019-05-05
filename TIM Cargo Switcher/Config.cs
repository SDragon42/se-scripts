using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        int _configHashCode = -1;

        const string SECTION_CONFIG = "TIM Cargo Switcher";
        readonly MyIniKey KEY_TimTag = new MyIniKey(SECTION_CONFIG, "TIM Tag");
        readonly MyIniKey KEY_CargoSwitcherTag = new MyIniKey(SECTION_CONFIG, "Cargo Switcher Tag");
        readonly MyIniKey KEY_ShowDebug = new MyIniKey(SECTION_CONFIG, "Show Debug Info");


        void LoadConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;
            _configHashCode = tmpHashCode;

            Ini.Clear();
            Ini.TryParse(Me.CustomData);

            Ini.Add(KEY_TimTag, "TIM");
            Ini.Add(KEY_CargoSwitcherTag, "timcs");
            Ini.Add(KEY_ShowDebug, false);

            ConfigApplied.TimTag = Ini.Get(KEY_TimTag).ToString();

            Debug = (Ini.Get(KEY_ShowDebug).ToBoolean())
                ? Echo
                : (text) => { };
            ConfigStorage.Debug = Debug;
            ConfigApplied.Debug = Debug;

            SaveConfig();
        }

        void SaveConfig() {
            var text = Ini.ToString();
            _configHashCode = text.GetHashCode();
            Me.CustomData = text;
        }
    }
}
