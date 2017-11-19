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

            //_log.AppendLine($"{msg.CarriageKey}|{msg.DisplayKey}");
            switch (msg.DisplayKey) {
                case Displays.DISPLAY_KEY_ALL_CARRIAGES:
                    _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f));
                    break;
                case Displays.DISPLAY_KEY_ALL_CARRIAGES_WIDE:
                    _displaysAllCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f));
                    break;
                case Displays.DISPLAY_KEY_ALL_PASSENGER_CARRIAGES:
                    _displaysAllPassengerCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f));
                    break;
                case Displays.DISPLAY_KEY_ALL_PASSENGER_CARRIAGES_WIDE:
                    _displaysAllPassengerCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f));
                    break;
                case Displays.DISPLAY_KEY_SINGLE_CARRIAGE:
                    DisplaySingleDisplays(msg, _displaysSingleCarriages);
                    break;
                case Displays.DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL:
                    DisplaySingleDisplays(msg, _displaysSingleCarriagesDetailed, true);
                    break;
            }

        }

        private void DisplaySingleDisplays(UpdateDisplayMessage msg, List<IMyTextPanel> displays, bool details = false) {
            if (string.IsNullOrWhiteSpace(msg.Text)) {
                //_log.AppendLine($"EMPTY|{msg.CarriageKey}|{msg.DisplayKey}");
                return;
            }
            foreach (var d in displays) {
                if (IsGateA1(d) && msg.CarriageKey == CARRIAGE_A1) {
                    _log.AppendLine(d.CustomName);
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                } else if (IsGateA2(d) && msg.CarriageKey == CARRIAGE_A2) {
                    _log.AppendLine(d.CustomName);
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                } else if (IsGateB1(d) && msg.CarriageKey == CARRIAGE_B1) {
                    _log.AppendLine(d.CustomName);
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                } else if (IsGateB2(d) && msg.CarriageKey == CARRIAGE_B2) {
                    _log.AppendLine(d.CustomName);
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                } else if (IsGateMaint(d) && msg.CarriageKey == CARRIAGE_MAINT) {
                    _log.AppendLine(d.CustomName);
                    Displays.Write2MonospaceDisplay(d, msg.Text, 0.97f);
                }
            }
        }

    }
}
