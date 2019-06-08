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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {
        class SendCarriageToMessage : BasePayloadMessage {
            public const string TYPE = "SendCarriageToMessage";
            public static SendCarriageToMessage CreateFromPayload(string message) {
                var obj = new SendCarriageToMessage();
                obj.LoadFromPayload(message);
                return obj;
            }

            private SendCarriageToMessage() : base(TYPE) { }
            public SendCarriageToMessage(string destination) : base(TYPE) {
                _msgParts = new string[] { destination };
            }

            public string Destination => _msgParts[0];
        }
    }
}
