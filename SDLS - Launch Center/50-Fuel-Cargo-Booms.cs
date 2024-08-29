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
    partial class Program {

        IEnumerator<bool> ConnectBoom(string tag, float velocity, RotorLimit toLimit) {
            GridTerminalSystem.GetBlocksOfType(_blocks, b => Collect.IsTagged(b, tag));
            var rotor = _blocks.Where(b => b is IMyMotorAdvancedStator).FirstOrDefault() as IMyMotorAdvancedStator;
            var connector = _blocks.Where(Collect.IsConnector).FirstOrDefault() as IMyShipConnector;
            if (rotor == null || connector == null) yield return false;

            var limit = Math.Round(
                (toLimit == RotorLimit.Low) ? rotor.LowerLimitRad : rotor.UpperLimitRad,
                3);
            while (Connector_Disconnect(connector)) yield return true;

            while (Rotor_RotateTo(rotor, velocity, limit)) yield return true;

            while (Connector_Connect(connector)) yield return true;
        }

        IEnumerator<bool> RetractBoom(string tag, float velocity, RotorLimit toLimit) {
            GridTerminalSystem.GetBlocksOfType(_blocks, b => Collect.IsTagged(b, tag));
            var rotor = _blocks.Where(b => b is IMyMotorAdvancedStator).FirstOrDefault() as IMyMotorAdvancedStator;
            var connector = _blocks.Where(Collect.IsConnector).FirstOrDefault() as IMyShipConnector;
            if (rotor == null || connector == null) yield return false;

            var limit = Math.Round(
                (toLimit == RotorLimit.Low) ? rotor.LowerLimitRad : rotor.UpperLimitRad,
                3);
            while (Connector_Disconnect(connector)) yield return true;

            while (Rotor_RotateTo(rotor, velocity, limit)) yield return true;
        }


        void GetBoom2Blocks(string tag, out List<IMyShipConnector> connectors, out IMyPistonBase piston) {
            GridTerminalSystem.GetBlocksOfType(_blocks, b => Collect.IsTagged(b, tag));
            connectors = _blocks.Where(Collect.IsConnector).Cast<IMyShipConnector>().ToList();
            piston = _blocks.Where(b => b is IMyPistonBase).FirstOrDefault() as IMyPistonBase;
        }
        IEnumerator<bool> ConnectBoom2(string tag, float velocity) {
            List<IMyShipConnector> connectors;
            IMyPistonBase piston;
            GetBoom2Blocks(tag, out connectors, out piston);
            if (piston == null || connectors == null) yield return false;

            while (connectors.Any(Connector_Disconnect)) yield return true;

            piston.Velocity = velocity;
            while (true) {
                if (connectors.Any(Collect.IsConnectorConnectable)) break;
                var pistonExtensionDiff = Math.Round(piston.MaxLimit, 3) - Math.Round(piston.CurrentPosition, 3);
                if (Math.Round(pistonExtensionDiff, 0) == 0) break;
                yield return true;
            }
            piston.Velocity = 0F;

            var waitTime = 0.0;
            while (connectors.Any(Connector_Connect) && waitTime < 3.0) {
                waitTime += Runtime.TimeSinceLastRun.TotalSeconds;
                yield return true;
            }
        }

        IEnumerator<bool> RetractBoom2(string tag, float velocity) {
            List<IMyShipConnector> connectors;
            IMyPistonBase piston;
            GetBoom2Blocks(tag, out connectors, out piston);
            if (piston == null || connectors == null) yield return false;

            //while (connectors.Any(Connector_Disconnect)) yield return true;
            connectors.ForEach(c => c.Disconnect());

            piston.Velocity = velocity;

            while (Piston_MoveTo(piston, velocity, piston.MinLimit)) yield return true;
        }

    }
}
