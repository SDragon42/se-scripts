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

        //-------------------------------------------------------------------------------
        //  COMMs
        //-------------------------------------------------------------------------------
        void SendResponseMessage(string carriageName, StationResponses response) {
            if (_antenna == null) return;
            var msgPayload = new StationResponseMessage(response);
            _log.AppendLine($"{DateTime.Now.ToLongTimeString()} To {carriageName} | Response: {response}");
            _comms.AddMessageToQueue(msgPayload, carriageName);
        }

        void SendCarriageRequestMessage(string terminal) {
            var msgPayload = new StationRequestMessage(StationRequests.RequestCarriage, terminal);
            _log.AppendLine($"{DateTime.Now.ToLongTimeString()} Request Carriage for terminal {terminal}");
            _comms.AddMessageToQueue(msgPayload, GridNameConstants.OpsCenter);
        }

        void SendCarriageToRequestMessage(string terminal, string station) {
            var msgPayload = new StationRequestMessage(StationRequests.SendCarriageTo, $"{terminal}  {station}");
            _log.AppendLine($"{DateTime.Now.ToLongTimeString()} Send Carriage to terminal {terminal}  {station}");
            _comms.AddMessageToQueue(msgPayload, GridNameConstants.OpsCenter);
        }

    }
}
