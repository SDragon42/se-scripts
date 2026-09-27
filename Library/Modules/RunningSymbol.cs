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
            const double MAX_TIME_SYMBOL = 0.2; // # of symbols (8) divided by the total display time of 1.6 seconds. ex: 1.6 / 8 = 0.2 seconds per symbol.
            readonly string[] SYMBOLS = { "(|    )", "( |   )", "(  |  )", "(   | )", "(    |)", "(   | )", "(  |  )", "( |   )" };
            double _time = 0;
            int _pos = -1;

            public string GetSymbol() {
                _pos++;
                if (IsOutOfRange(_pos)) { _pos = 0; }
                return SYMBOLS[_pos];
            }

            public string GetSymbol(IMyGridProgramRuntimeInfo runtime) {
                _time += runtime.TimeSinceLastRun.TotalSeconds;
                _pos = (runtime.UpdateFrequency & (UpdateFrequency.Update1 | UpdateFrequency.Update10)) != 0
                    ? Convert.ToInt32(_time / MAX_TIME_SYMBOL) // Converts elapsed seconds into “which symbol slot” you are in.
                    : _pos + 1;
                if (IsOutOfRange(_pos)) { _pos = 0; _time = 0; }
                return SYMBOLS[_pos];
            }

            bool IsOutOfRange(int pos) => pos >= SYMBOLS.Length || pos < 0;

        }

    }
}
