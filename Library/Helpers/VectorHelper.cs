﻿// <mdk sortorder="2000" />
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
        static class VectorHelper {
            public static Vector3D GpsToVector(string gpsCoordinate) {
                string name;
                Vector3D position;
                GpsToVector(gpsCoordinate, out name, out position);
                return position;
            }

            public static void GpsToVector(string gpsCoordinate, out string name, out Vector3D position) {
                name = string.Empty;
                position = Vector3D.Zero;

                var gpsParts = gpsCoordinate.Split(':');
                if (gpsParts == null || gpsParts.Length < 5) return;

                name = gpsParts[1];
                position = new Vector3D(
                    double.Parse(gpsParts[2]),
                    double.Parse(gpsParts[3]),
                    double.Parse(gpsParts[4]));
            }

            public static string VectortoGps(Vector3D v, string label = "Position") {
                return $"GPS:{label}:{v.X}:{v.Y}:{v.Z}:";
            }
        }
    }
}
