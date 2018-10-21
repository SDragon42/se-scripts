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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        void TurnOff() {
            ManualControl();
            Operations.Clear();
            AllThrusters.ForEach(t => t.Enabled = false);
        }

        void ManualControl() {
            Operations.Remove(OPS_Launch);
            Operations.Remove(OPS_GravAlign);
            VecAlign.gyrosOff(Gyros);
            AllThrusters.ForEach(t => t.Enabled = true);
        }

        void ScanGrids() {
            GridTerminalSystem.GetBlocks(TempBlocks);

            var grids = TempBlocks
                .Select(b => new { b.CubeGrid.EntityId, b.CubeGrid.CustomName })
                .Distinct()
                .ToList()
                ;

            grids.ForEach(g => Echo($"Grid: {g.CustomName}"));
        }

        void Align_Launch() {
            Runtime.UpdateFrequency |= UpdateFrequency.Update10;
            Operations.Add(OPS_GravAlign, Sequence_LaunchGravAlign(), true);
        }

        void Align_Land() {
            Runtime.UpdateFrequency |= UpdateFrequency.Update10;
            Operations.Add(OPS_GravAlign, Sequence_LandGravAlign(), true);
        }

        void Launch() {
            Runtime.UpdateFrequency |= UpdateFrequency.Update10;
            Operations.Add(OPS_Launch, Sequence_Launch(), true);
        }

    }
}
