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
            var parts = message.Split(DELIMITER);
            if (_msgParts != null && parts.Length != _msgParts.Length) return;
            _msgParts = parts;
        }

        public string MessageType { get; private set; }

        public override string ToString() => string.Join(DELIMITER.ToString(), _msgParts);

        protected string Set(string value) => value ?? string.Empty;
    }
}
