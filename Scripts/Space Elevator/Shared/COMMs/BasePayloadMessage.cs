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
    abstract class BasePayloadMessage {
        protected const char DELIMITER = '\t';
        protected string[] _msgParts;

        protected BasePayloadMessage(string messageType) {
            MessageType = messageType;
        }
        public void LoadFromPayload(string message) {
            _msgParts = message.Split(DELIMITER);
        }

        public string MessageType { get; private set; }

        public override string ToString() => string.Join(DELIMITER.ToString(), _msgParts);
    }
}
