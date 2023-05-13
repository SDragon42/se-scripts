// <mdk sortorder="1000" />
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
        class RunningSymbol {
            public double MaxTime { get; set; } = 1.6;
            readonly string[] Parts = { "(|    )", "( |   )", "(  |  )", "(   | )", "(    |)", "(   | )", "(  |  )", "( |   )" };

            double time = 0.0;
            int pos = -1;

            public string GetSymbol(IMyGridProgramRuntimeInfo runtime = null) {
                if (runtime == null) {
                    pos++;
                } else {
                    var timeUpdate = (runtime.UpdateFrequency & UpdateFrequency.Update10) == UpdateFrequency.Update10 || (runtime.UpdateFrequency & UpdateFrequency.Update1) == UpdateFrequency.Update1;

                    time += runtime.TimeSinceLastRun.TotalSeconds;
                    pos = timeUpdate ? Convert.ToInt32(time / (MaxTime / Parts.Length)) : pos + 1;
                }

                if (pos >= Parts.Length || pos < 0) {
                    pos = 0;
                    time = 0.0;
                }

                return Parts[pos];
            }

        }
    }
}
