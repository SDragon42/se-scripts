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
    partial class Program {

        // Modules
        readonly RunningSymbol _running = new RunningSymbol();
        readonly DockSecure _dockSecure;
        readonly Proximity _proximity = new Proximity();
        readonly BlocksByOrientation _orientation = new BlocksByOrientation();

        // Configurations
        readonly ScriptConfiguration _config = new ScriptConfiguration();
        readonly CameraConfig _cameraConfig = new CameraConfig();
        readonly DisplayConfig _displayConfig = new DisplayConfig();

        // Script Variables
        readonly List<IMyThrust> _liftThrusters = new List<IMyThrust>();
        readonly List<ProxCamera> _proximityCameraList = new List<ProxCamera>();
        readonly List<IMySoundBlock> _proximitySpeakerList = new List<IMySoundBlock>();
        readonly List<IMyFunctionalBlock> _toolList = new List<IMyFunctionalBlock>();
        readonly List<ScreenConfig> _screenList = new List<ScreenConfig>();
        readonly List<IMyTerminalBlock> TmpBlocks = new List<IMyTerminalBlock>();

        IMyShipController _sc = null;
        IMyCameraBlock _foreRangeCamera = null;
        bool _alertSounding = false;

        double _timeLastBlockLoad = BLOCK_RELOAD_TIME * 2;
        double _timeLastCleared = 0;
        string _proximityText = string.Empty;
        string _scanRangeText = string.Empty;

        //float MinimumTWR = 0;
        //int InventoryMultiplier = 0;
        //double? MaxOperationalCargoMass;

        //bool Flag_SaveConfig;

        public Action<string> Debug = (msg) => { };

        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        readonly string _instructions;

        public Program() {
            Runtime.UpdateFrequency = FREQ_NORMAL;
            //Debug = Echo;
            //ProximityModule.Debug = Echo;

            _dockSecure = new DockSecure(Me, GridTerminalSystem);
            _commands.Add("dock", _dockSecure.Dock);
            _commands.Add("undock", _dockSecure.UnDock);
            _commands.Add("dock-toggle", _dockSecure.ToggleDock);
            _commands.Add("tools-off", TurnOffTools);
            _commands.Add("tools-toggle", ToggleToolsOnOff);
            _commands.Add("scan-range", ScanAhead);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in _commands.Keys) sb.AppendLine(c);
            _instructions = sb.ToString();
        }

        void LoadBlocks() {
            var reloadBlocks = _timeLastBlockLoad >= BLOCK_RELOAD_TIME;
            if (!reloadBlocks) return;

            _timeLastBlockLoad = 0;
            _dockSecure.LoadBlocks();

            GridTerminalSystem.GetBlocksOfType(_toolList, b => Me.IsSameConstructAs(b) && IsToolBlock(b));
            GridTerminalSystem.GetBlocksOfType(_proximitySpeakerList, b => Me.IsSameConstructAs(b) && IsProximityBlock(b));

            GridTerminalSystem.GetBlocksOfType(TmpBlocks, b =>
                Me.IsSameConstructAs(b)
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

            GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(TmpBlocks, b => Me.IsSameConstructAs(b) && IsProximityBlock(b));
            _proximityCameraList.Clear();
            foreach (var b in TmpBlocks) {
                _cameraConfig.Initialize(b, GridTerminalSystem);
                _cameraConfig.Load();
                _proximityCameraList.Add(new ProxCamera((IMyCameraBlock)b, _cameraConfig.RangeOffset));
            }

            _foreRangeCamera = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCameraBlock>(b => Me.IsSameConstructAs(b) && IsForwardRangeBlock(b));

            _sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => Me.IsSameConstructAs(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => Me.IsSameConstructAs(b) && b is IMyCockpit,
                b => Me.IsSameConstructAs(b) && b is IMyRemoteControl);

            _orientation.Init(_sc);
            GridTerminalSystem.GetBlocksOfType(_liftThrusters, _orientation.IsDown);

            //if (InventoryMultiplier <= 0) {
            //    var b = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCargoContainer>(Collect.IsCargoContainer);
            //    if (b != null) {
            //        InventoryMultiplier = CargoHelper.GetInventoryMultiplier(b);
            //        Flag_SaveConfig = true;
            //    }
            //}
        }

        bool IsToolBlock(IMyTerminalBlock b) => b is IMyShipDrill || b is IMyShipWelder || b is IMyShipGrinder;
        bool IsProximityBlock(IMyTerminalBlock b) => IsTagged(b, _config.ProximityTag);
        bool IsForwardRangeBlock(IMyTerminalBlock b) => IsTagged(b, _config.ForwardScanTag);

    }
}
