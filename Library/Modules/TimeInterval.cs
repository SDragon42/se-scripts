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
        class TimeInterval {
            public TimeInterval(double seconds) {
                interval = Math.Max(seconds, 0);
                Reset();
            }

            bool toReset = false;
            TimeSpan time;
            double interval;

            public double Remaining => interval - time.TotalSeconds;
            public bool AtNextInterval => (time.TotalSeconds >= interval);

            public void Add(IMyGridProgramRuntimeInfo runtime) {
                if (toReset) Reset();
                time += runtime.TimeSinceLastRun;
                if (time.TotalSeconds < interval) return;
                toReset = true;
            }

            void Reset() {
                time = new TimeSpan();
                toReset = false;
            }
        }
    }
}
