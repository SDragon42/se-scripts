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
        const string KEY_ProgramBlockName = "Program Block";
        const string KEY_LogLinesToShow = "Lines to Show";
        const string KEY_LogDisplayName = "Log LCD Name";

        const string DEF_ProgName = "Program - Carriage Control";
        const string DEF_LogLcdName = "Display - COMM Log";
        const int DEF_NumLogLines = 20;

        readonly CustomDataConfig _config = new CustomDataConfig();
        readonly Logging _log = new Logging();
        readonly Queue<CommMessage> _msgQueue = new Queue<CommMessage>();
        readonly List<IMyTerminalBlock> _tempBlocks = new List<IMyTerminalBlock>();

        int _configHash = 0;
        IMyProgrammableBlock _targetProgram = null;
        IMyTextPanel _display = null;


        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _config.AddKey(KEY_ProgramBlockName,
                description: "The is the name of the program block forward messages to.",
                defaultValue: DEF_ProgName);
            _config.AddKey(KEY_LogDisplayName,
                description: "The LCD to display the log on. (OPTIONAL)",
                defaultValue: DEF_LogLcdName);
            _config.AddKey(KEY_LogLinesToShow,
                defaultValue: DEF_NumLogLines.ToString());


            ReloadConfig();

            Runtime.UpdateFrequency = UpdateFrequency.Once;
        }

        public void Main(string argument, UpdateType updateSource) {
            try {
                Echo("COMMS Reciever v1.1");
                ReloadConfig();

                _targetProgram = GetBlockWithName<IMyProgrammableBlock>(_config.GetValue(KEY_ProgramBlockName));
                _display = GetBlockWithName<IMyTextPanel>(_config.GetValue(KEY_LogDisplayName));

                if (_targetProgram == null) Echo("No target Program Block found.");
                if (_display == null) Echo("No Log Display.");

                if (_targetProgram == null) return;

                ProcessArgument(argument);
                ProcessQueue();

                var logText = _log.GetLogText();
                Echo(logText);

                if (_display != null) {
                    _display.ShowPublicTextOnScreen();
                    _display.WritePublicText(logText);
                }
            } catch (Exception ex) {
                Echo(ex.Message);
                Echo(ex.StackTrace);
                throw ex;
            } finally {
                if (_msgQueue.Count > 0)
                    Runtime.UpdateFrequency |= UpdateFrequency.Once;
            }

        }


        T GetBlockWithName<T>(string name) where T : class {
            GridTerminalSystem.GetBlocksOfType<T>(_tempBlocks, b => IsOnThisGrid(b) && (string.Compare(b.CustomName, name, true) == 0));
            return (_tempBlocks.Count > 0) ? (T)_tempBlocks[0] : null;
        }


        void ReloadConfig() {
            if (_configHash == Me.CustomData.GetHashCode())
                return;
            _config.ReadFromCustomData(Me);
            _config.SaveToCustomData(Me);
            _configHash = Me.CustomData.GetHashCode();
            _log.MaxTextLinesToKeep = _config.GetValue(KEY_LogLinesToShow).ToInt(DEF_NumLogLines);
        }


        void ProcessArgument(string argument) {
            if (argument.Length == 0) return;
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
                _msgQueue.Enqueue(msg);
            } finally {
                _log.AppendLine(text);
            }
        }


        void ProcessQueue() {
            if (_msgQueue.Count == 0) return;

            var msg = _msgQueue.Dequeue();
            if (msg == null) return;
            if (Me.CubeGrid.EntityId == msg.SenderGridEntityId) return;
            if (msg.TargetGridName.Length > 0) {
                if (string.Compare(msg.TargetGridName, Me.CubeGrid.CustomName, true) != 0) return;
            }

            if (!_targetProgram.TryRun(msg.ToString())) {
                _msgQueue.Enqueue(msg);
            }
        }


        bool IsOnThisGrid(IMyTerminalBlock b) => Me.CubeGrid == b.CubeGrid;

    }
}
