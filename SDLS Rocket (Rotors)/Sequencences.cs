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

        IEnumerator<bool> Delay(double milliseconds) {
            var time = 0.0;
            while (time < milliseconds) {
                time += Runtime.TimeSinceLastRun.TotalMilliseconds;
                yield return true;
            }
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
