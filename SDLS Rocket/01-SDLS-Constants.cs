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
    partial class Program : MyGridProgram {

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
                return (grid.CustomName == POD || grid.CustomName == PILOTED_POD);
            }
        }

        static class TAG {
            public const string GRID = "[SDLS]";

            public const string LAUNCH_CLAMP = "[launch-clamp]";
            public const string STAGE1_CLAMP = "[stage1-clamp]";
            public const string STAGE2_CLAMP = "[stage2-clamp]";
            //public const string STAGE_CLAMP = "[stage-clamp]";
            public const string BOOSTER_CLAMP = "[booster-clamp]";

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
            None = 0,
            Pod = 1 << 0,
            Stage2 = 1 << 1,
            CoreBooster = 1 << 2,
            SideBooster = 1 << 3,
            Rocket_Core = Pod + CoreBooster,
            Rocket_Stg2_Core = Pod + Stage2 + CoreBooster,
            Rocket_Core_2Side = Pod + CoreBooster + SideBooster,
            Rocket_Stg_Core_2Side = Pod + Stage2 + CoreBooster + SideBooster,
        }
    }
}
