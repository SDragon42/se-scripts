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

        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();
        TimeSpan UpTime = TimeSpan.Zero;
        //bool BlocksLoaded = false;
        bool IsMasterGrid = false;

        // Modules
        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(40);
        readonly DebugLogging Debug;
        readonly FlightDataRecorder Fdr;

        // Blocks



        public Program() {
            Debug = new DebugLogging(this) {
                EchoMessages = true,
                Enabled = true
            };

            Fdr = new FlightDataRecorder(new string[] { "" }, 1000) {
                Enabled = false
            };

            Commands.Add(CMD.SCAN, ScanGrids);

            TagSelf();
            NameGrids();
        }

        public void Save() {

        }

    }
}
