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
    partial class Program {

        // Modules
        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(40);
        readonly DebugLogging Debug;
        readonly FlightDataRecorder Fdr;
        readonly StateMachine<bool> Operations = new StateMachine<bool>(r => r);
        readonly BlocksByOrientation Orientation = new BlocksByOrientation();

        //Lists


        public Program() {
            Debug = new DebugLogging(this);

            Fdr = new FlightDataRecorder(new string[] { "" }, 1000);

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

    }
}
