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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {
        //-------------------------------------------------------------------------------
        //  SCRIPT COMMANDS
        //-------------------------------------------------------------------------------
        const string CMD_DockCarriage = "dock";
        const string CMD_UndockCarriage = "undock";
        const string CMD_RequestCarriage = "request-carriage";
        const string CMD_SendCarriage = "send";


        //-------------------------------------------------------------------------------
        //  CONSTANTS
        //-------------------------------------------------------------------------------
        const string TAG_A1 = "[A1]";
        const string TAG_A2 = "[A2]";
        const string TAG_B1 = "[B1]";
        const string TAG_B2 = "[B2]";
        const string TAG_MAINT = "[Maint]";

        const double TIME_ReloadBlockDelay = 10.0;

    }
}
