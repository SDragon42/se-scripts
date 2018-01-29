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
        class TwrInfo {
            public TwrInfo(List<IMyThrust> _thrusters, Direction direction, float totalMass) {
                var massNewtons = ConvertMass2Newtons(totalMass);

                Thrust_Direction = direction.ToString();
                NumThrusters = _thrusters.Count;
                Thrust = _thrusters.Sum(b => b.MaxThrust);
                TWR = Thrust / massNewtons;
                EffectiveThrust = _thrusters.Sum(b => b.MaxEffectiveThrust);
                EffectiveTWR = EffectiveThrust / massNewtons;
            }

            public string Thrust_Direction { get; private set; }
            public double Thrust { get; private set; }
            public double TWR { get; private set; }
            public double EffectiveThrust { get; private set; }
            public double EffectiveTWR { get; private set; }
            public int NumThrusters { get; private set; }

            double ConvertMass2Newtons(float mass_kg) => (mass_kg / 0.101971621);
        }
    }
}
