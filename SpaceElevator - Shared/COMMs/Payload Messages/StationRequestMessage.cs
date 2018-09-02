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
    partial class Program {
        enum StationRequests { RequestCarriage, SendCarriageTo }

        class StationRequestMessage : BasePayloadMessage {
            public const string TYPE = "StationRequestMessage";
            public static StationRequestMessage CreateFromPayload(string message) {
                var obj = new StationRequestMessage();
                obj.LoadFromPayload(message);
                return obj;
            }

            private StationRequestMessage() : base(TYPE) { }
            public StationRequestMessage(StationRequests request, string extra = null) : base(TYPE) {
                _msgParts = new string[] { request.ToString(), extra ?? string.Empty };
            }

            public StationRequests Request => _msgParts[0].ToEnum<StationRequests>();
            public string Extra => _msgParts[1];

        }
    }
}
