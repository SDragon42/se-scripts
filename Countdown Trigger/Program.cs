using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {

        readonly MyIni Ini = new MyIni();
        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();

        readonly StateMachine<bool> Operation = new StateMachine<bool>();

        public Program() {
            Commands.Add("start", Start);
            Commands.Add("abort", Abort);

            LoadConfig();
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            if (argument != string.Empty) argument = argument.ToLower();
            LoadConfig();

            Echo("Countdown Timer");

            if (Commands.ContainsKey(argument)) Commands[argument]?.Invoke();

            Operation.RunAll();

            if (Operation.HasTasks)
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            else
                Runtime.UpdateFrequency = UpdateFrequency.None;
        }

        void Start() {
            Operation.Clear();

            var _timer = GetBlock<IMyTimerBlock>(Ini.Get(Key_TimerBlock).ToString());
            var _display = GetBlock<IMyTextPanel>(Ini.Get(Key_DisplayBlock).ToString());

            if (_timer == null) Echo("No Timer block found");
            if (_display == null) Echo("No LCD block found");
            if (_timer == null) return;

            Operation.Add("undock", RunUndockSequence(_timer, _display, Ini.Get(Key_NumSeconds).ToDouble(), Ini.Get(Key_DisplayClearSeconds).ToDouble()), true);
        }
        void Abort() {
            Operation.Clear();

            var _display = GetBlock<IMyTextPanel>(Ini.Get(Key_DisplayBlock).ToString());
            _display?.WriteText(string.Empty);
        }

        IEnumerator<bool> RunUndockSequence(IMyTimerBlock _timer, IMyTextPanel _display, double timeRemaining, double clearDelaySeconds) {

            ShowCountdown(_display, timeRemaining);
            while (timeRemaining >= 0) {
                yield return true;
                timeRemaining -= Runtime.TimeSinceLastRun.TotalSeconds;
                ShowCountdown(_display, timeRemaining);
            }


            yield return true;
            _timer.Trigger();


            yield return true;
            while (clearDelaySeconds >= 0) {
                yield return true;
                clearDelaySeconds -= Runtime.TimeSinceLastRun.TotalSeconds;
            }

            _display?.WriteText(string.Empty);
            yield return false;
        }


        T GetBlock<T>(string name) where T : class, IMyTerminalBlock {
            GridTerminalSystem.GetBlocksOfType<T>(TmpBlocks, b => b.CustomName == name);
            return (TmpBlocks.Count == 1) ? (T)TmpBlocks[0] : default(T);
        }

        void ShowCountdown(IMyTextSurface _display, double timeRemaining) {
            var text = Math.Round(timeRemaining, MidpointRounding.AwayFromZero).ToString("N0");
            Echo("Countdown: " + text);
            _display?.WriteText(text);
        }
    }
}
