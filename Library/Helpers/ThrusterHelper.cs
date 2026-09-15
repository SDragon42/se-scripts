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
            //public static Action<string> Debug = (t) => { };
            //public static void GetThrusterOrientation(IMyTerminalBlock refBlock, IList<IMyThrust> unsortedThrusters, IList<IMyThrust> mainThrusters, IList<IMyThrust> otherThrusters) {
            //    var forwardDir = refBlock.Orientation.Forward;
            //    mainThrusters.Clear();
            //    otherThrusters.Clear();
            //    foreach (var thruster in unsortedThrusters) {
            //        var thrustDirn = Base6Directions.GetFlippedDirection(thruster.Orientation.Forward);
            //        if (thrustDirn == forwardDir)
            //            mainThrusters.Add(thruster);
            //        else
            //            otherThrusters.Add(thruster);
            //    }
            //}

            // Calculates the maximum liftable cargo mass for a ship based on the provided thrusters, world inverse multiplier, minimum thrust-to-weight ratio, and gravity.
            // param: "sc" - The ship controller of the grid.
            // param: "thrusters" - The list of thrusters providing lift.
            // param: "worldInvMultiplier" - The world inventory multiplier.
            // param: "twr" - The minimum thrust-to-weight ratio the grid should maintain.
            // param: "gravity" - The gravity factor (relative to Earth's gravity).
            // returns: The maximum liftable cargo mass.
            public static float GetMaxLiftableCargoMass(IMyShipController sc, List<IMyThrust> thrusters, int worldInvMultiplier, float twr, float gravity = float.NaN) {
                // if (float.IsNaN(gravity)) gravity = (float)sc.GetNaturalGravity().Length() / 9.81f;
                var gravityMagnitude = float.IsNaN(gravity)
                    ? (float)sc.GetNaturalGravity().Length()
                    : gravity * 9.81f; // m/s²
                if (gravityMagnitude <= 0f) return float.NaN;

                var effectiveThrust = thrusters.Sum(t => t.MaxEffectiveThrust);
                var twrThrust = effectiveThrust / twr;
                var gridMass = sc.CalculateShipMass().BaseMass;
                var maxCargoMass = ((twrThrust / gravityMagnitude) - gridMass) * worldInvMultiplier;
                return (float)maxCargoMass;
            }
        }
    }
}
