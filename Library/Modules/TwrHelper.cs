// <mdk sortorder="1000" />
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

            public static TwrInfo CalculateCurrentTWR(IMyShipController sc, List<IMyThrust> liftThrusters, int inventoryMultiplier, float twr = float.NaN, float gravity = float.NaN) {
                var shipMass = sc.CalculateShipMass();

                var gravityMagnitude = float.IsNaN(gravity) 
                    ? (float)sc.GetNaturalGravity().Length()
                    : gravity * StandardGravityMagnitude;

                var effectiveThrust = liftThrusters.Sum(t => t.MaxEffectiveThrust);
                if (float.IsNaN(twr)) {
                    var physMassNewtons = shipMass.PhysicalMass * gravityMagnitude;
                    twr = effectiveThrust / physMassNewtons;
                }

                var twrThrust = effectiveThrust / twr;
                var currentMaxCargoMass = ((twrThrust / gravityMagnitude) - shipMass.BaseMass) * inventoryMultiplier;

                return new TwrInfo(twr, currentMaxCargoMass);
            }

        }

        class TwrInfo {
            public TwrInfo(float twr, float maxCargoMass) {
                TWR = twr;
                MaxCargoMass = maxCargoMass;
            }
            public float TWR { get; private set; }
            public float MaxCargoMass { get; private set; }
        }
    }
}
