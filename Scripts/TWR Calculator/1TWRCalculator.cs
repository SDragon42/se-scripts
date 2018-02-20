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
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {
        const string KeyRCName = "Ship Ctrl Name";
        const string KeyDisplayName = "Display Name";
        const string KeyMass2Ignore = "Ignore Mass";

        readonly CustomDataConfig _config;
        readonly List<IMyThrust> _thrusters = new List<IMyThrust>();
        readonly List<Direction> _calcDirections = new List<Direction>();
        readonly Direction[] _allDirections = new Direction[]
        {
            Direction.Forward,
            Direction.Backward,
            Direction.Left,
            Direction.Right,
            Direction.Up,
            Direction.Down
        };

        IMyTextPanel _twrDisplay;
        IMyShipController _sc;
        BlocksByOrientation _orientation = new BlocksByOrientation();
        int _configHashCode = 0;

        public Program() {
            _config = new CustomDataConfig();

            LoadConfig();
        }

        void LoadConfig() {
            if (_configHashCode == Me.CustomData.GetHashCode())
                return;

            _config.AddKey(KeyRCName,
                description: "Name of the remote control block.",
                defaultValue: "");
            _config.AddKey(KeyDisplayName,
                description: "Name of the display to show results on.",
                defaultValue: "");
            _config.AddKey(KeyMass2Ignore,
                description: "The amount of mass to ignore from TWR calculations.",
                defaultValue: "0");

            _config.ReadFromCustomData(Me, true);
            _config.SaveToCustomData(Me);
            _configHashCode = Me.CustomData.GetHashCode();
        }

        public void Main(string argument) {
            LoadConfig();

            _sc = GridTerminalSystem.GetBlockWithName(_config.GetValue(KeyRCName)) as IMyShipController;
            if (_sc == null) {
                Echo($"config '{KeyRCName}' with name '{_config.GetValue(KeyRCName)}' was not found.");
                return;
            }

            _orientation.Init(_sc);

            _calcDirections.Clear();
            if (argument.Length > 0)
                _calcDirections.Add(DirectionHelper.GetDirectionFromString(argument));
            else
                _calcDirections.AddArray(_allDirections);

            var mass2Ignore = _config.GetValue(KeyMass2Ignore).ToInt();
            var totalMass = _sc.CalculateShipMass().TotalMass - mass2Ignore;
            var resultText = BuildText(totalMass);

            // Display results
            Echo(resultText);
            _twrDisplay = GridTerminalSystem.GetBlockWithName(_config.GetValue(KeyDisplayName)) as IMyTextPanel;
            _twrDisplay?.WritePublicText(resultText);
        }

        string BuildText(float totalMass) {
            var sb = new StringBuilder();
            sb.AppendLine($"Mass: {totalMass:N0} kg");
            sb.AppendLine();
            foreach (var dir in _calcDirections) {
                var info = CalcTwrInDirection(totalMass, dir);
                sb.AppendLine($"Accel Dir: {info.Thrust_Direction}");
                sb.AppendLine($"MAX Thrust: {info.Thrust / 1000.0:N0} kN");
                sb.AppendLine($"T/W R: {info.TWR:N2}");
                sb.AppendLine($"# Thrusters: {_thrusters.Count:N0}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        TwrInfo CalcTwrInDirection(float totalMass, Direction direction) {
            var massNewtons = ConvertMass2Newtons(totalMass);

            Func<IMyTerminalBlock, bool> isDirection;
            switch (direction) {
                case Direction.Forward: isDirection = _orientation.IsBackward; break;
                case Direction.Backward: isDirection = _orientation.IsForward; break;
                case Direction.Left: isDirection = _orientation.IsRight; break;
                case Direction.Right: isDirection = _orientation.IsLeft; break;
                case Direction.Up: isDirection = _orientation.IsDown; break;
                case Direction.Down: isDirection = _orientation.IsUp; break;
                default: isDirection = (b) => false; break;
            }
            GridTerminalSystem.GetBlocksOfType(_thrusters, b => IsOnThisGrid(b) && isDirection(b) && b.IsWorking);

            return new TwrInfo(_thrusters, direction, totalMass);
        }

        static double ConvertMass2Newtons(float mass_kg) { return (mass_kg / 0.101971621); }

        bool IsOnThisGrid(IMyTerminalBlock b) { return Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }

    }
}
