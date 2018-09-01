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
        const string CMD_DOCK = "dock";
        const string CMD_UNDOCK = "undock";
        const string CMD_TOGGLE = "toggle-dock";

        const double BLOCK_RELOAD_TIME = 10;

        readonly RunningSymbol _runSymbol = new RunningSymbol();
        readonly DockSecure _dockSecure = new DockSecure();
        readonly ScriptSettings _settings = new ScriptSettings();

        double _timeLastBlockLoad = BLOCK_RELOAD_TIME * 2;

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _settings.InitConfig(Me);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource) {
            _timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - _timeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);
            Echo("Dock-Secure v1.3.4 " + _runSymbol.GetSymbol(Runtime));
            Echo($"Scanning for blocks in {timeTilUpdate:N0} seconds.");
            Echo("");
            Echo("Configure script in 'Custom Data'");

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

            if (argument.Length > 0) {
                switch (argument.ToLower()) {
                    case CMD_DOCK: _dockSecure.Dock(); break;
                    case CMD_UNDOCK: _dockSecure.UnDock(); break;
                    case CMD_TOGGLE: _dockSecure.ToggleDock(); break;
                }
                return;
            }

            if ((updateSource & UpdateType.Update10) > 0) {
                _dockSecure.AutoToggleDock();
            }

        }

    }
}
