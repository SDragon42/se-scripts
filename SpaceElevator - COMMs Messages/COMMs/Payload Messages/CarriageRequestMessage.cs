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

    enum CarriageRequests { Dock, Depart }

    class CarriageRequestMessage : BasePayloadMessage {
        public const string TYPE = "CarriageRequestMessage";
        public static CarriageRequestMessage CreateFromPayload(string message) {
            var obj = new CarriageRequestMessage();
            obj.LoadFromPayload(message);
            return obj;
        }

        private CarriageRequestMessage() : base(TYPE) { }
        public CarriageRequestMessage(string carriageName, CarriageRequests request) : base(TYPE) {
            _msgParts = new string[] { carriageName, request.ToString() };
        }

        public string CarriageName { get { return _msgParts[0]; } }
        public CarriageRequests Request { get { return _msgParts[1].ToEnum<CarriageRequests>(); } }
    }
}
