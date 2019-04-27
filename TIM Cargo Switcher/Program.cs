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

        const string TAG_BASE = "timcs:";
        const string SCRIPT_NAME = "TIM Config Switcher v1.1";
        const string TIM_TAG = "TIM";
        const string CMD_CHANGE_CONFIG = "apply";

        readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        readonly char[] SPLITTER = new char[] { ' ' };

        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();

        readonly ConfigData ConfigStorage = new ConfigData();
        readonly TimBlockConfig ConfigApplied = new TimBlockConfig();

        Action<string> Debug = (text) => { };

        public Program() {
            Echo(SCRIPT_NAME);
            Commands.Add(CMD_CHANGE_CONFIG, CMD_SwitchTimConfig);
            Commands.Add("save", CMD_SaveTimConfig);

            Debug = Echo;
            ConfigStorage.Debug = Debug;
            ConfigApplied.Debug = Debug;
        }

        string targetTag;
        string configTag;

        public void Main(string argument, UpdateType updateSource) {
            Echo(SCRIPT_NAME);

            targetTag = string.Empty;
            configTag = string.Empty;

            var argParts = argument.Split(SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            if (argParts.Length < 2) {
                Echo("Invalid Args >> " + argument);
                Echo("");
                Echo("Format expected:");
                Echo("<block tag> <config data tag>");
                return;
            }

            targetTag = "[" + TAG_BASE + argParts[0] + "]";
            configTag = "[" + argParts[1] + "]";

            var command = string.Empty;
            if (argParts.Length >= 3)
                command = argParts[2];
            else
                command = CMD_CHANGE_CONFIG;

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



        class ConfigData {
            public Action<string> Debug = (text) => { };

            bool GetIndexes(IMyTerminalBlock b, string key, out int start, out int end) {
                start = b.CustomData.IndexOf(key);
                if (start < 0) {
                    start = -1;
                    end = -1;
                    return false;
                }

                start += key.Length;
                end = b.CustomData.IndexOf('[', start) - 1;
                if (end < 0) end = b.CustomData.Length - 1;
                return true;
            }
            public bool Get(IMyTerminalBlock b, string key, out string timConfig) {
                Debug("ConfigData.Get()");
                timConfig = string.Empty;

                int start, end;
                if (!GetIndexes(b, key, out start, out end)) return false;

                timConfig = b.CustomData
                    .Substring(start, end - start + 1)
                    .Replace('\n', ' ')
                    .Trim();
                return true;
            }
            public void Set(IMyTerminalBlock b, string key, string timConfig) {
                Debug("ConfigData.Set()");
                Debug(">> " + timConfig);
                int start, end;
                if (!GetIndexes(b, key, out start, out end)) {
                    b.CustomData += $"\n\n{key}";
                    if (!GetIndexes(b, key, out start, out end)) return;
                }

                var data = b.CustomData.Remove(start, end - start + 1);
                timConfig = timConfig.Replace(' ', '\n') + "\n\n";
                b.CustomData = data.Insert(start, timConfig);
            }
        }

        class TimBlockConfig {
            public Action<string> Debug = (text) => { };

            public string Get(IMyTerminalBlock b) {
                Debug("TimBlockConfig.Get()");
                var start = b.CustomName.IndexOf("[" + TIM_TAG);
                if (start < 0) return string.Empty;

                start += TIM_TAG.Length + 1;
                var end = b.CustomName.IndexOf(']', start);

                var timConfig = b.CustomName.Substring(start, end - start);
                Debug(">> " + timConfig);
                return timConfig;
            }

            public void Replace(IMyTerminalBlock b, string timConfig) {
                Debug("TimBlockConfig.Replace()");
                Remove(b);
                b.CustomName = $"{b.CustomName} [{TIM_TAG} {timConfig}]";
            }

            void Remove(IMyTerminalBlock b) {
                var start = b.CustomName.IndexOf("[" + TIM_TAG);
                if (start < 0) return;
                var end = b.CustomName.IndexOf(']', start);

                b.CustomName = b.CustomName.Remove(start, end - start + 1).Trim();
            }
        }

    }
}
