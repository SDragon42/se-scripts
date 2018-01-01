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
                _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, text, FontSizes.CARRIAGE_GFX));
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
            _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", DisplayKeys.ALL_CARRIAGES), FontSizes.CARRIAGE_GFX));
            _displaysAllCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", DisplayKeys.ALL_CARRIAGES_WIDE), FontSizes.CARRIAGE_GFX));

            _displaysAllPassengerCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", DisplayKeys.ALL_PASSENGER_CARRIAGES), FontSizes.CARRIAGE_GFX));
            _displaysAllPassengerCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, GetDisplayText("", DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE), FontSizes.CARRIAGE_GFX));

            foreach (var d in _displaysSingleCarriages) {

                var gridName = string.Empty;
                if (d.CustomName.Contains(TAG_A1)) gridName = GridNameConstants.A1;
                else if (d.CustomName.Contains(TAG_A2)) gridName = GridNameConstants.A2;
                else if (d.CustomName.Contains(TAG_B1)) gridName = GridNameConstants.B1;
                else if (d.CustomName.Contains(TAG_B2)) gridName = GridNameConstants.B2;
                else if (d.CustomName.Contains(TAG_MAINT)) gridName = GridNameConstants.MAINT;
                if (gridName.Length == 0) continue;
                var text = GetDisplayText(gridName, DisplayKeys.SINGLE_CARRIAGE_DETAIL);
                Displays.Write2MonospaceDisplay(d, text, FontSizes.CARRIAGE_GFX);
            }
        }



    }
}
