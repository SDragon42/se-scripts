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

        void CarriageRequestedProcessing(string stationName, string msgPayload) {
            //_carriageStatuses
            var msg = StationRequestMessage.CreateFromPayload(msgPayload);
            if (msg.Request != StationRequests.RequestCarriage) return;

            string[] carriageKeys;

            switch (msg.Extra) {
                case GridNameConstants.TERMINAL_A: carriageKeys = new string[] { GridNameConstants.A1, GridNameConstants.A2 }; break;
                case GridNameConstants.TERMINAL_B: carriageKeys = new string[] { GridNameConstants.B1, GridNameConstants.B2 }; break;
                case GridNameConstants.TERMINAL_1: carriageKeys = new string[] { GridNameConstants.A1, GridNameConstants.B1 }; break;
                case GridNameConstants.TERMINAL_2: carriageKeys = new string[] { GridNameConstants.A2, GridNameConstants.B2 }; break;
                case GridNameConstants.TERMINAL_M: carriageKeys = new string[] { GridNameConstants.MAINT }; break;
                default: return;
            }

            //_carriageStatuses.Any(c => c.Value.Mode == CarriageMode.Transit_Powered);
        }

    }
}
