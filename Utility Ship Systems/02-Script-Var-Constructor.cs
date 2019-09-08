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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {
        readonly RunningSymbol RunningModule = new RunningSymbol();
        readonly DockSecure DockSecureModule = new DockSecure();
        readonly Proximity ProximityModule = new Proximity();
        readonly BlocksByOrientation BlockOrientationModule = new BlocksByOrientation();

        readonly MyIni Ini = new MyIni();
        readonly MyIni CameraIni = new MyIni();

        readonly List<ProxCamera> ProxCameraList = new List<ProxCamera>();
        readonly List<IMySoundBlock> ProxSpeakerList = new List<IMySoundBlock>();
        readonly List<IMyFunctionalBlock> ToolList = new List<IMyFunctionalBlock>();
        readonly List<IMyTextPanel> DisplayList = new List<IMyTextPanel>();

        IMyShipController Sc = null;
        IMyCameraBlock ForeRangeCamera = null;
        bool AlertSounding = false;
        RangeInfo ForeRangeInfo;

        double TimeLastBlockLoad = BLOCK_RELOAD_TIME * 2;
        double TimeLastCleared = 0;
        string ProximityText = string.Empty;
        string ScanRangeText = string.Empty;


        string ProximityTag;
        bool ProximityAlert;
        double ProximityAlertRange;
        double ProximityAlertSpeed;

        string ForwardScanTag;
        double ForwardScanRange;
        double ForwardDisplayClearTime;

        float MinimumTWR = 0;
        int InventoryMultiplier = 0;
        double? MaxOperationalCargoMass;

        bool Flag_SaveConfig;

        public Action<string> Debug = (msg) => { };

        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();
        readonly string Instructions;

        public Program() {
            //Debug = Echo;
            //_proximity.Debug = Echo;

            Commands.Add("dock", DockSecureModule.Dock);
            Commands.Add("undock", DockSecureModule.UnDock);
            Commands.Add("dock-toggle", DockSecureModule.ToggleDock);
            Commands.Add("tools-off", TurnOffTools);
            Commands.Add("scan-range", ScanAhead);
            Commands.Add("tools-toggle", ToggleToolsOnOff);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(ToolList, b => IsOnThisGrid(b) && IsToolBlock(b));
            GridTerminalSystem.GetBlocksOfType(ProxSpeakerList, b => IsOnThisGrid(b) && IsProximityBlock(b));
            GridTerminalSystem.GetBlocksOfType(DisplayList, b => IsOnThisGrid(b) && (IsProximityBlock(b) || IsForwardRangeBlock(b)));

            GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(TmpBlocks, b => IsOnThisGrid(b) && IsProximityBlock(b));
            ProxCameraList.Clear();
            foreach (var b in TmpBlocks)
                LoadCameraProximityConfig((IMyCameraBlock)b);

            ForeRangeCamera = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCameraBlock>(b => IsOnThisGrid(b) && IsForwardRangeBlock(b));

            Sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => IsOnThisGrid(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl);

            BlockOrientationModule.Init(Sc);
            GridTerminalSystem.GetBlocksOfType(LiftThrusters, BlockOrientationModule.IsDown);
        }

        bool IsToolBlock(IMyTerminalBlock b) => b is IMyShipDrill || b is IMyShipWelder || b is IMyShipGrinder;
        bool IsProximityBlock(IMyTerminalBlock b) => Collect.IsTagged(b, ProximityTag);
        bool IsForwardRangeBlock(IMyTerminalBlock b) => Collect.IsTagged(b, ForwardScanTag);

    }
}
