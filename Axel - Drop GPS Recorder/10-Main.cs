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
    partial class Program : MyGridProgram {

        readonly ScriptConfig _config = new ScriptConfig();
        readonly RunningSymbol _runningSymbol = new RunningSymbol();
        readonly List<IMyShipMergeBlock> _currentMergeBlocks = new List<IMyShipMergeBlock>();
        readonly List<IMyShipMergeBlock> _disconnectedMergeBlocks = new List<IMyShipMergeBlock>();
        readonly List<IMyTextPanel> _lcdPanels = new List<IMyTextPanel>();


        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }


        bool _isFirstRun = true;

        public void Main(string argument, UpdateType updateSource) {
            Echo($"Drop GPS Recorder {_runningSymbol.GetSymbol()}");

            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();

            LoadBlocks();

            _currentMergeBlocks.ForEach(CheckForMergeDisconnect);
            _isFirstRun = false;
        }

        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(_lcdPanels, b => Me.IsSameConstructAs(b) && IsTagged(b, _config.LcdTag));
            GridTerminalSystem.GetBlocksOfType(_currentMergeBlocks, b => Me.IsSameConstructAs(b) && IsTagged(b, _config.MergeTag));
        }

        private void CheckForMergeDisconnect(IMyShipMergeBlock current) {
            var isInDisconnectList = _disconnectedMergeBlocks.Contains(current);

            if (current.IsConnected) {
                if (isInDisconnectList)
                    _disconnectedMergeBlocks.Remove(current);
                return;
            }

            if (!isInDisconnectList) {
                _disconnectedMergeBlocks.Add(current);
                if (!_isFirstRun)
                    LogPosition();
            }

        }

        void LogPosition() {
            var position = Me.GetPosition();
            var gps = VectorHelper.VectorToGps(position, _config.GpsLabel);
            foreach (IMyTextSurface lcd in _lcdPanels) {
                lcd.ContentType = ContentType.TEXT_AND_IMAGE;
                lcd.WriteText($"{gps}\n", true);
            }
        }

    }
}
