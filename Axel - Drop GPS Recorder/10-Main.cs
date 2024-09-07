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

        readonly ScriptConfig Config = new ScriptConfig();

        readonly RunningSymbol runningSymbol = new RunningSymbol();

        readonly List<IMyShipMergeBlock> currentMergeBlocks = new List<IMyShipMergeBlock>();
        readonly List<IMyShipMergeBlock> disconnectedMergeBlocks = new List<IMyShipMergeBlock>();
        readonly List<IMyTextPanel> lcdPanels = new List<IMyTextPanel>();


        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }




        bool isFirstRun = true;

        public void Main(string argument, UpdateType updateSource) {
            Echo($"Drop GPS Recorder {runningSymbol.GetSymbol(Runtime)}");

            Config.Load(Me);
            LoadBlocks();

            currentMergeBlocks.ForEach(CheckForMergeDisconnect);
            isFirstRun = false;
        }

        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(lcdPanels, b => {
                if (!b.IsSameConstructAs(Me)) return false;
                if (!b.CustomName.ToLower().Contains(Config.LcdTag)) return false;
                return true;
            });
            GridTerminalSystem.GetBlocksOfType(currentMergeBlocks, b => {
                if (!b.IsSameConstructAs(Me)) return false;
                if (Config.MergeTag.Length > 0)
                    if (!b.CustomName.ToLower().Contains(Config.MergeTag)) return false;
                return true;
            });
        }

        private void CheckForMergeDisconnect(IMyShipMergeBlock current) {
            var isInDisconnect = disconnectedMergeBlocks.Contains(current);

            if (current.IsConnected) {
                if (isInDisconnect)
                    disconnectedMergeBlocks.Remove(current);
                return;
            }

            if (!isInDisconnect) {
                disconnectedMergeBlocks.Add(current);
                if (!isFirstRun)
                    LogPosition();
            }

        }

        void LogPosition() {
            var position = Me.GetPosition();
            var gps = VectorHelper.VectortoGps(position, Config.GpsLabel);
            foreach (IMyTextSurface lcd in lcdPanels) {
                lcd.ContentType = ContentType.TEXT_AND_IMAGE;
                lcd.WriteText($"{gps}\n", true);
            }
        }

    }
}
