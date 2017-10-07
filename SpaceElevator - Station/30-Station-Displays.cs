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
        void UpdateDisplays() {
            if (_displaysAllCarriages.Count > 0) {
                var text = Displays.BuildAllCarriageDisplayText(_A1.Status, _A2.Status, _B1.Status, _B2.Status, _Maintenance.Status);
                _displaysAllCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, text, 0.97f));
            }

            if (_displaysAllCarriagesWide.Count > 0) {
                var text = Displays.BuildAllCarriageDisplayText(_A1.Status, _A2.Status, _B1.Status, _B2.Status, _Maintenance.Status, true);
                _displaysAllCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, text, 0.97f));
            }

            if (_displaysAllPassengerCarriages.Count > 0) {
                var text = Displays.BuildAllPassengerCarriageDisplayText(_A1.Status, _A2.Status, _B1.Status, _B2.Status);
                _displaysAllPassengerCarriages.ForEach(d => Displays.Write2MonospaceDisplay(d, text, 0.97f));
            }

            if (_displaysAllPassengerCarriagesWide.Count > 0) {
                var text = Displays.BuildAllPassengerCarriageDisplayText(_A1.Status, _A2.Status, _B1.Status, _B2.Status, true);
                _displaysAllPassengerCarriagesWide.ForEach(d => Displays.Write2MonospaceDisplay(d, text, 0.97f));
            }

            if (_displaysSingleCarriages.Count > 0) {
                var a1Text = string.Empty;
                var a2Text = string.Empty;
                var b1Text = string.Empty;
                var b2Text = string.Empty;
                var maintText = string.Empty;
                foreach (var d in _displaysSingleCarriages) {
                    if (IsGateA1(d)) {
                        if (string.IsNullOrEmpty(a1Text))
                            a1Text = Displays.BuildOneCarriageDisplay("Carriage A1", _A1.Status);
                        Displays.Write2MonospaceDisplay(d, a1Text, 0.97f);
                        continue;
                    }
                    if (IsGateA2(d)) {
                        if (string.IsNullOrEmpty(a2Text))
                            a2Text = Displays.BuildOneCarriageDisplay("Carriage A2", _A2.Status);
                        Displays.Write2MonospaceDisplay(d, a2Text, 0.97f);
                        continue;
                    }
                    if (IsGateB1(d)) {
                        if (string.IsNullOrEmpty(b1Text))
                            b1Text = Displays.BuildOneCarriageDisplay("Carriage B1", _B1.Status);
                        Displays.Write2MonospaceDisplay(d, b1Text, 0.97f);
                        continue;
                    }
                    if (IsGateB2(d)) {
                        if (string.IsNullOrEmpty(b2Text))
                            b2Text = Displays.BuildOneCarriageDisplay("Carriage B2", _B2.Status);
                        Displays.Write2MonospaceDisplay(d, b2Text, 0.97f);
                        continue;
                    }
                    if (IsGateMaint(d)) {
                        if (string.IsNullOrEmpty(maintText))
                            maintText = Displays.BuildOneCarriageDisplay("Maintenance Carriage", _Maintenance.Status);
                        Displays.Write2MonospaceDisplay(d, maintText, 0.97f);
                        continue;
                    }
                }
            }

        }

    }
}
