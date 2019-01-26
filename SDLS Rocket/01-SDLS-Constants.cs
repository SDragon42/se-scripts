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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {

        static class CMD {
            public const string SCAN = "scan";
        }

        static class GRID {
            //public const string STAGE1 = "Booster";
            public const string STAGE1B = "Side Booster";
            public const string STAGE1C = "Core Booster";
            public const string STAGE2 = "Stage 2";
            public const string POD = "Cargo Pod";
            public const string PILOTED_POD = "Cargo Shuttle";

            public static bool IsNamed(IMyCubeGrid grid) {
                //if (grid.CustomName == STAGE1) return true;
                if (grid.CustomName == STAGE1B) return true;
                if (grid.CustomName == STAGE1C) return true;
                if (grid.CustomName == STAGE2) return true;
                return IsMaster(grid);
            }

            public static bool IsMaster(IMyCubeGrid grid) {
                if (grid.CustomName == POD) return true;
                return (grid.CustomName == PILOTED_POD);
            }
        }

        static class TAG {
            public const string GRID = "[SDLS]";

            public const string LAUNCH_CLAMP = "[launch-clamp]";
            public const string STAGING_CLAMP = "[stage-clamp]";
            public const string BOOSTER_CLAMP = "[booster-clamp]";

            public const string MAIN = "[main]";
            public const string MANEUVER = "[maneuver]";
            public const string LANDING1 = "[landing-1]";
            public const string LANDING2 = "[landing-2]";
        }

    }
}
