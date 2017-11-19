﻿using Sandbox.Game.EntityComponents;
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
    enum StationResponses { DepartureOk, DockingComplete }

    class StationResponseMessage : BasePayloadMessage {
        public const string TYPE = "StationRequestMessage";
        public static StationResponseMessage CreateFromPayload(string message) {
            var obj = new StationResponseMessage();
            obj.LoadFromPayload(message);
            return obj;
        }

        private StationResponseMessage() : base(TYPE) { }
        public StationResponseMessage(StationResponses response) : base(TYPE) {
            _msgParts = new string[] { response.ToString() };
        }

        public StationResponses Response { get { return _msgParts[0].ToEnum<StationResponses>(); } }
    }
}