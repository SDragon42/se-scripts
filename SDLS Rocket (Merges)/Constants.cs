// <mdk sortorder="200" />
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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {

        const string ScriptName = "SDLS";

        static class TAGS {
            public const string MAIN = "[main]";
            public const string MANEUVER = "[maneuver]";
            public const string LANDING1 = "[landing-1]";
            public const string LANDING2 = "[landing-2]";
        }


        enum FlightMode {
            Off, Standby, Launch, Land
        }


        [Flags]
        enum RocketStructure {
            Unknown = 0,
            Pod = 1 << 0,
            Stage2 = 1 << 1,
            Stage1 = 1 << 2,
            Booster = 1 << 3,

            Rocket_S1 = Pod + Stage1,
            Rocket_S12 = Pod + Stage2 + Stage1,
            Rocket_S1B = Pod + Stage1 + Booster,
            Rocket_S12B = Pod + Stage2 + Stage1 + Booster,
        }

    }
}
