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
        const string CMD_SAFETY = "safety-cutoff";
        const string CMD_SCAN = "scan-range";

        readonly RunningSymbolModule _runSymbol;
        readonly DockSecureModule _dockSecure;
        readonly ProximityModule _proximity;
        readonly ProximityModule _forwardRange;
        readonly ScriptSettings _settings = new ScriptSettings();

        readonly List<IMyTerminalBlock> _tmp = new List<IMyTerminalBlock>();
        readonly List<IMyTextPanel> _proximityDisplays = new List<IMyTextPanel>();
        readonly List<IMyShipDrill> _drills = new List<IMyShipDrill>();
        IMyShipController _sc = null;

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _runSymbol = new RunningSymbolModule();
            _dockSecure = new DockSecureModule();
            _proximity = new ProximityModule();
            _forwardRange = new ProximityModule();
            _settings.InitConfig(Me, _dockSecure, _proximity, _forwardRange);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo("Miner ship v1.1a " + _runSymbol.GetSymbol(Runtime));

            if (argument?.Length == 0 && (updateSource & UpdateType.Trigger) > 0) return;

            _settings.LoadConfig(Me, _dockSecure, _proximity, _forwardRange);
            _dockSecure.Init(this);
            LoadBlocks();

            if (argument?.Length > 0) {
                switch (argument.ToLower()) {
                    case CMD_DOCK: _dockSecure.Dock(); break;
                    case CMD_UNDOCK: _dockSecure.UnDock(); break;
                    case CMD_TOGGLE: _dockSecure.DockUndock(); break;
                    case CMD_SAFETY: TurnOffDrills(); break;
                    case CMD_SCAN: break;
                }
                return;
            }

            if ((updateSource & UpdateType.Update10) > 0) {
                _dockSecure.AutoDockUndock();

                _proximity.RunScan(this, _sc);
                var text = BuildProximityDisplayText();
                WriteProximityDisplay(text);
            }
        }

        string BuildProximityDisplayText() {
            var txtUp = FormatRange2Text(_proximity.Up);
            var txtDown = FormatRange2Text(_proximity.Down);
            var txtLeft = FormatRange2Text(_proximity.Left);
            var txtRight = FormatRange2Text(_proximity.Right);
            var txtBack = FormatRange2Text(_proximity.Backward);
            return $"Prox  {txtUp}\n {txtLeft}<{txtBack}>{txtRight}\n      {txtDown}";
        }
        string FormatRange2Text(double? range) {
            if (!range.HasValue) return "----";
            return $"{range,4:N1}";
        }
        void WriteProximityDisplay(string text) {
            foreach (var display in _proximityDisplays) {
                display.Font = LCDFonts.MONOSPACE;
                display.FontSize = 1.7f;
                display.WritePublicText(text);
                display.ShowPublicTextOnScreen();
            }
        }

        void TurnOffDrills() {
            _drills.ForEach(b => b.Enabled = false);
        }


        void LoadBlocks() {
            _sc = GetShipControler();

            GridTerminalSystem.GetBlocksOfType(_proximityDisplays,
                b => IsOnThisGrid(b)
                && _proximity.ProximityTag?.Length > 0
                && b.CustomName.ToLower().Contains(_proximity.ProximityTag.ToLower()));

            GridTerminalSystem.GetBlocksOfType(_drills, IsOnThisGrid);
        }
        IMyShipController GetShipControler() {
            GridTerminalSystem.GetBlocksOfType<IMyCockpit>(_tmp, IsOnThisGrid);
            if (_tmp.Count > 0) return _tmp[0] as IMyShipController;
            GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(_tmp, IsOnThisGrid);
            if (_tmp.Count > 0) return _tmp[0] as IMyShipController;
            return null;
        }


        bool IsOnThisGrid(IMyTerminalBlock b) { return b.CubeGrid == Me.CubeGrid; }
    }
}
