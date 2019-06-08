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
        class BlocksByOrientation {
            readonly Matrix _identityMatrix = new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            public BlocksByOrientation(IMyShipController sc = null) {
                Init(sc);
            }

            Matrix _scMatrix;


            public void Init(IMyShipController sc) {
                if (sc == null) return;
                sc.Orientation.GetMatrix(out _scMatrix);
                Matrix.Transpose(ref _scMatrix, out _scMatrix);
            }

            public bool IsForward(IMyTerminalBlock b) => IsInDirection(b, _identityMatrix.Forward);
            public bool IsBackward(IMyTerminalBlock b) => IsInDirection(b, _identityMatrix.Backward);
            public bool IsUp(IMyTerminalBlock b) => IsInDirection(b, _identityMatrix.Up);
            public bool IsDown(IMyTerminalBlock b) => IsInDirection(b, _identityMatrix.Down);
            public bool IsLeft(IMyTerminalBlock b) => IsInDirection(b, _identityMatrix.Left);
            public bool IsRight(IMyTerminalBlock b) => IsInDirection(b, _identityMatrix.Right);

            bool IsInDirection(IMyTerminalBlock b, Vector3 direction) {
                Matrix blockMatrix;
                b.Orientation.GetMatrix(out blockMatrix);
                var blockDir = Vector3.Transform(blockMatrix.Forward, _scMatrix);
                return (blockDir == direction);
            }
        }

        //static class BlocksByOrientation2 {
        //    public static bool IsForward(IMyTerminalBlock refBlock, IMyTerminalBlock block) {
        //        return false;
        //    }

        //    //static bool IsInDirection<T>(IMyTerminalBlock refBlock, T b, Vector3 direction) where T : IMyTerminalBlock {
        //    static bool IsInDirection(IMyTerminalBlock refBlock,
        //        Base6Directions.Direction direction, IMyTerminalBlock block) {
        //        return (refBlock.Orientation.Forward == direction);
        //    }
        //}
    }
}
