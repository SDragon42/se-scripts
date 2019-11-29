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

            public static Action<string> Debug = (t) => { };


            public static void GetThrusterOrientation(IMyTerminalBlock refBlock, IList<IMyThrust> unsortedThrusters, IList<IMyThrust> mainThrusters, IList<IMyThrust> sideThrusters) {
                var forwardDirn = refBlock.Orientation.Forward;
                foreach (var thisThrust in unsortedThrusters) {
                    var thrustDirn = Base6Directions.GetFlippedDirection(thisThrust.Orientation.Forward);
                    if (thrustDirn == forwardDirn)
                        mainThrusters.Add(thisThrust);
                    else
                        sideThrusters.Add(thisThrust);
                }
            }


            public static double? GetMaxMass(IMyShipController sc, List<IMyThrust> thrusters, double minTwr, int worldInvMultiplier) {
                var gravVector = sc.GetNaturalGravity();
                var gravMS2 = Math.Sqrt(
                    Math.Pow(gravVector.X, 2) +
                    Math.Pow(gravVector.Y, 2) +
                    Math.Pow(gravVector.Z, 2));
                var inNaturalGravity = (gravMS2 > 0.0);
                if (!inNaturalGravity) return null;

                Debug($"gravMS2: {gravMS2:N2}");
                var baseMass = sc.CalculateShipMass().BaseMass;
                Debug($"baseMass: {baseMass:N2}");
                var maxEffectiveThrust = thrusters.Sum(t => t.MaxEffectiveThrust);
                Debug($"Thrust: {maxEffectiveThrust:N2}");
                var TwrThrust = maxEffectiveThrust / minTwr;
                Debug($"TwrThrust: {TwrThrust:N2}");
                var maxCargoMass = ((TwrThrust / gravMS2) - baseMass) * worldInvMultiplier;
                return maxCargoMass;
            }
        }
    }
}
