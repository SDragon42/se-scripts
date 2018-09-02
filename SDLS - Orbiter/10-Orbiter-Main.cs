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
            UpTime += Runtime.TimeSinceLastRun;
            if ((updateSource.HasFlag(UpdateType.Update10))) {
                Echo(Running.GetSymbol(Runtime));
            }

            try {
                LoadBlocks();
                ProcessArguments(argument.ToLower());
                Operations.RunAll();
                Operations.RemoveCompleted();
                Echo(Log.GetLogText());
            } catch (Exception ex) {
                Echo(ex.ToString());
                throw;
            } finally {
                Debug?.UpdateDisplay();
            }
            //if (updateSource.HasFlag(UpdateType.Terminal)) {
            //    switch (argument.ToLower()) {
            //        case CMD_STANDBY:
            //            Runtime.UpdateFrequency = UpdateFrequency.None;
            //            break;
            //        default:
            //            Runtime.UpdateFrequency |= UpdateFrequency.Update10;
            //            break;
            //    }
            //}
        }

        void ProcessArguments(string argument) {
            if (!Commands.ContainsKey(argument)) return;
            Commands[argument]?.Invoke();
        }

    }
}
