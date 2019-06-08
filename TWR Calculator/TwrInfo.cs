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
        class TwrInfo {
            public TwrInfo(List<IMyThrust> _thrusters, Direction direction, float totalMass) {
                var massNewtons = totalMass / 0.101971621;

                Thrust_Direction = direction;
                NumThrusters = _thrusters.Count;
                Thrust = new EffectiveMax<double>(
                    _thrusters.Sum(b => (double)b.MaxEffectiveThrust),
                    _thrusters.Sum(b => (double)b.MaxThrust));

                TWR = new EffectiveMax<double>(
                    Thrust.Effective / massNewtons,
                    Thrust.Maximum / massNewtons);
            }

            public Direction Thrust_Direction { get; private set; }
            public EffectiveMax<double> Thrust { get; private set; }
            public EffectiveMax<double> TWR { get; private set; }
            public int NumThrusters { get; private set; }


        }

        class EffectiveMax<T> {
            public EffectiveMax(T effective, T maximum) {
                Effective = effective;
                Maximum = maximum;
            }
            public T Effective { get; private set; }
            public T Maximum { get; private set; }
        }
    }
}
