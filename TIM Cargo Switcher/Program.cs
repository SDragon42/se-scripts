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

        const string SCRIPT_NAME = "TIM Config Switcher v1.2";
        const string CMD_USE = "use";

        readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        readonly char[] SPLITTER = new char[] { ' ' };

        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();

        readonly MyIni Ini = new MyIni();
        readonly TimBlockConfigData ConfigStorage = new TimBlockConfigData();
        readonly TimBlockName ConfigApplied = new TimBlockName();

        Action<string> Debug = (text) => { };

        public Program() {
            Commands.Add(CMD_USE, CMD_SwitchTimConfig);
            Commands.Add("save", CMD_SaveTimConfig);

            ConfigStorage.Echo = Echo;
            ConfigApplied.Echo = Echo;

            ShowCommands();
            LoadConfig();
        }

        string targetTag;
        string configTag;

        public void Main(string argument, UpdateType updateSource) {
            ShowCommands();
            LoadConfig();

            targetTag = string.Empty;
            configTag = string.Empty;

            var argParts = argument.Split(SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            if (argParts.Length < 2) {
                Echo("Invalid Args >> " + argument);
                Echo("");
                Echo("Format expected:");
                Echo("<block tag> <config tag> [<command>]");
                return;
            }

            targetTag = "[" + Ini.Get(KEY_CargoSwitcherTag).ToString() + " " + argParts[0] + "]";
            configTag = "[" + argParts[1] + "]";

            var command = string.Empty;
            if (argParts.Length >= 3)
                command = argParts[2];
            else
                command = CMD_USE;

            Debug("targetTag = " + targetTag);
            Debug("configTag = " + configTag);
            Debug("command = " + command);

            GridTerminalSystem.GetBlocksOfType(blocks, b => Collect.IsTagged(b, targetTag));
            Echo($"Found: {blocks.Count:N0} block(s)");

            if (Commands.ContainsKey(command))
                Commands[command]?.Invoke();
            else
                Echo($"Command '{command}' not recognized");
        }

        void ShowCommands() {
            Echo(SCRIPT_NAME);
            Echo("");
            Echo("Commands:");
            foreach (var k in Commands.Keys)
                if (k != string.Empty) Echo(k);
        }


        void CMD_SwitchTimConfig() {
            Debug("CMD_SwitchTimConfig");
            foreach (var b in blocks) {
                var timConfig = string.Empty;
                if (!ConfigStorage.Get(b, configTag, out timConfig)) continue;
                ConfigApplied.Replace(b, timConfig.Trim());
            }
        }

        void CMD_SaveTimConfig() {
            Debug("CMD_SaveTimConfig");
            foreach (var b in blocks) {
                var timConfig = ConfigApplied.Get(b);
                ConfigStorage.Set(b, configTag, timConfig);
            }
        }

    }
}
