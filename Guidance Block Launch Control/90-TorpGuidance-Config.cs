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
            public string referenceTag { get; private set; } = "Cockpit";
            public string torpedoPrimaryTag  { get; private set; } = "Torpedo Payload Guidance";
            public string torpedoBeaconTag  { get; private set; } = "Torpedo Payload Beacon";
            public string torpedoPowerCellTag { get; private set; } = "Torp Power Cell";
            public TorpedoSelectionMode selectionMode { get; private set; } = TorpedoSelectionMode.Random;

            const string SEC_TorpedoGuidanceTags = "Torpedo Guidance Tags";
            const string SEC_TorpedoLaunch = "Torpedo Launch";

            public void Load() {
                if (!LoadIni()) return;

                referenceTag = _ini.Add(SEC_TorpedoGuidanceTags, "Reference Block Tag", referenceTag).ToString().ToLower();
                torpedoPrimaryTag = _ini.Add(SEC_TorpedoGuidanceTags, "Guidance Tag", torpedoPrimaryTag).ToString().ToLower();
                torpedoBeaconTag = _ini.Add(SEC_TorpedoGuidanceTags, "Beacon Tag", torpedoBeaconTag).ToString().ToLower();
                torpedoPowerCellTag = _ini.Add(SEC_TorpedoGuidanceTags, "Battery Tag", torpedoPowerCellTag).ToString().ToLower();

                var mode = _ini.Add(SEC_TorpedoLaunch, "Launch Mode", (int)selectionMode, "Modes: 0 = Random, 1 = Closest, 2 = Furthest").ToInt32();
                if (Enum.IsDefined(typeof(TorpedoSelectionMode), mode))
                    selectionMode = (TorpedoSelectionMode)mode;

                Save();
            }
        }

    }
}
