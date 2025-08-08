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

        readonly DebugLogging Log;

        readonly Action<string> Debug = (text) => { };

        public Program() {
            Log = new DebugLogging(this);
            Log.EchoMessages = true;
            Log.Enabled = true;
            Log.MaxTextLinesToKeep = 20;
            Debug = (msg) => Log.AppendLine($"{DateTime.Now:HH:mm:ss.fff} {msg}");
        }

        public void Main(string argument, UpdateType updateSource) {
            var message = $"{argument}|{DateTime.Now.Ticks}";
            Debug(message);
            IGC.SendBroadcastMessage(Constants.CheckPointTag,
                                     message,
                                     TransmissionDistance.AntennaRelay);
            Log.UpdateDisplay();
        }
    }
}
