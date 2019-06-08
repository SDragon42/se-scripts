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
        bool IsTaggedStation(IMyTerminalBlock b) => Collect.IsTagged(b, _settings.StationTag);

        bool IsOnTerminal(IMyTerminalBlock b) => IsTaggedStation(b) && Collect.IsTagged(b, _settings.TerminalTag);
        bool IsOnTransferArm(IMyTerminalBlock b) => IsTaggedStation(b) && Collect.IsTagged(b, _settings.TransferTag);

        bool IsGateA1(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_A1);
        bool IsGateA2(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_A2);
        bool IsGateB1(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_B1);
        bool IsGateB2(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_B2);
        bool IsGateMaint(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_MAINT);
    }
}
