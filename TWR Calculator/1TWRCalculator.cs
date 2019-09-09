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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {

        readonly BlocksByOrientation _orientation = new BlocksByOrientation();
        readonly List<IMyThrust> _thrusters = new List<IMyThrust>();
        readonly List<Base6Directions.Direction> _calcDirections = new List<Base6Directions.Direction>();

        int _configHashCode = 0;
        string ShipControllerName = string.Empty;
        string DisplayName = string.Empty;
        int MassToIgnore = 0;


        public Program() {
            LoadConfig(true);
        }

        void LoadConfig(bool force = false) {
            if (_configHashCode == Me.CustomData.GetHashCode() && !force)
                return;

            MyIni ini = new MyIni();
            ini.TryParse(Me.CustomData);

            ShipControllerName = ini.Add("TWR", "Ship Ctrl Name", ShipControllerName, "Name of the remote control block.").ToString();
            DisplayName = ini.Add("TWR", "Display Name", DisplayName, "Name of the display to show results on.").ToString();
            MassToIgnore = ini.Add("TWR", "Ignore Mass", MassToIgnore, "The amount of mass to ignore from TWR calculations.").ToInt32();

            Me.CustomData = ini.ToString();
            _configHashCode = Me.CustomData.GetHashCode();
        }

        public void Main(string argument, UpdateType updateSource) {
            LoadConfig();

            var sc = GetShipController();
            if (sc == null) {
                Echo("No ship controller found.");
                return;
            }
            Echo($"Using: " + sc.CustomName);

            _orientation.Init(sc);
            _calcDirections.Clear();

            if (argument.Length > 0)
                _calcDirections.Add(DirectionHelper.GetDirectionFromString(argument));
            else
                _calcDirections.AddArray(Base6Directions.EnumDirections);

            var totalMass = sc.CalculateShipMass().PhysicalMass - MassToIgnore;
            var resultText = BuildText(totalMass);

            // Display results
            Echo(resultText);
            var twrDisplay = GridTerminalSystem.GetBlockWithName(DisplayName) as IMyTextPanel;
            if (twrDisplay != null) {
                twrDisplay.ContentType = ContentType.TEXT_AND_IMAGE;
                twrDisplay.WriteText(resultText);
            }
        }

        IMyShipController GetShipController() {
            var sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => b is IMyRemoteControl && b.CustomName == ShipControllerName,
                b => b is IMyCockpit && b.CustomName == ShipControllerName
                );
            if (sc != null) return sc;
            sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => b is IMyRemoteControl,
                b => b is IMyCockpit
                );
            return sc;
        }

        string BuildText(float totalMass) {
            var sb = new StringBuilder();
            sb.AppendLine($"Mass: {totalMass:N0} kg");
            sb.AppendLine();
            foreach (var dir in _calcDirections) {
                var info = CalcTwrInDirection(totalMass, dir);
                sb.AppendLine($"{_thrusters.Count:N0} {info.Thrust_Direction} Thrusters");
                sb.AppendLine("    Effective / Maximum");
                sb.AppendLine($"T: {info.Thrust.Effective / 1000.0,7:N0} kN / {info.Thrust.Maximum / 1000.0:N0} kN");
                sb.AppendLine($"TWR: {info.TWR.Effective,8:N2} / {info.TWR.Maximum:N2}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        TwrInfo CalcTwrInDirection(float totalMass, Base6Directions.Direction direction) {
            Func<IMyTerminalBlock, bool> IsDirection;
            switch (direction) {
                case Base6Directions.Direction.Forward: IsDirection = _orientation.IsBackward; break;
                case Base6Directions.Direction.Backward: IsDirection = _orientation.IsForward; break;
                case Base6Directions.Direction.Left: IsDirection = _orientation.IsRight; break;
                case Base6Directions.Direction.Right: IsDirection = _orientation.IsLeft; break;
                case Base6Directions.Direction.Up: IsDirection = _orientation.IsDown; break;
                case Base6Directions.Direction.Down: IsDirection = _orientation.IsUp; break;
                default: IsDirection = (b) => false; break;
            }
            GridTerminalSystem.GetBlocksOfType(_thrusters, b => IsOnThisGrid(b) && IsDirection(b) && b.IsWorking);

            return new TwrInfo(_thrusters, direction, totalMass);
        }

    }
}
