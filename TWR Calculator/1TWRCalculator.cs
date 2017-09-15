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

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        const string KeyRCName = "Ship Ctrl Name";
        const string KeyDisplayName = "Display Name";
        const string KeyMass2Ignore = "Ignore Mass";

        readonly CustomDataConfigModule _config;
        readonly List<IMyThrust> _thrusters = new List<IMyThrust>();
        readonly List<int> _calcDirections = new List<int>();
        readonly int[] _allDirections = new int[]
        {
            DirectionConst.Forward,
            DirectionConst.Backward,
            DirectionConst.Left,
            DirectionConst.Right,
            DirectionConst.Up,
            DirectionConst.Down
        };

        IMyTextPanel _twrDisplay;
        IMyShipController _sc;
        BlocksByOrientation _orientation = new BlocksByOrientation();

        public Program()
        {
            _config = new CustomDataConfigModule();

            _config.AddKey(KeyRCName,
                description: "Name of the remote control block.",
                defaultValue: "");
            _config.AddKey(KeyDisplayName,
                description: "Name of the display to show results on.",
                defaultValue: "");
            _config.AddKey(KeyMass2Ignore,
                description: "The amount of mass to ignore from TWR calcuations.",
                defaultValue: "0");
        }

        public void Main(string argument)
        {
            _config.ReadFromCustomData(Me, true);
            _config.SaveToCustomData(Me);

            _sc = GridTerminalSystem.GetBlockWithName(_config.GetString(KeyRCName)) as IMyShipController;
            _twrDisplay = GridTerminalSystem.GetBlockWithName(_config.GetString(KeyDisplayName)) as IMyTextPanel;
            if (_sc == null)
            {
                Echo("config '" + KeyRCName + "' with name '" + _config.GetString(KeyRCName) + "' was not found.");
                return;
            }
            _orientation.Init(_sc);

            _calcDirections.Clear();
            if (argument.Length > 0)
                _calcDirections.Add(DirectionConst.GetDirectionFromString(argument));
            else
                _calcDirections.AddArray(_allDirections);

            int mass2Ignore = _config.GetInt(KeyMass2Ignore);
            int totalMass = _sc.CalculateShipMass().TotalMass - mass2Ignore;

            var resultText = new StringBuilder();
            resultText.AppendLine($"Mass: {totalMass - mass2Ignore:N0} kg");
            resultText.AppendLine();

            foreach (var dir in _calcDirections)
            {
                var info = CalcTwrInDirection(totalMass, mass2Ignore, dir);

                // Display results
                resultText.AppendLine($"Accel Dir: {info.Thrust_Direction}");
                resultText.AppendLine($"MAX Thrust: {info.Thrust / 1000.0:N0} kN");
                resultText.AppendLine($"T/W R: {info.TWR:N2}");
                resultText.AppendLine($"# Thrusters: {_thrusters.Count:N0}");
                resultText.AppendLine();
            }
            
            if (_twrDisplay != null)
                _twrDisplay.WritePublicText(resultText.ToString());
            Echo(resultText.ToString());
        }

        TwrInfo CalcTwrInDirection(int totalMass, int mass2Ignore, int direction)
        {
            var info = new TwrInfo();
            Func<IMyTerminalBlock, bool> isDirection;
            switch (direction)
            {
                case DirectionConst.Forward:
                    info.Thrust_Direction = "Forward";
                    isDirection = _orientation.IsBackward;
                    break;
                case DirectionConst.Backward:
                    info.Thrust_Direction = "Backward";
                    isDirection = _orientation.IsForward;
                    break;
                case DirectionConst.Left:
                    info.Thrust_Direction = "Left";
                    isDirection = _orientation.IsRight;
                    break;
                case DirectionConst.Right:
                    info.Thrust_Direction = "Right";
                    isDirection = _orientation.IsLeft;
                    break;
                case DirectionConst.Up:
                    info.Thrust_Direction = "Up";
                    isDirection = _orientation.IsDown;
                    break;
                case DirectionConst.Down:
                    info.Thrust_Direction = "Down";
                    isDirection = _orientation.IsUp;
                    break;
                default:
                    info.Thrust_Direction = "Invalid";
                    isDirection = (b) => false;
                    break;
            }
            GridTerminalSystem.GetBlocksOfType(_thrusters, b => IsOnThisGrid(b) && isDirection(b));

            info.NumThrusters = _thrusters.Count;
            info.Thrust = Common.SumPropertyFloatToDouble(_thrusters, ThrusterHelper.GetMaxThrust);
            info.TWR = info.Thrust / ThrusterHelper.ConvertMass2Newtons(totalMass);

            info.EffectiveThrust = Common.SumPropertyFloatToDouble(_thrusters, ThrusterHelper.GetMaxEffectiveThrust);
            info.EffectiveTWR = info.EffectiveThrust / ThrusterHelper.ConvertMass2Newtons(totalMass);

            return info;
        }

        bool IsOnThisGrid(IMyTerminalBlock b) { return Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }

    }
}