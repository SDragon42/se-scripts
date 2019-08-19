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
        readonly DebugLogging Log;
        readonly StateMachineSets SequenceSets = new StateMachineSets();

        //Blocks

        readonly List<IMyGyro> Gyros = new List<IMyGyro>();
        readonly List<IMyGasTank> H2Tanks = new List<IMyGasTank>();
        readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();
        readonly List<IMyShipMergeBlock> Merges = new List<IMyShipMergeBlock>();
        readonly List<IMyParachute> Parachutes = new List<IMyParachute>();

        //Thrusters
        readonly List<IMyThrust> ThrustersPrimary = new List<IMyThrust>();
        readonly List<IMyThrust> ThrustersSecondary = new List<IMyThrust>();
        //readonly List<IMyThrust> BoosterMainThrusters => GetTaggedBlocks(Thrusters, Cfg.BoosterTag);
        //readonly List<IMyThrust> Stage1MainThrusters => GetTaggedBlocks(Thrusters, Cfg.Stage1Tag);
        //readonly List<IMyThrust> Stage2MainThrusters => GetTaggedBlocks(Thrusters, Cfg.Stage2Tag);
        //readonly List<IMyThrust> PodMainThrusters => GetTaggedBlocks(Thrusters, Cfg.PodTag);

        //Other
        bool IsStructureInited = false;

        RocketStructure Structure = RocketStructure.Unknown;
        string StructureTag = string.Empty;
        RocketStructure RocketType = RocketStructure.Unknown;

        FlightMode Mode = FlightMode.Off;

        Action<string> Debug = (msg) => { };



        public Program() {
            Log = new DebugLogging(this, "Cockpit [pod]", 3);
            Log.EchoMessages = true;
            Log.Enabled = true;
            Log.MaxTextLinesToKeep = 20;
            Debug = (msg) => Log.AppendLine($"{DateTime.Now:HHmmss.fff} " + msg);

            Commands.Add("check", CMD_CheckReadyToLaunch);
            Commands.Add("launch", CMD_Launch);
            Commands.Add("await-staging", CMD_AwaitStaging);
            Commands.Add("shutdown", CMD_Shutdown);
            Commands.Add("init", CMD_Init);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            Load();
        }

        void Load() {
            //var parts = Storage.Split('|');
            //if (parts.Length <= 1) return;
            //var i = 1;
            //switch (parts[0]) {
            //    case "1":
            //        if (parts.Length != 5) return;
            //        bool.TryParse(parts[i++], out IsStructureInited);
            //        bool.TryParse(parts[i++], out IsMaster);
            //        int tmp = 0;
            //        int.TryParse(parts[i++], out tmp);
            //        Structure = (RocketStructure)tmp;

            //        int.TryParse(parts[i++], out tmp);
            //        Mode = (FlightMode)tmp;

            //        break;
            //}
        }
        public void Save() {
            // Save State
            //Storage = "1" +
            //    $"|{IsStructureInited}" +
            //    //$"|{IsMaster}" +
            //    $"|{(int)RocketType}" +
            //    $"|{(int)Structure}" +
            //    $"|{(int)Mode}" +
            //    $"";
        }

        IEnumerable<T> GetTaggedBlocks<T>(List<T> blocks, string tag) where T : IMyTerminalBlock {
            return blocks.Where(b => b.CustomName.Contains(tag));
        }

    }
}
