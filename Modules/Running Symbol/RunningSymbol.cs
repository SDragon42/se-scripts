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
        class RunningSymbol {
            double _time = 0.0;

            public string GetSymbol(IMyGridProgramRuntimeInfo runtime) {
                _time += runtime.TimeSinceLastRun.TotalSeconds;
                if (_time < 0.2) return "[|    ]";
                if (_time < 0.4) return "[ |   ]";
                if (_time < 0.6) return "[  |  ]";
                if (_time < 0.8) return "[   | ]";
                if (_time < 1.0) return "[    |]";
                if (_time < 1.2) return "[   | ]";
                if (_time < 1.4) return "[  |  ]";
                if (_time < 1.6) return "[ |   ]";
                _time = 0;
                return "[|    ]";
            }
        }
    }
}
