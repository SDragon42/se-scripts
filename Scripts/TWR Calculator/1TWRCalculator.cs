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

        readonly BlocksByOrientation _orientation = new BlocksByOrientation();
        readonly ConfigCustom _config = new ConfigCustom();
        readonly List<IMyThrust> _thrusters = new List<IMyThrust>();
        readonly List<Direction> _calcDirections = new List<Direction>();

        int _configHashCode = 0;

        public Program() {
            LoadConfig(true);
        }

        void LoadConfig(bool force = false) {
            if (_configHashCode == Me.CustomData.GetHashCode() && !force)
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

            _config.Load(Me, true);
            _config.Save(Me);
            _configHashCode = Me.CustomData.GetHashCode();
        }

        public void Main(string argument) {
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
                _calcDirections.AddArray(new Direction[] { Direction.Forward, Direction.Backward, Direction.Left, Direction.Right, Direction.Up, Direction.Down });

            var mass2Ignore = _config.GetValue(KeyMass2Ignore).ToInt();
            var totalMass = sc.CalculateShipMass().PhysicalMass - mass2Ignore;
            var resultText = BuildText(totalMass);

            // Display results
            Echo(resultText);
            var twrDisplay = GridTerminalSystem.GetBlockWithName(_config.GetValue(KeyDisplayName)) as IMyTextPanel;
            if (twrDisplay != null) {
                twrDisplay.ShowPublicTextOnScreen();
                twrDisplay.WritePublicText(resultText);
            }
        }

        IMyShipController GetShipController() {
            var name = _config.GetValue(KeyRCName);
            var sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => b is IMyRemoteControl && b.CustomName == name,
                b => b is IMyCockpit && b.CustomName == name
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

        TwrInfo CalcTwrInDirection(float totalMass, Direction direction) {
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

        bool IsOnThisGrid(IMyTerminalBlock b) => Me.CubeGrid.EntityId == b.CubeGrid.EntityId;

    }
}
