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

        const string GROUP_ALL_PISTON = "Factory Piston Toggle";
        const string GROUP_WELDERS = "Welder Group";
        const float SPEED_OPERATION = 0.015F;
        const float SPEED_MOVE_TO_POSITION = 0.5F;


        readonly RunningSymbol Running = new RunningSymbol();
        readonly List<IMyPistonBase> PistonList = new List<IMyPistonBase>();
        //readonly List<IMyShipWelder> WelderList = new List<IMyShipWelder>();
        readonly StateMachine<bool> Operation = new StateMachine<bool>();

        string OperationMessage = string.Empty;


        Action<string> Debug = (message) => { };

        public Program() {
            //Debug = Echo;
        }


        public void Main(string argument, UpdateType updateSource) {
            Debug($"Update Rate: {Runtime.UpdateFrequency}");
            var autoRun = Runtime.UpdateFrequency.HasFlag(UpdateFrequency.Update10);
            if (autoRun)
                Echo("Running " + Running.GetSymbol(Runtime));
            argument = argument?.ToLower();
            switch (argument) {
                case "extend":
                    Operation.Clear();
                    Operation.Add("piston", MoveToStart(), true);
                    break;
                case "retract":
                    Operation.Clear();
                    Operation.Add("piston", ResetToEnd(), true);
                    break;
                    //case "info":
                    //    LoadBlocks();
                    //    var welder = WelderList.Where(b => b.CustomName == "Welder - Test")?.FirstOrDefault();
                    //    if (welder != null) {
                    //        welder.Enabled = !welder.Enabled;
                    //        Debug($"Welder: {welder.CustomName} | {welder.Enabled}");
                    //    }
                    //    Debug($"# Pistons: {PistonList.Count:N0}");
                    //    Debug($"# Welders: {WelderList.Count:N0}");
                    //    break;
            }

            Operation.RunAll();

            Debug($"Has Tasks: {Operation.HasTasks}");

            if (Operation.HasTasks)
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            else if (autoRun)
                Runtime.UpdateFrequency = UpdateFrequency.Once;
            else
                Runtime.UpdateFrequency = UpdateFrequency.None;

            Echo(OperationMessage);
        }

        IEnumerator<bool> MoveToStart() {
            OperationMessage = "Moving welders to start position";
            LoadBlocks();
            yield return true;

            //WelderList.ForEach(w => w.Enabled = false);
            //yield return true;

            PistonList.ForEach(p => p.Velocity = SPEED_MOVE_TO_POSITION);
            PistonList.ForEach(p => p.Extend());

            var allExtended = false;
            do {
                allExtended = PistonList.All(IsExtended);
                yield return true;
                //ShowStates("All Extended", allExtended);
                //WelderList.ForEach(w => w.Enabled = !w.Enabled);
            } while (!allExtended);

            PistonList.ForEach(p => p.Velocity = SPEED_OPERATION);
            yield return true;
            OperationMessage = "Extended";
            yield return false;
        }

        IEnumerator<bool> ResetToEnd() {
            OperationMessage = "Moving welders to retracted position";
            LoadBlocks();
            yield return true;

            //WelderList.ForEach(w => w.Enabled = false);
            //yield return true;

            PistonList.ForEach(p => p.Velocity = SPEED_MOVE_TO_POSITION);
            PistonList.ForEach(p => p.Retract());

            var allReteacted = false;
            do {
                yield return true;
                allReteacted = PistonList.All(IsReteacted);
                //ShowStates("All Retracted", allReteacted);
            } while (!allReteacted);

            PistonList.ForEach(p => p.Velocity = SPEED_OPERATION * -1.0F);
            yield return true;
            OperationMessage = "Retracted";
            yield return false;
        }


        void LoadBlocks() {
            PistonList.Clear();
            var group = GridTerminalSystem.GetBlockGroupWithName(GROUP_ALL_PISTON);
            group?.GetBlocksOfType(PistonList);

            //WelderList.Clear();
            //group = GridTerminalSystem.GetBlockGroupWithName(GROUP_WELDERS);
            //group?.GetBlocksOfType(WelderList);
        }

        bool IsExtended(IMyPistonBase piston) => Math.Round(piston.CurrentPosition, 3) >= Math.Round(piston.MaxLimit, 3);
        bool IsReteacted(IMyPistonBase piston) => Math.Round(piston.CurrentPosition, 3) <= Math.Round(piston.MinLimit, 3);

        //void ShowStates(string msg, bool state) {
        //    Debug($"{msg}: {state}");
        //    Debug($"Piston Pos: {PistonList.First().CurrentPosition}");
        //    Debug($"Piston MIN: {PistonList.First().MinLimit}");
        //    Debug($"Piston MAX: {PistonList.First().MaxLimit}");
        //    Debug($"Welder state: {WelderList.First().Enabled}");
        //}
    }
}
