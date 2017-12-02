﻿using Sandbox.Game.EntityComponents;
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
        public const string A1 = "Carriage A1";
        public const string A2 = "Carriage A2";
        public const string B1 = "Carriage B1";
        public const string B2 = "Carriage B2";
        public const string MAINT = "Maint Carriage";

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
}