using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {

        class Config : ConfigBase {

            // public string TimTag { get; set; } = "TIM";
            public string CargoSwitcherTag { get; set; } = "timcs";
            // public bool ShowDebug { get; set; } = false;

            const string SECTION_CONFIG = "TIM Cargo Switcher";

            public void Load(TimBlockName configApplied) {
                if (!LoadIni()) return;

                // TimTag = _ini.Add(SECTION_CONFIG, "TIM Tag", TimTag).ToString();
                configApplied.TimTag = _ini.Get(SECTION_CONFIG, "TIM Tag").ToString();
                CargoSwitcherTag = _ini.Add(SECTION_CONFIG, "Cargo Switcher Tag", CargoSwitcherTag).ToString();
                // ShowDebug = _ini.Add(SECTION_CONFIG, "Show Debug Info", ShowDebug).ToBoolean();

                Save();
            }
        }

    }
}
