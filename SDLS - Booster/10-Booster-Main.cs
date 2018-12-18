//using Sandbox.Game.EntityComponents;
//using Sandbox.ModAPI.Ingame;
//using Sandbox.ModAPI.Interfaces;
//using SpaceEngineers.Game.ModAPI.Ingame;
//using System.Collections.Generic;
//using System.Collections;
//using System.Linq;
//using System.Text;
//using System;
//using VRage.Collections;
//using VRage.Game.Components;
//using VRage.Game.ModAPI.Ingame;
//using VRage.Game.ObjectBuilders.Definitions;
//using VRage.Game;
//using VRageMath;

//namespace IngameScript {
//    partial class Program {
//        public void Main(string argument, UpdateType updateSource) {
//            UpTime += Runtime.TimeSinceLastRun;
//            if (updateSource.HasFlag(UpdateType.Update1) || updateSource.HasFlag(UpdateType.Update10) || updateSource.HasFlag(UpdateType.Update100)) {
//                Echo(Running.GetSymbol(Runtime));
//            }

//            try {
//                LoadBlocks();
//                ProcessArguments(argument.ToLower());
//                Operations.RunAll();
//                Echo(Log.GetLogText());
//            } catch (Exception ex) {
//                Echo(ex.ToString());
//                throw;
//            } finally {
//                Debug?.UpdateDisplay();
//            }
//        }


//        void ProcessArguments(string argument) {
//            if (!Commands.ContainsKey(argument)) return;
//            Commands[argument]?.Invoke();
//        }

//        void Reload() {
//            Log.AppendLine("CMD: " + CMD_RELOAD);
//            LoadBlocks(true);
//        }
//        void Shutdown() {
//            Log.AppendLine("CMD: " + CMD_SHUTDOWN);
//            if (Antenna != null) Antenna.Enabled = false;
//            if (Beacon != null) Beacon.Enabled = false;
//            AllThrusters.ForEach(DisableThruster);
//            Parachutes.ForEach(b => b.Enabled = false);
//            VAlign.gyrosOff(Gyros);
//            Gyros.ForEach(b => b.Enabled = false);
//            LandingGears.ForEach(b => b.AutoLock = false);
//            Operations.Clear();
//            Runtime.UpdateFrequency = UpdateFrequency.None;
//            Log.Clear();
//        }

//        void Stage() {
//            Log.AppendLine("CMD: " + CMD_STAGE);
//            Runtime.UpdateFrequency = UpdateFrequency.Update1;
//            Operations.Add("Staging", Sequence_Stage());
//        }

//    }
//}
