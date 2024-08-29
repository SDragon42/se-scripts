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
    partial class Program : MyGridProgram {

        // Modules
        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(20);
        readonly DebugLogging Debug;
        readonly StateMachineSets Operations = new StateMachineSets();

        //Lists
        readonly List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();

        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();
        string commandKey;
        string commandArgs;

        readonly char[] ArgumentSplitter = new char[] { ' ' };


        public Program() {
            Debug = new DebugLogging(this);
            Debug.EchoMessages = true;

            Commands.Add("connect-boom", ConnectBooms);
            Commands.Add("disconnect-boom", DisconnectBooms);
            Commands.Add("launch", Launch);

            Runtime.UpdateFrequency = UpdateFrequency.None;
        }

        public void Save() {
        }



        public void Main(string argument, UpdateType updateSource) {
            //Echo("Launch Center " + Running.GetSymbol(Runtime));
            try {
                // Process Arguments
                //ProcessArguments(argument);
                var argumentParts = argument.Split(ArgumentSplitter, 2);
                commandKey = argumentParts[0];
                commandArgs = argumentParts.Length < 2 ? string.Empty : argumentParts[1];

                if (Commands.ContainsKey(commandKey)) Commands[commandKey]?.Invoke();

                // Run State Machines
                //if ((updateSource & UpdateType.Update10) == UpdateType.Update10)
                Operations.RunAll();

                Runtime.UpdateFrequency = Operations.HasTasks
                    ? UpdateFrequency.Update10
                    : UpdateFrequency.None;

                // Display LOG
                Echo(Log.GetLogText());
            } catch (Exception ex) {
                Echo(ex.ToString());
                throw;
            } finally {
                Debug.UpdateDisplay();
            }
        }

        void ConnectBooms() {
            Operations.Add("boom", ConnectBoom2("launch-pad", 0.5F));
        }
        void DisconnectBooms() {
            Operations.Add("boom", RetractBoom2("launch-pad", -0.5F), replace: true);
        }
        void Launch() {
            Operations.Add("boom", RetractBoom2("launch-pad", -5F), replace: true);
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
