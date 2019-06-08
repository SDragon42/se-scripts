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
    partial class Program {
        class UpdateAllDisplaysMessage : BasePayloadMessage {
            public const string TYPE = "UpdateAllDisplaysMessage";
            public static UpdateAllDisplaysMessage CreateFromPayload(string message) {
                var obj = new UpdateAllDisplaysMessage();
                obj.LoadFromPayload(message);
                return obj;
            }

            public UpdateAllDisplaysMessage() : base(TYPE) {
                _msgParts = new string[14];
            }

            public string CarriageA1 {
                get { return _msgParts[0]; }
                set { _msgParts[0] = Set(value); }
            }
            public string CarriageA1Details {
                get { return _msgParts[1]; }
                set { _msgParts[1] = Set(value); }
            }

            public string CarriageA2 {
                get { return _msgParts[2]; }
                set { _msgParts[2] = Set(value); }
            }
            public string CarriageA2Details {
                get { return _msgParts[3]; }
                set { _msgParts[3] = Set(value); }
            }

            public string CarriageB1 {
                get { return _msgParts[4]; }
                set { _msgParts[4] = Set(value); }
            }
            public string CarriageB1Details {
                get { return _msgParts[5]; }
                set { _msgParts[5] = Set(value); }
            }

            public string CarriageB2 {
                get { return _msgParts[6]; }
                set { _msgParts[6] = Set(value); }
            }
            public string CarriageB2Details {
                get { return _msgParts[7]; }
                set { _msgParts[7] = Set(value); }
            }

            public string CarriageMaint {
                get { return _msgParts[8]; }
                set { _msgParts[8] = Set(value); }
            }
            public string CarriageMaintDetails {
                get { return _msgParts[9]; }
                set { _msgParts[9] = Set(value); }
            }


            public string AllCarriages {
                get { return _msgParts[10]; }
                set { _msgParts[10] = Set(value); }
            }
            public string AllCarriagesWide {
                get { return _msgParts[11]; }
                set { _msgParts[11] = Set(value); }
            }

            public string AllPassCarriages {
                get { return _msgParts[12]; }
                set { _msgParts[12] = Set(value); }
            }
            public string AllPassCarriagesWide {
                get { return _msgParts[13]; }
                set { _msgParts[13] = Set(value); }
            }
        }
    }
}
