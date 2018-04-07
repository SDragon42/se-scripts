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
        IEnumerator<bool> Sequence_Stage() {
            TimeSpan start = UpTime;
            TimeSpan delay;

            Action<string> WriteLog = (msg) => {
                var diff = UpTime.Subtract(start);
                Log.AppendLine($"{diff.Minutes,-2:N0}:{diff.Seconds,-2:N0} - " + msg);
            };

            WriteLog("Stage Start");
            // Enable Stage Thrusters
            ManeuverThrusters.ForEach(t => t.Enabled = true);
            StageThrusters.ForEach(t => { t.Enabled = true; t.ThrustOverridePercentage = 1F; });
            yield return true;

            // Detach booster
            WriteLog("Detach");
            StageClamps.ForEach(r => r.Detach());

            // Wait 2 seconds
            BoosterControl.DampenersOverride = false;
            delay = UpTime;
            yield return true;
            while (UpTime.Subtract(delay).TotalSeconds < 2.0) yield return true;

            // Turn on all Maneuver Thrusters
            WriteLog("Stop Drift");
            BoosterControl.DampenersOverride = true;
            StageThrusters.ForEach(t => t.ThrustOverridePercentage = 0F);
            AscentThrusters.ForEach(DisableThruster);
            Antenna.Enabled = true;
            Antenna.EnableBroadcasting = true;
            yield return true;

            // Wait till Alt < 5000
            while (true) {
                double elevation;
                if (!BoosterControl.TryGetPlanetElevation(MyPlanetElevation.Surface, out elevation)) yield return true;
                if (elevation < 5000) break;
                yield return true;
            }

            // setup for final descent
            WriteLog("Init Final");
            Parachutes.ForEach(p => p.Enabled = true);
            BoosterControl.DampenersOverride = false;
            LandingGears.ForEach(g => g.AutoLock = true);
            //Beacon.Enabled = true;
            var lastPos = BoosterControl.GetPosition();
            yield return true;

            // Wait till no longer moving
            while (true) {
                var currPos = BoosterControl.GetPosition();
                var dis = Vector3D.Distance(lastPos, currPos);
                if (Math.Round(dis, 3) <= 0) break;
                if (LandingGears.Any(Collect.IsLandingGearLocked)) break;
                lastPos = currPos;
                yield return true;
            }

            // Secure for recovery
            WriteLog("Secure for recovery");
            AllThrusters.ForEach(DisableThruster);
            LandingGears.ForEach(g => g.AutoLock = false);
        }

        static void DisableThruster(IMyThrust t) {
            t.Enabled = false;
            t.ThrustOverridePercentage = 0F;
        }
    }
}
