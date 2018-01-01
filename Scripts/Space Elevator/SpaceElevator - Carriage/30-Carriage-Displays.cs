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

        private void DisplayProcessing(string payload) {
            var msg = UpdateDisplayMessage.CreateFromPayload(payload);

            List<IMyTextPanel> displays = null;
            switch (msg.DisplayKey) {
                case DisplayKeys.ALL_CARRIAGES: displays = _displaysAllCarriages; break;
                case DisplayKeys.ALL_CARRIAGES_WIDE: displays = _displaysAllCarriagesWide; break;
                case DisplayKeys.ALL_PASSENGER_CARRIAGES: displays = _displaysAllPassengerCarriages; break;
                case DisplayKeys.ALL_PASSENGER_CARRIAGES_WIDE: displays = _displaysAllPassengerCarriagesWide; break;
            }
            displays?.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.Text, FontSizes.CARRIAGE_GFX));

        }

        void UpdateDisplays() {
            if (_displaysSingleCarriages.Count > 0) {
                var text = Displays.BuildOneCarriageDisplay(Me.CubeGrid.CustomName, _status, retransRingMarker: true);
                _displaysSingleCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, text, FontSizes.CARRIAGE_GFX));
            }

            if (_displaysSingleCarriagesDetailed.Count > 0) {
                var text = Displays.BuildOneCarriageDisplay(Me.CubeGrid.CustomName, _status, retransRingMarker: true, opsDetail: true);
                _displaysSingleCarriagesDetailed.ForEach(d => Displays.Write2MonospaceDisplay(d, text, FontSizes.CARRIAGE_GFX));
            }

            if (_displaySpeed.Count > 0) {
                var text = Displays.BuildSpeedDisplayText(_verticalSpeed, _rangeToDestination);
                _displaySpeed.ForEach(d => Displays.Write2MonospaceDisplay(d, text, FontSizes.SPEED));
            }

            if (_displayDestination.Count > 0) {
                var text = Displays.BuildDestinationDisplayText(_destination?.Name ?? GetMode().ToString().Replace('_', ' '));
                _displayDestination.ForEach(d => Displays.Write2MonospaceDisplay(d, text, FontSizes.DESTINATION));
            }

            if (_displayCargo.Count > 0) {
                var text = Displays.BuildCargoDisplayText(_cargoMass);
                _displayCargo.ForEach(d => Displays.Write2MonospaceDisplay(d, text, FontSizes.CARGO));
            }

            if (_displayFuel.Count > 0) {
                var text = Displays.BuildFuelDisplayText(_h2TankFilledPercent);
                _displayFuel.ForEach(d => Displays.Write2MonospaceDisplay(d, text, FontSizes.FUEL));
            }

        }

    }
}
