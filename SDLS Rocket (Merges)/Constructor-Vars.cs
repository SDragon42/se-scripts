// <mdk sortorder="-10" />
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
    partial class Program {
        readonly Dictionary<string, Action> Commands = new Dictionary<string, Action>();
        readonly string Instructions;

        //Modules
        readonly Config Cfg = new Config();
        readonly RunningSymbol RSymbol = new RunningSymbol();
        //readonly Logging Log = new Logging(50);
        readonly DebugLogging Log;
        readonly StateMachineQueue LaunchSequence = new StateMachineQueue();

        //Blocks
        readonly List<IMyThrust> Thrusters = new List<IMyThrust>();
        readonly List<IMyGyro> Gyros = new List<IMyGyro>();
        readonly List<IMyGasTank> H2Tanks = new List<IMyGasTank>();
        readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();
        readonly List<IMyShipMergeBlock> Merges = new List<IMyShipMergeBlock>();
        readonly List<IMyParachute> Parachutes = new List<IMyParachute>();

        //Other
        bool IsInited = false;
        bool IsMaster = false;
        RocketStructure Structure = RocketStructure.Unknown;
        FlightMode Mode = FlightMode.Off;

        Action<string> Debug = (msg) => { };



        public Program() {
            Log = new DebugLogging(this, "Cockpit [pod]", 3);
            Log.EchoMessages = true;
            Log.Enabled = true;
            Debug = (msg) => Log.AppendLine($"{DateTime.Now:HHmmss.fff} " + msg);

            Commands.Add("scan", CMD_Scan);
            Commands.Add("launch", CMD_Launch);
            Commands.Add("await-staging", CMD_AwaitStaging);
            Commands.Add("shutdown", CMD_Shutdown);
            Commands.Add("init", CMD_Init);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            //Runtime.UpdateFrequency = UpdateFrequency.Update10;

            Load();
        }

        void Load() {
            var parts = Storage.Split('|');
            if (parts.Length <= 1) return;
            var i = 1;
            switch (parts[0]) {
                case "1":
                    if (parts.Length != 5) return;
                    bool.TryParse(parts[i++], out IsInited);
                    bool.TryParse(parts[i++], out IsMaster);
                    int tmp = 0;
                    int.TryParse(parts[i++], out tmp);
                    Structure = (RocketStructure)tmp;

                    int.TryParse(parts[i++], out tmp);
                    Mode = (FlightMode)tmp;

                    break;
            }
        }
        public void Save() {
            // Save State
            Storage = "1" +
                $"|{IsInited}" +
                $"|{IsMaster}" +
                $"|{(int)Structure}" +
                $"|{(int)Mode}" +
                $"";
        }

        IEnumerable<T> GetTaggedBlocks<T>(List<T> blocks, string tag) where T : IMyTerminalBlock {
            return blocks.Where(b => b.CustomName.Contains(tag));
        }

        IEnumerable<IMyThrust> BoosterMainThrusters => GetTaggedBlocks(Thrusters, Cfg.BoosterTag);
        IEnumerable<IMyThrust> Stage1MainThrusters => GetTaggedBlocks(Thrusters, Cfg.Stage1Tag);
        IEnumerable<IMyThrust> Stage2MainThrusters => GetTaggedBlocks(Thrusters, Cfg.Stage2Tag);
        IEnumerable<IMyThrust> PodMainThrusters => GetTaggedBlocks(Thrusters, Cfg.PodTag);

    }
}
