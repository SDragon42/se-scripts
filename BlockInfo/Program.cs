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
    }
}
