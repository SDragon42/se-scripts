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
            Echo(Running.GetSymbol(Runtime));

            try {
                LoadBlocks();

                // Process Arguments
                ProcessArguments(argument);

                // Run State Machines
                if ((updateSource & UpdateType.Update10) == UpdateType.Update10) {
                    Operations.RunAll();
                }

                // Display LOG
                Echo(Log.GetLogText());
            } catch (Exception ex) {
                Echo(ex.ToString());
                throw;
            } finally {
                Debug?.UpdateDisplay();
            }
        }


        void ProcessArguments(string argument) {
            if (argument.Length == 0) return;
            switch (argument?.ToLower()) {
                case CMD_SHUTDOWN:
                    Shutdown();
                    break;
                case CMD_STANDBY:
                    Standby();
                    break;
                case CMD_STAGE:
                    Stage();
                    break;
            }
        }

        void Standby() {
            LoadBlocks();
        }

        void Shutdown() {
            AllThrusters.ForEach(DisableThruster);
            Operations.Clear();
            Runtime.UpdateFrequency = UpdateFrequency.None;
        }

        void Stage() {
            Operations.Add("stage", Sequence_Stage());
        }

        void Recovery() {
            Operations.Clear();
        }
    }
}
