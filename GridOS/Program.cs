// <mdk sortorder="10" />
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
    partial class Program : MyGridProgram {

        readonly RunningSymbol symbol = new RunningSymbol();
        readonly AutoDoorCloser autoDoorCloser = new AutoDoorCloser();
        readonly StateMachineSets sequences = new StateMachineSets();

        readonly Config config = new Config();

        readonly List<IMyDoor> autoDoors = new List<IMyDoor>();
        readonly List<IMyDoor> airlockDoors = new List<IMyDoor>();

        readonly Dictionary<string, Action> command = new Dictionary<string, Action>();
        readonly string instructions;

        double blockReload_Time = 10;
        double blockReload_TimeElapsed = 0;

        public Program() {
            config.Load(Me, this);

            command.Add("openhangar", null);
            command.Add("closehangar", null);
            command.Add("togglehangar", null);
            command.Add("closedoors", CloseDoors);

            // Instructions
            instructions = "Script Commands\n" + string.Join("\n", command.Keys.ToArray());

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            blockReload_TimeElapsed += Runtime.TimeSinceLastRun.TotalSeconds;
            Echo($"Grid OS {symbol.GetSymbol(Runtime)}");
            Echo(instructions);
            Echo($"Block Reload in {Math.Truncate(blockReload_Time - blockReload_TimeElapsed) + 1:N0} seconds.");

            config.Load(Me, this);
            LoadBlocks();

            ParseArgs(argument);
            if (command.ContainsKey(argCmd))
                command[argument]?.Invoke();

            if (config.ADCEnabled) autoDoorCloser.CloseOpenDoors(Runtime, autoDoors);
        }

        string argCmd = "";
        string argParams = "";
        void ParseArgs(string argument) {
            argCmd = "";
            argParams = "";
            argument = argument?.ToLower() ?? string.Empty;
            var argParts = argument.Split(new char[] { ' ' }, 2);
            if (argParts.Length >= 1) argCmd = argParts[0];
            if (argParts.Length >= 2) argParams = argParts[2];
        }


        void LoadBlocks(bool forceLoad = false) {
            if (!forceLoad && blockReload_TimeElapsed < blockReload_Time) return;

            GridTerminalSystem.GetBlocksOfType(autoDoors, b =>
                b.IsSameConstructAs(Me)
                && !Collect.IsTagged(b, config.ADCExclusionTag)
                && !Collect.IsTagged(b, config.AirlockTag)
                && !Collect.IsHangarDoor(b));

            GridTerminalSystem.GetBlocksOfType(airlockDoors, b =>
                b.IsSameConstructAs(Me)
                && Collect.IsTagged(b, config.AirlockTag)
                );

            blockReload_TimeElapsed = 0;
        }


        void CloseDoors() {
            autoDoors.ForEach(d => d.CloseDoor());
        }
        void OpenHangar() {


            sequences.Add("hangar_" + argParams, OpenHangar_Sequence());
            // alert sound
            // warning lights

        }
        void CloseHangar() {
        }


        IEnumerator<bool> OpenHangar_Sequence() {
            yield return false;
        }

    }
}
