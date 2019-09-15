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
        #region mdk preserve



        readonly RunningSymbol symbol = new RunningSymbol();
        readonly AutoDoorCloser autoDoorCloser = new AutoDoorCloser();

        readonly Config config = new Config();

        readonly List<IMyDoor> autoDoors = new List<IMyDoor>();

        double blockReload_Time = 10;
        double blockReload_TimeElapsed = 0;

        public Program() {
            config.Load(Me, this);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            blockReload_TimeElapsed += Runtime.TimeSinceLastRun.TotalSeconds;
            Echo($"Grid OS {symbol.GetSymbol(Runtime)}");
            Echo($"Block Reload in {Math.Truncate(blockReload_Time - blockReload_TimeElapsed) + 1:N0} seconds.");
            config.Load(Me, this);
            LoadBlocks();
            if (config.ADCEnabled) autoDoorCloser.CloseOpenDoors(Runtime, autoDoors);
        }


        void LoadBlocks(bool forceLoad = false) {
            if (!forceLoad && blockReload_TimeElapsed < blockReload_Time) return;

            GridTerminalSystem.GetBlocksOfType(autoDoors, (block) => {
                if (Collect.IsTagged(block, config.ADCExclusionTag)) return false;
                if (Collect.IsHangarDoor(block)) return false;
                return true;
            });

            blockReload_TimeElapsed = 0;
        }


        #endregion
    }
}
