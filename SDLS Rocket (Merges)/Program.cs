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
    partial class Program : MyGridProgram {

        public void Main(string argument, UpdateType updateSource) {
            Echo($"SDLS {RSymbol.GetSymbol(Runtime)}");
            Echo(Instructions);
            Cfg.LoadConfig(Me);

            LoadBlocks();


            CheckMerges();
            Log.UpdateDisplay();
        }


        void LoadBlocks() {

        }


        void CheckMerges() {
            if (!Cfg.HasGridName) return;
            GridTerminalSystem.GetBlocksOfType(Merges, b => b.IsSameConstructAs(Me) && b.IsConnected);

            var gridName = (Merges.Count > 0) ? Cfg.GridName_Merged : Cfg.GridName;
            if (gridName.Length == 0) return;
            if (gridName == Me.CubeGrid.CustomName) return;
            Log.AppendLine($"{DateTime.Now:HHmmss.f} - GridName={gridName}");
            Me.CubeGrid.CustomName = gridName;
        }



        void CMD_Launch() {

        }
        void CMD_AwaitStaging() {

        }
        void CMD_Shutdown() {

        }
    }
}
