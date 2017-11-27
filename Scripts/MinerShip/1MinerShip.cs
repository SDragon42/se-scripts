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
        const string CMD_SAFETY = "safety-cutoff";
        const string CMD_SCAN = "scan-range";

        readonly RunningSymbol _runSymbol;
        readonly DockSecure _dockSecure;
        readonly Proximity _proximity;
        readonly ScriptSettings _settings = new ScriptSettings();

        readonly List<IMyTerminalBlock> _tmp = new List<IMyTerminalBlock>();
        readonly List<IMyTextPanel> Displays = new List<IMyTextPanel>();
        readonly List<IMyShipDrill> Drills = new List<IMyShipDrill>();
        IMyShipController _sc = null;

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _runSymbol = new RunningSymbol();
            _dockSecure = new DockSecure();
            _proximity = new Proximity();
            _settings.InitConfig(Me, _dockSecure, _proximity);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo("Miner ship v1.1b " + _runSymbol.GetSymbol(Runtime));

            if (argument.Length == 0 && (updateSource & UpdateType.Trigger) > 0) {
                Echo("Execution via Timer block is no longer needed.");
                return;
            }

            _settings.LoadConfig(Me, _dockSecure, _proximity);
            _dockSecure.Init(this);
            LoadBlocks();

            if (argument.Length > 0) {
                switch (argument.ToLower()) {
                    case CMD_DOCK: _dockSecure.Dock(); break;
                    case CMD_UNDOCK: _dockSecure.UnDock(); break;
                    case CMD_TOGGLE: _dockSecure.DockUndock(); break;
                    case CMD_SAFETY: TurnOffDrills(); break;
                    case CMD_SCAN: ScanAhead(); break;
                }
                return;
            }

            if ((updateSource & UpdateType.Update10) > 0) {
                _dockSecure.AutoDockUndock();

                _proximity.RunScan(this, _sc);
                var text = BuildProximityDisplayText();
                Displays.ForEach(d => Write2Display(d, text));
            }
        }

        string BuildProximityDisplayText() {
            var txtUp = GetFormattedRange(Direction.Up);
            var txtDown = GetFormattedRange(Direction.Down);
            var txtLeft = GetFormattedRange(Direction.Left);
            var txtRight = GetFormattedRange(Direction.Right);
            var txtBack = GetFormattedRange(Direction.Backward);
            return $"Prox  {txtUp}\n {txtLeft}<{txtBack}>{txtRight}\n      {txtDown}";
        }
        string GetFormattedRange(Direction dir) {
            var range = _proximity.GetRange(dir);
            if (!range.HasValue) return "----";
            return (range.Value < 100.0)
                ? $"{range,4:N1}"
                : $"{range,4:N0}";
        }
        void Write2Display(IMyTextPanel display, string text) {
            display.Font = LCDFonts.MONOSPACE;
            display.FontSize = 1.7f;
            display.WritePublicText(text);
            display.ShowPublicTextOnScreen();
        }

        void TurnOffDrills() {
            Drills.ForEach(b => b.Enabled = false);
        }

        void ScanAhead() {
        }


        void LoadBlocks() {
            _sc = GetShipControler();

            Echo($"Prox. Tag: {_proximity.ProximityTag}");
            GridTerminalSystem.GetBlocksOfType(Displays,
                b => IsOnThisGrid(b)
                && _proximity.ProximityTag?.Length > 0
                && b.CustomName.ToLower().Contains(_proximity.ProximityTag));
            Echo($"Prox. LCDs: {Displays.Count}");

            GridTerminalSystem.GetBlocksOfType(Drills, IsOnThisGrid);
            Echo($"Drills: {Drills.Count}");
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
