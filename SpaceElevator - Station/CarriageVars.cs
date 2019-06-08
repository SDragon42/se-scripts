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
        class CarriageVars {
            public CarriageVars(string gridName) {
                GridName = gridName ?? string.Empty;
                GateState = HookupState.Disconnecting;
            }

            public string GridName { get; private set; }
            public bool Connect { get; set; }
            public bool SendResponseMsg { get; set; }
            public HookupState GateState { get; set; }
            public CarriageStatusMessage Status { get; set; }

            public override string ToString() {
                return $"{Connect}:{SendResponseMsg}:{GateState}";
            }

            public void FromString(string stateData) {
                if (string.IsNullOrWhiteSpace(stateData)) return;
                var parts = stateData.Split(':');
                if (parts.Length != 3) return;
                Connect = parts[0].ToBoolean();
                SendResponseMsg = parts[1].ToBoolean();
                GateState = parts[2].ToEnum<HookupState>();
            }
        }
    }
}
