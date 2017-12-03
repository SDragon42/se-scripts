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

        bool IsOnThisGrid(IMyTerminalBlock b) => Me.CubeGrid == b.CubeGrid;

        bool IsTaggedStation(IMyTerminalBlock b) => Collect.IsTagged(b, _settings.StationTag);
        bool IsTaggedStationOnThisGrid(IMyTerminalBlock b) => (IsOnThisGrid(b) && IsTaggedStation(b));
        bool IsDoorOnStationOnly(IMyTerminalBlock b) => IsTaggedStation(b) && !IsTaggedTerminal(b) && !IsTaggedTransfer(b) && Collect.IsHumanDoor(b);

        bool IsTaggedTerminal(IMyTerminalBlock b) => Collect.IsTagged(b, _settings.TerminalTag);
        bool IsOnTerminal(IMyTerminalBlock b) => IsTaggedStation(b) && IsTaggedTerminal(b);
        bool IsDoorOnTerminal(IMyTerminalBlock b) => IsTaggedStation(b) && IsTaggedTerminal(b) && Collect.IsHumanDoor(b);

        bool IsTaggedTransfer(IMyTerminalBlock b) => Collect.IsTagged(b, _settings.TransferTag);
        bool IsOnTransferArm(IMyTerminalBlock b) => IsTaggedStation(b) && IsTaggedTransfer(b);
        bool IsLightOnTransferArm(IMyTerminalBlock b) => IsTaggedStation(b) && IsTaggedTransfer(b) && (b is IMyInteriorLight || b is IMyReflectorLight);

        bool IsGateA1(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_A1);
        bool IsGateA2(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_A2);
        bool IsGateB1(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_B1);
        bool IsGateB2(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_B2);
        bool IsGateMaint(IMyTerminalBlock b) => Collect.IsTagged(b, TAG_MAINT);
    }
}
