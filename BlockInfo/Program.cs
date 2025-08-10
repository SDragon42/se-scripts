using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    public partial class Program : MyGridProgram {
        public Program() {
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            //ShowBlockInfo("Action Relay");
            //ShowGridNameOnConnector();
            BroadcastMessage("Hello, World!");
        }

        void ShowGridNameOnConnector() {
           var connectors = new List<IMyShipConnector>();
           GridTerminalSystem.GetBlocksOfType(connectors, c => c.CubeGrid == Me.CubeGrid && c.CustomName.Contains("[starting]"));
           if (connectors.Count == 0) {
               Echo("No connectors found with [starting] in the name.");
               return;
           }
           var other = connectors[0].OtherConnector;
           if (other == null) {
               Echo("Nothing connected.");
               return;
           }
           var gridName = other.CubeGrid.CustomName;
           Echo($"Connected to grid: {gridName}");
        }

        void ShowBlockInfo() {
           var textBlocks = new List<IMyTextPanel>();
           GridTerminalSystem.GetBlocksOfType(textBlocks, b => b.CubeGrid == Me.CubeGrid);
           foreach (var b in textBlocks) {
               b.ContentType = ContentType.TEXT_AND_IMAGE;
               var d = b as IMyTextSurface;
               var lines = new string[] {
                   d.SurfaceSize.ToString(),
                   // b.BlockDefinition.SubtypeId,
                   // b.BlockDefinition.SubtypeIdAttribute,
                   // b.BlockDefinition.SubtypeName,
                   // b.BlockDefinition.TypeIdString,
                   // b.BlockDefinition.TypeIdStringAttribute,
               };
               b.WriteText(string.Join("\n\n", lines));
           }
        }

        void ShowBlockInfo(string blockName) {
           var block = GridTerminalSystem.GetBlockWithName(blockName);
           if (block == null) {
               Echo($"Block '{blockName}' not found.");
               return;
           }

           Echo($"Block '{blockName}' found");
           Echo(block.ToString());
        }

        void BroadcastMessage(string message) {
            //IGC.SendBroadcastMessage("VVC.BlockInfo", message, TransmissionDistance.AntennaRelay);
            //Echo($"Broadcasted message: {message}");
            var block = GridTerminalSystem.GetBlockWithName("Action Relay") as IMyTransponder;
            if (block == null) {
                Echo("Action Relay block not found.");
                return;
            }

            if (block.IsFunctional) {
                block.SendSignal(1);
            }
        }
    }
}
