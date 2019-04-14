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
    partial class Program
    {
        class Profiler
        {
            const int DEFAULT_MAX_EXECUTIONS = 60;
            const string DEFAULT_SCREEN_NAME = "PROFILE";

            readonly int _maxExecutions = DEFAULT_MAX_EXECUTIONS;
            readonly List<double> _logTime = new List<double>();
            readonly List<float> _logCost = new List<float>();
            readonly string _screenName;
            readonly List<IMyTextPanel> _displays = new List<IMyTextPanel>();

            int _count = 0;
            string _results = string.Empty;
            double _avgExecutionTime = 0;
            float _avgExecutionCost = 0;

            public Profiler(string screenName = DEFAULT_SCREEN_NAME, int maxExecutionsToLog = DEFAULT_MAX_EXECUTIONS)
            {
                _screenName = screenName?.ToLower() ?? string.Empty;
                if (_screenName.Length == 0)
                    _screenName = DEFAULT_SCREEN_NAME;
                _maxExecutions = (maxExecutionsToLog > 0) ? maxExecutionsToLog : DEFAULT_MAX_EXECUTIONS;
            }

            public void ProfilerGraph(MyGridProgram thisObj)
            {
                if (_count < _maxExecutions)
                {
                    _count++;
                    thisObj.Echo($"Profiler:Add - {_maxExecutions - _count}");
                    _logTime.Add(thisObj.Runtime.LastRunTimeMs);
                    _logCost.Add(CalcInstuctionCostPercentage(thisObj));
                }
                else
                {
                    thisObj.Echo("Profiler:Display");
                    if (_results.Length == 0)
                    {
                        _results = BuildResults();
                        DisplayResults(thisObj, _results);
                    }
                    thisObj.Echo($"Profiler:Avg Time: {_avgExecutionTime:N6} ms");
                    thisObj.Echo($"Profiler:Avg Cost: {_avgExecutionCost:N6} %");
                }
            }
            public void ResetProfiler(MyGridProgram thisObj)
            {
                _count = 0;
                _logTime.Clear();
                _results = string.Empty;
                DisplayResults(thisObj, string.Empty);
            }

            static void RemoveExtreams<T>(IList<T> log)
            {
                var min = log.Min();
                var max = log.Max();
                log.Remove(min);
                log.Remove(max);
            }

            string BuildResults()
            {
                RemoveExtreams(_logTime);
                _avgExecutionTime = _logTime.Average();
                RemoveExtreams(_logCost);
                _avgExecutionCost = _logCost.Average();

                var sb = new StringBuilder();
                sb.Append($"AVG Time: {_avgExecutionTime:N3} ms\n");
                sb.Append($"AVG Cost: {_avgExecutionCost:N2} %\n");
                sb.Append("FULL LOG:\n");
                foreach (var l in _logTime) sb.Append($"{l:N6}\n");
                return sb.ToString();
            }
            void DisplayResults(MyGridProgram thisObj, string results)
            {
                thisObj.GridTerminalSystem.GetBlocksOfType(_displays, b =>
                {
                    if (b.CubeGrid != thisObj.Me.CubeGrid) return false;
                    if (b.CustomName.ToLower() != _screenName) return false;
                    return true;
                });
                if (_displays.Count == 0) return;
                foreach (var screen in _displays)
                {
                    screen.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                    screen.WriteText(results);
                }
            }



            /// <summary>Shows the percentage of instructions executed at the current moment.
            /// </summary>
            /// <param name="thisObj"></param>
            /// <returns></returns>
            /// <remarks>
            /// This code is provided by Wicorel.
            /// https://forums.keenswh.com/threads/how-to-measure-the-performance-impact-of-certain-changes-to-ones-code.7395259/#post-1287057132
            /// </remarks>
            public static float ShowExecutionCost(MyGridProgram thisObj)
            {
                var percentage = CalcInstuctionCostPercentage(thisObj);
                thisObj.Echo($"Instructions: {percentage:N2} %");
                return percentage;
            }
            private static float CalcInstuctionCostPercentage(MyGridProgram thisObj)
            {
                var fper = thisObj.Runtime.CurrentInstructionCount / (float)thisObj.Runtime.MaxInstructionCount;
                var percentage = fper * 100;
                return percentage;
            }

        }
    }
}
