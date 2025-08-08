using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
    public partial class Program : MyGridProgram {

        // Do not change these values, they are used by the script.

        // Broadcast tag for IGC
        const string CHECKPOINT_TAG = "VVC.Checkpoint";


        public void Main(string argument, UpdateType updateSource) {
            var message = $"{argument}|{DateTime.Now.Ticks}";
            IGC.SendBroadcastMessage(CHECKPOINT_TAG, message, TransmissionDistance.AntennaRelay);
        }
    }
}
