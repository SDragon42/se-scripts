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

        // Modules
        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(20);
        readonly DebugLogging Debug;
        readonly StateMachine<bool> Operations = new StateMachine<bool>();

        //Lists
        readonly List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();


        public Program() {
            Debug = new DebugLogging(this);
            Debug.EchoMessages = true;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }



        public void Main(string argument, UpdateType updateSource) {
            Echo("Launch Center " + Running.GetSymbol(Runtime));
            try {
                // Process Arguments
                ProcessArguments(argument);

                // Run State Machines
                if (updateSource.HasFlag(UpdateType.Update10))
                    Operations.RunAll();

                // Display LOG
                Echo(Log.GetLogText());
            } catch (Exception ex) {
                Echo(ex.ToString());
                throw;
            } finally {
                Debug.UpdateDisplay();
            }
        }

        void ProcessArguments(string argument) {
            if (argument.Length == 0) return;
            Log.AppendLine($"{DateTime.Now.ToShortTimeString()} - {argument}");
            switch (argument?.ToLower()) {
                case CMD_CONNECT_BOOMS:
                    Operations.Add("boom", ConnectBoom2("launch-pad", 0.5F));
                    break;
                case CMD_DISCONNECT_BOOMS:
                    Operations.Add("boom", RetractBoom2("launch-pad", -0.5F), replace: true);
                    break;
                case CMD_LAUNCH:
                    Operations.Add("boom", RetractBoom2("launch-pad", -5F), replace: true);
                    break;
                    //case "extend":
                    //    MoveBoomBooster(true);
                    //    MoveBoomOrbiter(true);
                    //    break;
                    //case "extend-booster":
                    //    MoveBoomBooster(true);
                    //    break;
                    //case "extend-orbiter":
                    //    MoveBoomOrbiter(true);
                    //    break;
                    //case "extend-crew":
                    //    break;
                    //case "retract":
                    //    MoveBoomBooster(false);
                    //    MoveBoomOrbiter(false);
                    //    break;
                    //case "retract-booster":
                    //    MoveBoomBooster(false);
                    //    break;
                    //case "retract-orbiter":
                    //    MoveBoomOrbiter(false);
                    //    break;
                    //case "retract-crew":
                    //    break;

            }
        }

        //void MoveBoomBooster(bool extend) {
        //    //if (boomBooster) return;
        //    //boomBooster = true;
        //    var op = extend
        //        ? ConnectBoom("[boom-booster]", 1f, RotorLimit.Low)
        //        : RetractBoom("[boom-booster]", 1f, RotorLimit.High);
        //    _Operations.Add(op);
        //}
        //void MoveBoomOrbiter(bool extend) {
        //    //if (boomCargo) return;
        //    //boomCargo = true;
        //    var op = extend
        //        ? ConnectBoom("[boom-orbiter]", 1f, RotorLimit.High)
        //        : RetractBoom("[boom-orbiter]", 1f, RotorLimit.Low);
        //    _Operations.Add(op);
        //}




        enum RotorLimit { Low, High }

    }
}
