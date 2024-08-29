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
        const double BLOCK_RELOAD_TIME = 10;

        readonly RunningSymbol _runSymbol = new RunningSymbol();
        readonly DockSecure _dockSecure = new DockSecure();
        readonly ScriptSettings _settings = new ScriptSettings();

        double _timeLastBlockLoad = BLOCK_RELOAD_TIME * 2;

        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();
        readonly string Instructions;

        public Program() {
            Commands.Add("dock", _dockSecure.Dock);
            Commands.Add("undock", _dockSecure.UnDock);
            Commands.Add("toggle-dock", _dockSecure.ToggleDock);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            _settings.InitConfig(Me);

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource) {
            if (argument != string.Empty) argument = argument.ToLower();
            _timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - _timeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);

            Echo("Dock-Secure v1.3.4 " + _runSymbol.GetSymbol(Runtime));
            Echo($"Scanning for blocks in {timeTilUpdate:N0} seconds.");
            Echo("");
            Echo("Configure script in 'Custom Data'");
            Echo(Instructions);

            if (argument.Length == 0 && (updateSource & UpdateType.Trigger) > 0) {
                Echo("Execution via Timer block is no longer needed.");
                return;
            }

            _settings.LoadConfig(Me, _dockSecure);

            var reloadBlocks = (_timeLastBlockLoad >= BLOCK_RELOAD_TIME);
            _dockSecure.Init(this, reloadBlocks);
            if (reloadBlocks) {
                _timeLastBlockLoad = 0;
            }

            if (Commands.ContainsKey(argument))
                Commands[argument]?.Invoke();

            _dockSecure.AutoToggleDock();
        }

    }
}
