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
    class TempTesting : TestingBase, ITestingBase {

        public TempTesting(MyGridProgram thisObj) : base(thisObj) { }

        public void Main(string argument) {
            var thrusters = new List<IMyThrust>();
            GridTerminalSystem.GetBlocksOfType(thrusters);

            var displays = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType(displays);

            var sb = new StringBuilder();
            foreach (var t in thrusters) {
                sb.AppendLine($"{t.CustomName}");
                if (t.IsWorking)
                    sb.AppendLine($"   MEF: {t.MaxEffectiveThrust}");
                else
                    sb.AppendLine($"   MEF: Not Working");
                //t.is
                //sb.AppendLine($"  *MEF: {ThrusterHelper.GetMaxEffectiveThrust(t)}");
                sb.AppendLine();
            }

            var msg = sb.ToString();

            foreach (var d in displays) {
                d.WritePublicText(msg);
                d.ShowPublicTextOnScreen();
            }
        }

    }
}
