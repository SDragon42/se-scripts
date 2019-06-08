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
    partial class Program {
        public void Main(string argument, UpdateType updateSource) {
            UpTime += Runtime.TimeSinceLastRun;
            TagSelf();
            if ((updateSource.HasFlag(UpdateType.Update10))) {
                Echo(Running.GetSymbol(Runtime));
            }

            if (!Me.CubeGrid.CustomName.EndsWith(TAG.GRID))
                Me.CubeGrid.CustomName += " " + TAG.GRID;

            try {
                LoadBlocks();
                ProcessArguments(argument.ToLower());
                if (RunSequences())
                    Runtime.UpdateFrequency = UpdateFrequency.None;
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

        bool RunSequences() {
            var hasTasks = false;

            GravityAlign.Run();
            hasTasks |= GravityAlign.HasTasks;

            //Operations.RunAll();
            //hasTasks |= Operations.HasTasks;

            return hasTasks;
        }


        bool IsOnRocket(IMyTerminalBlock b) => rocketParts.Contains(b.CubeGrid);

        void ProcessArguments(string argument) {
            if (!Commands.ContainsKey(argument)) return;
            Commands[argument]?.Invoke();
        }




    }
}
