// <mdk sortorder="50" />
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

        IEnumerator<bool> Delay(double milliseconds) {
            var time = 0.0;
            do {
                yield return (time < milliseconds);
                time += Runtime.TimeSinceLastRun.TotalMilliseconds;
            } while (time < milliseconds);
        }

        IEnumerator<bool> SEQ_CheckReadyToLaunch() {

            // unlock landing gears
            // unlock connectors
            LandingGears.ForEach(b => b.Unlock());
            Connectors.ForEach(b => b.Disconnect());
            yield return true;

            // calc launch TWR

            // set go/nogo

            yield return false;
        }


        IEnumerator<bool> SEQ_Launch() {
            LoadBlocks();

            LandingGears.ForEach(b => b.Unlock());
            Connectors.ForEach(b => b.Disconnect());

            yield return true;

            // calc launch TWR
            // set go/nogo

            //GridTerminalSystem.GetBlocksOfType(ThrustersPrimary, collector);


            // Flight loop
            // get thrusters

            // Set thruster Power

            yield return false;
        }
        /// <summary>
        /// Common Core
        /// </summary>
        /// <param name="primaryTag"></param>
        /// <param name="secondaryTag"></param>
        /// <returns></returns>
        IEnumerator<bool> SEQ_LaunchStage(string primaryTag, string secondaryTag, bool stage)
        {

            yield return false;
        }


        IEnumerator<bool> SEQ_AwaitStaging() {

            // Wait for stage separation
            while (ConnectedMerges.Count > 0)
                yield return true;

            LoadBlocks();

            yield return false;
        }
        IEnumerator<bool> SEQ_ParachuteLanding() {

            Parachutes.ForEach(p => p.Enabled = true);
            yield return false;
        }

    }
}
