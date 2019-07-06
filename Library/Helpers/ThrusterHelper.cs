// <mdk sortorder="2000" />
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
        public static class ThrusterHelper {
            public static void GetThrusterOrientation(IMyTerminalBlock refBlock, IList<IMyThrust> unsortedThrusters, IList<IMyThrust> mainThrusters, IList<IMyThrust> sideThrusters) {
                var forwardDirn = refBlock.Orientation.Forward;
                foreach (var thisThrust in unsortedThrusters) {
                    var thrustDirn = Base6Directions.GetFlippedDirection(thisThrust.Orientation.Forward);
                    if (thrustDirn == forwardDirn)
                        mainThrusters.Add(thisThrust);
                    else
                        sideThrusters.Add(thisThrust);
                }
            }
        }
    }
}
