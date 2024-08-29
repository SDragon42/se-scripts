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

        // Modules
        readonly BlocksByOrientation BlockOrientation = new BlocksByOrientation();

        readonly MyIni Ini = new MyIni();


        // Block Lists
        readonly List<IMyThrust> LiftThrusters = new List<IMyThrust>();
        IMyShipController _sc;

        Action<string> Debug = (text) => { };


        public Program() {
            //Debug = Echo;
            //LiftCapacity.Debug = Echo;
            LoadConfig(true);
        }

        public void Save() {
        }


        // Vars
        float MinimumTWR = 0;
        int InventoryMultiplier = 0;
        string GroupName = string.Empty;

        public void Main(string argument, UpdateType updateSource) {
            LoadConfig();
            LoadBlocks();
            if (_sc == null) return;
            if (InventoryMultiplier <= 0) {
                Echo("ERROR: Inventory Multiplier is not set!");
                return;
            }
            var x = ThrusterHelper.GetMaxMass(_sc, LiftThrusters, MinimumTWR, InventoryMultiplier);
            Echo($"At TWR {MinimumTWR:N1}");
            Echo($"Max Mass: {x:N2} kg");

            Echo("");
            Echo($"At TWR {1.0:N1}");
            var x2 = ThrusterHelper.GetMaxMass(_sc, LiftThrusters, 1.0, InventoryMultiplier);
            Echo($"Max Mass: {x2:N2} kg");
        }


        // Configuration
        readonly MyIniKey KEY_MinimumTWR = new MyIniKey("Section", "Minimum TWR");
        readonly MyIniKey KEY_WorldInvMulti = new MyIniKey("Section", "Inventory Multiplier");
        readonly MyIniKey KEY_ThrusterGroup = new MyIniKey("Section", "Thruster Group");

        int _lastConfigHash = 0;
        void LoadConfig(bool force = false) {
            var configHash = Me.CustomData.GetHashCode();
            if (configHash == _lastConfigHash && !force) return;

            Ini.TryParse(Me.CustomData);

            Ini.Add(KEY_MinimumTWR, 1.5, comment: "The minimum TWR to use for calc maximum cargo capacity.");
            Ini.Add(KEY_WorldInvMulti, 0, comment: "The World setting for Inventory Multiplier");
            Ini.Add(KEY_ThrusterGroup, "", comment: "The Group name to use for thrusters");

            MinimumTWR = Ini.Get(KEY_MinimumTWR).ToSingle();
            InventoryMultiplier = Ini.Get(KEY_WorldInvMulti).ToInt32();
            GroupName = Ini.Get(KEY_ThrusterGroup).ToString();

            if (InventoryMultiplier <= 0) {
                var b = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCargoContainer>(Collect.IsCargoContainer);
                if (b != null) {
                    InventoryMultiplier = CargoHelper.GetInventoryMultiplier(b);
                    Ini.Set(KEY_WorldInvMulti, InventoryMultiplier);
                }
            }

            Me.CustomData = Ini.ToString();
            _lastConfigHash = Me.CustomData.GetHashCode();
        }

        void LoadBlocks() {
            _sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => IsOnThisGrid(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl && ((IMyRemoteControl)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl);
            if (_sc == null) {
                Echo($"No Cockpit or RemoteControl found.");
                return;
            }
            Echo($"SC: {_sc.CustomName}");

            BlockOrientation.Init(_sc);

            Func<IMyTerminalBlock, bool> collect;
            LiftThrusters.Clear();
            if (!string.IsNullOrWhiteSpace(GroupName)) {
                var bg = GridTerminalSystem.GetBlockGroupWithName(GroupName);
                bg?.GetBlocksOfType(LiftThrusters, BlockOrientation.IsDown);
            }
            if (LiftThrusters.Count == 0)
                GridTerminalSystem.GetBlocksOfType(LiftThrusters, BlockOrientation.IsDown);
            Echo($"# Thrusters: {LiftThrusters.Count}");
        }
    }
}
