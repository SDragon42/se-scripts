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
    partial class Program {
        public void Main(string argument, UpdateType updateSource) {
            UpTime += Runtime.TimeSinceLastRun;
            TagSelf();
            if ((updateSource & UpdateType.Update10) == UpdateType.Update10) {
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
            //if ((updateSource & UpdateType.Terminal) == UpdateType.Terminal) {
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
