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
        static class TwrHelper {
            public const float StandardGravity = 1.0f;
            public const float StandardGravityMagnitude = 9.81f;

            public static Action<string> Debug = (text) => { };

            public static TwrInfo CalculateEffectiveTWR(IMyShipController sc, List<IMyThrust> liftThrusters, int inventoryMultiplier, float twr = float.NaN, float gravity = float.NaN)
                => CalculateTWR(sc, liftThrusters, t => t.MaxEffectiveThrust, inventoryMultiplier, twr, gravity);
    
            public static TwrInfo CalculateMaxTWR(IMyShipController sc, List<IMyThrust> liftThrusters, int inventoryMultiplier, float twr = float.NaN, float gravity = float.NaN)
                => CalculateTWR(sc, liftThrusters, t => t.MaxThrust, inventoryMultiplier, twr, gravity);

            private static TwrInfo CalculateTWR(IMyShipController sc, List<IMyThrust> liftThrusters, Func<IMyThrust, float> thrustSelector, int inventoryMultiplier, float twr, float gravity) {
                var shipMass = sc.CalculateShipMass();

                //Debug($"gravity: {gravity}");
                var gravityMagnitude = float.IsNaN(gravity)
                    ? (float)sc.GetNaturalGravity().Length()
                    : gravity * StandardGravityMagnitude;
                //Debug($"gravityMagnitude: {gravityMagnitude}");

                var thrust = liftThrusters.Sum(thrustSelector);
                //Debug($"thrust: {thrust}");

                //Debug($"twr: {twr}");
                if (float.IsNaN(twr)) {
                    twr = thrust / (shipMass.PhysicalMass * gravityMagnitude);
                }
                
                Debug($"twr: {twr}");

                var twrThrust = thrust * twr;
                Debug($"thrust: {thrust}");
                Debug($"twrThrust: {twrThrust}");
                //Debug($"gMagnitude: {gravityMagnitude}");
                //Debug($"T / GM: {twrThrust / gravityMagnitude}");
                //Debug($"SM: {shipMass.BaseMass}");
                //Debug($"A: {(twrThrust / gravityMagnitude) - shipMass.BaseMass}");
                //Debug($"B: {((twrThrust / gravityMagnitude) - shipMass.BaseMass) * inventoryMultiplier}");
                var cargoMass = ((twrThrust / gravityMagnitude) - shipMass.BaseMass) * inventoryMultiplier;
                //Debug($"cargoMass: {cargoMass}");

                return new TwrInfo() {
                    TWR = twr,
                    CargoMass = cargoMass,
                    Thrust = thrust
                };
            }

        }

        class TwrInfo {
            public double Thrust { get; set; }
            public float TWR { get; set; }
            public float CargoMass { get; set; }
        }
    }
}
