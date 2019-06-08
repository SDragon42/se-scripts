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
        //  Displays
        //-------------------------------------------------------------------------------
        void BuildSingleDisplays(string displayKey, string displayKeyDetail, string carriageName, bool retransRingMarker = false) {
            var status = _carriageStatuses[carriageName];

            SetDisplayText(
                displayKey,
                Displays.BuildOneCarriageDisplay(carriageName, status, retransRingMarker: retransRingMarker));

            SetDisplayText(
                displayKeyDetail,
                Displays.BuildOneCarriageDisplay(carriageName, status, opsDetail: true, retransRingMarker: retransRingMarker));
        }
        void BuildDisplays() {
            var a1Status = _carriageStatuses[GridNameConstants.A1];
            var a2Status = _carriageStatuses[GridNameConstants.A2];
            var b1Status = _carriageStatuses[GridNameConstants.B1];
            var b2Status = _carriageStatuses[GridNameConstants.B2];
            var maintStatus = _carriageStatuses[GridNameConstants.MAINT];

            if (_displaysAllCarriages.Count > 0) {
                var text = Displays.BuildAllCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, maintStatus);
                _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, text, FontSizes.CARRIAGE_GFX));
            }

            SetDisplayText(
                DisplayKeys.ALL_CARRIAGES,
                Displays.BuildAllCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, maintStatus));
            SetDisplayText(
                DisplayKeys.ALL_CARRIAGES_WIDE,
                Displays.BuildAllCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, maintStatus, true));

            SetDisplayText(
                DisplayKeys.ALL_PASSENGER_CARRIAGES,
                Displays.BuildAllPassengerCarriageDisplayText(a1Status, a2Status, b1Status, b2Status));
            SetDisplayText(
                DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE,
                Displays.BuildAllPassengerCarriageDisplayText(a1Status, a2Status, b1Status, b2Status, true));

            BuildSingleDisplays(DisplayKeys.CARRIAGE_A1, DisplayKeys.CARRIAGE_A1_DETAIL, GridNameConstants.A1);
            BuildSingleDisplays(DisplayKeys.CARRIAGE_A2, DisplayKeys.CARRIAGE_A2_DETAIL, GridNameConstants.A2);
            BuildSingleDisplays(DisplayKeys.CARRIAGE_B1, DisplayKeys.CARRIAGE_B1_DETAIL, GridNameConstants.B1);
            BuildSingleDisplays(DisplayKeys.CARRIAGE_B2, DisplayKeys.CARRIAGE_B2_DETAIL, GridNameConstants.B2);
            BuildSingleDisplays(DisplayKeys.CARRIAGE_MAINT, DisplayKeys.CARRIAGE_MAINT_DETAIL, GridNameConstants.MAINT, true);
        }


        void UpdateDisplays() {
            _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText(DisplayKeys.ALL_CARRIAGES), FontSizes.CARRIAGE_GFX));
            _displaysAllCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText(DisplayKeys.ALL_CARRIAGES_WIDE), FontSizes.CARRIAGE_GFX));

            _displaysAllPassengerCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText(DisplayKeys.ALL_PASSENGER_CARRIAGES), FontSizes.CARRIAGE_GFX));
            _displaysAllPassengerCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText(DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE), FontSizes.CARRIAGE_GFX));

            foreach (var d in _displaysSingleCarriages) {
                if (Collect.IsTagged(d, TAG_A1)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(DisplayKeys.CARRIAGE_A1_DETAIL), FontSizes.CARRIAGE_GFX);
                } else if (Collect.IsTagged(d, TAG_A2)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(DisplayKeys.CARRIAGE_A2_DETAIL), FontSizes.CARRIAGE_GFX);
                } else if (Collect.IsTagged(d, TAG_B1)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(DisplayKeys.CARRIAGE_B1_DETAIL), FontSizes.CARRIAGE_GFX);
                } else if (Collect.IsTagged(d, TAG_B2)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(DisplayKeys.CARRIAGE_B2_DETAIL), FontSizes.CARRIAGE_GFX);
                } else if (Collect.IsTagged(d, TAG_MAINT)) {
                    Displays.Write2MonospaceDisplay(d, GetDisplayText(DisplayKeys.CARRIAGE_MAINT_DETAIL), FontSizes.CARRIAGE_GFX);
                }
            }
        }



    }
}
