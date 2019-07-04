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
        readonly List<IMyTerminalBlock> _tmp = new List<IMyTerminalBlock>();

        readonly IDictionary<string, Action<string[]>> Commands = new Dictionary<string, Action<string[]>>();
        readonly string Instructions;

        Action<string> Debug = (s) => { };

        public Program() {
            //Debug = Echo;

            Commands.Add("basic", Cmd_BasicRename);
            Commands.Add("test", Cmd_TestMethod);
            Commands.Add("undefined", Cmd_GetUndefinedTypeNames);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            Echo(Instructions);
        }

        public void Main(string argument) {
            Echo(Instructions);
            Echo("");
            Echo($"CMD: {argument}");

            argument = argument.ToLower();
            var parts = argument.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var cmd = parts[0];
            var options = parts.Skip(1).ToArray();
            if (Commands.ContainsKey(cmd)) {
                Commands[cmd]?.Invoke(options);
            } else {
                Echo("** not recognized **");
            }
        }


        void Cmd_TestMethod(string[] options) {
            GridTerminalSystem.GetBlocks(_tmp);
            _tmp.ForEach(b => {
                Echo(b.CustomName);
                Echo(b.BlockDefinition.SubtypeId);
                Echo("");
            });
        }

        void Cmd_GetUndefinedTypeNames(string[] options) {
            Debug("GetUndefinedTypeNames()");
            GridTerminalSystem.GetBlocks(_tmp);

            var nameSource = new BasicBlockNames();
            var blockTypeKeys = _tmp
                .Select(BlockHelper.GetKey)
                .Distinct()
                .OrderBy(b => b);

            var sb = new StringBuilder();
            Action<string, List<string>> Build = (title, values) => {
                if (values.Count > 0) {
                    sb.AppendLine(title);
                    values.ForEach(t => sb.AppendLine(t));
                    sb.AppendLine();
                }
            };

            var undefinedKeys = blockTypeKeys
                .Where(key => !nameSource.HasTypeKey(key))
                .ToList();
            Build("== Undefined ==", undefinedKeys);

            var unnamedKeys = blockTypeKeys
                .Where(nameSource.HasTypeKey)
                .Where(k => nameSource.GetName(k).Length == 0)
                .ToList();
            Build("== Unnamed ==", unnamedKeys);

            Me.CustomData = sb.ToString();
        }

        void Cmd_BasicRename(string[] options) {
            Debug("BasicRename()");
            GridTerminalSystem.GetBlocks(_tmp);
            _tmp.ForEach(SetDefaults);

            var nameSource = new BasicBlockNames();

            GridTerminalSystem.GetBlocksOfType(_tmp, b => nameSource.GetName(b).Length > 0);
            Echo($"Num Blocks: {_tmp.Count:N0}");

            var addNumbers = options.Contains("num");

            if (!addNumbers) {
                _tmp.ForEach(b => b.CustomName = nameSource.GetName(b));
                return;
            }

            var keyedBlocks = _tmp
                .Select(b => new { block = b, key = BlockHelper.GetKey(b) })
                .ToList();

            var uniqueKeys = keyedBlocks
                .Select(b => b.key)
                .Distinct()
                .ToList();
            foreach (var key in uniqueKeys) {
                var targetBlocks = keyedBlocks
                    .Where(b => b.key == key)
                    .ToList();
                var totalCount = targetBlocks.Count();
                var numFormat = string.Empty.PadRight(totalCount.ToString().Length, '0');

                Action<IMyTerminalBlock, string, int, string> RenameMethod;
                if (totalCount > 1)
                    RenameMethod = RenameBlockNumberAll;
                else
                    RenameMethod = RenameBlock;

                var num = 0;
                foreach (var item in targetBlocks) {
                    num++;
                    RenameMethod(item.block, nameSource.GetName(item.key), num, numFormat);
                }
            }
        }

        void RenameBlockNumberAll(IMyTerminalBlock b, string name, int number, string numberFormat) {
            b.CustomName = name + " " + number.ToString(numberFormat);
        }
        void RenameBlockNumber2plus(IMyTerminalBlock b, string name, int number, string numberFormat) {
            b.CustomName = (number > 1)
                ? name + " " + number.ToString(numberFormat)
                : name;
        }
        void RenameBlock(IMyTerminalBlock b, string name, int number, string numberFormat) {
            b.CustomName = name;
        }



        void SetNoTerminalNoToolbar(IMyTerminalBlock b) {
            b.ShowInTerminal = false;
            b.ShowInToolbarConfig = false;
        }

        void SetDefaults(IMyTerminalBlock b) {
            b.ShowInTerminal = true;
            b.ShowInToolbarConfig = true;
            b.ShowOnHUD = false;
        }


        void SetInteriorLight(IMyTerminalBlock b) {
            var light = b as IMyInteriorLight;
            if (light == null) return;
            light.Color = new Color(100, 100, 100);
            light.Radius = light.GetMaximum<float>("Radius");
            //light.Intensity = 
            //light.Falloff = 
            light.BlinkIntervalSeconds = 0f;
            light.BlinkLength = 0.1f;
            light.BlinkOffset = 0f;
        }

        bool IsNavLight(IMyTerminalBlock b) {
            if (!(b is IMyInteriorLight)) return false;
            return (b.CustomName.ToLower().Contains("nav light"));
        }
        bool IsNavLightPort(IMyTerminalBlock b) {
            if (!IsNavLight(b)) return false;
            return (b.CustomName.ToLower().Contains("- port"));
        }
        bool IsNavLightStarboard(IMyTerminalBlock b) {
            if (!IsNavLight(b)) return false;
            return (b.CustomName.ToLower().Contains("- starboard"));
        }
        bool IsNavLightTop(IMyTerminalBlock b) {
            if (!IsNavLight(b)) return false;
            return (b.CustomName.ToLower().Contains("- top"));
        }
        bool IsNavLightBottom(IMyTerminalBlock b) {
            if (!IsNavLight(b)) return false;
            return (b.CustomName.ToLower().Contains("- bottom"));
        }

        void SetNavLightPort(IMyTerminalBlock b) {
            var light = b as IMyInteriorLight;
            if (light == null) return;
            SetNavLight_Core(light);
            light.Color = new Color(150, 0, 0);
            light.BlinkIntervalSeconds = 1.5f;
            light.BlinkLength = 75f;
        }
        void SetNavLightStarboard(IMyTerminalBlock b) {
            SetNavLightPort(b);
            var light = b as IMyInteriorLight;
            if (light == null) return;
            light.Color = new Color(0, 150, 0);
        }
        void SetNavLightTop(IMyTerminalBlock b) {
            var light = b as IMyInteriorLight;
            if (light == null) return;
            SetNavLight_Core(light);
            light.Color = new Color(150, 150, 150);
            light.BlinkIntervalSeconds = 1f;
            light.BlinkLength = 10f;
        }
        void SetNavLightBottom(IMyTerminalBlock b) {
            SetNavLightTop(b);
            var light = b as IMyInteriorLight;
            if (light == null) return;
            light.Color = new Color(150, 150, 0);
        }
        void SetNavLight_Core(IMyInteriorLight light) {
            light.Intensity = light.GetMaximum<float>("Intensity");
            light.BlinkOffset = 0f;
            if (light.BlockDefinition.SubtypeId.StartsWith("LargeBlock")) {

            } else {
                light.Radius = 2f;
                light.Intensity = 1.5f;
            }
        }

        void SetSpotlight(IMyTerminalBlock b) {
            var light = b as IMyReflectorLight;
            if (light == null) return;
            light.Color = new Color(100, 100, 100);
            light.Radius = light.GetMaximum<float>("Radius");
        }



    }
}
