// <mdk sortorder="2000" />
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
            public static Base6Directions.Direction GetDirectionFromString(string directionName) {
                directionName = directionName.ToLower().Trim();
                switch (directionName) {
                    case "forward": return Base6Directions.Direction.Forward;
                    case "front": return Base6Directions.Direction.Forward;
                    case "fore": return Base6Directions.Direction.Forward;
                    case "backward": return Base6Directions.Direction.Backward;
                    case "back": return Base6Directions.Direction.Backward;
                    case "aft": return Base6Directions.Direction.Backward;
                    case "up": return Base6Directions.Direction.Up;
                    case "down": return Base6Directions.Direction.Down;
                    case "left": return Base6Directions.Direction.Left;
                    case "right": return Base6Directions.Direction.Right;
                    default: return Base6Directions.Direction.Forward;
                }
            }
        }
    }
}
