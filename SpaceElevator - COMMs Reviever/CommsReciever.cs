﻿using Sandbox.Game.EntityComponents;
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
    partial class Program : MyGridProgram
    {
        const string VERSION = "v1.0";

        readonly CustomDataConfigModule _custConfig = new CustomDataConfigModule();
        int _configHash = 0;
        const string KEY_ProgramBlockName = "Program Block";
        const string KEY_TimerBlockName = "Timer Block";
        const string KEY_LogLinesToShow = "Lines to Show";
        const string KEY_LogDisplayName = "Log LCD Name";

        LogModule _log;

        IMyTimerBlock _timer = null;
        IMyProgrammableBlock _targetProgram = null;
        IMyTextPanel _display = null;

        List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();

        readonly Queue<CommMessage> _messageQueue = new Queue<CommMessage>();

        public Program()
        {
            Echo = (t) => { }; // Disable Echo
            _log = new LogModule();

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

        public void Main(string argument)
        {
            try
            {
                Echo("COMMS Reciever " + VERSION);
                ReloadConfig();

                _targetProgram = GetBlockWithName<IMyProgrammableBlock>(_custConfig.GetString(KEY_ProgramBlockName));
                _timer = GetBlockWithName<IMyTimerBlock>(_custConfig.GetString(KEY_TimerBlockName));
                _display = GetBlockWithName<IMyTextPanel>(_custConfig.GetString(KEY_LogDisplayName));

                if (_targetProgram == null) Echo("No target Program Block found.");
                if (_timer == null) Echo("No timer block found.");
                if (_display == null) Echo("No Log Display.");
                if (_targetProgram == null || _timer == null) return;

                ProcessArgument(argument);
                ProcessQueue();

                var logText = _log.GetLogText();
                Echo(logText);

                if (_display != null)
                {
                    _display.SetValue("FontSize", 0.6f); //for large grid
                    _display.SetValue<long>("Font", 1147350002);
                    _display.ShowTextureOnScreen();
                    _display.ShowPublicTextOnScreen();
                    _display.WritePublicText(logText);
                }
            }
            catch (Exception ex)
            {
                Echo(ex.Message);
                Echo(ex.StackTrace);
                throw ex;
            }
        }

        T GetBlockWithName<T>(string blockName) where T : class
        {
            GridTerminalSystem.GetBlocksOfType<T>(_blocks, b => {
                if (!IsOnSameGrid(b)) return false;
                return (string.Compare(b.CustomName, blockName, true) == 0);
            });

            if (_blocks.Count > 0)
                return _blocks[0] as T;
            return default(T);
        }



        //-------------------------------------------------------------------------------
        void ReloadConfig()
        {
            if (_configHash == Me.CustomData.GetHashCode())
                return;
            _custConfig.ReadFromCustomData(Me);
            _custConfig.SaveToCustomData(Me);
            _configHash = Me.CustomData.GetHashCode();
            _log.SetMaxTextLinesToKeep(_custConfig.GetInt(KEY_LogLinesToShow, 10));
        }



        //-------------------------------------------------------------------------------
        void ProcessArgument(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument)) return;
            CommMessage msg = null;
            var text = DateTime.Now.ToLongTimeString() + " | ";
            try
            {
                if (!CommMessage.TryParse(argument, out msg))
                {
                    text += "Malformed Msg";
                    return;
                }
                if (!msg.IsValid())
                {
                    text += "Invalid Msg";
                    return;
                }
                text += msg.GetSenderGridName() + " | " + msg.GetPayloadType();
                _messageQueue.Enqueue(msg);
            }
            finally
            {
                _log.AppendLine(text);
            }
        }



        //-------------------------------------------------------------------------------
        void ProcessQueue()
        {
            if (_messageQueue.Count <= 0)
            {
                _log.AppendLine("No MSGs");
                return;
            }

            var msg = _messageQueue.Dequeue();
            if (msg == null)
            {
                _log.AppendLine("Null Msg from queue");
                return;
            }
            if (Me.CubeGrid.EntityId == msg.GetSenderGridEntityId())
            {
                _log.AppendLine("Discarded - Sent by me");
                return;
            }

            if (msg.GetTargetGridName().Length > 0)
            {
                if (string.Compare(msg.GetTargetGridName(), Me.CubeGrid.CustomName, true) != 0)
                {
                    _log.AppendLine("Discarded - Not for me");
                    return;
                }
            }

            var arg = msg.ToString();
            if (!_targetProgram.TryRun(arg))
            {
                _messageQueue.Enqueue(msg);
                _log.AppendLine("Re-Queued");
            }
            _log.AppendLine("Sent to controller");

            if (_messageQueue.Count > 0) _timer.Trigger();
        }


        public bool IsOnSameGrid(IMyTerminalBlock b) { return (b != null) ? (Me.CubeGrid == b.CubeGrid) : false; }

    }
}