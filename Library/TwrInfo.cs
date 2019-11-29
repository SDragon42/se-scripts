// <mdk sortorder="900" />
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
            public TwrInfo(List<IMyThrust> _thrusters, Base6Directions.Direction direction, float totalMass) {
                var massNewtons = totalMass / 0.101971621;

                Thrust_Direction = direction;
                NumThrusters = _thrusters.Count;

                EffectiveThrust = _thrusters.Sum(b => (double)b.MaxEffectiveThrust);
                MaxThrust = _thrusters.Sum(b => (double)b.MaxThrust);

                EffectiveTWR = EffectiveThrust / massNewtons;
                MaxTWR = MaxThrust / massNewtons;
            }

            public Base6Directions.Direction Thrust_Direction { get; private set; }
            public double EffectiveThrust { get; private set; }
            public double MaxThrust { get; private set; }
            public double EffectiveTWR { get; private set; }
            public double MaxTWR { get; private set; }
            public int NumThrusters { get; private set; }
        }
    }
}
