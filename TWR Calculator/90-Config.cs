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
    partial class Program : MyGridProgram {

        class Config : ConfigBase {
            public float MinimumTWR { get; private set; } = 1.2f;
            public int InventoryMultiplier { get; private set; } = 0;
            public string ShipControllerName { get; private set; } = string.Empty;
            public string DisplayName { get; private set; } = string.Empty;
            public int MassToIgnore { get; private set; } = 0;

            // Configuration
            const string SECTION_TWR = "TWR";
            readonly MyIniKey KEY_WorldInvMulti = new MyIniKey(SECTION_TWR, "Inventory Multiplier");

            public void Load() {
                if (!LoadIni()) return;

                MinimumTWR = _ini.Add(SECTION_TWR, "Minimum TWR", MinimumTWR, "The minimum TWR to use for calc maximum cargo capacity").ToSingle();
                InventoryMultiplier = _ini.Add(KEY_WorldInvMulti, InventoryMultiplier, "The World setting for Inventory Multiplier").ToInt32();
                ShipControllerName = _ini.Add(SECTION_TWR, "Ship Ctrl Name", ShipControllerName, "Name of the remote control block").ToString();
                DisplayName = _ini.Add(SECTION_TWR, "Display Name", DisplayName, "Name of the display to show results on").ToString();
                MassToIgnore = _ini.Add(SECTION_TWR, "Ignore Mass", MassToIgnore, "The amount of mass to ignore from TWR calculations").ToInt32();

                if (InventoryMultiplier <= 0) {
                    var b = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCargoContainer>(IsCargoContainer);
                    if (b != null) {
                        InventoryMultiplier = CargoHelper.GetInventoryMultiplier(b);
                        _ini.Set(KEY_WorldInvMulti, InventoryMultiplier);
                    }
                }
                
                Save();
            }
        }

    }
}
