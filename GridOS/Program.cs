// <mdk sortorder="10" />
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

        readonly RunningSymbol _running = new RunningSymbol();
        readonly AutoDoorCloser _autoDoorCloser = new AutoDoorCloser();
        readonly StateMachineSets _sequences = new StateMachineSets();

        readonly Config _config = new Config();

        readonly List<IMyDoor> _autoDoors = new List<IMyDoor>();
        readonly List<IMyDoor> _airlockDoors = new List<IMyDoor>();

        readonly Dictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        readonly string _instructions;

        double _blockReload_TimeElapsed = 0;

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            // Initialize and load configuration
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load(_autoDoorCloser);

            // Commands
            _commands.Add("openhangar", null);
            _commands.Add("closehangar", null);
            _commands.Add("togglehangar", null);
            _commands.Add("closedoors", CloseDoors);

            // Instructions
            _instructions = "Script Commands\n" + string.Join("\n", _commands.Keys);
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            _blockReload_TimeElapsed += Runtime.TimeSinceLastRun.TotalSeconds;
            Echo($"Grid OS {_running.GetSymbol()}");
            Echo(_instructions);
            Echo($"Block Reload in {Math.Truncate(_config.BlockReloadTime - _blockReload_TimeElapsed) + 1:N0} seconds.");

            _config.Load(_autoDoorCloser);
            LoadBlocks();

            ParseArgs(argument);
            if (_commands.ContainsKey(_argCmd)) _commands[_argCmd]?.Invoke();

            if (_config.ADCEnabled) _autoDoorCloser.CloseOpenDoors(Runtime, _autoDoors, Me);
        }

        string _argCmd = "";
        string _argParams = "";
        void ParseArgs(string argument) {
            _argCmd = "";
            _argParams = "";
            argument = argument?.ToLower() ?? string.Empty;
            var argParts = argument.Split(new char[] { ' ' }, 2);
            if (argParts.Length >= 1) _argCmd = argParts[0];
            if (argParts.Length >= 2) _argParams = argParts[1];
        }


        void LoadBlocks(bool forceLoad = false) {
            if (!forceLoad && _blockReload_TimeElapsed < _config.BlockReloadTime) return;

            GridTerminalSystem.GetBlocksOfType(_autoDoors, b =>
                Me.IsSameConstructAs(b)
                && !IsTagged(b, _config.ADCExclusionTag)
                && !IsTagged(b, _config.AirlockTag)
                && !IsHangarDoor(b));
            GridTerminalSystem.GetBlocksOfType(_airlockDoors, b =>
                Me.IsSameConstructAs(b)
                && IsTagged(b, _config.AirlockTag));

            _blockReload_TimeElapsed = 0;
        }


        void CloseDoors() => _autoDoors.ForEach(d => d.CloseDoor());
        void OpenHangar() {
            _sequences.Add("hangar_" + _argParams, OpenHangar_Sequence());
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
