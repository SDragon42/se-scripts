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
    partial class Program {

        class RunningSymbol_Time : BaseRunningSymbol {
            double _time = 0.0;

            protected override void OnConstrained() {
                _time = 0.0;
                base.OnConstrained();
            }
            public string GetSymbol(IMyGridProgramRuntimeInfo runtime) {
                _time += runtime.TimeSinceLastRun.TotalSeconds;
                _pos = Convert.ToInt32(_time / (1.6 / 8)) + 1;
                return MakeSymbol();
            }
        }

        class RunningSymbol_Step : BaseRunningSymbol {
            public string GetSymbol() {
                _pos++;
                return MakeSymbol();
            }
        }

        abstract class BaseRunningSymbol {
            protected int _pos = 0;

            protected virtual void OnConstrained() { }
            protected string MakeSymbol() {
                if (_pos > 8 || _pos < 1) {
                    _pos = 1;
                    OnConstrained();
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
