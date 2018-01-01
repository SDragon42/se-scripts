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
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    static class RotorConstants {
        public const int RADIAN_ROUND_DIGITS = 3;
        public const float ROTOR_VELOCITY = 3f;
    }

    static class GridNameConstants {
        public const string A1 = "A1 Carriage";
        public const string A2 = "A2 Carriage";
        public const string B1 = "B1 Carriage";
        public const string B2 = "B2 Carriage";
        public const string MAINT = "Maint Carriage";
        public const string OpsCenter = "OPS Center";

        static readonly List<string> _allCarriages;
        static readonly List<string> _allPassCarriages;
        static GridNameConstants() {
            _allCarriages = new List<string>(new string[] { A1, A2, B1, B2, MAINT });
            _allPassCarriages = new List<string>(new string[] { A1, A2, B1, B2 });
        }

        public static List<string> AllCarriages => _allCarriages;
        public static List<string> AllPassengerCarriages => _allPassCarriages;
    }

    static class DisplayKeys {
        public const string ALL_CARRIAGES = "[all-carriages]";
        public const string ALL_CARRIAGES_WIDE = "[all-carriages-wide]";
        public const string ALL_PASSENGER_CARRIAGES = "[all-passenger-carriages]";
        public const string ALL_PASSENGER_CARRIAGES_WIDE = "[all-passenger-carriages-wide]";
        public const string SINGLE_CARRIAGE = "[single-carriage]";
        public const string SINGLE_CARRIAGE_DETAIL = "[single-carriage-detail]";

        public const string FLAT_SPEED = "[lcd-speed]";
        public const string FLAT_DESTINATION = "[lcd-destination]";
        public const string FLAT_CARGO = "[lcd-cargo]";
        public const string FLAT_FUEL = "[lcd-fuel]";
    }

    static class FontSizes {
        public const float CARRIAGE_GFX = 0.97f;
        public const float DESTINATION = 1.75f;
        public const float SPEED = 1.3f;
        public const float FUEL = 1.3f;
        public const float CARGO = 1.3f;
    }
}
