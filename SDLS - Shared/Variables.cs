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

        // Modules
        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(40);
        readonly DebugLogging Debug;
        readonly FlightDataRecorder Fdr;
        //readonly StateMachine<bool> Operations = new StateMachine<bool>();
        readonly StateMachineQueue GravityAlign = new StateMachineQueue();
        readonly BlocksByOrientation Orientation = new BlocksByOrientation();
        readonly VectorAlign VecAlign = new VectorAlign();


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
        //readonly List<IMyTerminalBlock> TempBlocks = new List<IMyTerminalBlock>();
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


    }
}
