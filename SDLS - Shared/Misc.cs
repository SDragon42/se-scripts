﻿using System;
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

        void TagSelf() {
            //if (!Me.CustomName.Contains(TAG.GRID)) Me.CustomName = Me.CustomName.Trim() + " " + TAG.GRID;
            if (!IsSDLS(Me)) Me.CustomName = Me.CustomName.Trim() + " " + TAG.GRID;
        }

    }
}
