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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {
        readonly RunningSymbol _running = new RunningSymbol();
        readonly DockSecure _dockSecure = new DockSecure();
        readonly Proximity _proximity = new Proximity();
        readonly BlocksByOrientation BlockOrientation = new BlocksByOrientation();

        readonly MyIni Ini = new MyIni();
        readonly MyIni CameraIni = new MyIni();

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


        public Program() {
            //Debug = Echo;
            //_proximity.Debug = Echo;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(_toolList, b => IsOnThisGrid(b) && IsToolBlock(b));
            GridTerminalSystem.GetBlocksOfType(_proxSpeakerList, b => IsOnThisGrid(b) && IsProximityBlock(b));
            GridTerminalSystem.GetBlocksOfType(_displayList, b => IsOnThisGrid(b) && (IsProximityBlock(b) || IsForwardRangeBlock(b)));
            GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(_tmpList, b => IsOnThisGrid(b) && IsProximityBlock(b));
            _proxCameraList.Clear();
            foreach (var b in _tmpList)
                LoadCameraProximityConfig(b);

            _foreRangeCamera = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyCameraBlock>(b => IsOnThisGrid(b) && IsForwardRangeBlock(b));

            _sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => IsOnThisGrid(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => IsOnThisGrid(b) && b is IMyCockpit,
                b => IsOnThisGrid(b) && b is IMyRemoteControl);

            BlockOrientation.Init(_sc);
            GridTerminalSystem.GetBlocksOfType(LiftThrusters, BlockOrientation.IsDown);
        }

        bool IsToolBlock(IMyTerminalBlock b) => b is IMyShipDrill || b is IMyShipWelder || b is IMyShipGrinder;
        bool IsProximityBlock(IMyTerminalBlock b) => Collect.IsTagged(b, ProximityTag);
        bool IsForwardRangeBlock(IMyTerminalBlock b) => Collect.IsTagged(b, ForwardScanTag);


    }
}
