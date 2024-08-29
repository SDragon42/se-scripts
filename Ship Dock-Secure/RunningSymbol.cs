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
        class RunningSymbol {
            readonly double MaxTime;
            readonly string[] Parts;

            public RunningSymbol(double loopTime = 1.6) {
                MaxTime = loopTime;
                Parts = new string[] { "[|    ]", "[ |   ]", "[  |  ]", "[   | ]", "[    |]", "[   | ]", "[  |  ]", "[ |   ]" };
            }

            double time = 0.0;
            int pos = -1;

            public string GetSymbol(IMyGridProgramRuntimeInfo runtime) {
                var timeUpdate = (runtime.UpdateFrequency & UpdateFrequency.Update10) == UpdateFrequency.Update10 || (runtime.UpdateFrequency & UpdateFrequency.Update1) == UpdateFrequency.Update1;

                time += runtime.TimeSinceLastRun.TotalSeconds;
                pos = timeUpdate ? Convert.ToInt32(time / (MaxTime / Parts.Length)) : pos + 1;

                if (pos > 7 || pos < 0) {
                    pos = 0;
                    time = 0.0;
                }

                return Parts[pos];
            }

        }
    }
}
