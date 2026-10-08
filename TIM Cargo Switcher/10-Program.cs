using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {

        readonly List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
        readonly char[] SPLITTER = new char[] { ' ' };

        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        string _instructions;

        readonly TimBlockConfigData _configStorage = new TimBlockConfigData();
        readonly TimBlockName _configApplied = new TimBlockName();
        readonly Config _config = new Config();
        // Action<string> Debug = (text) => { };

        string _targetTag;
        string _configTag;

        public Program() {
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load(_configApplied);

            _commands.Add("use", CMD_SwitchTimConfig);
            _commands.Add("save", CMD_SaveTimConfig);
            // ConfigStorage.Echo = Echo;
            // ConfigApplied.Echo = Echo;

            _instructions = "Commands:\n" + string.Join("\n", _commands.Keys);
            ShowCommands();
        }

        public void Main(string argument, UpdateType updateSource) {
            ShowCommands();
            _config.Load(_configApplied);

            _targetTag = string.Empty;
            _configTag = string.Empty;

            var argParts = argument.Split(SPLITTER, 3, StringSplitOptions.RemoveEmptyEntries);
            if (argParts.Length < 3) {
                Echo("Invalid Args >> " + argument);
                Echo("");
                Echo("Format expected:");
                Echo("<block tag> <command> <config tag>");
                return;
            }

            _targetTag = "[" + _config.CargoSwitcherTag + ":" + argParts[0] + "]";
            _configTag = "[" + argParts[2].Trim() + "]";
            var command = argParts[1];
            // Debug("targetTag = " + targetTag.Replace("[","").Replace("]",""));
            // Debug("configTag = " + configTag.Replace("[","").Replace("]",""));
            // Debug("command = " + command);

            GridTerminalSystem.GetBlocksOfType(_blocks, b => IsTagged(b, _targetTag));
            Echo($"Found: {_blocks.Count:N0} block(s)");

            if (_commands.ContainsKey(command))
                _commands[command]?.Invoke();
            else
                Echo($"Command '{command}' not recognized");
        }

        void ShowCommands() {
            Echo("TIM Config Switcher v$VERSION$");
            Echo("");
            Echo(_instructions);
        }


        void CMD_SwitchTimConfig() {
            // Debug("CMD_SwitchTimConfig");
            foreach (var b in _blocks) {
                var timConfig = string.Empty;
                if (!_configStorage.Get(b, _configTag, out timConfig)) continue;
                _configApplied.Replace(b, timConfig.Trim());
            }
        }

        void CMD_SaveTimConfig() {
            // Debug("CMD_SaveTimConfig");
            foreach (var b in _blocks) {
                var timConfig = _configApplied.Get(b);
                _configStorage.Set(b, _configTag, timConfig);
            }
        }

    }
}
