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

        void DisplayProcessing(string payload) {
            var msg = UpdateAllDisplaysMessage.CreateFromPayload(payload);
            _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.AllCarriages, FontSizes.CARRIAGE_GFX));
            _displaysAllCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.AllCarriagesWide, FontSizes.CARRIAGE_GFX));
            _displaysAllPassengerCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.AllPassCarriages, FontSizes.CARRIAGE_GFX));
            _displaysAllPassengerCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.AllPassCarriagesWide, FontSizes.CARRIAGE_GFX));
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
