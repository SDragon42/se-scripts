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

namespace IngameScript {
    partial class Program : MyGridProgram {
        const string CMD_DOCK = "dock";
        const string CMD_UNDOCK = "undock";
        const string CMD_TOGGLE = "toggle-dock";

        readonly RunningSymbolModule _runSymbol;
        readonly DockSecureModule _dockSecure;
        readonly ScriptSettings _settings = new ScriptSettings();

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _runSymbol = new RunningSymbolModule();
            _dockSecure = new DockSecureModule();
            _settings.InitConfig(Me, _dockSecure);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType ut) {
            Echo("Dock-Secure v1.3.2 " + _runSymbol.GetSymbol(Runtime));

            if (argument?.Length == 0 && (ut & UpdateType.Trigger) > 0) return;

            _settings.LoadConfig(Me, _dockSecure);
            _dockSecure.Init(this);

            if (argument?.Length > 0) {
                switch (argument.ToLower()) {
                    case CMD_DOCK: _dockSecure.Dock(); break;
                    case CMD_UNDOCK: _dockSecure.UnDock(); break;
                    case CMD_TOGGLE: _dockSecure.DockUndock(); break;
                }
                return;
            }

            if ((ut & UpdateType.Update10) > 0) {
                _dockSecure.AutoDockUndock();
            }

        }

    }
}
