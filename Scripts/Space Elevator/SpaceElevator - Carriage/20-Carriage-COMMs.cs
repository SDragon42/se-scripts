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

        void SetStatuses() {
            _status.Position = _rc.GetPosition();
            _status.VerticalSpeed = _verticalSpeed;
            _status.FuelLevel = _h2TankFilledPercent;
            _status.CargoMass = _cargoMass;
            _status.Range2Bottom = _rangeToGround;
            _status.Range2Top = _rangeToSpace;
        }
        void SendStatsMessage() {
            SetStatuses();
            if (!_settings.SendStatusMessages) return;
            if (_antenna == null) return;
            _comms.AddMessageToQueue(_status);
        }
        void SendDockedMessage(string stationName) {
            var payload = new CarriageRequestMessage(Me.CubeGrid.CustomName, CarriageRequests.Dock);
            _comms.AddMessageToQueue(payload, stationName);
        }
        void SendRequestDepartureClearance(string stationName) {
            var payload = new CarriageRequestMessage(Me.CubeGrid.CustomName, CarriageRequests.Depart);
            _comms.AddMessageToQueue(payload, stationName);
        }

    }
}
