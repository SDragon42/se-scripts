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
    class ProfilerModule
    {
        const int DEFAULT_MAX_EXECUTIONS = 60;

        int _count = 1;
        readonly int _maxExecutions = DEFAULT_MAX_EXECUTIONS;
        readonly StringBuilder _profile = new StringBuilder();
        readonly string _screenName;

        public ProfilerModule(string screenName = "PROFILE", int maxExecutionsToLog = DEFAULT_MAX_EXECUTIONS)
        {
            _screenName = screenName;
            _maxExecutions = (maxExecutionsToLog > 0) ? maxExecutionsToLog : DEFAULT_MAX_EXECUTIONS;
        }

        public void ProfilerGraph(MyGridProgram thisObj)
        {
            if (_count <= _maxExecutions)
            {
                thisObj.Echo("Profiler:Add");
                var timeToRunCode = thisObj.Runtime.LastRunTimeMs;
                _profile
                    .Append(timeToRunCode.ToString())
                    .Append("\n");
                _count++;
            }
            else
            {
                thisObj.Echo("Profiler:Display");
                DisplayResults(thisObj, _profile.ToString());
            }
        }

        public void ResetProfiler(MyGridProgram thisObj)
        {
            _count = 1;
            _profile.Clear();
            DisplayResults(thisObj, string.Empty);
        }

        void DisplayResults(MyGridProgram thisObj, string results)
        {
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
        public static float ShowExecutionCost(MyGridProgram thisObj)
        {
            var fper = thisObj.Runtime.CurrentInstructionCount / (float)thisObj.Runtime.MaxInstructionCount;
            var percentage = fper * 100;
            thisObj.Echo("Instructions: " + percentage.ToString() + "%");
            return percentage;
        }

    }
}
