// <mdk sortorder="80" />
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
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
    public partial class Program : MyGridProgram {

        class Configuration : ConfigBase {
            public string TagName { get; private set; } = "Tunnel-Drill";
            public float ExtensionVelocity { get; private set; } = 0.08f;
            public float RetractionVelocity { get; private set; } = 0.5f;
            public float HoldInventoryPercentage { get; private set; } = 0.95f;
            public float ResumeInventoryPercentage { get; private set; } = 0.5f;

            const string SECTION_NAME = "Tunnel-Drill";

            public void Load() {
                if (!LoadIni()) return;

                _ini.AddSection(SECTION_NAME);
                _ini.SetSectionComment(SECTION_NAME, "Configuration for the tunnel drill");

                TagName = _ini.Add(SECTION_NAME, "TagName", TagName).ToString();
                ExtensionVelocity = Math.Abs(_ini.Add(SECTION_NAME, "ExtensionVelocity", ExtensionVelocity).ToSingle());
                RetractionVelocity = Math.Abs(_ini.Add(SECTION_NAME, "RetractionVelocity", RetractionVelocity).ToSingle());
                HoldInventoryPercentage = Math.Min(1f, _ini.Add(SECTION_NAME, "HoldInventoryPercentage", HoldInventoryPercentage).ToSingle());
                ResumeInventoryPercentage = Math.Max(0f, _ini.Add(SECTION_NAME, "ResumeInventoryPercentage", ResumeInventoryPercentage).ToSingle());

                // Ensure ResumeCargoPercentage is less than HoldCargoPercentage
                if (ResumeInventoryPercentage >= HoldInventoryPercentage) {
                    ResumeInventoryPercentage = Math.Max(0f, HoldInventoryPercentage - 0.2f);
                }

                Save();
            }
        }

    }
}
