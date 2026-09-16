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


        // Block Lists
        readonly List<IMyThrust> _liftThrusters = new List<IMyThrust>();
        IMyShipController _sc;


        readonly StringBuilder _resultsBuilder = new StringBuilder();
        readonly Config _cfg;


        public Program() {
            _cfg = new Config(Me, GridTerminalSystem);
            _cfg.Load(true);
        }



        public void Main(string argument, UpdateType updateSource) {
            _resultsBuilder.Clear();
            try {
                _cfg.Load();
                LoadBlocks();

                if (_sc == null) return;
                if (_cfg.InventoryMultiplier <= 0) {
                    _resultsBuilder.AppendLine("ERROR: Inventory Multiplier is not set!");
                    return;
                }

                var twrInfo = TwrHelper.CalculateCurrentTWR(_sc, _liftThrusters, _cfg.InventoryMultiplier, _cfg.MinimumTWR);
                AddTwrInfoToOutput(twrInfo);

                _resultsBuilder.AppendLine("");

                twrInfo = TwrHelper.CalculateCurrentTWR(_sc, _liftThrusters, _cfg.InventoryMultiplier, 1.0f);
                AddTwrInfoToOutput(twrInfo);

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

        private void AddTwrInfoToOutput(TwrInfo info, string label = "") {
            if (!string.IsNullOrWhiteSpace(label))
                _resultsBuilder.AppendLine(label);
            _resultsBuilder.AppendLine($"At TWR {info.TWR:N1}");
            _resultsBuilder.AppendLine($"Max Mass: {info.MaxCargoMass:N2} kg");
        }



        void LoadBlocks() {
            if (!string.IsNullOrWhiteSpace(_cfg.ShipControllerName))
                _sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                    b => IsOnThisGrid(b) && b is IMyCockpit && b.CustomName == _cfg.ShipControllerName,
                    b => IsOnThisGrid(b) && b is IMyRemoteControl && b.CustomName == _cfg.ShipControllerName);
            if (_sc == null)
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

            _liftThrusters.Clear();
            if (!string.IsNullOrWhiteSpace(_cfg.ThrusterGroupName))
                GridTerminalSystem.GetBlockGroupWithName(_cfg.ThrusterGroupName)?.GetBlocksOfType(_liftThrusters, IsLiftThruster);
            if (_liftThrusters.Count == 0)
                GridTerminalSystem.GetBlocksOfType(_liftThrusters, IsLiftThruster);
            _resultsBuilder.AppendLine($"# Thrusters: {_liftThrusters.Count}");
        }

        bool IsLiftThruster(IMyTerminalBlock b) => IsOnThisGrid(b) && _blockOrientation.IsDown(b) && b.IsWorking;
    }
}
