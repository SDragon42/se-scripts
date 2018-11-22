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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public static class LiftCapacity {

            public static double GetMaxMass1(IMyShipController sc, IList<IMyThrust> thrusters, double minTwr, int inventoryMultiplier) {
                var gravVec = sc.GetNaturalGravity();
                //gravity in m/s^2
                var gravMS2 = Math.Sqrt(
                    Math.Pow(gravVec.X, 2) +
                    Math.Pow(gravVec.Y, 2) +
                    Math.Pow(gravVec.Z, 2));
                var inNaturalGravity = (gravMS2 > 0.0);
                if (!inNaturalGravity) return 0.0;


                var totalMass = sc.CalculateShipMass().TotalMass;
                // mass of the grid without cargo
                var baseMass = sc.CalculateShipMass().BaseMass;
                var cargoMass = totalMass - baseMass;
                // the mass the game uses for physics calculation
                var actualMass = baseMass + (cargoMass / inventoryMultiplier);

                var gravityForceOnShip = (actualMass - cargoMass);

                var maxEffectiveThrust = thrusters.Sum(t => t.MaxEffectiveThrust);


                // maxMass = (thrust / TWR) * MassNewtons

                return 0.0;
            }

            public static double? GetMaxMass2(IMyShipController sc, IList<IMyThrust> thrusters, double minTwr, int inventoryMultiplier) {
                var gravVec = sc.GetNaturalGravity();
                //gravity in m/s^2
                var gravMS2 = Math.Sqrt(
                    Math.Pow(gravVec.X, 2) +
                    Math.Pow(gravVec.Y, 2) +
                    Math.Pow(gravVec.Z, 2));
                var inNaturalGravity = (gravMS2 > 0.0);
                if (!inNaturalGravity) return null;


                var gridMass = sc.CalculateShipMass().BaseMass;
                var maxEffectiveThrust = thrusters.Sum(t => t.MaxEffectiveThrust);

                var hoverOverridePower = gridMass / maxEffectiveThrust; // TWR = 1.0

                return null;
            }
        }
    }
}
