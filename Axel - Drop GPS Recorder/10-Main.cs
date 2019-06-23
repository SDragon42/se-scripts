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
    partial class Program : MyGridProgram {

        const string DefaultTag = "[drop-gps]";
        const string DefaultGpsLabel = "Probe Dropped";

        readonly RunningSymbol runningSymbol = new RunningSymbol();

        readonly List<IMyShipMergeBlock> currentMergeBlocks = new List<IMyShipMergeBlock>();
        readonly List<IMyShipMergeBlock> disconnectedMergeBlocks = new List<IMyShipMergeBlock>();
        readonly List<IMyTextPanel> lcdPanels = new List<IMyTextPanel>();


        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }


        string Tag = DefaultTag;
        string GpsLabel = DefaultGpsLabel;

        bool isFirstRun = true;

        public void Main(string argument, UpdateType updateSource) {
            Echo($"Drop GPS Recorder {runningSymbol.GetSymbol(Runtime)}");

            LoadConfig();
            LoadBlocks();

            currentMergeBlocks.ForEach(CheckForMergeDisconnect);
            isFirstRun = false;
        }

        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(lcdPanels, IsBlockIWant);
            GridTerminalSystem.GetBlocksOfType(currentMergeBlocks, IsBlockIWant);
        }

        bool IsBlockIWant(IMyTerminalBlock b) {
            if (!b.IsSameConstructAs(Me)) return false;
            if (!b.CustomName.ToLower().Contains(Tag)) return false;
            return true;
        }

        private void CheckForMergeDisconnect(IMyShipMergeBlock current) {
            if (current.IsConnected) {
                if (disconnectedMergeBlocks.Contains(current))
                    disconnectedMergeBlocks.Remove(current);
            } else {
                if (!disconnectedMergeBlocks.Contains(current)) {
                    disconnectedMergeBlocks.Add(current);
                    if (!isFirstRun)
                        LogPosition();
                }
            }
        }

        void LogPosition() {
            var position = Me.GetPosition();
            var gps = VectorHelper.VectortoGps(position, GpsLabel);
            foreach (IMyTextSurface lcd in lcdPanels) {
                lcd.ContentType = ContentType.TEXT_AND_IMAGE;
                lcd.WriteText($"{gps}\n", true);
            }
        }

    }
}
