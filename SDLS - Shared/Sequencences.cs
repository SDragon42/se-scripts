using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {

        IEnumerator<double> Delay(double milliseconds) {
            var time = 0.0;
            do {
                yield return (milliseconds - time);
                time += Runtime.TimeSinceLastRun.TotalMilliseconds;
            } while (time < milliseconds);
        }

        IEnumerator<bool> Sequence_LaunchGravAlign() {
            while (true) {
                VecAlign.AlignWithGravity(Remote, Base6Directions.Direction.Backward, Gyros, true);
                yield return true;
            }
        }

        IEnumerator<bool> Sequence_LandGravAlign() {
            while (true) {
                VecAlign.AlignWithGravity(Remote, Base6Directions.Direction.Down, Gyros);
                yield return true;
            }
        }
    }
}
