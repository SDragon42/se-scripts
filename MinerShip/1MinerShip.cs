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
        readonly TimeIntervalModule _dockSecureInterval;
        readonly TimeIntervalModule _proximityInterval;
        readonly DockSecureModule _dockSecure;
        readonly ProximityModule _proximity;
        readonly ScriptSettings _settings = new ScriptSettings();

        readonly List<IMyTerminalBlock> _tmp = new List<IMyTerminalBlock>();

        public Program() {
            //Echo = (t) => { }; // Disable Echo

            _runSymbol = new RunningSymbolModule();
            _dockSecureInterval = new TimeIntervalModule(0.1);
            _proximityInterval = new TimeIntervalModule(0.1);
            _dockSecure = new DockSecureModule();

            _proximity = new ProximityModule();

            _settings.InitConfig(Me, _dockSecure, _proximity, SetExecutionInterval);
        }

        public void Main(string argument) {
            Echo("Miner ship v0.1 " + _runSymbol.GetSymbol(Runtime));
            _dockSecureInterval.RecordTime(Runtime);
            _proximityInterval.RecordTime(Runtime);

            var keepRunning = false;
            keepRunning |= argument?.Length > 0;
            keepRunning |= _dockSecureInterval.AtNextInterval;
            keepRunning |= _proximityInterval.AtNextInterval;
            if (!keepRunning) return;

            _settings.LoadConfig(Me, _dockSecure, _proximity, SetExecutionInterval);

            _dockSecure.Init(this);

            if (argument?.Length > 0) {
                switch (argument.ToLower()) {
                    case CMD_DOCK: _dockSecure.Dock(); return;
                    case CMD_UNDOCK: _dockSecure.UnDock(); return;
                    case CMD_TOGGLE: _dockSecure.DockUndock(); return;
                    default: return;
                }
            }

            if (_dockSecureInterval.AtNextInterval) _dockSecure.AutoDockUndock();
            if (_proximityInterval.AtNextInterval) RunProximityCheck();
        }

        void SetExecutionInterval() {
            _dockSecureInterval.SetInterval(1.0 / _settings.DockSecureInterval);
            _proximityInterval.SetInterval(1.0 / _settings.ProximityInterval);
        }

        void RunProximityCheck() {
            var sc = GetShipControler();
            var display = GetProximityDisplay();
            if (sc == null || display == null) return;
            _proximity.RunScan(this, sc);
            display.Font = LCDFonts.NONOSPACE;
            display.FontSize = 1.7f;

            var txtUp = FormatRange2Text(_proximity.Up);
            var txtDown = FormatRange2Text(_proximity.Down);
            var txtLeft = FormatRange2Text(_proximity.Left);
            var txtRight = FormatRange2Text(_proximity.Right);
            var txtBack = FormatRange2Text(_proximity.Backward);

            display.WritePublicText($"Prox  {txtUp}\n {txtLeft}<{txtBack}>{txtRight}\n      {txtDown}");
            display.ShowPublicTextOnScreen();
        }
        string FormatRange2Text(double? range) {
            if (!range.HasValue) return "----";
            return $"{range,4:N1}";
        }

        IMyShipController GetShipControler() {
            GridTerminalSystem.GetBlocksOfType<IMyCockpit>(_tmp, IsOnThisGrid);
            if (_tmp.Count > 0) return _tmp[0] as IMyShipController;
            GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(_tmp, IsOnThisGrid);
            if (_tmp.Count > 0) return _tmp[0] as IMyShipController;
            return null;
        }
        IMyTextPanel GetProximityDisplay() {
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(_tmp, b => IsOnThisGrid(b) && b.CustomName.ToLower().Contains(_proximity.ProximityTag.ToLower()));
            if (_tmp.Count > 0) return _tmp[0] as IMyTextPanel;
            return null;
        }

        bool IsOnThisGrid(IMyTerminalBlock b) { return b.CubeGrid.EntityId == Me.CubeGrid.EntityId; }
    }
}
