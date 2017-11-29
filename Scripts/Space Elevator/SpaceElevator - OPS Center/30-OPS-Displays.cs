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

        void BuildSingleDisplays(string key, bool retransRingMarker = false) {
            var status = _carriageStatuses[key];
            SetDisplayText(
                key,
                DisplayKeys.SINGLE_CARRIAGE,
                Displays.BuildOneCarriageDisplay(key, status, retransRingMarker: retransRingMarker));

            SetDisplayText(
                key,
                DisplayKeys.SINGLE_CARRIAGE_DETAIL,
                Displays.BuildOneCarriageDisplay(key, status, opsDetail: true, retransRingMarker: retransRingMarker));
        }
        void BuildDisplays() {
            var a1Status = _carriageStatuses[GridNameConstants.A1];
            var a2Status = _carriageStatuses[GridNameConstants.A2];
            var b1Status = _carriageStatuses[GridNameConstants.B1];
            var b2Status = _carriageStatuses[GridNameConstants.B2];
            var maintStatus = _carriageStatuses[GridNameConstants.MAINT];

            if (_displaysAllCarriages.Count > 0) {
                var text = Displays.BuildAllCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, maintStatus);
                _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, text, 0.97f));
            }

            SetDisplayText("",
                DisplayKeys.ALL_CARRIAGES,
                Displays.BuildAllCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, maintStatus));
            SetDisplayText("",
                DisplayKeys.ALL_CARRIAGES_WIDE,
                Displays.BuildAllCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, maintStatus, true));

            SetDisplayText("",
                DisplayKeys.ALL_PASSENGER_CARRIAGES,
                Displays.BuildAllPassengerCarriageDisplayText(a1Status, a2Status, b1Status, b2Status));
            SetDisplayText("",
                DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE,
                Displays.BuildAllPassengerCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, true));

            GridNameConstants.AllPassengerCarriages.ForEach(key => BuildSingleDisplays(key));
            BuildSingleDisplays(GridNameConstants.MAINT, true);
        }


        void UpdateDisplays() {
            _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", DisplayKeys.ALL_CARRIAGES), 0.97f));
            _displaysAllCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", DisplayKeys.ALL_CARRIAGES_WIDE), 0.97f));

            _displaysAllPassengerCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", DisplayKeys.ALL_PASSENGER_CARRIAGES), 0.97f));
            _displaysAllPassengerCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE), 0.97f));

            foreach (var d in _displaysSingleCarriages) {
                if (d.CustomName.Contains(TAG_A1)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(GridNameConstants.A1, DisplayKeys.SINGLE_CARRIAGE_DETAIL), 0.97f);
                } else if (d.CustomName.Contains(TAG_A2)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(GridNameConstants.A2, DisplayKeys.SINGLE_CARRIAGE_DETAIL), 0.97f);
                } else if (d.CustomName.Contains(TAG_B1)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(GridNameConstants.B1, DisplayKeys.SINGLE_CARRIAGE_DETAIL), 0.97f);
                } else if (d.CustomName.Contains(TAG_B2)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(GridNameConstants.B2, DisplayKeys.SINGLE_CARRIAGE_DETAIL), 0.97f);
                } else if (d.CustomName.Contains(TAG_MAINT)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(GridNameConstants.MAINT, DisplayKeys.SINGLE_CARRIAGE_DETAIL), 0.97f);
                }
            }
        }



    }
}
