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
    partial class Program : MyGridProgram {
        const string CMD_Standby = "standby";

        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(40);

        enum Mode { Standby }


        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo(Running.GetSymbol(Runtime));

            if (updateSource.HasFlag(UpdateType.Terminal)) {
                switch (argument.ToLower()) {
                    case CMD_Standby:
                        Runtime.UpdateFrequency = UpdateFrequency.None;
                        break;
                    default:
                        Runtime.UpdateFrequency |= UpdateFrequency.Update10;
                        break;
                }
            }
        }
    }
}
