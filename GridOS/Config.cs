﻿// <mdk sortorder="100" />
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

        public class Config {
            readonly MyIni ini = new MyIni();
            int hash = 0;

            const string SEC_adc = "Auto Door Closer";
            public bool ADCEnabled { get; private set; } = true;
            public string ADCExclusionTag { get; private set; } = "[exclude]";

            const string SEC_Airlock = "Airlocks";
            public string AirlockTag { get; private set; } = "[airlock]";

            public void Load(IMyTerminalBlock b, Program thiso) {
                if (hash == b.CustomData.GetHashCode()) return;

                ini.Clear();
                ini.TryParse(b.CustomData);

                thiso.blockReload_Time = ini.Add("Grid OS", "Block Reload Delay", thiso.blockReload_Time).ToDouble();

                ADCEnabled = ini.Add(SEC_adc, "Enabled", ADCEnabled).ToBoolean();
                thiso.autoDoorCloser.CloseDelay = ini.Add(SEC_adc, "Delay", thiso.autoDoorCloser.CloseDelay).ToDouble();
                ADCExclusionTag = ini.Add(SEC_adc, "Exclude Tag", ADCExclusionTag).ToString();

                AirlockTag = ini.Add(SEC_Airlock, "Airlock Tag", AirlockTag).ToString();

                SaveConfig(b);
            }

            //public void Save(IMyTerminalBlock b) {
            //    ini.Set(SEC_AutoDoorCloser, KEY_Enabled, AutoDoorCloserEnabled);

            //    SaveConfig(b);
            //}

            void SaveConfig(IMyTerminalBlock b) {
                b.CustomData = ini.ToString();
                hash = b.CustomData.GetHashCode();
            }
        }
    }
}
