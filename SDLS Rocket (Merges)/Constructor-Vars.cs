﻿// <mdk sortorder="-10" />
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
    partial class Program {
        readonly Dictionary<string, Action> Commands = new Dictionary<string, Action>();
        readonly string Instructions;

        //Modules
        readonly Config Cfg = new Config();
        readonly RunningSymbol RSymbol = new RunningSymbol();
        readonly DebugLogging Log;
        readonly StateMachineSets SequenceSets = new StateMachineSets();

        //Other
        bool IsStructureInited = false;

        RocketStructure Structure = RocketStructure.Unknown;
        string StructureTag = string.Empty;
        RocketStructure RocketType = RocketStructure.Unknown;

        FlightMode Mode = FlightMode.Off;

        Action<string> Debug = (msg) => { };



        public Program() {
            Log = new DebugLogging(this);
            Log.EchoMessages = true;
            Log.Enabled = true;
            Log.MaxTextLinesToKeep = 20;
            Debug = (msg) => Log.AppendLine($"{DateTime.Now:HHmmss.fff} " + msg);

            Commands.Add(CMD_Check, Command_CheckReadyToLaunch);
            Commands.Add(CMD_Launch, Command_Launch);
            Commands.Add(CMD_AwaitStaging, Command_AwaitStaging);
            Commands.Add(CMD_Shutdown, Command_Shutdown);
            Commands.Add(CMD_Init, Command_Init);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            Load();

            // Should not be needed
            collecter = Me.IsSameConstructAs;
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




        //Blocks
        readonly List<IMyGyro> Gyros = new List<IMyGyro>();
        readonly List<IMyGasTank> H2Tanks = new List<IMyGasTank>();
        readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();
        readonly List<IMyShipConnector> Connectors = new List<IMyShipConnector>();
        readonly List<IMyShipMergeBlock> ConnectedMerges = new List<IMyShipMergeBlock>();
        readonly List<IMyParachute> Parachutes = new List<IMyParachute>();

        IMyRadioAntenna Antenna = null;

        //Thrusters
        readonly List<IMyThrust> ThrustersPrimary = new List<IMyThrust>();
        readonly List<IMyThrust> ThrustersSecondary = new List<IMyThrust>();
        //readonly List<IMyThrust> BoosterMainThrusters => GetTaggedBlocks(Thrusters, Cfg.BoosterTag);
        //readonly List<IMyThrust> Stage1MainThrusters => GetTaggedBlocks(Thrusters, Cfg.Stage1Tag);
        //readonly List<IMyThrust> Stage2MainThrusters => GetTaggedBlocks(Thrusters, Cfg.Stage2Tag);
        //readonly List<IMyThrust> PodMainThrusters => GetTaggedBlocks(Thrusters, Cfg.PodTag);
        readonly List<IMyThrust> ManuverThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> AllThrusters = new List<IMyThrust>();

        Func<IMyTerminalBlock, bool> collecter = null;
        Func<IMyTerminalBlock, bool> collecterPrimary = null;
        Func<IMyTerminalBlock, bool> collecterSecondary = null;

        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(Gyros, collecter);
            GridTerminalSystem.GetBlocksOfType(LandingGears, collecter);
            GridTerminalSystem.GetBlocksOfType(Connectors, collecter);
            GridTerminalSystem.GetBlocksOfType(Parachutes, collecter);
            GridTerminalSystem.GetBlocksOfType(AllThrusters, collecter);
        }

        void LoadInAllProgramBlocks(List<IMyTerminalBlock> list) {
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(list, b => Me.IsSameConstructAs(b) && Collect.IsTagged(b, ScriptName));
        }

    }
}
