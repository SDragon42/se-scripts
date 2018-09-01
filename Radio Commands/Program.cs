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
    partial class Program : MyGridProgram {

        readonly Logging Log = new Logging(50);
        readonly RunningSymbol_Time Symbol = new RunningSymbol_Time();

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
        }
    }
}
