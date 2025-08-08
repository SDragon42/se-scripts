using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Scripting;
using VRageMath;

namespace IngameScript {
    public partial class Program : MyGridProgram {
        const string CMD_START = "start";
        const string CMD_STOP = "stop";
        const string CMD_RESET = "reset";
        const string CMD_CHECKPOINT = "checkpoint";

        long _startTime;
        long _currentTime;
        bool _isRaceActive;
        readonly Queue<string> _checkpointLog = new Queue<string>(100);

        List<IMyTextPanel> _displaySurfaces = new List<IMyTextPanel>();

        readonly RunningSymbol RunningModule = new RunningSymbol();
        readonly DebugLogging Log;
        IMyBroadcastListener _listener = null;

        readonly Action<string> Debug = (text) => { };

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            _listener = IGC.RegisterBroadcastListener(Constants.CheckPointTag);
            _listener.SetMessageCallback(CMD_CHECKPOINT); // Runs this script with the argument as the message received.

            GridTerminalSystem.GetBlocksOfType(_displaySurfaces, surface => surface.CustomName == "LCD Panel - Race Info");

            Log = new DebugLogging(this);
            Log.EchoMessages = true;
            Log.Enabled = true;
            Log.MaxTextLinesToKeep = 20;
            Debug = (msg) => Log.AppendLine($"{DateTime.Now:HH:mm:ss.fff} {msg}");

            CommandReset(); // Reset the timer on startup.
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo($"VVC Race Timer {RunningModule.GetSymbol()}");
            try {
                var commandParts = SplitArgument(argument);
                switch (commandParts.Command) {
                    case "": break;
                    case CMD_START: CommandStart(); break;
                    case CMD_STOP: CommandStop(); break;
                    case CMD_RESET: CommandReset(); break;
                    case CMD_CHECKPOINT: CommandCheckpoint(); break;

                    default: Debug($"Unknown: {commandParts.Command} | {commandParts.Data}"); break;
                }
            } finally {
                DisplayRaceInfo();
                Log.UpdateDisplay();
            }

        }

        void CommandStart() {
            Debug($"=> {CMD_START}");
            _startTime = DateTime.Now.Ticks;
            _currentTime = _startTime;
            _isRaceActive = true;
        }

        void CommandStop() {
            Debug($"=> {CMD_STOP}");
            _currentTime = DateTime.Now.Ticks;
            _isRaceActive = false;
        }

        void CommandReset() {
            Debug($"=> {CMD_RESET}");
            _startTime = DateTime.Now.Ticks;
            _currentTime = _startTime;
            _isRaceActive = false;
            _checkpointLog.Clear();
        }

        void CommandCheckpoint() {
            Debug($"=> {CMD_CHECKPOINT}");
            var commsData = GetTimeInfo(_listener.AcceptMessage().Data as string);
            var logMessage = commsData.Ticks.HasValue
                ? $"{commsData.Checkpoint} : {CalculateElapsedTime(commsData.Ticks.Value).ToRaceTimeString()}"
                : $"{commsData.Checkpoint}";
            _checkpointLog.Enqueue(logMessage);
        }

        CmdParts SplitArgument(string argument) => new CmdParts(argument ?? string.Empty);
        CheckpointInfo GetTimeInfo(string data) => new CheckpointInfo(data ?? string.Empty);




        private void DisplayRaceInfo() {
            var message = new StringBuilder();
            var nowTime = _isRaceActive ? DateTime.Now.Ticks : _currentTime;
            var elapsedTime = CalculateElapsedTime(nowTime).ToRaceTimeString();
            message.AppendLine($"Time: {elapsedTime}");
            message.AppendLine();
            message.AppendLines(_checkpointLog);
            WriteToAllDisplays(message.ToString());
        }
        private void WriteToAllDisplays(string text) {
            foreach (IMyTextSurface surface in _displaySurfaces) {
                //surface.ContentType = ContentType.TEXT_AND_IMAGE;
                surface.WriteText(text);
            }
        }

        private TimeSpan CalculateElapsedTime(long currentTicks) {
            return new TimeSpan(currentTicks - _startTime);
        }



        struct CmdParts {
            public readonly string Command;
            public readonly string Data;

            public CmdParts(string argument) {
                var parts = argument.Split(Constants.ArgSplitChar, 2);
                Command = parts[0].ToLower();
                Data = parts.Length > 1 ? parts[1] : string.Empty;
            }
        }

        struct CheckpointInfo {
            public readonly string Checkpoint;
            public readonly long? Ticks;

            public CheckpointInfo(string argument) {
                var parts = argument.Split(Constants.ArgSplitChar);
                Checkpoint = parts[0];
                Ticks = null;
                if (parts.Length > 1) {
                    long ts;
                    if (long.TryParse(parts[1], out ts)) {
                        Ticks = ts;
                    }
                }
            }
        }

    }
}
