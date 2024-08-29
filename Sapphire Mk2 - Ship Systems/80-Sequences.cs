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

        IEnumerator<bool> SEQ_Delay(double milliseconds) {
            Debug($"SEQ_Delay: {milliseconds}");
            var time = 0.0;
            do {
                yield return (time < milliseconds);
                time += Runtime.TimeSinceLastRun.TotalMilliseconds;
            } while (time < milliseconds);
        }

        IEnumerator<bool> SEQ_DisconnectConnector(string blockTag) {
            Debug("SEQ_DisconnectConnector: "+ blockTag);
            var c1 = myConnectors.Where(b => Collect.IsTagged(b, blockTag)).ToArray();

            foreach (var b in c1) b.Disconnect();
            yield return true;
            foreach (var b in c1) b.Enabled = false;
            yield return true;
        }

        IEnumerator<bool> SEQ_DisconnectMerge(string blockTag) {
            Debug("SEQ_DisconnectMerge: " + blockTag);
            var c1 = myMerges.Where(b => Collect.IsTagged(b, blockTag)).ToArray();

            foreach (var b in c1) b.Enabled = false;
            yield return true;
        }

        IEnumerator<bool> SEQ_AwaitConnectorClear(string blockTag) {
            Debug("SEQ_AwaitConnectorClear: " + blockTag);
            var c1 = myConnectors.Where(b => Collect.IsTagged(b, blockTag)).ToArray();

            while (c1.Any(b => b.Status != MyShipConnectorStatus.Unconnected))
                yield return true;
            yield return true;
        }

        IEnumerator<bool> SEQ_EnableConnector(string blockTag) {
            Debug("SEQ_EnableConnector: " + blockTag);
            var c1 = myConnectors.Where(b => Collect.IsTagged(b, blockTag)).ToArray();

            foreach (var b in c1) b.Enabled = true;
            yield return true;
        }

        IEnumerator<bool> SEQ_EnableMerge(string blockTag) {
            Debug("SEQ_EnableMerge: " + blockTag);
            var c1 = myMerges.Where(b => Collect.IsTagged(b, blockTag)).ToArray();

            foreach (var b in c1) b.Enabled = true;
            yield return true;
        }

    }
}
