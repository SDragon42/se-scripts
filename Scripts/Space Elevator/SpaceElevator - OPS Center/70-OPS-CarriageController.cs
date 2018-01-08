using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;

namespace IngameScript {
    partial class Program {

        void CarriageRequestedProcessing(string fromStationName, string msgPayload) {
            //_carriageStatuses
            var msg = StationRequestMessage.CreateFromPayload(msgPayload);
            if (msg.Request != StationRequests.RequestCarriage) return;

            _log.AppendLine($"Car RQ - S:{fromStationName}  C:{msg.Extra}");

            string[] carriageKeys;

            switch (msg.Extra) {
                case GridNameConstants.TERMINAL_A: carriageKeys = new string[] { GridNameConstants.A1, GridNameConstants.A2 }; break;
                case GridNameConstants.TERMINAL_B: carriageKeys = new string[] { GridNameConstants.B1, GridNameConstants.B2 }; break;
                case GridNameConstants.TERMINAL_1: carriageKeys = new string[] { GridNameConstants.A1, GridNameConstants.B1 }; break;
                case GridNameConstants.TERMINAL_2: carriageKeys = new string[] { GridNameConstants.A2, GridNameConstants.B2 }; break;
                case GridNameConstants.TERMINAL_M: carriageKeys = new string[] { GridNameConstants.MAINT }; break;
                default: return;
            }

            string carKey = null;
            foreach (var x in carriageKeys) {
                if (!_carriageStatuses.ContainsKey(x)) continue;
                var car = _carriageStatuses[x];
                if (car.Destination == fromStationName) return; // carriage already on the way
                if (car.InTransit) continue;
                if (car.Destination == "Docked") {
                    if (fromStationName == GridNameConstants.GroundStation && car.Range2Bottom < car.Range2Top && Math.Abs(car.Range2Top - car.Range2Bottom) > 10000.0)
                        return; // already docked at station
                    if (fromStationName == GridNameConstants.SpaceStation && car.Range2Bottom > car.Range2Top && Math.Abs(car.Range2Top - car.Range2Bottom) > 10000.0)
                        return; // already docked at station
                    if (fromStationName == GridNameConstants.RetransStation && Math.Abs(car.Range2Top - car.Range2Bottom) < 10000.0)
                        return; // already docked at station
                }
                carKey = x;
            }

            if (carKey != null)
                SendCarriageTo(carKey, fromStationName);
        }

    }
}
