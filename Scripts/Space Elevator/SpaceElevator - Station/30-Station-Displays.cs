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
        //  Displays
        //-------------------------------------------------------------------------------
        private void DisplayProcessing(string payload) {
            var msg = UpdateDisplayMessage.CreateFromPayload(payload);

            switch (msg.DisplayKey) {
                case DisplayKeys.ALL_CARRIAGES:
                    _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f));
                    break;
                case DisplayKeys.ALL_CARRIAGES_WIDE:
                    _displaysAllCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f));
                    break;
                case DisplayKeys.ALL_PASSENGER_CARRIAGES:
                    _displaysAllPassengerCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f));
                    break;
                case DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE:
                    _displaysAllPassengerCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f));
                    break;
                case DisplayKeys.SINGLE_CARRIAGE:
                    DisplaySingleDisplays(msg, _displaysSingleCarriages);
                    break;
                case DisplayKeys.SINGLE_CARRIAGE_DETAIL:
                    DisplaySingleDisplays(msg, _displaysSingleCarriagesDetailed, true);
                    break;
            }

        }

        private void DisplaySingleDisplays(UpdateDisplayMessage msg, List<IMyTextPanel> displays, bool details = false) {
            if (string.IsNullOrWhiteSpace(msg.Text)) {
                return;
            }
            foreach (var d in displays) {
                if (IsGateA1(d) && msg.CarriageKey == GridNameConstants.A1) {
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                } else if (IsGateA2(d) && msg.CarriageKey == GridNameConstants.A2) {
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                } else if (IsGateB1(d) && msg.CarriageKey == GridNameConstants.B1) {
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                } else if (IsGateB2(d) && msg.CarriageKey == GridNameConstants.B2) {
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                } else if (IsGateMaint(d) && msg.CarriageKey == GridNameConstants.MAINT) {
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                }
            }
        }

    }
}
