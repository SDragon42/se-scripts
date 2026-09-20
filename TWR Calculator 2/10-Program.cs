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

                var EffectiveTwr = ThrusterHelper.CalculateEffectiveTWR(_sc, _liftThrusters);

                var minimumTwr = 1.2f;
                var cargoMass = ThrusterHelper.CalculateEffectiveLiftableCargoMass(_sc, _liftThrusters, _cfg.InventoryMultiplier, minimumTwr: minimumTwr);

                _resultsBuilder.AppendLine($"TWR: {EffectiveTwr}");
                _resultsBuilder.AppendLine("");
                _resultsBuilder.AppendLine($"At Min. TWR: {minimumTwr}");
                _resultsBuilder.AppendLine($"Cargo: " + FormatMass(cargoMass));
                _resultsBuilder.AppendLine("");
            } finally {
                var resultString = _resultsBuilder.ToString();
                Echo(resultString.Replace("[", "(").Replace("]", ")"));
                var display = (Me as IMyTextSurfaceProvider)?.GetSurface(0);
                if (display != null) {
                    display.ContentType = ContentType.TEXT_AND_IMAGE;
                    display.WriteText(resultString, append: false);
                }
            }
        }

        private string FormatMass(float mass) {
            if (mass < 1000f)
                return $"{mass:N2} kg";
            else if (mass < 1000000f)
                return $"{mass / 1000f:N2} t";
            else
                return $"{mass / 1000000f:N2} kt";
        }

        string FormatForce(float force) {
            if (force < 1000f)
                return $"{force:N2} N";
            else if (force < 1000000f)
                return $"{force / 1000f:N2} kN";
            else
                return $"{force / 1000000f:N2} MN";
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

            _blockOrientation.Init(_sc);

            _liftThrusters.Clear();
            if (!string.IsNullOrWhiteSpace(_cfg.ThrusterGroupName))
                GridTerminalSystem.GetBlockGroupWithName(_cfg.ThrusterGroupName)?.GetBlocksOfType(_liftThrusters, IsLiftThruster);
            if (_liftThrusters.Count == 0)
                GridTerminalSystem.GetBlocksOfType(_liftThrusters, IsLiftThruster);
        }

        bool IsLiftThruster(IMyTerminalBlock b) => IsOnThisGrid(b) && _blockOrientation.IsDown(b) && b.IsWorking;
    }
}
