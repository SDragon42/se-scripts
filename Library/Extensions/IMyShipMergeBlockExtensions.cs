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
    static class MyShipMergeBlockExtensions {
        /// <summary>
        /// Checks if the merge block is merged to another merge block. This a temp fix until Keen fixed the built in IsConnected property.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsMerged(this IMyShipMergeBlock b) {
            //Find direction that block merges to
            Matrix mat;
            b.Orientation.GetMatrix(out mat);
            var right1 = new Vector3I(mat.Right);

            //Check if there is a block in front of merge face
            //Check if the other block is actually a merge block
            var b2 = b.CubeGrid.GetCubeBlock(b.Position + right1)?.FatBlock as IMyShipMergeBlock;
            if (b2 == null) return false;

            //Check that other block is correctly oriented
            b2.Orientation.GetMatrix(out mat);
            var right2 = new Vector3I(mat.Right);
            return right2 == -right1;
        }
    }
}
