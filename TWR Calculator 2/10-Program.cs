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
        readonly BlocksByOrientation _blockOrientation = new BlocksByOrientation();
        readonly MyIni _ini = new MyIni();


        // Block Lists
        readonly List<IMyThrust> _liftThrusters = new List<IMyThrust>();
        IMyShipController _sc;

        readonly StringBuilder _resultsBuilder = new StringBuilder();


        public Program() {
            LoadConfig(true);
        }


        // Vars
        float _minimumTWR = 0;
        int _inventoryMultiplier = 0;
        string _groupName = string.Empty;

        public void Main(string argument, UpdateType updateSource) {
            _resultsBuilder.Clear();
            try {
                LoadConfig();
                LoadBlocks();
                if (_sc == null) return;
                if (_inventoryMultiplier <= 0) {
                    _resultsBuilder.AppendLine("ERROR: Inventory Multiplier is not set!");
                    return;
                }
                var x = ThrusterHelper.GetMaxMass(_sc, _liftThrusters, _minimumTWR, _inventoryMultiplier);
                _resultsBuilder.AppendLine($"At TWR {_minimumTWR:N1}");
                _resultsBuilder.AppendLine($"Max Mass: {x:N2} kg");

                _resultsBuilder.AppendLine("");
                _resultsBuilder.AppendLine($"At TWR {1.0:N1}");
                var x2 = ThrusterHelper.GetMaxMass(_sc, _liftThrusters, 1.0, _inventoryMultiplier);
                _resultsBuilder.AppendLine($"Max Mass: {x2:N2} kg");
            } finally {
                var resultString = _resultsBuilder.ToString();
                Echo(resultString);
                var display = (Me as IMyTextSurfaceProvider)?.GetSurface(0);
                if (display != null) {
                    display.ContentType = ContentType.TEXT_AND_IMAGE;
                    display.WriteText(resultString, append: false);
                }
            }
        }


        // Configuration
        readonly MyIniKey KEY_MinimumTWR = new MyIniKey("Section", "Minimum TWR");
        readonly MyIniKey KEY_WorldInvMulti = new MyIniKey("Section", "Inventory Multiplier");
        readonly MyIniKey KEY_ThrusterGroup = new MyIniKey("Section", "Thruster Group");

        int _lastConfigHash = 0;
        void LoadConfig(bool force = false) {
            var configHash = Me.CustomData.GetHashCode();
            if (configHash == _lastConfigHash && !force) return;

            _ini.TryParse(Me.CustomData);

            _ini.Add(KEY_MinimumTWR, 1.5, comment: "The minimum TWR to use for calc maximum cargo capacity.");
            _ini.Add(KEY_WorldInvMulti, 0, comment: "The World setting for Inventory Multiplier");
            _ini.Add(KEY_ThrusterGroup, "", comment: "The Group name to use for thrusters");

            _minimumTWR = _ini.Get(KEY_MinimumTWR).ToSingle();
            _inventoryMultiplier = _ini.Get(KEY_WorldInvMulti).ToInt32();
            _groupName = _ini.Get(KEY_ThrusterGroup).ToString();

            if (_inventoryMultiplier <= 0) {
                var b = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCargoContainer>(Collect.IsCargoContainer);
                if (b != null) {
                    _inventoryMultiplier = CargoHelper.GetInventoryMultiplier(b);
                    _ini.Set(KEY_WorldInvMulti, _inventoryMultiplier);
                }
            }

            Me.CustomData = _ini.ToString();
            _lastConfigHash = Me.CustomData.GetHashCode();
        }

        void LoadBlocks() {
            _sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => IsOnThisGrid(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl && ((IMyRemoteControl)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl);
            if (_sc == null) {
                _resultsBuilder.AppendLine($"No Cockpit or RemoteControl found.");
                return;
            }
            _resultsBuilder.AppendLine($"SC: {_sc.CustomName}");

            _blockOrientation.Init(_sc);

            //Func<IMyTerminalBlock, bool> collect;
            _liftThrusters.Clear();
            if (!string.IsNullOrWhiteSpace(_groupName)) {
                var bg = GridTerminalSystem.GetBlockGroupWithName(_groupName);
                bg?.GetBlocksOfType(_liftThrusters, _blockOrientation.IsDown);
            }
            if (_liftThrusters.Count == 0)
                GridTerminalSystem.GetBlocksOfType(_liftThrusters, _blockOrientation.IsDown);
            _resultsBuilder.AppendLine($"# Thrusters: {_liftThrusters.Count}");
        }
    }
}
