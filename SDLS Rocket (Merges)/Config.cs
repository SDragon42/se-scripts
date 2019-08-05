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
        class Config {
            public Config() { }

            const string SEC_RocketTags = "SDLS Rocket Tags";
            const string SEC_RocketGrid = "SDLS Rocket Grid";

            int _configHash = 0;

            public string PodTag { get; set; } = "[pod]";
            public string Stage2Tag { get; set; } = "[stage2]";
            public string Stage1Tag { get; set; } = "[stage1]";
            public string BoosterTag { get; set; } = "[booster]";


            public string GridName { get; set; } = string.Empty;
            public string GridName_Merged { get; set; } = "SDLS Rocket";
            public bool HasGridNames { get; set; }

            public void LoadConfig(IMyProgrammableBlock me) {
                if (_configHash == me.CustomData.GetHashCode()) return;

                var ini = new MyIni();
                ini.TryParse(me.CustomData);

                PodTag = ini.Add(SEC_RocketTags, "Pod", PodTag).ToString();
                Stage2Tag = ini.Add(SEC_RocketTags, "Stage2", Stage2Tag).ToString();
                Stage1Tag = ini.Add(SEC_RocketTags, "Stage1", Stage1Tag).ToString();
                BoosterTag = ini.Add(SEC_RocketTags, "Booster", BoosterTag).ToString();

                GridName = ini.Add(SEC_RocketGrid, "Grid Name", GridName).ToString();
                GridName_Merged = ini.Add(SEC_RocketGrid, "Merged Name", GridName_Merged).ToString();
                HasGridNames = (GridName.Length > 0) || (GridName_Merged.Length > 0);

                var newConfig = ini.ToString();
                var newConfigHash = newConfig.GetHashCode();
                if (newConfigHash != _configHash) {
                    me.CustomData = newConfig;
                    _configHash = newConfigHash;
                }
            }

        }
    }
}
