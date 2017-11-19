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
        bool IsTaggedBlock(IMyTerminalBlock b) {
            if (string.IsNullOrWhiteSpace(_settings.BlockTag))
                return true;
            return (b.CustomName.Contains(_settings.BlockTag));
        }
        bool IsTaggedBlockOnThisGrid(IMyTerminalBlock b) { return (IsOnThisGrid(b) && IsTaggedBlock(b)); }

    }
}
