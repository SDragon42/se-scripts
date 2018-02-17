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

        readonly CustomDataConfig _config = new CustomDataConfig();
        readonly ScriptSettings _settings = new ScriptSettings();
        readonly Logging _log = new Logging();
        readonly Queue<CommMessage> _msgQueue = new Queue<CommMessage>();
        readonly List<IMyTerminalBlock> _tempBlocks = new List<IMyTerminalBlock>();

        int _configHash = 0;
        IMyProgrammableBlock _targetProgram = null;
        IMyTextPanel _display = null;


        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _settings.InitConfig(_config);
            _log.Enabled = false;
        }

        public void Main(string argument, UpdateType updateSource) {
            try {
                Echo("COMMs Receiver");
                LoadConfigSettings();

                _targetProgram = GetBlockWithName<IMyProgrammableBlock>(_settings.ProgramBlockName);
                _display = GetBlockWithName<IMyTextPanel>(_settings.LogLcdName);

                if (_targetProgram == null) Echo("No target Program Block found.");
                if (_display == null) Echo("No Log Display.");

                if (_targetProgram == null) return;

                ProcessArgument(argument);
                ProcessQueue();

                if (_log.Enabled) {
                    var logText = "TIME | FROM | MSG Type\n" + _log.GetLogText();
                    Echo(logText);

                    if (_display != null) {
                        _display.ShowPublicTextOnScreen();
                        _display.WritePublicText(logText);
                    }
                }
            } catch (Exception ex) {
                Echo("##########");
                Echo(ex.Message);
                Echo(ex.StackTrace);
                Echo("##########");
                throw ex;
            } finally {
                Runtime.UpdateFrequency = (_msgQueue.Count > 0)
                    ? UpdateFrequency.Update10
                    : UpdateFrequency.None;
            }
        }


        T GetBlockWithName<T>(string name) where T : class {
            GridTerminalSystem.GetBlocksOfType<T>(_tempBlocks, b => IsOnThisGrid(b) && (string.Compare(b.CustomName, name, true) == 0));
            return (_tempBlocks.Count > 0) ? (T)_tempBlocks[0] : null;
        }


        void LoadConfigSettings() {
            var hash = Me.CustomData.GetHashCode();
            if (hash == _configHash) return;
            _config.ReadFromCustomData(Me);
            _settings.LoadFromSettingDict(_config);
            _config.SaveToCustomData(Me);
            _configHash = hash;
            _log.MaxTextLinesToKeep = _settings.LogLines2Show;
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
