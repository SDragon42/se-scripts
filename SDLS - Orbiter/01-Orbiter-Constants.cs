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

        const string CMD_OFF = "off";
        const string CMD_MANUAL = "manual";
        const string CMD_STANDBY = "standby";
        const string CMD_SCAN = "scan";
        const string CMD_ALIGN_LAUNCH = "align-launch";
        const string CMD_ALIGN_LAND = "align-land";
    }
}
