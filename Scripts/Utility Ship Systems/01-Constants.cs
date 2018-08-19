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
        const string CMD_DOCK_TOGGLE = "dock-toggle";
        const string CMD_DOCK = "dock";
        const string CMD_UNDOCK = "undock";
        const string CMD_SCAN = "scan-range";
        const string CMD_TOOLS_TOGGLE = "tools-toggle";
        const string CMD_TOOLS_OFF = "tools-off";

        const double BLOCK_RELOAD_TIME = 10;

        const string VERSION = "1.6";
    }
}
