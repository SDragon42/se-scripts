﻿using Sandbox.Game.EntityComponents;
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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        static class GRID {
            public const string STAGE1 = "Stage 1";
            public const string STAGE1B = "Stage 1b";
            public const string STAGE1C = "Stage 1c";
            public const string STAGE2 = "Stage 2";
        }

        static class TAG {
            public const string GRID = "[SDLS]";

            public const string LAUNCH_CLAMP = "[launch-clamp]";
            public const string STAGING_CLAMP = "[stage-clamp]";

            public const string MAIN = "[main]";
            public const string MANEUVER = "[maneuver]";
            public const string LANDING1 = "[landing-1]";
            public const string LANDING2 = "[landing-2]";
        }

    }
}
