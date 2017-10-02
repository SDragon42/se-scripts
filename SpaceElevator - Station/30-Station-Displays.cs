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
                _displaysAllCarriages.ForEach(d => Write2MonospaceDisplay(d, text, 0.97f));
            }

            if (_displaysAllCarriagesWide.Count > 0) {
                var text = Displays.BuildAllCarriageDisplayText(_A1.Status, _A2.Status, _B1.Status, _B2.Status, _Maintenance.Status, true);
                _displaysAllCarriagesWide.ForEach(d => Write2MonospaceDisplay(d, text, 0.97f));
            }

            if (_displaysAllPassengerCarriages.Count > 0) {
                var text = Displays.BuildAllPassengerCarriageDisplayText(_A1.Status, _A2.Status, _B1.Status, _B2.Status);
                _displaysAllPassengerCarriages.ForEach(d => Write2MonospaceDisplay(d, text, 0.97f));
            }

            if (_displaysAllPassengerCarriagesWide.Count > 0) {
                var text = Displays.BuildAllPassengerCarriageDisplayText(_A1.Status, _A2.Status, _B1.Status, _B2.Status, true);
                _displaysAllPassengerCarriagesWide.ForEach(d => Write2MonospaceDisplay(d, text, 0.97f));
            }

        }
        void Write2MonospaceDisplay(IMyTextPanel display, string text, float fontSize) {
            LCDHelper.SetFont_Monospaced(display);
            LCDHelper.SetFontSize(display, fontSize);
            display.WritePublicText(text);
            display.ShowPublicTextOnScreen();
        }

    }
}
