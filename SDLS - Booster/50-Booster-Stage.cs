//using Sandbox.Game.EntityComponents;
//using Sandbox.ModAPI.Ingame;
//using Sandbox.ModAPI.Interfaces;
//using SpaceEngineers.Game.ModAPI.Ingame;
//using System.Collections.Generic;
//using System.Collections;
//using System.Linq;
//using System.Text;
//using System;
//using VRage.Collections;
//using VRage.Game.Components;
//using VRage.Game.ModAPI.Ingame;
//using VRage.Game.ObjectBuilders.Definitions;
//using VRage.Game;
//using VRageMath;

//namespace IngameScript {
//    partial class Program {
//        IEnumerator<bool> Sequence_GravAlign() {
//            while (true) {
//                VAlign.AlignWithGravity(Remote, Direction.Down, Gyros);
//                yield return true;
//            }
//        }

//        IEnumerator<bool> Sequence_Stage() {
//            TimeSpan start = UpTime;
//            TimeSpan delay;

//            Action<string> WriteLog = (msg) => {
//                var diff = UpTime.Subtract(start);
//                Log.AppendLine($"{diff.Minutes,-2:N0}:{diff.Seconds,-2:N0} - " + msg);
//            };

//            WriteLog("Stage Start");
//            // Enable Stage Thrusters
//            ManeuverThrusters.ForEach(t => t.Enabled = true);
//            StageThrusters.ForEach(t => { t.Enabled = true; t.ThrustOverridePercentage = 1F; });
//            yield return true;

//            // Detach booster
//            WriteLog("Detach");
//            StageClamps.ForEach(r => r.Detach());

//            // Wait 2 seconds
//            Remote.DampenersOverride = false;
//            delay = UpTime;
//            yield return true;
//            while (UpTime.Subtract(delay).TotalSeconds < 2.0) yield return true;

//            // Wait 10 seconds
//            delay = UpTime;
//            StageThrusters.ForEach(t => t.ThrustOverridePercentage = 0F);
//            yield return true;
//            while (UpTime.Subtract(delay).TotalSeconds < 10.0) yield return true;

//            // Turn on all Maneuver Thrusters
//            Operations.Add("VAlign", Sequence_GravAlign());
//            WriteLog("Stop Drift");
//            Remote.DampenersOverride = true;
//            AscentThrusters.ForEach(DisableThruster);
//            Antenna.Enabled = true;
//            Antenna.EnableBroadcasting = true;
//            yield return true;

//            // Wait till Alt < 5000
//            while (true) {
//                double elevation;
//                if (!Remote.TryGetPlanetElevation(MyPlanetElevation.Surface, out elevation)) yield return true;
//                if (elevation < 5000) break;
//                yield return true;
//            }

//            // setup for final descent
//            WriteLog("Init Final");
//            Parachutes.ForEach(p => p.Enabled = true);
//            Remote.DampenersOverride = false;
//            LandingGears.ForEach(g => g.AutoLock = true);
//            //Beacon.Enabled = true;
//            var lastPos = Remote.GetPosition();
//            yield return true;

//            // Wait till no longer moving
//            while (true) {
//                var currPos = Remote.GetPosition();
//                var dis = Vector3D.Distance(lastPos, currPos);
//                if (Math.Round(dis, 3) <= 0) break;
//                if (LandingGears.Any(Collect.IsLandingGearLocked)) break;
//                lastPos = currPos;
//                yield return true;
//            }

//            // Secure for recovery
//            WriteLog("Secure for recovery");
//            AllThrusters.ForEach(DisableThruster);
//            LandingGears.ForEach(g => g.AutoLock = false);

//            // Pulse Beacon
//            if (Beacon == null) {
//                yield return false;
//            } else {
//                delay = UpTime;
//                double delayAmount = 0.0;
//                while (true) {
//                    var diff = UpTime.Subtract(delay).TotalSeconds;
//                    if (diff >= delayAmount) {
//                        Beacon.Enabled = !Beacon.Enabled;
//                        delay = UpTime;
//                        delayAmount = Beacon.Enabled
//                            ? 2.0
//                            : 5.0;
//                    }
//                    yield return true;
//                }
//            }
//        }

//        static void DisableThruster(IMyThrust t) {
//            t.Enabled = false;
//            t.ThrustOverridePercentage = 0F;
//        }
//    }
//}
