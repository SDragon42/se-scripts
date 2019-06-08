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

        //void TurnOff() {
        //    ManualControl();
        //    //Operations.Clear();
        //    Align_Off();
        //    AllThrusters.ForEach(t => t.Enabled = false);
        //}

        //void ManualControl() {
        //    //Operations.Remove(OPS_Launch);
        //    //Operations.Remove(OPS_GravAlign);
        //    Align_Off();
        //    AllThrusters.ForEach(t => t.Enabled = true);
        //}

        void ScanGrids() {
            //GridTerminalSystem.GetBlocks(TmpBlocks);
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(TmpBlocks, IsSDLS);

            var grids = TmpBlocks
                .Select(b => new { b.CubeGrid.EntityId, b.CubeGrid.CustomName })
                .Distinct()
                .ToList()
                ;

            grids.ForEach(g => Echo($"Grid: {g.CustomName}"));
        }

        //void Align_Launch() {
        //    Runtime.UpdateFrequency |= UpdateFrequency.Update10;
        //    //Operations.Add(OPS_GravAlign, Sequence_LaunchGravAlign(), true);
        //    GravityAlign.Clear();
        //    GravityAlign.Add(Sequence_LaunchGravAlign());
        //}
        //void Align_Land() {
        //    Runtime.UpdateFrequency |= UpdateFrequency.Update10;
        //    //Operations.Add(OPS_GravAlign, Sequence_LandGravAlign(), true);
        //    GravityAlign.Clear();
        //    GravityAlign.Add(Sequence_LandGravAlign());
        //}
        //void Align_Off() {
        //    GravityAlign.Clear();
        //    VecAlign.gyrosOff(Gyros);
        //}

        //void Launch() {
        //    Runtime.UpdateFrequency |= UpdateFrequency.Update10;
        //    VecAlign.gyrosOff(Gyros);
        //    //Operations.Add(OPS_Launch, Sequence_Launch(), true);
        //    Align_Launch();
        //}

        //void Launch_221() { // 2 boosters, 2 stages, 1 orbiter

        //}

        //void Launch_211() { // 2 boosters, 1 stage, 1 orbiter

        //}

        //void Launch_021() { // 2 stages, 1 orbiter

        //}

        //void Launch_011() { // 1 stage, 1 orbiter

        //}

        //void Launch_001() { // 1 orbiter

        //}

    }
}
