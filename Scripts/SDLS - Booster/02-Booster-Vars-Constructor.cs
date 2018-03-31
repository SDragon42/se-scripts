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
    partial class Program {

        // Modules
        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(100);
        readonly DebugLogging Debug;
        readonly StateMachine Operations = new StateMachine();
        readonly BlocksByOrientation Orientation = new BlocksByOrientation();

        //Lists
        readonly List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
        readonly List<IMyParachute> Parachutes = new List<IMyParachute>();
        readonly List<IMyGyro> Gyros = new List<IMyGyro>();
        readonly List<IMyMotorStator> LaunchClamps = new List<IMyMotorStator>();
        readonly List<IMyMotorStator> StageClamps = new List<IMyMotorStator>();
        readonly List<IMyThrust> AllThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> ManeuverThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> StageThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> BreakingThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> LandingThrusters1 = new List<IMyThrust>();
        readonly List<IMyThrust> LandingThrusters2 = new List<IMyThrust>();
        readonly List<IMyThrust> LandingThrusters3 = new List<IMyThrust>();
        readonly List<IMyThrust> AscentThrusters = new List<IMyThrust>();


        IMyRemoteControl BoosterControl = null;

        public Program() {
            Debug = new DebugLogging(this);
            //Debug.Enabled = false;

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

    }
}
