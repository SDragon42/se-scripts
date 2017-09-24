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
    static class ConnectorHelper
    {
        public static bool IsConnector(IMyTerminalBlock b) { return b is IMyShipConnector; }
        public static bool IsConnectorConnectable(IMyTerminalBlock b) { return IsConnectorConnectable(b as IMyShipConnector); }
        public static bool IsConnectorConnectable(IMyShipConnector b)
        {
            if (b == null) return false;
            return (b.Status == MyShipConnectorStatus.Connectable);
        }
        public static bool IsConnectorConnected(IMyTerminalBlock b) { return IsConnectorConnected(b as IMyShipConnector); }
        public static bool IsConnectorConnected(IMyShipConnector b)
        {
            if (b == null) return false;
            return (b.Status == MyShipConnectorStatus.Connected);
        }
        public static bool IsConnectorUnconnected(IMyTerminalBlock b) { return IsConnectorUnconnected(b as IMyShipConnector); }
        public static bool IsConnectorUnconnected(IMyShipConnector b)
        {
            if (b == null) return false;
            return (b.Status == MyShipConnectorStatus.Unconnected);
        }
    }
}
