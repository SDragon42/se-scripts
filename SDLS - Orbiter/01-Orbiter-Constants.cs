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
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program : MyGridProgram {

        const string CMD_OFF = "off";
        const string CMD_MANUAL = "manual";
        const string CMD_STANDBY = "standby";
        const string CMD_SCAN = "scan";

        const string RCFG_ORBITER = "";





        // Modules
        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(40);
        readonly DebugLogging Debug;
        readonly FlightDataRecorder Fdr = new FlightDataRecorder(new string[] { "" }, 1000);
        readonly StateMachine<bool> Operations = new StateMachine<bool>();
        readonly BlocksByOrientation Orientation = new BlocksByOrientation();

        //Lists
        readonly List<IMyParachute> Parachutes = new List<IMyParachute>();
        readonly List<IMyGyro> Gyros = new List<IMyGyro>();
        readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();
        readonly List<IMyMotorStator> LaunchClamps = new List<IMyMotorStator>();
        readonly List<IMyMotorStator> StageClamps = new List<IMyMotorStator>();
        readonly List<IMyThrust> AllThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> ManeuverThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> StageThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> AscentThrusters = new List<IMyThrust>();
        readonly List<IMyGasTank> H2Tanks = new List<IMyGasTank>();
        readonly List<IMyTerminalBlock> TempBlocks = new List<IMyTerminalBlock>();
        readonly Dictionary<string, List<IMyGasTank>> StageH2Tank = new Dictionary<string, List<IMyGasTank>>();

        //readonly List<IMyCubeGrid>
        readonly HashSet<IMyCubeGrid> rocketParts = new HashSet<IMyCubeGrid>();

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
            //Debug.Enabled = false;

            //Fdr = new FlightDataRecorder(new string[] { "" }, 1000);
            Fdr.Enabled = false;

            Runtime.UpdateFrequency = UpdateFrequency.None;

            Commands.Add(CMD_OFF, TurnOff);
            Commands.Add(CMD_MANUAL, ManualControl);
            Commands.Add(CMD_SCAN, ScanGrids);
        }

        public void Save() {
        }

    }
}
