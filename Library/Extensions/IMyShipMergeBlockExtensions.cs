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
        public static bool IsMerged(this IMyShipMergeBlock mergeBlock) {
            //Find direction that block merges to
            Matrix mat;
            mergeBlock.Orientation.GetMatrix(out mat);
            var right1 = new Vector3I(mat.Right);

            //Check if there is a block in front of merge face
            var sb = mergeBlock.CubeGrid.GetCubeBlock(mergeBlock.Position + right1);
            if (sb == null) {
                return false;
            }

            //Check if the other block is actually a merge block
            var mrg2 = sb.FatBlock as IMyShipMergeBlock;
            if (mrg2 == null) {
                return false;
            }

            //Check that other block is correctly oriented
            mrg2.Orientation.GetMatrix(out mat);
            var right2 = new Vector3I(mat.Right);
            return right2 == -right1;
        }
    }
}
