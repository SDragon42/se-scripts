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

        class Config {
            public float MinimumTWR { get; private set; } = 1.2f;
            public int InventoryMultiplier { get; private set; } = 1;
            public string ShipControllerName { get; private set; } = string.Empty;
            public string DisplayName { get; private set; } = string.Empty;
            public int MassToIgnore { get; private set; } = 0;

            // Configuration
            readonly MyIniKey KEY_MinimumTWR = new MyIniKey("TWR", "Minimum TWR");
            readonly MyIniKey KEY_WorldInvMulti = new MyIniKey("TWR", "Inventory Multiplier");
            readonly MyIniKey KEY_ShipCtrlName = new MyIniKey("TWR", "Ship Ctrl Name");
            readonly MyIniKey KEY_DisplayName = new MyIniKey("TWR", "Display Name");
            readonly MyIniKey KEY_IgnoreMass = new MyIniKey("TWR", "Ignore Mass");

            int _lastConfigHash = 0;
            readonly MyIni _ini = new MyIni();

            readonly IMyProgrammableBlock Me;
            readonly IMyGridTerminalSystem GridTerminalSystem;
            public Config(IMyProgrammableBlock Me, IMyGridTerminalSystem GridTerminalSystem) {
                this.Me = Me;
                this.GridTerminalSystem = GridTerminalSystem;
            }

            public void Load(bool force = false) {
                var configHash = Me.CustomData.GetHashCode();
                if (configHash == _lastConfigHash && !force) return;

                _ini.TryParse(Me.CustomData);

                MinimumTWR = _ini.Add(KEY_MinimumTWR, MinimumTWR, "The minimum TWR to use for calc maximum cargo capacity").ToSingle();
                InventoryMultiplier = _ini.Add(KEY_WorldInvMulti, InventoryMultiplier, "The World setting for Inventory Multiplier").ToInt32();
                ShipControllerName = _ini.Add(KEY_ShipCtrlName, ShipControllerName, "Name of the remote control block").ToString();
                DisplayName = _ini.Add(KEY_DisplayName, DisplayName, "Name of the display to show results on").ToString();
                MassToIgnore = _ini.Add(KEY_IgnoreMass, MassToIgnore, "The amount of mass to ignore from TWR calculations").ToInt32();

                if (InventoryMultiplier <= 0) {
                    var b = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCargoContainer>(Collect.IsCargoContainer);
                    InventoryMultiplier = b != null
                        ? CargoHelper.GetInventoryMultiplier(b)
                        : 1;
                    _ini.Set(KEY_WorldInvMulti, InventoryMultiplier);
                }

                Me.CustomData = _ini.ToString();
                _lastConfigHash = Me.CustomData.GetHashCode();
            }
        }

    }
}
