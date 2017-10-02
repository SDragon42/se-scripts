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
            var allCarriagesDisplay = Displays.BuildAllCarriagePositionSummary(_A1.Status, _A2.Status, _B1.Status, _B2.Status, _Maintenance.Status);
            //var allCarriagesWideDisplay = Displays.BuildAllCarriagePositionSummaryWide(_A1.Status, _A2.Status, _B1.Status, _B2.Status, _Maintenance.Status);

            foreach (var d in _displays) {
                if (Displays.IsAllCarriagesDisplay(d)) Write2MonospaceDisplay(d, allCarriagesDisplay, 0.97f);
                //else if (Displays.IsAllCarriagesWideDisplay(d)) Write2MonospaceDisplay(d, allCarriagesWideDisplay, 0.97f);
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
