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

            //TwrHelper.Debug = Echo;
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

                _resultsBuilder.AppendLine("    Effective / Maximum");
                var effectiveTwr = TwrHelper.CalculateEffectiveTWR(_sc, _thrusters, _cfg.InventoryMultiplier, twr: 1f);
                //TwrHelper.CalculateEffectiveTWR(_sc, _thrusters, _cfg.InventoryMultiplier, twr: 1.5f);
                //TwrHelper.CalculateEffectiveTWR(_sc, _thrusters, _cfg.InventoryMultiplier, twr: 1.59f);
                //TwrHelper.CalculateEffectiveTWR(_sc, _thrusters, _cfg.InventoryMultiplier, twr: 1.592f);
                //TwrHelper.CalculateEffectiveTWR(_sc, _thrusters, _cfg.InventoryMultiplier, twr: 1.5925f);
                var maxTwr = TwrHelper.CalculateMaxTWR(_sc, _thrusters, _cfg.InventoryMultiplier, twr: 2f);
                _resultsBuilder.AppendLine($"T:  {effectiveTwr.Thrust / 1000.0,7:N0} kN / {maxTwr.Thrust / 1000.0:N0} kN");
                _resultsBuilder.AppendLine($"TWR: {effectiveTwr.TWR,8:N2} / {maxTwr.TWR:N2}");
                _resultsBuilder.AppendLine($"Cargo: {effectiveTwr.CargoMass,8:N2} kg / {maxTwr.CargoMass:N2} kg");
                //_resultsBuilder.AppendLine($"C: {effectiveTwr.CargoMass:N2} kg");
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
