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
    class COMMsModule {

        readonly Queue<CommMessage> _messageQueue = new Queue<CommMessage>();
        readonly IMyProgrammableBlock _prog;

        public COMMsModule(IMyProgrammableBlock program) {
            _prog = program;
        }

        public void AddMessageToQueue(BasePayloadMessage payload, params string[] recievers) {
            if (recievers == null || recievers.Length == 0)
                recievers = new string[] { string.Empty };
            foreach (var reciever in recievers) {
                var message = new CommMessage(_prog, reciever, payload.MessageType, payload.ToString());
                _messageQueue.Enqueue(message);
            }
        }

        public void TransmitQueue(IMyRadioAntenna transmitter) {
            if (transmitter == null) return;
            if (_messageQueue.Count == 0) return;
            var message = _messageQueue.Dequeue();
            var success = transmitter.TransmitMessage(message.ToString());
            if (!success) _messageQueue.Enqueue(message);
        }
    }
}
