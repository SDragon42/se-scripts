// <mdk sortorder="100" />
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
        #region mdk preserve
        public class Config {
            readonly MyIni ini = new MyIni();
            int hash = 0;

            const string SEC_adc = "Auto Door Closer";
            public bool ADCEnabled { get; private set; } = true;
            public string ADCExclusionTag { get; private set; } = "[exclude]";

            public void Load(IMyTerminalBlock b, Program thiso) {
                if (hash == b.CustomData.GetHashCode()) return;

                ini.Clear();
                ini.TryParse(b.CustomData);

                thiso.blockReload_Time = ini.Add("Grid OS", "Block Reload Delay", thiso.blockReload_Time).ToDouble();

                ADCEnabled = ini.Add(SEC_adc, "Enabled", ADCEnabled).ToBoolean();
                thiso.autoDoorCloser.CloseDelay = ini.Add(SEC_adc, "Delay", thiso.autoDoorCloser.CloseDelay).ToDouble();
                ADCExclusionTag = ini.Add(SEC_adc, "Exclude Tag", ADCExclusionTag).ToString();

                

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
        #endregion
    }
}
