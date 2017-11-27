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
    class Profiler {
        const int DEFAULT_MAX_EXECUTIONS = 60;

        readonly int _maxExecutions = DEFAULT_MAX_EXECUTIONS;
        readonly List<double> _log = new List<double>();
        readonly string _screenName;

        int _count = 0;
        string _results = string.Empty;
        double _avgExecution = 0;

        public Profiler(string screenName = "PROFILE", int maxExecutionsToLog = DEFAULT_MAX_EXECUTIONS) {
            _screenName = screenName;
            _maxExecutions = (maxExecutionsToLog > 0) ? maxExecutionsToLog : DEFAULT_MAX_EXECUTIONS;
        }

        public void ProfilerGraph(MyGridProgram thisObj) {
            if (_count < _maxExecutions) {
                _count++;
                thisObj.Echo($"Profiler:Add - {_maxExecutions - _count}");
                _log.Add(thisObj.Runtime.LastRunTimeMs);
            } else {
                thisObj.Echo("Profiler:Display");
                if (_results.Length == 0) {
                    _results = BuildResults();
                    DisplayResults(thisObj, _results);
                }
                thisObj.Echo($"Profiler:Avg: {_avgExecution:N6}");
            }
        }

        public void ResetProfiler(MyGridProgram thisObj) {
            _count = 0;
            _log.Clear();
            _results = string.Empty;
            DisplayResults(thisObj, string.Empty);
        }

        void RemoveExtreams() {
            var min = _log.Min();
            var max = _log.Max();
            _log.Remove(min);
            _log.Remove(max);
        }

        string BuildResults() {
            RemoveExtreams();
            _avgExecution = _log.Average();

            var sb = new StringBuilder();
            sb.Append($"AVG: {_avgExecution:N6}\n");
            sb.Append("FULL LOG:\n");
            foreach (var l in _log) sb.Append($"{l:N6}\n");
            return sb.ToString();
        }
        void DisplayResults(MyGridProgram thisObj, string results) {
            var screen = thisObj.GridTerminalSystem.GetBlockWithName(_screenName) as IMyTextPanel;
            if (screen == null) return;
            screen.WritePublicText(results);
            screen.ShowPublicTextOnScreen();
        }


        /// <summary>Shows the percentage of instructions executed at the current moment.
        /// </summary>
        /// <param name="thisObj"></param>
        /// <returns></returns>
        /// <remarks>
        /// This code is provided by Wicorel.
        /// https://forums.keenswh.com/threads/how-to-measure-the-performance-impact-of-certain-changes-to-ones-code.7395259/#post-1287057132
        /// </remarks>
        public static float ShowExecutionCost(MyGridProgram thisObj) {
            var fper = thisObj.Runtime.CurrentInstructionCount / (float)thisObj.Runtime.MaxInstructionCount;
            var percentage = fper * 100;
            thisObj.Echo("Instructions: " + percentage.ToString() + "%");
            return percentage;
        }

    }
}
