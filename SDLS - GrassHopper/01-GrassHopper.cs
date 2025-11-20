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
    partial class Program : MyGridProgram {
        const string TAG_LAUNCH = "[main]";
        const string TAG_LANDING1 = "[landing-1]";
        const string TAG_LANDING2 = "[landing-2]";
        const string TAG_MANEUVER = "[maneuver]";

        const string GPS_LZ1 = "GPS:LZ-1/Grasshopper:-20189.71:18615.27:54263.73:";
        const string GPS_LZ2 = "GPS:LZ-2/Grasshopper:-19891.45:18820.13:54302.99:";

        //readonly Vector3D LZ1_Vector;
        //readonly Vector3D LZ2_Vector;

        readonly RunningSymbol RunSymbol = new RunningSymbol();
        readonly Logging Log = new Logging(20);
        readonly VectorAlign VecAlign = new VectorAlign();
        readonly StateMachineQueue SequenceVectorAlign = new StateMachineQueue();
        readonly StateMachineQueue SequenceGrassHop = new StateMachineQueue();


        // Blocks
        IMyShipController shipController = null;
        readonly List<IMyThrust> Thrusters_Main = new List<IMyThrust>();
        readonly List<IMyThrust> Thrusters_Landing1 = new List<IMyThrust>();
        readonly List<IMyThrust> Thrusters_Landing2 = new List<IMyThrust>();
        readonly List<IMyThrust> Thrusters_Maneuver = new List<IMyThrust>();
        readonly List<IMyGyro> Gyros = new List<IMyGyro>();
        readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();


        public Program() {
            //LZ1_Vector = VectorHelper.GpsToVector(GPS_LZ1);
            //LZ2_Vector = VectorHelper.GpsToVector(GPS_LZ2);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            LoadConfig();
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo("GrassHopper " + RunSymbol.GetSymbol(Runtime));
            LoadConfig();
            LoadBlocks();
            try {
                // Process Arguments
                ProcessArguments(argument);

                // Run State Machines
                if ((updateSource & UpdateType.Update10) == UpdateType.Update10) {
                    if (!SequenceVectorAlign.Run()) SequenceVectorAlign.Clear();
                    if (!SequenceGrassHop.Run()) SequenceGrassHop.Clear();
                }

                // Display LOG
                Echo(Log.GetLogText());
            } catch (Exception ex) {
                // Display LOG
                Echo(Log.GetLogText());
                Echo("*************");
                Echo(ex.ToString());
                throw;
            }
        }

        void ProcessArguments(string argument) {
            if (argument.Length == 0) return;
            Log.AppendLine($"{DateTime.Now:t} - {argument}");
            switch (argument?.ToLower()) {
                case "go":
                    if (SequenceVectorAlign.HasTasks) break;
                    SequenceVectorAlign.Add(Sequence_GravityAlignOn());

                    startPos = shipController.GetPosition();
                    addSequenceRange(SequenceGrassHop,
                        Delay(1.0),
                        FlightTest_Init(),
                        Delay(1.0),
                        FlightTest_GearUnlock(),
                        FlightTest_FlyUp(flyDistance),
                        FlightTest_Hover(),
                        Delay(5.0),
                        FlightTest_DropTo(powerDescentDistance),
                        FlightTest_FlyDown(),
                        FlightTest_Shutdown(),
                        Delay(0.5),
                        FlightTest_GearLock()
                        );

                    break;

                case "abort":
                    SequenceGrassHop.Clear();
                    SequenceGrassHop.Add(FlightTest_Abort());
                    SequenceVectorAlign.Clear();
                    SequenceVectorAlign.Add(Sequence_GravityAlignOff());
                    break;

                case "fly":
                    SequenceGrassHop.Clear();
                    SequenceGrassHop.Add(FlightTest_Manual());

                    SequenceVectorAlign.Clear();
                    SequenceVectorAlign.Add(Sequence_GravityAlignOff());
                    break;
            }
        }

        void addSequenceRange(StateMachineQueue sm, params IEnumerator<bool>[] steps) {
            foreach (var step in steps)
                sm.Add(step);
        }

        Vector3D startPos = Vector3D.Zero;



        double GetGravityForceN(IMyShipController sc) {
            var gVector = sc.GetNaturalGravity();
            var gMs = Math.Sqrt(
                Math.Pow(gVector.X, 2) +
                Math.Pow(gVector.Y, 2) +
                Math.Pow(gVector.Z, 2));
            var mass = sc.CalculateShipMass().PhysicalMass;
            var gravityForce = mass * gMs;
            return gravityForce;
        }

        void Thruster_Off_NoThrust(IMyThrust t) {
            t.Enabled = false;
            t.ThrustOverride = 0f;
        }
        void Thruster_Off_FullThrust(IMyThrust t) {
            t.Enabled = false;
            t.ThrustOverride = t.MaxThrust;
        }
        void Thruster_On_NoThrust(IMyThrust t) {
            t.Enabled = true;
            t.ThrustOverride = 0f;
        }
        void Thruster_On(IMyThrust t) => t.Enabled = true;

        void GtsBlocks<T>(List<T> list, string tag) where T : class, IMyTerminalBlock {
            GridTerminalSystem.GetBlocksOfType(list, b => b.IsSameConstructAs(Me) && Collect.IsTagged(b, tag));
        }


        void LoadBlocks() {
            if (shipController != null) return;
            shipController = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>();

            GtsBlocks(Thrusters_Main, TAG_LAUNCH);
            GtsBlocks(Thrusters_Landing1, TAG_LANDING1);
            GtsBlocks(Thrusters_Landing2, TAG_LANDING2);
            GtsBlocks(Thrusters_Maneuver, TAG_MANEUVER);
            GridTerminalSystem.GetBlocksOfType(Gyros);
            GridTerminalSystem.GetBlocksOfType(LandingGears);
        }


        const string SECTION_Alpha = "Alpha";
        readonly MyIniKey KEY_Distance = new MyIniKey(SECTION_Alpha, "Distance");
        readonly MyIniKey KEY_PoweredDistance = new MyIniKey(SECTION_Alpha, "Powered Distance");

        int configHash = 0;
        readonly MyIni ini = new MyIni();
        void LoadConfig() {
            if (configHash == Me.CustomData.GetHashCode()) return;

            ini.TryParse(Me.CustomData);

            flyDistance = ini.Add(KEY_Distance, flyDistance).ToDouble();
            powerDescentDistance = ini.Add(KEY_PoweredDistance, powerDescentDistance).ToDouble();

            Me.CustomData = ini.ToString();
            configHash = Me.CustomData.GetHashCode();
        }

        double flyDistance = 1000;
        double powerDescentDistance = 500;

        //  Flight calculations
        Vector3D _gravityVector;
        double _gravityMs2;
        bool _inNaturalGravity;
        double _actualMass;
        //double _cargoMass;
        double _gravityForceOnShip;
        //double _rangeToGround, _rangeToGroundLast, _rangeToSpace, _rangeToDestination;
        //double _verticalSpeed;
        //double _h2TankFilledPercent;

        void Calculations() {
            _gravityVector = shipController.GetNaturalGravity();
            _inNaturalGravity = !Vector3D.IsZero(_gravityVector);
            _gravityMs2 = Math.Sqrt(
                Math.Pow(_gravityVector.X, 2) +
                Math.Pow(_gravityVector.Y, 2) +
                Math.Pow(_gravityVector.Z, 2));

            var shipMass = shipController.CalculateShipMass();
            _actualMass = shipMass.BaseMass + ((shipMass.TotalMass - shipMass.BaseMass) / 1);
            _gravityForceOnShip = _actualMass * _gravityMs2;


        }
    }
}
