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
