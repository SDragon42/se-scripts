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

        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();
        TimeSpan UpTime = TimeSpan.Zero;
        bool IsMasterGrid = false;
        RocketStructure Structure = RocketStructure.None;

        // Modules
        readonly RunningSymbol Running = new RunningSymbol();
        readonly Logging Log = new Logging(40);
        readonly DebugLogging Debug;
        readonly FlightDataRecorder Fdr;
        readonly VectorAlign VecAlign = new VectorAlign();
        readonly BlocksByOrientation Orientation = new BlocksByOrientation();
        readonly StateMachineQueue<bool> QueueSequence = new StateMachineQueue<bool>();
        readonly StateMachineQueue<bool> QueueGravityAlign = new StateMachineQueue<bool>();


        // Local Grid Only
        IMyShipController Remote = null;
        IMyRadioAntenna Antenna = null;
        IMyBeacon Beacon = null;
        readonly List<IMyParachute> Parachutes = new List<IMyParachute>();
        readonly List<IMyGyro> Gyros = new List<IMyGyro>();
        readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();
        readonly List<IMyThrust> AllThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> ManeuverThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> LandingThrusters = new List<IMyThrust>();
        readonly List<IMyGasTank> H2Tanks = new List<IMyGasTank>();

        // Full Rocket Stack
        readonly List<IMyGyro> StackGyros = new List<IMyGyro>();
        readonly List<IMyMotorStator> StackLaunchClamps = new List<IMyMotorStator>();
        readonly List<IMyMotorStator> StackStageClamps = new List<IMyMotorStator>();
        readonly List<IMyMotorStator> StackBoosterClamps = new List<IMyMotorStator>();
        readonly List<IMyThrust> StackAscentThrusters = new List<IMyThrust>();
        readonly List<IMyThrust> StackStageThrusters = new List<IMyThrust>();
        readonly List<IMyGasTank> StackH2Tanks = new List<IMyGasTank>();
        readonly List<IMyProgrammableBlock> GridPrograms = new List<IMyProgrammableBlock>();


        public Program() {
            IsMasterGrid = false;
            Debug = new DebugLogging(this) {
                EchoMessages = true,
                Enabled = true
            };

            Fdr = new FlightDataRecorder(new string[] { "" }, 1000) {
                Enabled = true
            };

            Commands.Add("scan", CMD_ScanGrids);
            Commands.Add("off", CMD_Off);
            Commands.Add("standby", CMD_MG_Standby);
            Commands.Add("launch", CMD_MG_Lanuch);

            TagSelf();
            NameGrids();
        }

        public void Save() {

        }

        bool localBlocksLoaded = false;
        void LoadLocalBlocks(bool force = false) {
            if (localBlocksLoaded && !force) return;

            Remote = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>(IsOnThisGrid);
            Antenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(IsOnThisGrid);
            Beacon = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyBeacon>(IsOnThisGrid);

            Orientation.Init(Remote);

            GridTerminalSystem.GetBlocksOfType(Parachutes, IsOnThisGrid);
            GridTerminalSystem.GetBlocksOfType(Gyros, IsOnThisGrid);
            GridTerminalSystem.GetBlocksOfType(LandingGears, IsOnThisGrid);
            GridTerminalSystem.GetBlocksOfType(StackLaunchClamps, b => IsOnThisGrid(b) && Collect.IsTagged(b, TAG.LAUNCH_CLAMP));
            GridTerminalSystem.GetBlocksOfType(StackStageClamps, b => IsOnThisGrid(b) && Collect.IsTagged(b, TAG.STAGE1_CLAMP));
            //GridTerminalSystem.GetBlocksOfType(LaunchClamps, IsOnThisGrid);
            //GridTerminalSystem.GetBlocksOfType(StageClamps, b => IsOnThisGrid(b) && Orientation.IsUp(b));
            GridTerminalSystem.GetBlocksOfType(StackBoosterClamps, b => IsOnThisGrid(b) && (Orientation.IsLeft(b) || Orientation.IsRight(b)));

            // Thrusters
            ManeuverThrusters.Clear();
            StackStageThrusters.Clear();
            StackAscentThrusters.Clear();
            GridTerminalSystem.GetBlocksOfType(AllThrusters, b => {
                if (!IsOnThisGrid(b)) return false;
                if (Collect.IsTagged(b, TAG.MANEUVER)) ManeuverThrusters.Add(b);
                if (Collect.IsTagged(b, TAG.MAIN)) StackAscentThrusters.Add(b);
                return true;
            });

            // Staging Thrusters
            StackStageThrusters.AddRange(AllThrusters.Where(Orientation.IsUp)); // slow down thrusters
            if (StackStageClamps.Count > 0) {
                var a = StackStageClamps[0].Orientation.TransformDirectionInverse(Base6Directions.Direction.Down);
                var st = ManeuverThrusters.Where(t => a == t.Orientation.TransformDirection(Base6Directions.Direction.Forward));
                StackStageThrusters.AddRange(st);
            }

            //Debug.AppendLine($"Gyros: {Gyros.Count}");
            //Debug.AppendLine($"Parachutes: {Parachutes.Count}");
            //Debug.AppendLine($"Launch Clamps: {LaunchClamps.Count}");
            //Debug.AppendLine($"Stage Clamps: {StageClamps.Count}");
            //Debug.AppendLine($"Booster Clamps: {BoosterClamps.Count}");
            //Debug.AppendLine($"Ascent T: {AscentThrusters.Count}");
            //Debug.AppendLine($"ManeuverThrusters T: {ManeuverThrusters.Count}");
            //Debug.AppendLine($"Staging T: {StageThrusters.Count}");
            //Debug.AppendLine($"Landing1 T: {LandingThrusters1.Count}");
            //Debug.AppendLine($"Landing2 T: {LandingThrusters2.Count}");
            //Debug.AppendLine($"Landing3 T: {LandingThrusters3.Count}");

            localBlocksLoaded = true;
        }
        void LoadStackBlocks() {

        }

    }
}
