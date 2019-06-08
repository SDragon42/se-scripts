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
    partial class Program : MyGridProgram {

        readonly Logging Log = new Logging(50);
        readonly Queue<string> MessageQueue = new Queue<string>();

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Once;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            try {
                Echo("COMM Commands");

                if (argument.Length > 0)
                    MessageQueue.Enqueue(argument);

                ProcessQueue();
                DisplayLog();

            } catch (Exception ex) {
                Echo("##########");
                Echo(ex.Message);
                Echo(ex.StackTrace);
                Echo("##########");
                throw ex;

            } finally {
                Runtime.UpdateFrequency = (MessageQueue.Count > 0)
                    ? UpdateFrequency.Update10
                    : UpdateFrequency.None;
            }
        }

        void ProcessQueue() {
            if (MessageQueue.Count == 0) return;

            var msg = MessageQueue.Dequeue();
        }

        void DisplayLog() {
            if (!Log.Enabled) return;
        }


        class CommMessage {
            public CommMessage(string message) {

            }
        }
    }
}
