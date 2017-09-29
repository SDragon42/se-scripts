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
    class TimeIntervalModule {
        public TimeIntervalModule(double intervalInSeconds) {
            SetIntervalInSeconds(intervalInSeconds);
            Reset();
        }
        public TimeIntervalModule(int numIntervalsPerSecond = 10) {
            SetNumIntervalsPerSecond(numIntervalsPerSecond);
            Reset();
        }

        bool _resetTime = false;

        public TimeSpan Time { get; private set; }
        public double IntervalInSeconds { get; private set; }

        public void SetIntervalInSeconds(double intervalInSeconds) {
            if (intervalInSeconds < 0) intervalInSeconds = 0;
            IntervalInSeconds = intervalInSeconds;
        }
        public void SetNumIntervalsPerSecond(int numIntervalsPerSecond) {
            numIntervalsPerSecond = MathHelper.Clamp(numIntervalsPerSecond, 1, 60);
            IntervalInSeconds = (1.0 / numIntervalsPerSecond);
        }

        public void RecordTime(IMyGridProgramRuntimeInfo runtime) {
            if (_resetTime) Reset();
            Time += runtime.TimeSinceLastRun;
            if (Time.TotalSeconds < IntervalInSeconds) return;
            _resetTime = true;
        }
        public bool AtNextInterval() {
            return (Time.TotalSeconds >= IntervalInSeconds);
        }
        public void Reset() {
            Time = new TimeSpan();
            _resetTime = false;
        }
    }
}
