using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
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
    public partial class Program : MyGridProgram {

        enum MyDrillState {
            Initializing = 0,
            Stopped = 1,
            Drilling = 2,
            OnHold = 3
        }

        readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        readonly string _instructions;
        readonly Configuration _config = new Configuration();


        StateMachineQueue _operationQueue = new StateMachineQueue();


        public Program() {
            _commands.Add("reset", () => {});
            _commands.Add("start", StartDrilling);
            _commands.Add("stop", StopDrilling);
            _commands.Add("hold", HoldDrilling);

            // Initialize the configuration
            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();

            // Load the current state from storage
            var state = new StateConfig(Storage);
        }

        public void Save() {
            // Save the current state to storage
            var state = new StateConfig();
            Storage = state.ToString();
        }

        public void Main(string argument, UpdateType updateSource) {
            _operationQueue.Run();
        }


        // #####################################################################
        class Configuration : ConfigBase {
            public string TagName { get; private set; } = "Tunnel-Drill";
            public float ExtensionVelocity { get; private set; } = 0.08f;
            public float RetractionVelocity { get; private set; } = 0.5f;
            public float HoldCargoPercentage { get; private set; } = 0.95f;
            public float ResumeCargoPercentage { get; private set; } = 0.5f;

            const string SECTION_NAME = "Tunnel-Drill";
            public void Load() {
                if (!LoadIni()) return;

                _ini.AddSection(SECTION_NAME);
                _ini.SetSectionComment(SECTION_NAME, "Configuration for the tunnel drill");

                TagName = _ini.Add(SECTION_NAME, "TagName", TagName).ToString();
                ExtensionVelocity = _ini.Add(SECTION_NAME, "ExtensionVelocity", ExtensionVelocity).ToSingle();
                RetractionVelocity = _ini.Add(SECTION_NAME, "RetractionVelocity", RetractionVelocity).ToSingle();
                HoldCargoPercentage = Math.Min(1f, _ini.Add(SECTION_NAME, "HoldCargoPercentage", HoldCargoPercentage).ToSingle());
                ResumeCargoPercentage = Math.Max(0f, _ini.Add(SECTION_NAME, "ResumeCargoPercentage", ResumeCargoPercentage).ToSingle());

                // Ensure ResumeCargoPercentage is less than HoldCargoPercentage
                if (ResumeCargoPercentage >= HoldCargoPercentage) {
                    ResumeCargoPercentage = Math.Max(0f, HoldCargoPercentage - 0.2f);
                }
                
                Save();
            }
        }

        // #####################################################################
        class StateConfig {

            public MyDrillState State { get; set; } = MyDrillState.Initializing;


            readonly MyIniKey StateKey = new MyIniKey("Drill-Control", "State");

            public StateConfig() { }
            public StateConfig(string storage) {
                var ini = new MyIni();
                if (!ini.TryParse(storage)) return;

                // State = (MyDrillState)ini.Add(StateKey, State).ToInt32();
            }

            public override string ToString() {
                var ini = new MyIni();

                // ini.Add(StateKey, (int)State);

                return ini.ToString();
            }

        }

    }
}
