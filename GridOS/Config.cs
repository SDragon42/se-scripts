// <mdk sortorder="100" />
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
            public double BlockReloadTime { get; private set; } = 10;
            public bool ADCEnabled { get; private set; } = true;
            public string ADCExclusionTag { get; private set; } = "[exclude]";
            public string AirlockTag { get; private set; } = "[airlock]";

            const string SEC_GRID_OS = "Grid OS";
            const string SEC_AUTO_DOOR_CLOSER = "Auto Door Closer";
            const string SEC_AIRLOCK = "Airlocks";
            
            public void Load(AutoDoorCloser autoDoorCloser) {
                if (!LoadIni()) return;

                BlockReloadTime = _ini.Add(SEC_GRID_OS, "Block Reload Delay", BlockReloadTime).ToDouble();

                ADCEnabled = _ini.Add(SEC_AUTO_DOOR_CLOSER, "Enabled", ADCEnabled).ToBoolean();
                autoDoorCloser.CloseDelay = _ini.Add(SEC_AUTO_DOOR_CLOSER, "Delay", autoDoorCloser.CloseDelay).ToDouble();
                ADCExclusionTag = _ini.Add(SEC_AUTO_DOOR_CLOSER, "Exclude Tag", ADCExclusionTag).ToString();
                
                AirlockTag = _ini.Add(SEC_AIRLOCK, "Airlock Tag", AirlockTag).ToString();

                Save();
            }

            public override void Save() {
                _ini.Set(SEC_GRID_OS, "Block Reload Delay", BlockReloadTime);
                base.Save();
            }
        }

    }
}
