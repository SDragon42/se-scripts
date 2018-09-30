﻿using Sandbox.Game.EntityComponents;
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
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        // Modules
        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(100);
        readonly DebugLogging Debug;
        readonly StateMachine<bool> Operations = new StateMachine<bool>();
        readonly BlocksByOrientation Orientation = new BlocksByOrientation();
        readonly VectorAlign VAlign = new VectorAlign();

        // Lists
        readonly List<IMyParachute> Parachutes = new List<IMyParachute>();
        readonly List<IMyGyro> Gyros = new List<IMyGyro>();
        readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();
        //readonly List<IMyMotorStator> LaunchClamps = new List<IMyMotorStator>();
        readonly List<IMyMotorStator> StageClamps = new List<IMyMotorStator>();
        readonly List<IMyThrust> AllThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> ManeuverThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> StageThrusters = new List<IMyThrust>();
        //readonly List<IMyThrust> LandingThrusters1 = new List<IMyThrust>();
        //readonly List<IMyThrust> LandingThrusters2 = new List<IMyThrust>();
        //readonly List<IMyThrust> LandingThrusters3 = new List<IMyThrust>();
        readonly List<IMyThrust> AscentThrusters = new List<IMyThrust>();
        readonly List<IMyGasTank> H2Tanks = new List<IMyGasTank>();

        // Single Blocks
        IMyRemoteControl Remote = null;
        IMyRadioAntenna Antenna = null;
        IMyBeacon Beacon = null;

        // Other
        TimeSpan UpTime = TimeSpan.Zero;
        bool BlocksLoaded = false;

        readonly Dictionary<string, Action> Commands = new Dictionary<string, Action>();

        public Program() {
            Debug = new DebugLogging(this);
            Debug.EchoMessages = true;
            Debug.Enabled = false;

            Commands.Add(CMD_RELOAD, Reload);
            Commands.Add(CMD_STAGE, Stage);
            Commands.Add(CMD_SHUTDOWN, Shutdown);
        }

        public void Save() {
        }


        void LoadBlocks(bool force = false) {
            if (BlocksLoaded && !force) return;

            Remote = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>(IsSameGrid);
            Antenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(IsSameGrid);
            Beacon = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyBeacon>(IsSameGrid);

            Orientation.Init(Remote);

            GridTerminalSystem.GetBlocksOfType(Parachutes, IsSameGrid);
            GridTerminalSystem.GetBlocksOfType(Gyros, IsSameGrid);
            GridTerminalSystem.GetBlocksOfType(LandingGears, IsSameGrid);
            GridTerminalSystem.GetBlocksOfType(StageClamps, b => IsSameGrid(b) && Collect.IsTagged(b, TAG_STAGING_CLAMP));
            GridTerminalSystem.GetBlocksOfType(H2Tanks, IsSameGrid);

            // Thrusters
            ManeuverThrusters.Clear();
            StageThrusters.Clear();
            AscentThrusters.Clear();
            GridTerminalSystem.GetBlocksOfType(AllThrusters, b => {
                if (!IsSameGrid(b)) return false;
                if (Collect.IsTagged(b, TAG_MANEUVER)) ManeuverThrusters.Add(b);
                if (Collect.IsTagged(b, TAG_MAIN)) AscentThrusters.Add(b);
                return true;
            });

            // Staging Thrusters
            StageThrusters.AddRange(AllThrusters.Where(Orientation.IsUp)); // slow down thrusters
            if (StageClamps.Count > 0) {
                var a = StageClamps[0].Orientation.TransformDirectionInverse(Base6Directions.Direction.Down);
                var st = ManeuverThrusters.Where(t => a == t.Orientation.TransformDirection(Base6Directions.Direction.Forward));
                StageThrusters.AddRange(st);
            }

            Debug.AppendLine($"Gyros: {Gyros.Count}");
            Debug.AppendLine($"Parachutes: {Parachutes.Count}");
            Debug.AppendLine($"Stage Clamps: {StageClamps.Count}");
            Debug.AppendLine($"Ascent T: {AscentThrusters.Count}");
            Debug.AppendLine($"ManeuverThrusters T: {ManeuverThrusters.Count}");
            Debug.AppendLine($"Staging T: {StageThrusters.Count}");

            BlocksLoaded = true;
        }

        bool IsSameGrid(IMyTerminalBlock b) => Me.CubeGrid == b.CubeGrid;
    }
}
