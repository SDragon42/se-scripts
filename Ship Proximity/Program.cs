using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
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
    public partial class Program : MyGridProgram {

        const double BLOCK_RELOAD_TIME = 10;
        const float DEFAULT_SCREEN_WIDTH = 512f;
        const float DISPLAY_RANGE_FONT_SIZE = 3.5f;
        const float DISPLAY_PROX_FONT_SIZE = 3.45f;
        const UpdateFrequency DEFAULT_UPDATE_FREQUENCY = UpdateFrequency.Update10;

        // Modules
        readonly RunningSymbol _running = new RunningSymbol();
        readonly Proximity _proximity = new Proximity();

        // Configurations
        readonly ScriptConfiguration _config = new ScriptConfiguration();
        readonly CameraConfig _cameraConfig = new CameraConfig();
        readonly DisplayConfig _displayConfig = new DisplayConfig();

        // Blocks
        IMyShipController _shipController = null;
        readonly List<IMyTerminalBlock> TmpBlocks = new List<IMyTerminalBlock>();
        readonly List<ProxCamera> _proximityCameraList = new List<ProxCamera>();
        readonly List<IMyInteriorLight> _proximityLightList = new List<IMyInteriorLight>();
        IMyCameraBlock _foreRangeCamera = null;
        readonly List<ScreenConfig> _screenList = new List<ScreenConfig>();

        double _timeLastBlockLoad = BLOCK_RELOAD_TIME * 2;
        double _timeLastCleared = 0;
        string _proximityText = string.Empty;
        string _scanRangeText = string.Empty;

        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        readonly string _instructions;

        public Program() {
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load(_proximity);

            _commands.Add("scan-range", ScanAhead);
            _commands.Add("on", TurnOn);
            _commands.Add("off", TurnOff);
            _instructions = "Script Commands:\n" + string.Join("\n", _commands.Keys);

            var updateFrequency = DEFAULT_UPDATE_FREQUENCY;
            var storageParts = Storage.Split('|');
            if (storageParts.Length >= 2) {
                var i = 0;
                switch (storageParts[0]) {
                    case "1":
                        if (storageParts.Length != 2) break;
                        Enum.TryParse(storageParts[++i], out updateFrequency);
                        if (updateFrequency == UpdateFrequency.None) updateFrequency = UpdateFrequency.Once;
                        break;
                    case "2":
                        if (storageParts.Length != 4) break;
                        Enum.TryParse(storageParts[++i], out updateFrequency);
                        if (updateFrequency == UpdateFrequency.None) updateFrequency = UpdateFrequency.Once;
                        _proximityText = storageParts[++i];
                        _scanRangeText = storageParts[++i];
                        break;
                    default: break;
                }
            }

            Runtime.UpdateFrequency = updateFrequency;
        }

        public void Save() {
            Storage = "2|" + Runtime.UpdateFrequency + "|" + _proximityText + "|" + _scanRangeText;
        }

        public void Main(string argument, UpdateType updateSource) {
            try {
                var isRunning = Runtime.UpdateFrequency == UpdateFrequency.Update10;
                var runningStatus = isRunning ? _running.GetSymbol() : "( OFF )";
                Echo("Proximity & Range v$VERSION$ " + runningStatus);
                if (isRunning) {
                    _timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
                    _timeLastCleared += Runtime.TimeSinceLastRun.TotalSeconds;
                    var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - _timeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);
                    Echo($"Scanning for blocks in {timeTilUpdate:N0} seconds.\n");
                }
                Echo("Configure script in 'Custom Data'\n");
                Echo(_instructions);
                _config.Load(_proximity);
                LoadBlocks();
                _proximity.Init(_shipController);

                if (argument.Length > 0) argument = argument.ToLower();
                if (_commands.ContainsKey(argument)) _commands[argument]?.Invoke();

                if (Runtime.UpdateFrequency == UpdateFrequency.None) return;

                // Automatic Operations
                UpdateProximity();
            } finally {
                UpdateScreens();
            }
        }

        // Implementation for turning on the system
        void TurnOn() {
            Runtime.UpdateFrequency = DEFAULT_UPDATE_FREQUENCY;
        }

        // Implementation for turning off the system
        void TurnOff() {
            Runtime.UpdateFrequency = UpdateFrequency.None;
            SetProximityAlert(false);
            _timeLastBlockLoad = BLOCK_RELOAD_TIME * 2;
            _proximityText = "\nSTANDBY";
            _scanRangeText = "";
        }

        // Scan ahead using the forward range camera
        void ScanAhead() {
            if (_foreRangeCamera == null) return;
            MyDetectedEntityInfo _foreRangeInfo;
            RangeHelper.TryGetDetailedRange(_foreRangeCamera, _config.ForwardScanRange, out _foreRangeInfo);
            _scanRangeText = BuildForwardDisplayText(_foreRangeInfo, _foreRangeCamera);
            _timeLastCleared = 0;
        }

        // Load all necessary blocks from the grid
        void LoadBlocks() {
            var reloadBlocks = _timeLastBlockLoad >= BLOCK_RELOAD_TIME;
            if (!reloadBlocks) return;

            _timeLastBlockLoad = 0;

            // Ship Controller
            _shipController = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => Me.IsSameConstructAs(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => Me.IsSameConstructAs(b) && b is IMyRemoteControl && ((IMyRemoteControl)b).IsMainCockpit,
                b => Me.IsSameConstructAs(b) && b is IMyCockpit,
                b => Me.IsSameConstructAs(b) && b is IMyRemoteControl);

            // Forward Range Camera
            _foreRangeCamera = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCameraBlock>(b => Me.IsSameConstructAs(b) && IsForwardRangeBlock(b));

            // Proximity Cameras
            GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(TmpBlocks, b => Me.IsSameConstructAs(b) && IsProximityBlock(b));
            _proximityCameraList.Clear();
            foreach (var b in TmpBlocks) {
                _cameraConfig.Initialize(b, GridTerminalSystem);
                _cameraConfig.Load();
                _proximityCameraList.Add(new ProxCamera((IMyCameraBlock)b, _cameraConfig.RangeOffset));
            }

            GridTerminalSystem.GetBlocksOfType(_proximityLightList, b => Me.IsSameConstructAs(b) && IsProximityBlock(b));

            // Display Screens
            GridTerminalSystem.GetBlocksOfType(TmpBlocks, b => Me.IsSameConstructAs(b)
                                                            && ((b is IMyTextSurfaceProvider) || (b is IMyTextSurface))
                                                            && (IsProximityBlock(b) || IsForwardRangeBlock(b)));
            _screenList.Clear();
            foreach (var b in TmpBlocks) {
                var surface = b as IMyTextSurface;
                if (surface != null) {
                    _screenList.Add(new ScreenConfig(surface, IsProximityBlock(b), IsForwardRangeBlock(b)));
                    continue;
                }

                var surfaceProv = b as IMyTextSurfaceProvider;
                if (surfaceProv != null) {
                    _displayConfig.Initialize(b, GridTerminalSystem);
                    _displayConfig.Load();
                    var pIdx = _displayConfig.ProximityScreenNumber;
                    var rIdx = _displayConfig.RangeScreenNumber;
                    for (var i = 0; i < surfaceProv.SurfaceCount; i++) {
                        if (i != pIdx && i != rIdx) continue;
                        surface = surfaceProv.GetSurface(i);
                        _screenList.Add(new ScreenConfig(surface, i == pIdx, i == rIdx));
                    }
                }
            }

            TmpBlocks.Clear();
        }

        bool IsProximityBlock(IMyTerminalBlock b) => IsTagged(b, _config.ProximityTag);
        bool IsForwardRangeBlock(IMyTerminalBlock b) => IsTagged(b, _config.ForwardScanTag);

        void UpdateProximity() {
            _proximity.RunScan(_proximityCameraList);
            if (_config.ProximityAlert) {
                var closestRange = _proximity.GetClosestRange();
                var alertActive = closestRange.HasValue && closestRange.Value <= _config.ProximityAlertRange;
                SetProximityAlert(alertActive);
            } else {
                SetProximityAlert(false);
            }
            _proximityText = BuildProximityDisplayText();
        }
        // Turns the proximity alert lights on or off based on the active state
        void SetProximityAlert(bool active) {
            _proximityLightList.ForEach(b => b.Enabled = active);
        }

        string BuildProximityDisplayText() {
            var txtUp = GetFormattedRange(Base6Directions.Direction.Up);
            var txtDown = GetFormattedRange(Base6Directions.Direction.Down);
            var txtLeft = GetFormattedRange(Base6Directions.Direction.Left);
            var txtRight = GetFormattedRange(Base6Directions.Direction.Right);
            var txtBack = GetFormattedRange(Base6Directions.Direction.Backward);
            var txtForward = GetFormattedRange(Base6Directions.Direction.Forward);
            var txtForward2 = string.Empty.PadRight(txtForward.Length, ' ');
            return $"{txtForward} {txtUp} {txtForward2}\n{txtLeft}<{txtBack}>{txtRight}\n{txtDown}";
        }

        string GetFormattedRange(Base6Directions.Direction dir) {
            var range = _proximity.GetRange(dir);
            if (!range.HasValue) return "----";
            return (range.Value < 100.0)
                ? $"{range,4:N1}"
                : $"{range,4:N0}";
        }



        string BuildForwardDisplayText(MyDetectedEntityInfo detectedInfo, IMyCameraBlock camera) {
            if (detectedInfo.IsEmpty()) {
                return "\nNo Entity Detected";
            }
            var range = Vector3D.Distance(camera.GetPosition(), detectedInfo.HitPosition ?? detectedInfo.Position);
            return $"Entity: {detectedInfo.Type}\n" +
                $"Name: {detectedInfo.Name}\n" +
                $"Range: {TextHelper.FormatDistance(range)}";
        }

        void UpdateScreens() {
            if (_timeLastCleared >= _config.ForwardScanRangeDisplayTime && _scanRangeText.Length > 0) {
                _scanRangeText = string.Empty;
                _timeLastCleared = 0;
            }

            foreach (var sc in _screenList) {
                if (sc.IsRange && (!sc.IsProx || _scanRangeText.Length > 0)) {
                    InitDisplay(sc.Screen, fontName: LCDFonts.DEBUG, fontSize: DISPLAY_RANGE_FONT_SIZE, alignment: TextAlignment.CENTER);
                    sc.Screen.WriteText(_scanRangeText);
                    continue;
                }
                if (sc.IsProx) {
                    InitDisplay(sc.Screen, fontName: LCDFonts.MONOSPACE, fontSize: DISPLAY_PROX_FONT_SIZE, alignment: TextAlignment.CENTER);
                    sc.Screen.WriteText(_proximityText);
                }
            }
        }

        void InitDisplay(IMyTextSurface display, string fontName = LCDFonts.DEBUG, float fontSize = 1f, TextAlignment alignment = TextAlignment.LEFT, float padding = 0f) {
            display.Font = fontName;
            display.TextPadding = padding;
            display.Alignment = alignment;
            display.ContentType = ContentType.TEXT_AND_IMAGE;

            if (display.TextureSize.X < DEFAULT_SCREEN_WIDTH) fontSize /= 2;
            display.FontSize = fontSize;
        }

        //================================================================================
        class ScreenConfig {
            public ScreenConfig(IMyTextSurface screen, bool isProx, bool isRange) {
                Screen = screen;
                IsProx = isProx;
                IsRange = isRange;
            }
            public IMyTextSurface Screen { get; private set; }
            public bool IsProx { get; private set; }
            public bool IsRange { get; private set; }
        }

        //================================================================================
        static class ConfigSections {
            public const string PROXIMITY = "Proximity";
            public const string RANGE = "Range";
        }

        //================================================================================
        class ScriptConfiguration : ConfigBase {
            //public ScriptConfiguration(IMyTerminalBlock block, IMyGridTerminalSystem GridTerminalSystem) : base(block, GridTerminalSystem) { }
            public string ProximityTag { get; private set; } = "[proximity]";
            public bool ProximityAlert { get; private set; } = false;
            public double ProximityAlertRange { get; private set; } = 10;

            public string ForwardScanTag { get; private set; } = "[range]";
            public double ForwardScanRange { get; private set; } = 15000;
            public double ForwardScanRangeDisplayTime { get; private set; } = 5;

            public void Load(Proximity proximity) {
                if (!LoadIni()) return;

                // Proximity settings
                ProximityTag = _ini.Add(ConfigSections.PROXIMITY, "Tag", ProximityTag).ToString();
                proximity.ScanRange = _ini.Add(ConfigSections.PROXIMITY, "Range (m)", proximity.ScanRange).ToDouble();
                ProximityAlert = _ini.Add(ConfigSections.PROXIMITY, "Alert On/Off", ProximityAlert).ToBoolean();
                ProximityAlertRange = _ini.Add(ConfigSections.PROXIMITY, "Alert Range (m)", ProximityAlertRange).ToDouble();

                // Range Scanning settings
                ForwardScanTag = _ini.Add(ConfigSections.RANGE, "Tag", ForwardScanTag).ToString();
                ForwardScanRange = _ini.Add(ConfigSections.RANGE, "Scan Range (m)", ForwardScanRange).ToDouble(); //TODO: Should this be a float?
                ForwardScanRangeDisplayTime = _ini.Add(ConfigSections.RANGE, "Display Time (seconds)", ForwardScanRangeDisplayTime).ToDouble(); //TODO: Should this be a float?

                Save();
            }
        }

        //================================================================================
        class CameraConfig : ConfigBase {
            //public CameraConfig(IMyTerminalBlock block, IMyGridTerminalSystem GridTerminalSystem) : base(block, GridTerminalSystem) { }
            public double RangeOffset { get; private set; } = 0;

            public void Load() {
                if (!LoadIni()) return;

                RangeOffset = _ini.Add(ConfigSections.PROXIMITY, "Range Offset", RangeOffset).ToDouble();
                Save();
            }
        }

        //================================================================================

        class DisplayConfig : ConfigBase {
            //public DisplayConfig(IMyTerminalBlock block, IMyGridTerminalSystem GridTerminalSystem) : base(block, GridTerminalSystem) { }
            public int ProximityScreenNumber { get; private set; } = 0;
            public int RangeScreenNumber { get; private set; } = 0;

            public void Load() {
                if (!LoadIni()) return;

                ProximityScreenNumber = _ini.Add(ConfigSections.PROXIMITY, "Screen Number", ProximityScreenNumber, null).ToInt32();
                RangeScreenNumber = _ini.Add(ConfigSections.RANGE, "Screen Number", RangeScreenNumber, null).ToInt32();
                Save();
            }
        }
    }
}
