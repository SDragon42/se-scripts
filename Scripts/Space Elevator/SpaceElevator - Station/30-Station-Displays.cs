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
        void DisplayProcessing(string payload) {
            var msg = UpdateAllDisplaysMessage.CreateFromPayload(payload);

            _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.AllCarriages, FontSizes.CARRIAGE_GFX));
            _displaysAllCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.AllCarriagesWide, FontSizes.CARRIAGE_GFX));
            _displaysAllPassengerCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.AllPassCarriages, FontSizes.CARRIAGE_GFX));
            _displaysAllPassengerCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, msg.AllPassCarriagesWide, FontSizes.CARRIAGE_GFX));

            foreach (var d in _displaysSingleCarriages) {
                if (IsGateA1(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageA1, FontSizes.CARRIAGE_GFX);
                } else if (IsGateA2(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageA2, FontSizes.CARRIAGE_GFX);
                } else if (IsGateB1(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageB1, FontSizes.CARRIAGE_GFX);
                } else if (IsGateB2(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageB2, FontSizes.CARRIAGE_GFX);
                } else if (IsGateMaint(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageMaint, FontSizes.CARRIAGE_GFX);
                }
            }

            foreach (var d in _displaysSingleCarriagesDetailed) {
                if (IsGateA1(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageA1Details, FontSizes.CARRIAGE_GFX);
                } else if (IsGateA2(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageA2Details, FontSizes.CARRIAGE_GFX);
                } else if (IsGateB1(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageB1Details, FontSizes.CARRIAGE_GFX);
                } else if (IsGateB2(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageB2Details, FontSizes.CARRIAGE_GFX);
                } else if (IsGateMaint(d)) {
                    Displays.Write2MonospaceDisplay(d, msg.CarriageMaintDetails, FontSizes.CARRIAGE_GFX);
                }
            }
        }

    }
}
