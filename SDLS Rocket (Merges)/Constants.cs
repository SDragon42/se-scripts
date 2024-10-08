﻿// <mdk sortorder="200" />
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

        const string ScriptName = "SDLS";

        const string CMD_Check = "check";
        const string CMD_Launch = "launch";
        const string CMD_AwaitStaging = "await-staging";
        const string CMD_Shutdown = "shutdown";
        const string CMD_Init = "init";


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
            Rocket_S2 = Pod + Stage2,
            Rocket_S12 = Pod + Stage2 + Stage1,
            Rocket_S1B = Pod + Stage1 + Booster,
            Rocket_S12B = Pod + Stage2 + Stage1 + Booster,
        }

    }
}
