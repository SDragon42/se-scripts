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

        IMyTextPanel _twrDisplay;
        IMyShipController _sc;

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

            var direction = DirectionConst.GetDirectionFromString(argument);

            int mass2Ignore = _config.GetInt(KeyMass2Ignore);

            // Weight
            int totalMass = _sc.CalculateShipMass().TotalMass - mass2Ignore;

            // Trust to Weight Ratio
            var thrusters = ThrusterHelper.GetThrustersInDirection(GridTerminalSystem, _sc, direction, IsOnThisGrid);
            var maximumThrust = Common.SumPropertyFloatToDouble(thrusters, ThrusterHelper.GetMaxThrust);
            var twr = maximumThrust / ThrusterHelper.ConvertMass2Newtons(totalMass);

            // Display results
            var resultText = new StringBuilder();
            resultText.AppendFormat("Accel Dir: {0}\n", argument);
            resultText.AppendFormat("Mass: {0:N0} kg\n", totalMass);
            resultText.AppendFormat("Thrust: {0:N0} kN\n", maximumThrust / 1000.0);
            resultText.AppendFormat("T/W R: {0:N2}\n\n", twr);
            resultText.AppendFormat("# Thrusters: {0:N0}\n", thrusters.Count);
            if (_twrDisplay != null)
                _twrDisplay.WritePublicText(resultText.ToString());
            Echo(resultText.ToString());
        }

        int GetNumberOfGyroscopes()
        {
            var tmpBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyGyro>(tmpBlocks, IsOnThisGrid);
            return tmpBlocks.Count;
        }

        bool IsOnThisGrid(IMyTerminalBlock b) { return Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }

    }
}