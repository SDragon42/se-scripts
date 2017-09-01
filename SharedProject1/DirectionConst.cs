using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    static class DirectionConst
    {
        public const int Forward = 1;
        public const int Backward = 2;
        public const int Up = 3;
        public const int Down = 4;
        public const int Left = 5;
        public const int Right = 6;


        public static int GetInverseDirection(int direction)
        {
            switch (direction)
            {
                case Forward: return Backward;
                case Backward: return Forward;
                case Up: return Down;
                case Down: return Up;
                case Left: return Right;
                case Right: return Left;
                default: goto case Forward;
            }
        }

        public static int GetDirectionFromString(string directionName)
        {
            directionName = directionName.ToLower().Trim();
            switch (directionName)
            {
                case "forward": return Forward;
                case "front": return Forward;
                case "fore": return Forward;
                case "backward": return Backward;
                case "back": return Backward;
                case "aft": return Backward;
                case "up": return Up;
                case "down": return Down;
                case "left": return Left;
                case "right": return Right;
                default: return Forward;
            }
        }
    }
}
