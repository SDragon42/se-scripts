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
        IEnumerator<bool> FlightTest_Hover() {
            Log.AppendLine("HFT - Start");
            const double hoverDist = 50.0;
            var sc = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>() as IMyShipController;
            if (sc == null) {
                Log.AppendLine("HFT - SC not found");
                yield return false;
            }
            GtsBlocks(Thrusters_Launch, TAG_LAUNCH);
            GtsBlocks(Thrusters_Landing1, TAG_LANDING1);
            GtsBlocks(Thrusters_Landing2, TAG_LANDING2);
            GtsBlocks(Thrusters_Maneuver, TAG_MANEUVER);
            GtsBlocks(Thrusters_StageSep, TAG_STAGE_SEP);
            GtsBlocks(LaunchClamps, TAG_LAUNCH_CLAMP);

            yield return true;

            Log.AppendLine("HFT - Init");
            Thrusters_Launch.ForEach(Thruster_Off_NoThrust);
            Thrusters_Maneuver.ForEach(Thruster_Off_NoThrust);
            Thrusters_StageSep.ForEach(Thruster_Off_NoThrust);
            yield return true;

            // Launch
            Log.AppendLine("HFT - Fly Up");
            var startPos = sc.GetPosition();
            sc.DampenersOverride = true;
            var gForceN = getGravityForceN(sc);
            var maxThrust = Thrusters_Landing1.Sum(t => (double)t.MaxEffectiveThrust);

            var hoverThrust = gForceN / Thrusters_Landing1.Count;
            var thrustDiff = (maxThrust - gForceN) / Thrusters_Landing1.Count;

            Thrusters_Maneuver.ForEach(Thruster_On);
            Thrusters_Landing1.ForEach(Thruster_On);

            // Wait for
            while (true) {
                var pos = sc.GetPosition();
                var dist = Vector3D.Distance(startPos, pos);
                if (dist >= hoverDist)
                    break;
                Thrusters_Landing1.ForEach(t => t.ThrustOverride = (float)hoverThrust + 5000f);
                yield return true;
            }

            Log.AppendLine("HFT - Hover");
            Thrusters_Landing1.ForEach(t => t.ThrustOverride = 0f);
            yield return true;

            var timeToWait = 5.0;

            while (timeToWait > 0) {
                timeToWait -= this.Runtime.TimeSinceLastRun.TotalSeconds;

                yield return true;
            }

            Log.AppendLine("HFT - Fly Down");
            Thrusters_Landing1.ForEach(t => t.ThrustOverride = (float)hoverThrust - 5000f);

            while (true) {
                var pos = sc.GetPosition();
                var dist = Vector3D.Distance(startPos, pos);
                if (dist <= 1.0)
                    break;
                yield return true;
            }


            Log.AppendLine("HFT - Shutdown");
            Thrusters_Launch.ForEach(Thruster_Off_NoThrust);
            Thrusters_Maneuver.ForEach(Thruster_Off_NoThrust);
            Thrusters_StageSep.ForEach(Thruster_Off_NoThrust);
            yield return true;
        }
    }
}
