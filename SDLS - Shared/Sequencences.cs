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

        IEnumerator<double> Delay(double milliseconds) {
            var time = 0.0;
            do {
                yield return (milliseconds - time);
                time += Runtime.TimeSinceLastRun.TotalMilliseconds;
            } while (time < milliseconds);
        }

        IEnumerator<bool> Sequence_LaunchGravAlign() {
            while (true) {
                VecAlign.AlignWithGravity(Remote, Direction.Backward, Gyros, true);
                yield return true;
            }
        }

        IEnumerator<bool> Sequence_LandGravAlign() {
            while (true) {
                VecAlign.AlignWithGravity(Remote, Direction.Down, Gyros);
                yield return true;
            }
        }
    }
}
