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
        const string CMD_DOCK_TOGGLE = "dock-toggle";
        const string CMD_DOCK = "dock";
        const string CMD_UNDOCK = "undock";
        const string CMD_SCAN = "scan-range";
        const string CMD_TOOLS_TOGGLE = "tools-toggle";
        const string CMD_TOOLS_OFF = "tools-off";

        const double BLOCK_RELOAD_TIME = 10;

        readonly ScriptSettings _settings = new ScriptSettings();
        readonly RunningSymbol _running = new RunningSymbol();
        readonly DockSecure _dockSecure = new DockSecure();
        readonly Proximity _proximity = new Proximity();
        readonly CustomDataConfig2 _proxConfig = new CustomDataConfig2("Proximity");

        readonly List<IMyTerminalBlock> _tmpList = new List<IMyTerminalBlock>();
        readonly List<ProxCamera> _proxCameraList = new List<ProxCamera>();
        readonly List<IMySoundBlock> _proxSpeakerList = new List<IMySoundBlock>();
        readonly List<IMyFunctionalBlock> _toolList = new List<IMyFunctionalBlock>();
        readonly List<IMyTextPanel> _displayList = new List<IMyTextPanel>();

        IMyShipController _sc = null;
        IMyCameraBlock _foreRangeCamera = null;
        bool _alertSounding = false;
        RangeInfo _foreRangeInfo;

        double _timeLastBlockLoad = BLOCK_RELOAD_TIME * 2;
        double _timeLastCleared = 0;
        bool _reloadBlocks;
        string _proximityText = string.Empty;
        string _scanRangeText = string.Empty;

        public Program() {
            //Echo = (t) => { }; // Disable Echo
            _settings.InitConfig(Me);
            _proximity.ScanRange = _settings.ProximityScanRange;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            _proxConfig.AddKey("Range Offset", "0.0");
        }

        public void Main(string argument, UpdateType updateSource) {
            _timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            _timeLastCleared += Runtime.TimeSinceLastRun.TotalSeconds;
            var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - _timeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);
            Echo("Utility Ship Systems v1.4.1 " + _running.GetSymbol(Runtime));
            Echo($"Scanning for blocks in {timeTilUpdate:N0} seconds.");
            Echo("");
            Echo("Configure script in 'Custom Data'");

            _settings.LoadConfig(Me, _dockSecure);
            _proximity.ScanRange = _settings.ProximityScanRange;

            _reloadBlocks = (_timeLastBlockLoad >= BLOCK_RELOAD_TIME);
            _dockSecure.Init(this, _reloadBlocks);
            if (_reloadBlocks) {
                LoadBlocks();
                _timeLastBlockLoad = 0;
            }

            if (argument.Length > 0) {
                switch (argument.ToLower()) {
                    case CMD_DOCK: _dockSecure.Dock(); break;
                    case CMD_UNDOCK: _dockSecure.UnDock(); break;
                    case CMD_DOCK_TOGGLE: _dockSecure.ToggleDock(); break;
                    case CMD_TOOLS_OFF: TurnOffTools(); break;
                    case CMD_SCAN: ScanAhead(); break;
                    case CMD_TOOLS_TOGGLE: ToggleToolsOnOff(); break;
                }
            }

            if ((updateSource & UpdateType.Update10) > 0) {
                _dockSecure.AutoToggleDock();
                UpdateProximity();
            }

            if (_timeLastCleared >= _settings.ForwardDisplayClearTime && _scanRangeText.Length > 0) {
                _scanRangeText = string.Empty;
                _timeLastCleared = 0;
            }
            foreach (var d in _displayList) {
                var isRange = IsForwardRangeBlock(d);
                var isProx = IsProximityBlock(d);

                if (isRange && (!isProx || _scanRangeText.Length > 0)) {
                    Write2ForeDisplay(d, _scanRangeText);
                    continue;
                }
                if (isProx) {
                    Write2ProximityDisplay(d, _proximityText);
                }
            }
        }

        void UpdateProximity() {
            if (!_dockSecure.IsDocked) {
                _proximity.RunScan(this, _sc, _proxCameraList);
                CheckAlert();
                _proximityText = BuildProximityDisplayText();
            } else {
                _proximityText = $"\n     Docked";
                TurnOffProximityAlert();
            }
        }
        void CheckAlert() {
            var speed = _sc.GetShipSpeed();
            var alertValid = false;
            alertValid |= SetAlert(Direction.Up, speed);
            if (alertValid) return;
            alertValid |= SetAlert(Direction.Down, speed);
            if (alertValid) return;
            alertValid |= SetAlert(Direction.Left, speed);
            if (alertValid) return;
            alertValid |= SetAlert(Direction.Right, speed);
            if (alertValid) return;
            alertValid |= SetAlert(Direction.Backward, speed);
            if (alertValid) return;
            TurnOffProximityAlert();
        }
        bool SetAlert(Direction dir, double speed) {
            var range = _proximity.GetRange(dir);
            var diff = _proximity.GetRangeDiff(dir);
            if (diff < 0 && speed >= _settings.ProximityAlertSpeed && range <= _settings.ProximityAlertRange) {
                TurnOnProximityAlert();
                return true;
            }
            return false;
        }
        void TurnOnProximityAlert() {
            if (_alertSounding)
                return;
            if (!_settings.ProximityAlert)
                return;
            _proxSpeakerList.ForEach(s => s.Play());
            _alertSounding = true;
        }
        void TurnOffProximityAlert() {
            if (_alertSounding)
                _proxSpeakerList.ForEach(s => s.Stop());
            _alertSounding = false;
        }
        string BuildProximityDisplayText() {
            var txtUp = GetFormattedRange(Direction.Up);
            var txtDown = GetFormattedRange(Direction.Down);
            var txtLeft = GetFormattedRange(Direction.Left);
            var txtRight = GetFormattedRange(Direction.Right);
            var txtBack = GetFormattedRange(Direction.Backward);
            return $"      {txtUp}\n {txtLeft}<{txtBack}>{txtRight}\n      {txtDown}";
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
            display.FontSize = 1.65f;
            display.WritePublicText(text);
            display.ShowPublicTextOnScreen();
        }

        void ScanAhead() {
            if (_foreRangeCamera == null) return;
            _foreRangeInfo = Ranger.GetDetailedRange(_foreRangeCamera, _settings.ForwardScanRange);
            BuildForwardDisplayText();
            _timeLastCleared = 0;
        }
        void BuildForwardDisplayText() {
            _scanRangeText =
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

        void TurnOffTools() {
            _toolList.ForEach(b => b.Enabled = false);
        }
        void ToggleToolsOnOff() {
            _toolList.ForEach(b => b.Enabled = !b.Enabled);
        }

        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(_toolList, b => IsOnThisGrid(b) && IsToolBlock(b));
            GridTerminalSystem.GetBlocksOfType(_proxSpeakerList, b => IsOnThisGrid(b) && IsProximityBlock(b));
            GridTerminalSystem.GetBlocksOfType(_displayList, b => IsOnThisGrid(b) && (IsProximityBlock(b) || IsForwardRangeBlock(b)));
            GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(_tmpList, b => IsOnThisGrid(b) && IsProximityBlock(b));
            _proxCameraList.Clear();
            foreach (var b in _tmpList) {
                _proxConfig.SetValue("Range Offset", 0.0);
                _proxConfig.Load(b);
                _proxCameraList.Add(new ProxCamera((IMyCameraBlock)b, _proxConfig.GetValue("Range Offset").ToDouble(0)));
                _proxConfig.Save(b);
            }

            _foreRangeCamera = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCameraBlock>(b => IsOnThisGrid(b) && IsForwardRangeBlock(b));

            _sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => IsOnThisGrid(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl);
        }

        bool IsOnThisGrid(IMyTerminalBlock b) => b.CubeGrid == Me.CubeGrid;
        bool IsToolBlock(IMyTerminalBlock b) => b is IMyShipDrill || b is IMyShipWelder || b is IMyShipGrinder;
        bool IsProximityBlock(IMyTerminalBlock b) => Collect.IsTagged(b, _settings.ProximityTag);
        bool IsForwardRangeBlock(IMyTerminalBlock b) => Collect.IsTagged(b, _settings.ForwardScanTag);
    }
}
