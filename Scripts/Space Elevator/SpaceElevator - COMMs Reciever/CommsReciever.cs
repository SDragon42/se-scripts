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
    partial class Program : MyGridProgram {
        readonly CustomDataConfig _custConfig = new CustomDataConfig();
        int _configHash = 0;
        const string KEY_ProgramBlockName = "Program Block";
        const string KEY_TimerBlockName = "Timer Block";
        const string KEY_LogLinesToShow = "Lines to Show";
        const string KEY_LogDisplayName = "Log LCD Name";

        Logging _log;

        IMyTimerBlock _timer = null;
        IMyProgrammableBlock _targetProgram = null;
        IMyTextPanel _display = null;

        readonly Queue<CommMessage> _messageQueue = new Queue<CommMessage>();

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _log = new Logging();

            _custConfig.AddKey(KEY_ProgramBlockName,
                description: "The is the name of the program block forward messages to.");
            _custConfig.AddKey(KEY_TimerBlockName,
                description: "The is the name of the timer block that is setup to execute this program.");
            _custConfig.AddKey(KEY_LogLinesToShow,
                description: "The number of lines to retain in the display log.",
                defaultValue: "20");
            _custConfig.AddKey(KEY_LogDisplayName,
                description: "(OPTIONAL) The LCD to display the log on.");

            ReloadConfig();
        }

        public void Main(string argument) {
            try {
                Echo("COMMS Reciever v1.0");
                ReloadConfig();

                _targetProgram = GetBlockWithName<IMyProgrammableBlock>(_custConfig.GetValue(KEY_ProgramBlockName));
                _timer = GetBlockWithName<IMyTimerBlock>(_custConfig.GetValue(KEY_TimerBlockName));
                _display = GetBlockWithName<IMyTextPanel>(_custConfig.GetValue(KEY_LogDisplayName));

                if (_targetProgram == null) Echo("No target Program Block found.");
                if (_timer == null) Echo("No timer block found.");
                if (_display == null) Echo("No Log Display.");
                if (_targetProgram == null || _timer == null) return;

                ProcessArgument(argument);
                ProcessQueue();

                var logText = _log.GetLogText();
                Echo(logText);

                if (_display != null) {
                    _display.Font = LCDFonts.MONOSPACE;
                    _display.FontSize = 0.6f;
                    _display.ShowPublicTextOnScreen();
                    _display.WritePublicText(logText);
                }
            } catch (Exception ex) {
                Echo(ex.Message);
                Echo(ex.StackTrace);
                throw ex;
            }
        }

        List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
        T GetBlockWithName<T>(string blockName) where T : class {
            GridTerminalSystem.GetBlocksOfType<T>(_blocks, b => IsOnThisGrid(b) && (string.Compare(b.CustomName, blockName, true) == 0));
            return (_blocks.Count > 0) ? (T)_blocks[0] : null;
        }


        void ReloadConfig() {
            if (_configHash == Me.CustomData.GetHashCode())
                return;
            _custConfig.ReadFromCustomData(Me);
            _custConfig.SaveToCustomData(Me);
            _configHash = Me.CustomData.GetHashCode();
            _log.MaxTextLinesToKeep = _custConfig.GetValue(KEY_LogLinesToShow).ToInt(10);
        }


        void ProcessArgument(string argument) {
            if (string.IsNullOrWhiteSpace(argument)) return;
            var text = DateTime.Now.ToLongTimeString() + " | ";
            try {
                CommMessage msg = null;
                if (!CommMessage.TryParse(argument, out msg)) {
                    text += "Malformed Msg";
                    return;
                }
                if (!msg.IsValid()) {
                    text += "Invalid Msg";
                    return;
                }
                text += msg.SenderGridName + " | " + msg.PayloadType;
                _messageQueue.Enqueue(msg);
            } finally {
                _log.AppendLine(text);
            }
        }


        void ProcessQueue() {
            if (_messageQueue.Count <= 0) return;

            var msg = _messageQueue.Dequeue();
            if (msg == null) return;
            if (Me.CubeGrid.EntityId == msg.SenderGridEntityId) return;
            if (msg.TargetGridName.Length > 0) {
                if (string.Compare(msg.TargetGridName, Me.CubeGrid.CustomName, true) != 0) return;
            }

            var arg = msg.ToString();
            if (!_targetProgram.TryRun(arg)) {
                _messageQueue.Enqueue(msg);
            }

            if (_messageQueue.Count > 0) _timer.Trigger();
        }


        bool IsOnThisGrid(IMyTerminalBlock b) { return Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }

    }
}
