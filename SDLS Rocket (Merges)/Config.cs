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
            const string SectionTags = "SDLS Rocket Tags";
            const string SectionGrid = "SDLS Rocket Grid";

            readonly MyIni ini = new MyIni();


            int hash = 0;


            public string PodTag { get; set; } = "[pod]";
            public string Stage2Tag { get; set; } = "[stage2]";
            public string Stage1Tag { get; set; } = "[stage1]";
            public string BoosterTag { get; set; } = "[booster]";
            public string GridName { get; set; } = string.Empty;
            public string GridName_Merged { get; set; } = "SDLS Rocket";
            public float StageDryMass { get; set; } = 0F;

            public bool HasGridNames => ((GridName.Length > 0) || (GridName_Merged.Length > 0));

            

            public void Load(IMyProgrammableBlock me) {
                if (hash == me.CustomData.GetHashCode()) return;

                ini.Clear();
                ini.TryParse(me.CustomData);

                PodTag = ini.Add(SectionTags, "Pod", PodTag).ToString();
                Stage2Tag = ini.Add(SectionTags, "Stage2", Stage2Tag).ToString();
                Stage1Tag = ini.Add(SectionTags, "Stage1", Stage1Tag).ToString();
                BoosterTag = ini.Add(SectionTags, "Booster", BoosterTag).ToString();
                GridName = ini.Add(SectionGrid, "Grid Name", GridName).ToString();
                GridName_Merged = ini.Add(SectionGrid, "Merged Name", GridName_Merged).ToString();
                StageDryMass = ini.Add(SectionGrid, "Stage Dry Mass", StageDryMass).ToSingle();

                SaveConfig(me);
            }

            public void Save(IMyProgrammableBlock me) {
                ini.Set(SectionTags, "Pod", PodTag);
                ini.Set(SectionTags, "Stage2", Stage2Tag);
                ini.Set(SectionTags, "Stage1", Stage1Tag);
                ini.Set(SectionTags, "Booster", BoosterTag);
                ini.Set(SectionGrid, "Grid Name", GridName);
                ini.Set(SectionGrid, "Merged Name", GridName_Merged);
                ini.Set(SectionGrid, "Stage Dry Mass", StageDryMass);

                SaveConfig(me);
            }

            void SaveConfig(IMyProgrammableBlock me) {
                me.CustomData = ini.ToString();
                hash = me.CustomData.GetHashCode();
            }
        }
    }
}
