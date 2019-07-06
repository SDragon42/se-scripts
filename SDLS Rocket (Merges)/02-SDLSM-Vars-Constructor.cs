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

        public Action<string> Debug = (msg) => { };

        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();
        readonly string Instructions;

        //Modules
        readonly Config config = new Config();

        public Program() {

            //Commands.Add("", null);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            //Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo("");
            Echo(Instructions);
            config.LoadConfig(Me);
        }


        

        class Config {
            public Config() { }

            const string SEC_RocketTags = "SDLS Rocket Tags";

            int _configHash = 0;

            public string PodTag { get; set; } = string.Empty;

            public void LoadConfig(IMyProgrammableBlock me) {
                if (_configHash == me.CustomData.GetHashCode()) return;

                var ini = new MyIni();
                ini.TryParse(me.CustomData);

                PodTag = ini.Add(SEC_RocketTags, "Pod", PodTag).ToString();

                var newConfig = ini.ToString();
                var newConfigHash = newConfig.GetHashCode();
                if (newConfigHash != _configHash) {
                    me.CustomData = newConfig;
                    _configHash = newConfigHash;
                }
            }

        }
    }
}
