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
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {

        // Modules
        readonly BlocksByOrientation BlockOrientation = new BlocksByOrientation();


        // Block Lists
        readonly List<IMyThrust> LiftThrusters = new List<IMyThrust>();
        IMyShipController _sc;


        public Program() {
        }

        public void Save() {
        }


        // Vars
        float MinimumTWR = 0;
        int InventoryMultiplier = 0;

        public void Main(string argument, UpdateType updateSource) {
            //LiftCapacity.Debug = Echo;
            LoadConfig();
            LoadBlocks();
            if (_sc == null) return;
            if (InventoryMultiplier <= 0) {
                Echo("ERROR: Inventory Multiplier is not set!");
                return;
            }
            var x = LiftCapacity.GetMaxMass(_sc, LiftThrusters, MinimumTWR, InventoryMultiplier);
            Echo($"Max Mass: {x:N2} kg");

            var x2 = LiftCapacity.GetMaxMass(_sc, LiftThrusters, 1.0, InventoryMultiplier);
            Echo($"Max Mass: {x2:N2} kg");
        }


        // Configuration
        readonly MyIniKey KEY_MinimumTWR = new MyIniKey("Section", "Minimum TWR");
        readonly MyIniKey KEY_WorldInvMulti = new MyIniKey("Section", "Inventory Multiplier");
        readonly MyIniKey KEY_Tag = new MyIniKey("Section", "Block Tag");
        int _lastConfigHash = 0;
        void LoadConfig() {
            var configHash = Me.CustomData.GetHashCode();
            if (configHash == _lastConfigHash) return;

            var ini = new MyIni();
            ini.TryParse(Me.CustomData);

            ini.Add(KEY_MinimumTWR, 1.5, comment: "The minimum TWR to use for calc maximum cargo capacity.");
            ini.Add(KEY_WorldInvMulti, 0, comment: "The World setting for Inventory Multiplier");

            MinimumTWR = ini.Get(KEY_MinimumTWR).ToSingle();
            InventoryMultiplier = ini.Get(KEY_WorldInvMulti).ToInt32();

            if (InventoryMultiplier <= 0) {
                var b = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCargoContainer>(Collect.IsCargoContainer);
                if (b != null) {
                    InventoryMultiplier = CargoHelper.GetInventoryMultiplier(b);
                    ini.Set(KEY_WorldInvMulti, InventoryMultiplier);
                }
            }

            Me.CustomData = ini.ToString();
            _lastConfigHash = Me.CustomData.GetHashCode();
        }

        void LoadBlocks() {
            _sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                //b => IsOnThisGrid(b) && Collect.IsTagged(b, ""),
                b => IsOnThisGrid(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl && ((IMyRemoteControl)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl);

            LiftThrusters.Clear();

            if (_sc == null) {
                Echo($"No Cockpit or RemoteControl found.");
                return;
            }

            Echo($"SC: {_sc.CustomName}");

            BlockOrientation.Init(_sc);
            GridTerminalSystem.GetBlocksOfType(LiftThrusters, BlockOrientation.IsDown);
            Echo($"# Thrusters: {LiftThrusters.Count}");
        }
    }
}
