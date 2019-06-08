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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {
        static class RotorConstants {
            public const int RADIAN_ROUND_DIGITS = 3;
            public const float ROTOR_VELOCITY = 3f;
        }

        static class GridNameConstants {
            public const string A1 = "A1 Carriage";
            public const string A2 = "A2 Carriage";
            public const string B1 = "B1 Carriage";
            public const string B2 = "B2 Carriage";
            public const string MAINT = "M Carriage";
            public const string OpsCenter = "OPS Center";
            public const string SpaceStation = "Space Station";
            public const string RetransStation = "Retrans Station";
            public const string GroundStation = "Ground Station";

            public const string TERMINAL_A = "A";
            public const string TERMINAL_A1 = "A1";
            public const string TERMINAL_A2 = "A2";
            public const string TERMINAL_B = "B";
            public const string TERMINAL_B1 = "B1";
            public const string TERMINAL_B2 = "B2";
            public const string TERMINAL_1 = "1";
            public const string TERMINAL_2 = "2";
            public const string TERMINAL_M = "M";

            static readonly List<string> _allCarriages;
            static GridNameConstants() {
                _allCarriages = new List<string>(new string[] { A1, A2, B1, B2, MAINT });
            }

            public static List<string> AllCarriages => _allCarriages;
        }

        static class DisplayKeys {
            public const string ALL_CARRIAGES = "[all-carriages]";
            public const string ALL_CARRIAGES_WIDE = "[all-carriages-wide]";
            public const string ALL_PASSENGER_CARRIAGES = "[all-passenger-carriages]";
            public const string ALL_PASSENGER_CARRIAGES_WIDE = "[all-passenger-carriages-wide]";
            public const string SINGLE_CARRIAGE = "[single-carriage]";
            public const string SINGLE_CARRIAGE_DETAIL = "[single-carriage-detail]";

            public static readonly string CARRIAGE_A1 = $"{GridNameConstants.A1}|{SINGLE_CARRIAGE}";
            public static readonly string CARRIAGE_A1_DETAIL = $"{GridNameConstants.A1}|{SINGLE_CARRIAGE_DETAIL}";

            public static readonly string CARRIAGE_A2 = $"{GridNameConstants.A2}|{SINGLE_CARRIAGE}";
            public static readonly string CARRIAGE_A2_DETAIL = $"{GridNameConstants.A2}|{SINGLE_CARRIAGE_DETAIL}";

            public static readonly string CARRIAGE_B1 = $"{GridNameConstants.B1}|{SINGLE_CARRIAGE}";
            public static readonly string CARRIAGE_B1_DETAIL = $"{GridNameConstants.B1}|{SINGLE_CARRIAGE_DETAIL}";

            public static readonly string CARRIAGE_B2 = $"{GridNameConstants.B2}|{SINGLE_CARRIAGE}";
            public static readonly string CARRIAGE_B2_DETAIL = $"{GridNameConstants.B2}|{SINGLE_CARRIAGE_DETAIL}";

            public static readonly string CARRIAGE_MAINT = $"{GridNameConstants.MAINT}|{SINGLE_CARRIAGE}";
            public static readonly string CARRIAGE_MAINT_DETAIL = $"{GridNameConstants.MAINT}|{SINGLE_CARRIAGE_DETAIL}";

            public const string FLAT_SPEED = "[lcd-speed]";
            public const string FLAT_DESTINATION = "[lcd-destination]";
            public const string FLAT_CARGO = "[lcd-cargo]";
            public const string FLAT_FUEL = "[lcd-fuel]";

            public const string CONTROL_LOG = "[control-log]";
            public const string COMMS_LOG = "[comm-log]";
        }

        static class FontSizes {
            public const float CARRIAGE_GFX = 0.97f;
            public const float DESTINATION = 1.75f;
            public const float SPEED = 1.3f;
            public const float FUEL = 1.3f;
            public const float CARGO = 1.3f;
            public const float LOG = 0.5f;
        }
    }
}
