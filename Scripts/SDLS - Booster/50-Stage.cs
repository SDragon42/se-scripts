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
        IEnumerator<bool> Sequence_Stage() {

            // Enable Stage Thrusters
            StageThrusters.ForEach(t => t.ThrustOverridePercentage = 1F);
            yield return true;

            // Detach booster
            StageClamps.ForEach(r => r.Detach());
            yield return true;

            BoosterControl.DampenersOverride = false;
            // Wait 2 seconds
            var delay = new TimeInterval(2);
            while (!delay.AtNextInterval) { yield return true; delay.RecordTime(Runtime); }

            // Turn on all Maneuver Thrusters
            BoosterControl.DampenersOverride = true;
            StageThrusters.ForEach(t => t.ThrustOverridePercentage = 0F);
            AscentThrusters.ForEach(DisableThruster);

            BreakingThrusters.ForEach(t => { t.Enabled = true; t.ThrustOverridePercentage = 1F; });
            //yield return true;

            // Wait 2 seconds
            delay = new TimeInterval(2);
            while (!delay.AtNextInterval) { yield return true; delay.RecordTime(Runtime); }

            // setup for final descent
            BreakingThrusters.ForEach(DisableThruster);
            Parachutes.ForEach(p => p.Enabled = true);
            BoosterControl.DampenersOverride = false;
            LandingGears.ForEach(g => g.AutoLock = true);
            Antenna.Enabled = true;
            Antenna.EnableBroadcasting = true;
            //Beacon.Enabled = true;
            yield return true;

            var lastPos = BoosterControl.GetPosition();
            yield return true;

            while (true) {
                var currPos = BoosterControl.GetPosition();
                var dis = Vector3D.Distance(lastPos, currPos);
                if (Math.Round(dis, 3) <= 0) break;
                if (LandingGears.Any(Collect.IsLandingGearLocked)) break;
                lastPos = currPos;
                yield return true;
            }

            AllThrusters.ForEach(DisableThruster);
            LandingGears.ForEach(g => g.AutoLock = false);
        }

        static void DisableThruster(IMyThrust t) {
            t.Enabled = false;
            t.ThrustOverridePercentage = 0F;
        }
    }
}
