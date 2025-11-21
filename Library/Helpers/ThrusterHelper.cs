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
        /// <summary>
        /// 
        /// </summary>
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

            /// <summary>
            /// Calculates the maximum mass a grid can have based on the desired thrust to weight ratio (at the current altitude)
            /// </summary>
            /// <param name="sc">a ship Controller</param>
            /// <param name="thrusters">The "lifting" thrusters.</param>
            /// <param name="minTwr"></param>
            /// <param name="worldInvMultiplier"></param>
            /// <returns></returns>
            public static double? GetMaxMass(IMyShipController sc, List<IMyThrust> thrusters, double minTwr, int worldInvMultiplier) {
                var gVec = sc.GetNaturalGravity();
                var gravityMs2 = Math.Sqrt(Math.Pow(gVec.X, 2) + Math.Pow(gVec.Y, 2) + Math.Pow(gVec.Z, 2)); // m/s²
                if (gravityMs2 <= 0.0) return null;

                //Debug($"gravityMs2: {gravityMs2:N2}");
                var baseMass = sc.CalculateShipMass().BaseMass;
                //Debug($"baseMass: {baseMass:N2}");
                var maxEffectiveThrust = thrusters.Sum(t => t.MaxEffectiveThrust);
                //Debug($"Thrust: {maxEffectiveThrust:N2}");
                var TwrThrust = maxEffectiveThrust / minTwr;
                //Debug($"TwrThrust: {TwrThrust:N2}");
                var maxCargoMass = ((TwrThrust / gravityMs2) - baseMass) * worldInvMultiplier;
                return maxCargoMass;
            }
        }
    }
}
