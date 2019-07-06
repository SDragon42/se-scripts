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
            readonly double MaxTime;

            public RunningSymbol(double loopTime = 1.6) {
                MaxTime = loopTime;
            }

            double _time = 0.0;
            int _pos = 0;

            public string GetSymbol(IMyGridProgramRuntimeInfo runtime) {
                if (runtime.UpdateFrequency.HasFlag(UpdateFrequency.Update10) || runtime.UpdateFrequency.HasFlag(UpdateFrequency.Update1)) {
                    _time += runtime.TimeSinceLastRun.TotalSeconds;
                    _pos = Convert.ToInt32(_time / (MaxTime / 8)) + 1;
                } else {
                    _pos++;
                }

                if (_pos > 8 || _pos < 1) {
                    _pos = 1;
                    _time = 0.0;
                }

                switch (_pos) {
                    case 1: return "[|    ]";
                    case 2: return "[ |   ]";
                    case 3: return "[  |  ]";
                    case 4: return "[   | ]";
                    case 5: return "[    |]";
                    case 6: goto case 4;
                    case 7: goto case 3;
                    case 8: goto case 2;
                    default: goto case 1;
                }
            }
        }
    }
}
