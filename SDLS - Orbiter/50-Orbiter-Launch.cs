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

        const string OPS_GravAlign = "GravAlign";
        const string OPS_Launch = "FlightOps";



        IEnumerator<bool> Sequence_Launch() {
            yield return true;

            // Turn on Thrusters
            // Activate gravity align
            // boosters to full
            // disconnect launch clamps
        }

        IEnumerator<bool> Sequence_LaunchGravAlign() {
            while (true) {
                VecAlign.AlignWithGravity(Remote, Direction.Backward, Gyros);
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
