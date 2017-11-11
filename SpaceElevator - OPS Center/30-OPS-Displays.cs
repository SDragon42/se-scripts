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

        void BuildSingleDisplays(string carriageKey, CarriageStatusMessage status, bool retransRingMarker = false) {
            SetDisplayText(
                carriageKey,
                Displays.DISPLAY_KEY_SINGLE_CARRIAGE,
                Displays.BuildOneCarriageDisplay(carriageKey, status, retransRingMarker: retransRingMarker));

            SetDisplayText(
                carriageKey,
                Displays.DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL,
                Displays.BuildOneCarriageDisplay(carriageKey, status, opsDetail: true, retransRingMarker: retransRingMarker));
        }
        void BuildDisplays() {
            var a1Status = _carriageStatuses[CARRIAGE_A1];
            var a2Status = _carriageStatuses[CARRIAGE_A2];
            var b1Status = _carriageStatuses[CARRIAGE_B1];
            var b2Status = _carriageStatuses[CARRIAGE_B2];
            var maintStatus = _carriageStatuses[CARRIAGE_MAINT];

            if (_displaysAllCarriages.Count > 0) {
                var text = Displays.BuildAllCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, maintStatus);
                _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, text, 0.97f));
            }

            SetDisplayText("",
                Displays.DISPLAY_KEY_ALL_CARRIAGES,
                Displays.BuildAllCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, maintStatus));
            SetDisplayText("",
                Displays.DISPLAY_KEY_ALL_CARRIAGES_WIDE,
                Displays.BuildAllCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, maintStatus, true));

            SetDisplayText("",
                Displays.DISPLAY_KEY_ALL_PASSENGER_CARRIAGES,
                Displays.BuildAllPassengerCarriageDisplayText(a1Status, a2Status, b1Status, b2Status));
            SetDisplayText("",
                Displays.DISPLAY_KEY_ALL_PASSENGER_CARRIAGES_WIDE,
                Displays.BuildAllPassengerCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, true));

            BuildSingleDisplays(CARRIAGE_A1, a1Status);
            BuildSingleDisplays(CARRIAGE_A2, a2Status);
            BuildSingleDisplays(CARRIAGE_B1, b1Status);
            BuildSingleDisplays(CARRIAGE_B2, b2Status);
            BuildSingleDisplays(CARRIAGE_MAINT, maintStatus, true);
        }


        void UpdateDisplays() {
            _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", Displays.DISPLAY_KEY_ALL_CARRIAGES), 0.97f));
            _displaysAllCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", Displays.DISPLAY_KEY_ALL_CARRIAGES_WIDE), 0.97f));

            _displaysAllPassengerCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", Displays.DISPLAY_KEY_ALL_PASSENGER_CARRIAGES), 0.97f));
            _displaysAllPassengerCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", Displays.DISPLAY_KEY_ALL_PASSENGER_CARRIAGES_WIDE), 0.97f));

            foreach (var d in _displaysSingleCarriages) {
                if (d.CustomName.Contains(TAG_A1)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(CARRIAGE_A1, Displays.DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL), 0.97f);
                } else if (d.CustomName.Contains(TAG_A2)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(CARRIAGE_A2, Displays.DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL), 0.97f);
                } else if (d.CustomName.Contains(TAG_B1)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(CARRIAGE_B1, Displays.DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL), 0.97f);
                } else if (d.CustomName.Contains(TAG_B2)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(CARRIAGE_B2, Displays.DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL), 0.97f);
                } else if (d.CustomName.Contains(TAG_MAINT)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(CARRIAGE_MAINT, Displays.DISPLAY_KEY_SINGLE_CARRIAGE_DETAIL), 0.97f);
                }
            }
        }



    }
}
