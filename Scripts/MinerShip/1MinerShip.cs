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

        readonly RunningSymbol _runSymbol;
        readonly DockSecure _dockSecure;
        readonly Proximity _proximity;
        readonly ScriptSettings _settings = new ScriptSettings();
        readonly TimeInterval _clearRangeInterval;

        readonly List<IMyTerminalBlock> _tmp = new List<IMyTerminalBlock>();
        readonly List<IMyTextPanel> ProxDisplays = new List<IMyTextPanel>();
        readonly List<IMyTextPanel> ForeRangeDisplays = new List<IMyTextPanel>();
        readonly List<IMyShipDrill> Drills = new List<IMyShipDrill>();
        IMyShipController _sc = null;
        IMyCameraBlock _foreRangeCamera = null;

        RangeInfo _foreRangeInfo;

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _runSymbol = new RunningSymbol();
            _dockSecure = new DockSecure();
            _proximity = new Proximity();
            _settings.InitConfig(Me, _dockSecure, _proximity);
            _clearRangeInterval = new TimeInterval(_settings.ForwardDisplayClearTime);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo("Miner ship v1.1b " + _runSymbol.GetSymbol(Runtime));

            _settings.LoadConfig(Me, _dockSecure, _proximity);
            _dockSecure.Init(this);
            _clearRangeInterval.RecordTime(Runtime);
            LoadBlocks();

            if (argument.Length > 0) {
                switch (argument.ToLower()) {
                    case CMD_DOCK: _dockSecure.Dock(); break;
                    case CMD_UNDOCK: _dockSecure.UnDock(); break;
                    case CMD_TOGGLE: _dockSecure.DockUndock(); break;
                    case CMD_SAFETY: TurnOffDrills(); break;
                    case CMD_SCAN: ScanAhead(); break;
                }
            }

            if ((updateSource & UpdateType.Update10) > 0) {
                _dockSecure.AutoDockUndock();
                UpdateProximity();
            }

            if (_clearRangeInterval.AtNextInterval) {
                ForeRangeDisplays.ForEach(d => Write2ForeDisplay(d, ""));
            }
        }

        void UpdateProximity() {
            _proximity.RunScan(this, _sc);
            var text = BuildProximityDisplayText();
            ProxDisplays.ForEach(d => Write2ProximityDisplay(d, text));
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
        void Write2ProximityDisplay(IMyTextPanel display, string text) {
            display.Font = LCDFonts.MONOSPACE;
            display.FontSize = 1.7f;
            display.WritePublicText(text);
            display.ShowPublicTextOnScreen();
        }

        void ScanAhead() {
            Echo("ScanAhead()");
            if (_foreRangeCamera == null) return;
            _foreRangeInfo = Ranger.GetDetailedRange(_foreRangeCamera, _settings.ForwardScanRange);
            var text = BuildForwardDisplayText();
            Echo(text);
            ForeRangeDisplays.ForEach(d => Write2ForeDisplay(d, text));
            _clearRangeInterval.Reset();
        }
        string BuildForwardDisplayText() {
            return
                $"Entity: {_foreRangeInfo.DetectedEntity.Type}\n" +
                $"Name: {_foreRangeInfo.DetectedEntity.Name}\n" +
                $"Range: {_foreRangeInfo.Range:N1} m";
        }
        void Write2ForeDisplay(IMyTextPanel display, string text) {
            display.Font = LCDFonts.DEBUG;
            display.FontSize = 1.7f;
            display.WritePublicText(text);
            display.ShowPublicTextOnScreen();
        }

        void TurnOffDrills() {
            Drills.ForEach(b => b.Enabled = false);
        }




        void LoadBlocks() {
            _sc = GetShipControler();

            Echo($"Prox. Tag: {_proximity.Tag}");
            GridTerminalSystem.GetBlocksOfType(ProxDisplays,
                b => IsOnThisGrid(b)
                && _proximity.Tag?.Length > 0
                && b.CustomName.Contains(_proximity.Tag));
            Echo($"Prox. LCDs: {ProxDisplays.Count}");

            Echo($"Fore. Tag: {_settings.ForwardScanTag}");
            GridTerminalSystem.GetBlocksOfType(ForeRangeDisplays,
                b => IsOnThisGrid(b)
                && _settings.ForwardScanTag?.Length > 0
                && b.CustomName.Contains(_settings.ForwardScanTag));
            Echo($"Fore. LCDs: {ForeRangeDisplays.Count}");
            Echo($"Clear: {_settings.ForwardDisplayClearTime} s");

            _foreRangeCamera = GetForwardRangeCamera();
            Echo($"Fore. Camera: {(_foreRangeCamera != null ? 1 : 0)}");

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
        IMyCameraBlock GetForwardRangeCamera() {
            GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(_tmp,
                b => IsOnThisGrid(b)
                && _settings.ForwardScanTag?.Length > 0
                && b.CustomName.Contains(_settings.ForwardScanTag));
            if (_tmp.Count > 0) return _tmp[0] as IMyCameraBlock;
            return null;
        }


        bool IsOnThisGrid(IMyTerminalBlock b) { return b.CubeGrid == Me.CubeGrid; }
    }
}
