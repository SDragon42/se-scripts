// <mdk sortorder="50" />
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

        void Command_CheckReadyToLaunch() {
            Debug("CMD_CheckReadyToLaunch");
            if (Structure != RocketStructure.Pod) return;
            if (Mode != FlightMode.Off) return;

            if (SequenceSets.HasTask("CheckReady")) return;
            SequenceSets.Add("CheckReady", SEQ_CheckReadyToLaunch(), true);
        }


        IEnumerator<bool> SEQ_CheckReadyToLaunch() {

            // unlock landing gears
            // unlock connectors
            // calc launch TWR
            // set go/nogo

            yield return false;
        }



        void Command_Launch() {
            Debug("CMD_Launch");
            if (Structure != RocketStructure.Pod) return;
            if (Mode != FlightMode.Off) return;
            //Mode = FlightMode.Launch;

        }

        IEnumerator<bool> SEQ_Launch() {

            SendCmdToOtherParts(CMD_AwaitStaging);

            // unlock landing gears
            // unlock connectors
            // calc launch TWR
            // set go/nogo

            yield return false;
        }

    }
}
