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

        bool IsOnThisGrid(IMyTerminalBlock b) { return Me.CubeGrid.EntityId == b.CubeGrid.EntityId; }

        bool IsTaggedStation(IMyTerminalBlock b) { return (b.CustomName.Contains(_settings.StationTag)); }
        //bool IsTaggedStationOnThisGrid(IMyTerminalBlock b) { return (IsOnThisGrid(b) && IsTaggedStation(b)); }
        //bool IsDoorOnStationOnly(IMyTerminalBlock b) { return IsTaggedStation(b) && Collect.IsHumanDoor(b); }

    }
}
