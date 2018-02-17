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

        void Add2Comms_Status() {
            if (!_settings.SendStatusMessages) return;
            if (_antenna == null) return;
            SetStatuses();
            _comms.AddMessageToQueue(_status, GridNameConstants.OpsCenter);
        }

        void Add2Comms_Request(string stationName, CarriageRequests request) {
            if (_antenna == null) return;
            var payload = new CarriageRequestMessage(Me.CubeGrid.CustomName, request);
            _comms.AddMessageToQueue(payload, stationName);
        }

    }
}
