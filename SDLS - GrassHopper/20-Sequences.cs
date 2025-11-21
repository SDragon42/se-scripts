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

        IEnumerator<bool> Sequence_GravityAlignOn() {
            while (true) {
                VecAlign.AlignWithGravity(shipController, Base6Directions.Direction.Down, Gyros, true);
                yield return true;
            }
        }
        IEnumerator<bool> Sequence_GravityAlignOff() {
            VectorAlign.SetGyrosOff(Gyros);
            yield return true;
        }

        IEnumerator<bool> FlightTest_Abort() {
            Thrusters_Main.ForEach(Thruster_Off_NoThrust);
            Thrusters_Maneuver.ForEach(Thruster_Off_NoThrust);
            yield return true;
        }
        IEnumerator<bool> FlightTest_Manual() {
            Thrusters_Main.ForEach(Thruster_On_NoThrust);
            Thrusters_Maneuver.ForEach(Thruster_On_NoThrust);
            yield return true;
        }

        IEnumerator<bool> FlightTest_Init() {
            Log.AppendLine("HFT - Init");
            Thrusters_Main.ForEach(Thruster_Off_NoThrust);
            Thrusters_Landing1.ForEach(Thruster_Off_NoThrust);
            Thrusters_Landing2.ForEach(Thruster_Off_NoThrust);
            Thrusters_Maneuver.ForEach(Thruster_Off_NoThrust);
            yield return true;
        }
        IEnumerator<bool> FlightTest_GearUnlock() {
            Log.AppendLine("HFT - Gear Unlock");
            LandingGears.ForEach(lg => lg.Unlock());
            yield return true;
        }
        IEnumerator<bool> FlightTest_FlyUp(double hoverDist) {
            // calc target Vector
            //var targetPos = shipController.d

            // Launch
            Log.AppendLine("HFT - Fly Up");
            var lastPos = startPos;
            shipController.DampenersOverride = true;
            var gForceN = GetGravityForceN(shipController);
            var hoverThrust = gForceN / Thrusters_Landing1.Count;

            Thrusters_Maneuver.ForEach(Thruster_On);
            Thrusters_Main.ForEach(t => {
                Thruster_On(t);
                t.ThrustOverridePercentage = 1.0f;
            });

            // Wait for
            while (true) {
                var currentPos = shipController.GetPosition();
                var dist = Vector3D.Distance(startPos, currentPos);
                if (dist >= hoverDist)
                    break;
                yield return true;
            }
            yield return true;
        }
        IEnumerator<bool> FlightTest_Hover() {
            Log.AppendLine("HFT - Hover");
            Thrusters_Main.ForEach(t => t.ThrustOverride = 0f);
            yield return true;
        }
        IEnumerator<bool> FlightTest_DropTo(double dropToDistance) {
            Thrusters_Main.ForEach(Thruster_Off_NoThrust);
            yield return true;

            // Wait for
            while (true) {
                var currentPos = shipController.GetPosition();
                var dist = Vector3D.Distance(startPos, currentPos);
                if (dist <= dropToDistance)
                    break;
                yield return true;
            }

            Thrusters_Main.ForEach(Thruster_On_NoThrust);
            yield return true;

            while (true) {
                if (shipController.GetShipSpeed() <= 10.0)
                    break;
                yield return true;
            }
            yield return true;

            Thrusters_Main.ForEach(Thruster_Off_NoThrust);
            Thrusters_Landing1.ForEach(Thruster_On_NoThrust);

            while (true) {
                var current = shipController.GetPosition();

                var gForceN = GetGravityForceN(shipController);
                var hoverThrust = gForceN / Thrusters_Landing1.Count;

                if (shipController.GetShipSpeed() >= 5.0)
                    Thrusters_Landing1.ForEach(t => t.ThrustOverride = (float)hoverThrust);
                var dist = Vector3D.Distance(startPos, current);
                if (dist <= 1.0)
                    break;
                yield return true;
            }
            yield return true;
        }
        IEnumerator<bool> FlightTest_FlyDown() {
            Log.AppendLine("HFT - Fly Down");

            var gForceN = GetGravityForceN(shipController);
            var hoverThrust = gForceN / Thrusters_Landing1.Count;

            Thrusters_Landing1.ForEach(t => t.ThrustOverride = (float)hoverThrust - 30000f);

            while (true) {
                var current = shipController.GetPosition();
                if (shipController.GetShipSpeed() >= 5.0)
                    Thrusters_Landing1.ForEach(t => t.ThrustOverride = (float)hoverThrust);
                var dist = Vector3D.Distance(startPos, current);
                if (dist <= 1.0)
                    break;
                yield return true;
            }
            yield return true;
        }
        IEnumerator<bool> FlightTest_Shutdown() {
            Log.AppendLine("HFT - Shutdown");
            Thrusters_Main.ForEach(Thruster_Off_NoThrust);
            Thrusters_Maneuver.ForEach(Thruster_Off_NoThrust);
            yield return true;
        }
        IEnumerator<bool> FlightTest_GearLock() {
            Log.AppendLine("HFT - Lock Landing Gears");
            LandingGears.ForEach(lg => lg.Lock());
            yield return true;
        }

        IEnumerator<bool> Delay(double secondsToWait) {
            while (secondsToWait > 0) {
                secondsToWait -= this.Runtime.TimeSinceLastRun.TotalSeconds;
                yield return true;
            }
            yield return true;
        }
    }
}
