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

        //-------------------------------------------------------------------------------
        //  COMMs
        //-------------------------------------------------------------------------------
        void SendAllCOMMsDisplays() {
            if (_antenna == null) return;

            var msg = new UpdateAllDisplaysMessage();
            msg.AllCarriages = _displayText[DisplayKeys.ALL_CARRIAGES];
            msg.AllCarriagesWide = _displayText[DisplayKeys.ALL_CARRIAGES_WIDE];

            msg.AllPassCarriages = _displayText[DisplayKeys.ALL_PASSENGER_CARRIAGES];
            msg.AllPassCarriagesWide = _displayText[DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE];

            msg.CarriageA1 = _displayText[DisplayKeys.CARRIAGE_A1];
            msg.CarriageA1Details = _displayText[DisplayKeys.CARRIAGE_A1_DETAIL];

            msg.CarriageA2 = _displayText[DisplayKeys.CARRIAGE_A2];
            msg.CarriageA2Details = _displayText[DisplayKeys.CARRIAGE_A2_DETAIL];

            msg.CarriageB1 = _displayText[DisplayKeys.CARRIAGE_B1];
            msg.CarriageB1Details = _displayText[DisplayKeys.CARRIAGE_B1_DETAIL];

            msg.CarriageB2 = _displayText[DisplayKeys.CARRIAGE_B2];
            msg.CarriageB2Details = _displayText[DisplayKeys.CARRIAGE_B2_DETAIL];

            msg.CarriageMaint = _displayText[DisplayKeys.CARRIAGE_MAINT];
            msg.CarriageMaintDetails = _displayText[DisplayKeys.CARRIAGE_MAINT_DETAIL];

            _comms.AddMessageToQueue(msg);
        }

        void SendCarriageTo(string carriageKey, string destination) {
            if (_antenna == null) return;
            var msgPayload = new SendCarriageToMessage(destination);
            _comms.AddMessageToQueue(msgPayload, carriageKey);
        }

    }
}
