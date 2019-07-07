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

        public Action<string> Debug = (msg) => { };



        public Program() {
            Log = new DebugLogging(this, "DEBUG");
            Debug = (msg) => { Echo(msg); Log.AppendLine(msg); };

            Commands.Add("launch", CMD_Launch);
            Commands.Add("await-staging", CMD_AwaitStaging);
            Commands.Add("shutdown", CMD_Shutdown);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
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
