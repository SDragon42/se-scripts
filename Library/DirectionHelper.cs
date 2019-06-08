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
        static class DirectionHelper {
            public static Direction GetInverseDirection(Direction direction) {
                switch (direction) {
                    case Direction.Forward: return Direction.Backward;
                    case Direction.Backward: return Direction.Forward;
                    case Direction.Up: return Direction.Down;
                    case Direction.Down: return Direction.Up;
                    case Direction.Left: return Direction.Right;
                    case Direction.Right: return Direction.Left;
                    default: goto case Direction.Forward;
                }
            }

            public static Direction GetDirectionFromString(string directionName) {
                directionName = directionName.ToLower().Trim();
                switch (directionName) {
                    case "forward": return Direction.Forward;
                    case "front": return Direction.Forward;
                    case "fore": return Direction.Forward;
                    case "backward": return Direction.Backward;
                    case "back": return Direction.Backward;
                    case "aft": return Direction.Backward;
                    case "up": return Direction.Up;
                    case "down": return Direction.Down;
                    case "left": return Direction.Left;
                    case "right": return Direction.Right;
                    default: return Direction.Forward;
                }
            }
        }
    }
}
