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

        readonly RunningSymbolModule _runSymbol;
        readonly TimeIntervalModule _executionInterval;
        readonly DockSecureModule _dockSecure;
        readonly ScriptSettings _settings = new ScriptSettings();

        public Program() {
            //Echo = (t) => { }; // Disable Echo

            _runSymbol = new RunningSymbolModule();
            _executionInterval = new TimeIntervalModule(0.1);
            _dockSecure = new DockSecureModule();

            _settings.InitConfig(Me, _dockSecure, SetExecutionInterval);
        }

        public void Main(string argument) {
            Echo("Dock-Secure v1.3.1 " + _runSymbol.GetSymbol(Runtime));
            _executionInterval.RecordTime(Runtime);

            var keepRunning = false;
            keepRunning |= argument?.Length > 0;
            keepRunning |= _executionInterval.AtNextInterval;
            if (!keepRunning) return;

            _settings.LoadConfig(Me, _dockSecure, SetExecutionInterval);

            _dockSecure.Init(this);

            if (argument?.Length > 0) {
                switch (argument.ToLower()) {
                    case CMD_DOCK: _dockSecure.Dock(); return;
                    case CMD_UNDOCK: _dockSecure.UnDock(); return;
                    case CMD_TOGGLE: _dockSecure.DockUndock(); return;
                    default: return;
                }
            }

            _dockSecure.AutoDockUndock();
        }

        void SetExecutionInterval() {
            _executionInterval.SetInterval(1.0 / _settings.RunInterval);
        }
    }
}
