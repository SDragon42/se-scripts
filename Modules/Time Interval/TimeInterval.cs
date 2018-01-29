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

namespace IngameScript
{
    partial class Program {
        class TimeInterval {
            public TimeInterval(double seconds) {
                SetInterval(seconds);
                Reset();
            }

            bool _resetTime = false;

            public TimeSpan Time { get; private set; }
            public double Interval { get; private set; }
            public bool AtNextInterval => (Time.TotalSeconds >= Interval);

            public void SetInterval(double seconds) {
                Interval = (seconds >= 0) ? seconds : 0.0;
            }

            public void RecordTime(IMyGridProgramRuntimeInfo runtime) {
                if (_resetTime) Reset();
                Time += runtime.TimeSinceLastRun;
                if (Time.TotalSeconds < Interval) return;
                _resetTime = true;
            }

            public void Reset() {
                Time = new TimeSpan();
                _resetTime = false;
            }
        }
    }
}
