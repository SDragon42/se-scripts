// <mdk sortorder="2000" />
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
        public static class ThrusterHelper {
            public const float StandardGravity = 1.0f;
            public const float StandardGravityMagnitude = 9.81f;

            // Used for debugging purposes, can be set to a custom action to log messages.
            public static Action<string> Debug = (t) => { };


            public static float CalculateEffectiveTWR(IMyShipController sc, List<IMyThrust> liftThrusters, float gravity = float.NaN)
                => CalculateTWR(sc, liftThrusters, t => t.MaxEffectiveThrust, gravity);

            public static float CalculateMaxTWR(IMyShipController sc, List<IMyThrust> liftThrusters, float gravity = float.NaN)
                => CalculateTWR(sc, liftThrusters, t => t.MaxThrust, gravity);

            private static float CalculateTWR(IMyShipController sc, List<IMyThrust> liftThrusters, Func<IMyThrust, float> thrustSelector, float gravity) {
                var gravityMagnitude = GetGravityMagnitude(sc, gravity);
                //Debug($"gravityMagnitude: {gravityMagnitude}");
                var thrust = liftThrusters.Sum(thrustSelector);
                //Debug($"thrust: {thrust}");
                var physicalMass = sc.CalculateShipMass().PhysicalMass;
                var twr = float.IsNaN(gravityMagnitude) || gravityMagnitude <= 0f
                    ? thrust / physicalMass
                    : thrust / (physicalMass * gravityMagnitude);
                //Debug($"twr: {twr}");
                return (float)Math.Round(twr, 2);
            }


            // Calculates the maximum liftable cargo mass for a ship based on the provided thrusters, world inverse multiplier, minimum thrust-to-weight ratio, and gravity.
            // param: "sc" - The ship controller of the grid.
            // param: "thrusters" - The list of thrusters providing lift.
            // param: "worldInvMultiplier" - The world inventory multiplier.
            // param: "minimumTwr" - The minimum thrust-to-weight ratio the grid should maintain.
            // param: "gravity" - The gravity factor (relative to Earth's gravity).
            // returns: The maximum liftable cargo mass.
            public static float CalculateEffectiveLiftableCargoMass(IMyShipController sc, List<IMyThrust> thrusters, int worldInvMultiplier, float minimumTwr = float.NaN, float gravity = float.NaN)
                => CalculateLiftableCargoMass(sc, thrusters, t => t.MaxEffectiveThrust, worldInvMultiplier, minimumTwr, gravity);
            // Calculates the maximum liftable cargo mass for a ship based on the provided thrusters, world inverse multiplier, minimum thrust-to-weight ratio, and gravity.
            // param: "sc" - The ship controller of the grid.
            // param: "thrusters" - The list of thrusters providing lift.
            // param: "worldInvMultiplier" - The world inventory multiplier.
            // param: "minimumTwr" - The minimum thrust-to-weight ratio the grid should maintain.
            // param: "gravity" - The gravity factor (relative to Earth's gravity).
            // returns: The maximum liftable cargo mass.
            public static float CalculateMaxLiftableCargoMass(IMyShipController sc, List<IMyThrust> thrusters, int worldInvMultiplier, float minimumTwr = float.NaN, float gravity = float.NaN)
                => CalculateLiftableCargoMass(sc, thrusters, t => t.MaxThrust, worldInvMultiplier, minimumTwr, gravity);

            static float CalculateLiftableCargoMass(IMyShipController sc, List<IMyThrust> thrusters, Func<IMyThrust, float> thrustSelector, int worldInvMultiplier, float minimumTwr = float.NaN, float gravity = float.NaN) {
                if (worldInvMultiplier <= 0) return float.NaN;
                minimumTwr = float.IsNaN(minimumTwr) ? 1.0f : minimumTwr;
                if (float.IsNaN(minimumTwr) || minimumTwr <= 0f) return float.NaN;
                //Debug($"minimumTwr: {minimumTwr}");
                var gravityMagnitude = GetGravityMagnitude(sc, gravity);
                //Debug($"gravityMagnitude: {gravityMagnitude}");
                if (float.IsNaN(gravityMagnitude) || gravityMagnitude <= 0f) return float.NaN;
                var shipMass = sc.CalculateShipMass();
                var gravAdjustedThrust = thrusters.Sum(thrustSelector) / gravityMagnitude;
                //Debug($"gravAdjustedThrust: {gravAdjustedThrust}");
                var safeMaxMass3 = gravAdjustedThrust / minimumTwr;
                //Debug($"safeMaxMass3: {safeMaxMass3}");
                var liftableCargoMass = (safeMaxMass3 - shipMass.BaseMass) * worldInvMultiplier;
                //Debug($"liftableCargoMass: {liftableCargoMass}");
                var remainingCargoMass = liftableCargoMass - (shipMass.PhysicalMass - shipMass.BaseMass);
                //Debug($"remainingCargoMass: {remainingCargoMass}");
                return remainingCargoMass;
            }

            static float GetGravityMagnitude(IMyShipController sc, float gravity)
                => float.IsNaN(gravity)
                    ? (float)sc.GetNaturalGravity().Length()
                    : gravity * StandardGravityMagnitude;
        }
    }
}
