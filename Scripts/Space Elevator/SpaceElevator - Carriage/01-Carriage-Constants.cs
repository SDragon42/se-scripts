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
        //-------------------------------------------------------------------------------
        //  SCRIPT COMMANDS
        //-------------------------------------------------------------------------------
        const string CMD_Reset = "reset";
        const string CMD_Goto = "goto";


        //-------------------------------------------------------------------------------
        //  CONSTANTS
        //-------------------------------------------------------------------------------
        const double SWITCH_TO_AUTOPILOT_RANGE = 1;
        const double DOCKED_AT_STATION_RANGE = 25.0;

        const float GRAV_RANGE_RampsDown = 42.5f;
        const float GRAV_RANGE_Rampsup = 12.5f;

        const double GRAV_Force_Earth = 9.81;

    }
}
