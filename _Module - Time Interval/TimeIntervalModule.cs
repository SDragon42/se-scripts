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
    class TimeIntervalModule
    {
        public TimeIntervalModule(double intervalInSeconds)
        {
            SetIntervalInSeconds(intervalInSeconds);
        }
        public TimeIntervalModule(int numIntervalsPerSecond = 10)
        {
            SetNumIntervalsPerSecond(numIntervalsPerSecond);
        }

        bool _resetTime = false;

        TimeSpan _time = new TimeSpan();
        public TimeSpan GetTime() { return _time; }

        double _intervalInSeconds = 0.0;
        public double GetIntervalInSeconds() { return _intervalInSeconds; }
        public void SetIntervalInSeconds(double intervalInSeconds)
        {
            if (intervalInSeconds < 0) intervalInSeconds = 0;
            _intervalInSeconds = intervalInSeconds;
        }
        public void SetNumIntervalsPerSecond(int numIntervalsPerSecond)
        {
            numIntervalsPerSecond = MathHelper.Clamp(numIntervalsPerSecond, 1, 60);
            _intervalInSeconds = (1.0 / numIntervalsPerSecond);
        }

        public void RecordTime(IMyGridProgramRuntimeInfo runtime)
        {
            if (_resetTime) Reset();
            _time += runtime.TimeSinceLastRun;
            if (_time.TotalSeconds < _intervalInSeconds) return;
            _resetTime = true;
        }
        public bool AtNextInterval()
        {
            return (_time.TotalSeconds >= _intervalInSeconds);
        }
        public void Reset()
        {
            _time = new TimeSpan();
            _resetTime = false;
        }
    }
}
