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
    partial class Program {
        bool Connector_Disconnect(IMyShipConnector connector) {
            if (!Collect.IsConnectorConnected(connector))
                return false;
            connector.Disconnect();
            return true;
        }
        bool Connector_Connect(IMyShipConnector connector) {
            if (Collect.IsConnectorConnected(connector))
                return false;
            if (!Collect.IsConnectorConnectable(connector))
                return true;
            connector.Connect();
            return true;
        }

        bool Rotor_RotateTo(IMyMotorStator rotor, float velocity, double limit) {
            var currAngle = Math.Round(rotor.Angle, 3);
            var diff = currAngle - limit;
            if (diff == 0.0) return false;
            velocity = velocity * (currAngle > limit ? -1 : 1);
            rotor.TargetVelocityRPM = velocity;

            return true;
        }

        bool Piston_MoveTo(IMyPistonBase piston, float velocity, double limit) {
            var currPos = Math.Round(piston.CurrentPosition, 3);
            limit = Math.Round(limit, 3);
            var diff = currPos - limit;
            diff = Math.Round(diff, 3);
            //Debug.AppendLine($"Piston - c{currPos}  l{limit}  d{diff}");
            if (diff == 0.0) return false;
            piston.Velocity = velocity;
            return true;
        }
    }
}
