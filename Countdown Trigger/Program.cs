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
    partial class Program : MyGridProgram {

        readonly MyIni Ini = new MyIni();
        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();

        readonly StateMachine Operation = new StateMachine();

        public Program() {
            LoadConfig();
        }

        public void Save() {
        }

        static readonly char[] ArgSplit = new char[] { ' ' };

        public void Main(string argument, UpdateType updateSource) {
            Echo("Countdown Timer");
            LoadConfig();

            var argumentParts = argument.Split(ArgSplit, 2);
            if (argumentParts.Length == 2) {
                var cmd = argumentParts[0].ToLower();
                var tag = argumentParts[1];

                switch (cmd) {
                    case "start": StartUndock(tag); break;
                    case "abort": Abort(tag); break;
                    default: break;
                }
            }

            Operation.RunAll();

            if (Operation.HasTasks)
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            else
                Runtime.UpdateFrequency = UpdateFrequency.None;
        }

        void Abort(string tag) {
            if (!Operation.HasTask(tag)) return;
            Operation.Remove(tag);
            var displayBlocks = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType(displayBlocks, b => Collect.IsTagged(b, tag));
            displayBlocks.ForEach(d => d.WriteText(string.Empty));
        }

        void StartUndock(string tag) {
            if (Operation.HasTask(tag)) return;
            Operation.Add(tag, RunUndockSequence(tag), true);
        }

        IEnumerator<bool> RunUndockSequence(string tag) {
            var timerBlocks = new List<IMyTimerBlock>();
            GridTerminalSystem.GetBlocksOfType(timerBlocks, b => Collect.IsTagged(b, tag));

            var displayBlocks = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType(displayBlocks, b => Collect.IsTagged(b, tag));

            if (timerBlocks.Count == 0) Echo("No Timer block found");
            if (displayBlocks.Count == 0) Echo("No LCD block found");
            if (timerBlocks.Count > 0) {

                var timeRemaining = CountdownSeconds;
                ShowCountdown(displayBlocks, timeRemaining);
                while (timeRemaining >= 0) {
                    yield return true;
                    timeRemaining -= Runtime.TimeSinceLastRun.TotalSeconds;
                    ShowCountdown(displayBlocks, timeRemaining);
                }


                yield return true;
                timerBlocks.ForEach(t => t.Trigger());

                yield return true;
                var clearDelaySeconds = DisplayClearSeconds;
                while (clearDelaySeconds >= 0) {
                    yield return true;
                    clearDelaySeconds -= Runtime.TimeSinceLastRun.TotalSeconds;
                }

                displayBlocks.ForEach(d => d.WriteText(string.Empty));
                yield return false;
            }
        }

        void ShowCountdown(List<IMyTextPanel> displays, double timeRemaining) {
            var text = Math.Round(timeRemaining, MidpointRounding.AwayFromZero).ToString("N0");
            Echo("Countdown: " + text);
            displays.ForEach(d => d.WriteText(text));
        }


        int _configHashCode = 0;

        readonly MyIniKey Key_NumSeconds = new MyIniKey("Timer", "Countdown Time");
        readonly MyIniKey Key_DisplayClearSeconds = new MyIniKey("Timer", "Display Clear Time");

        void LoadConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;

            Ini.Clear();
            Ini.TryParse(Me.CustomData);

            Ini.Add(Key_NumSeconds, 30);
            Ini.Add(Key_DisplayClearSeconds, 5);

            Me.CustomData = Ini.ToString();
            _configHashCode = Me.CustomData.GetHashCode();
        }

        double CountdownSeconds => Ini.Get(Key_NumSeconds).ToDouble();
        double DisplayClearSeconds => Ini.Get(Key_DisplayClearSeconds).ToDouble();
    }
}
