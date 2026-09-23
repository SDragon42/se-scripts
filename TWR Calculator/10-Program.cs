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

        readonly BlocksByOrientation _orientation = new BlocksByOrientation();
        readonly Config _cfg;

        readonly List<IMyThrust> _thrusters = new List<IMyThrust>();
        readonly List<Base6Directions.Direction> _calcDirections = new List<Base6Directions.Direction>();
        IMyShipController _sc;

        readonly StringBuilder _resultsBuilder = new StringBuilder();


        public Program() {
            _cfg = new Config(Me, GridTerminalSystem);
            _cfg.Load(true);
        }

        public void Main(string argument, UpdateType updateSource) {
            _resultsBuilder.Clear();
            _cfg.Load();

            try {
                _sc = GetShipController();
                if (_sc == null) {
                    _resultsBuilder.AppendLine("No ship controller found.");
                    return;
                }
                Echo($"Using: {_sc.CustomName}");

                _orientation.Init(_sc);

                _calcDirections.Clear();
                if (argument.Length > 0)
                    _calcDirections.Add(DirectionHelper.GetDirectionFromString(argument));
                else
                    _calcDirections.AddArray(Base6Directions.EnumDirections);

                BuildText();
            } finally {
                // Display results
                var resultText = _resultsBuilder.ToString();
                Echo(resultText);
                var display = (Me as IMyTextSurfaceProvider)?.GetSurface(0);
                if (display != null) {
                    display.ContentType = ContentType.TEXT_AND_IMAGE;
                    display.WriteText(resultText, append: false);
                }
                var twrDisplay = GridTerminalSystem.GetBlockWithName(_cfg.DisplayName) as IMyTextPanel;
                if (twrDisplay != null) {
                    twrDisplay.ContentType = ContentType.TEXT_AND_IMAGE;
                    twrDisplay.WriteText(resultText);
                }
            }

        }

        IMyShipController GetShipController() {
            var sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => IsOnThisGrid(b) && b is IMyRemoteControl && b.CustomName == _cfg.ShipControllerName,
                b => IsOnThisGrid(b) && b is IMyCockpit && b.CustomName == _cfg.ShipControllerName
                );
            if (sc != null) return sc;
            sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => IsOnThisGrid(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl && ((IMyRemoteControl)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl,
                b => IsOnThisGrid(b) && b is IMyCockpit
                );
            return sc;
        }

        void BuildText() {
            var totalMass = _sc.CalculateShipMass().PhysicalMass - _cfg.MassToIgnore;

            _resultsBuilder.AppendLine($"Mass: {totalMass:N0} kg");
            _resultsBuilder.AppendLine();

            foreach (var direction in _calcDirections) {
                LoadThrustersInDirection(direction);
                _resultsBuilder.AppendLine($"{_thrusters.Count:N0} {direction} Thrusters");

                _resultsBuilder.AppendLine("    Current / Maximum");

                var currentThrust = _thrusters.Sum(t => t.MaxEffectiveThrust);
                var maxThrust = _thrusters.Sum(t => t.MaxThrust);
                _resultsBuilder.AppendLine($"T:  {TextHelper.FormatForce(currentThrust)} / {TextHelper.FormatForce(maxThrust)}");

                var currentTwr = ThrusterHelper.CalculateEffectiveTWR(_sc, _thrusters);
                var maxTwr = ThrusterHelper.CalculateMaxTWR(_sc, _thrusters);
                _resultsBuilder.AppendLine($"TWR: {currentTwr,8:N2} / {maxTwr:N2}");

                var currentLiftCargo = ThrusterHelper.CalculateEffectiveLiftableCargoMass(_sc, _thrusters, _cfg.InventoryMultiplier, minimumTwr: _cfg.MinimumTWR);
                var maxLiftCargo = ThrusterHelper.CalculateMaxLiftableCargoMass(_sc, _thrusters, _cfg.InventoryMultiplier, minimumTwr: _cfg.MinimumTWR);
                _resultsBuilder.AppendLine($"Effective Cargo: {TextHelper.FormatMass(currentLiftCargo)}");
                _resultsBuilder.AppendLine($"Maximum Cargo: {TextHelper.FormatMass(maxLiftCargo)}");

                _resultsBuilder.AppendLine();
            }
        }

        void LoadThrustersInDirection(Base6Directions.Direction direction) {
            Func<IMyTerminalBlock, bool> IsInDirection;
            switch (direction) {
                case Base6Directions.Direction.Forward: IsInDirection = _orientation.IsBackward; break;
                case Base6Directions.Direction.Backward: IsInDirection = _orientation.IsForward; break;
                case Base6Directions.Direction.Left: IsInDirection = _orientation.IsRight; break;
                case Base6Directions.Direction.Right: IsInDirection = _orientation.IsLeft; break;
                case Base6Directions.Direction.Up: IsInDirection = _orientation.IsDown; break;
                case Base6Directions.Direction.Down: IsInDirection = _orientation.IsUp; break;
                default: IsInDirection = (b) => false; break;
            }
            GridTerminalSystem.GetBlocksOfType(_thrusters, b => IsOnThisGrid(b) && IsInDirection(b) && b.IsWorking);
        }

    }
}
