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

        DateTime _startTime = DateTime.UtcNow;
        DateTime _currentTime = DateTime.UtcNow;
        bool _isRaceActive = false;
        readonly Queue<string> _checkpointLog = new Queue<string>(100);

        List<IMyTextPanel> _displaySurfaces = new List<IMyTextPanel>();

        readonly RunningSymbol RunningModule = new RunningSymbol();
        readonly DebugLogging Log;
        IMyBroadcastListener _listener = null;

        readonly Action<string> Debug = (text) => { };

        readonly IDictionary<string, Action<string>> Commands = new Dictionary<string, Action<string>>();

        public Program() {
            GridTerminalSystem.GetBlocksOfType(_displaySurfaces, surface => surface.CustomName == "LCD Panel - Race Info");
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            _listener = IGC.RegisterBroadcastListener(Constants.CheckPointTag);
            _listener.SetMessageCallback(CMD_CHECKPOINT);

            Log = new DebugLogging(this);
            Log.EchoMessages = true;
            Log.Enabled = true;
            Log.MaxTextLinesToKeep = 20;
            Debug = (msg) => Log.AppendLine($"{DateTime.Now:HH:mm:ss.fff} {msg}");
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo($"Race Timer {RunningModule.GetSymbol()}");
            //if (updateSource == UpdateType.Trigger)
            //    Debug($"trig: [{argument}]");

            try {
                switch (updateSource) {
                    case UpdateType.IGC:
                        Debug($"IGC: [{argument}]");
                        ProcessCommunication(argument);
                        break;

                    default:
                        ProcessCommand(argument);
                        break;
                }
            } finally {
                DisplayRaceInfo();
                Log.UpdateDisplay();
            }

        }

        void CommandStart() {
            Debug($"=> {CMD_START}");
            _startTime = DateTime.UtcNow;
            _currentTime = _startTime;
            _isRaceActive = true;
        }

        void CommandStop() {
            Debug($"=> {CMD_STOP}");
            _isRaceActive = false;
            _currentTime = DateTime.UtcNow;
        }

        void CommandReset() {
            Debug($"=> {CMD_RESET}");
            _startTime = DateTime.UtcNow;
            _currentTime = _startTime;
            _isRaceActive = false;
            _checkpointLog.Clear();
        }

        void CommandCheckpoint(string argument) {
            Debug($"=> {CMD_CHECKPOINT}");
            var commsData = GetTimeInfo(argument);
            _checkpointLog.Enqueue($"{commsData.Checkpoint} : {commsData.TimeStamp}");
        }

        private void ProcessCommand(string argument) {
            if (string.IsNullOrEmpty(argument))
                return;

            var commandParts = SplitArgument(argument);
            switch (commandParts.Command) {
                case "start":
                    Debug($"=> {commandParts.Command}");
                    _startTime = DateTime.UtcNow;
                    _currentTime = _startTime;
                    _isRaceActive = true;
                    break;

                case "stop":
                    Debug($"=> {commandParts.Command}");
                    _isRaceActive = false;
                    _currentTime = DateTime.UtcNow;
                    break;

                case "reset":
                    Debug($"=> {commandParts.Command}");
                    _startTime = DateTime.UtcNow;
                    _currentTime = _startTime;
                    _isRaceActive = false;
                    _checkpointLog.Clear();
                    break;

                default:
                    Debug("UNKNWON: " + argument);
                    break;
            }

        }

        private void ProcessCommunication(string argument) {
            var commsData = GetTimeInfo(argument);
            _checkpointLog.Enqueue($"{commsData.Checkpoint} : {commsData.TimeStamp}");
        }

        CmdParts SplitArgument(string argument) => new CmdParts(argument ?? string.Empty);
        CheckpointInfo GetTimeInfo(string data) => new CheckpointInfo(data ?? string.Empty);




        private void DisplayRaceInfo() {
            var nowTime = _isRaceActive ? DateTime.UtcNow : _currentTime;

            var message = new StringBuilder();
            var elapsedTime = nowTime - _startTime;
            message.AppendLine($"Time: {elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}.{elapsedTime.Milliseconds:00}");
            message.AppendLine();
            foreach (var checkpointTime in _checkpointLog) {
                message.AppendLine(checkpointTime);
            }

            foreach (IMyTextSurface surface in _displaySurfaces) {
                surface.WriteText(message.ToString());
            }

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
            public readonly DateTime? TimeStamp;

            public CheckpointInfo(string argument) {
                var parts = argument.Split(Constants.ArgSplitChar);
                Checkpoint = parts[0];
                TimeStamp = null;
                if (parts.Length > 1) {
                    DateTime ts;
                    if (DateTime.TryParse(parts[1], out ts)) {
                        TimeStamp = ts;
                    }
                }
            }
        }

    }
}
