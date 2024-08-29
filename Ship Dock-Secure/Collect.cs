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
        class Collect {
            public static bool IsTagged(IMyTerminalBlock b, string tag) => b.CustomName.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0;

            public static bool IsConnectorConnected(IMyTerminalBlock b) => IsConnectorConnected(b as IMyShipConnector);
            public static bool IsConnectorConnected(IMyShipConnector b) => b?.Status == MyShipConnectorStatus.Connected;

            public static bool IsLandingGearLocked(IMyTerminalBlock b) => IsLandingGearLocked(b as IMyLandingGear);
            public static bool IsLandingGearLocked(IMyLandingGear b) => b.LockMode == LandingGearMode.Locked;
        }
    }
}
