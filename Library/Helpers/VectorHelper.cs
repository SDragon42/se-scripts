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
        /// <summary>
        /// 
        /// </summary>
        static class VectorHelper {
            /// <summary>
            /// Gets the Vector3D from a GPS coordinate string.
            /// </summary>
            /// <param name="gpsCoordinate"></param>
            /// <returns></returns>
            public static Vector3D GpsToVector(string gpsCoordinate) {
                string name;
                Vector3D position;
                GpsToVector(gpsCoordinate, out name, out position);
                return position;
            }

            /// <summary>
            /// Gets the Vector3D and name from a GPS coordinate string.
            /// </summary>
            /// <param name="gpsCoordinate"></param>
            /// <param name="name"></param>
            /// <param name="position"></param>
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

            /// <summary>
            /// Creates a GPS location format.
            /// </summary>
            /// <param name="v">Location of the GPS coordinates.</param>
            /// <param name="name">label for the GPS coordinates.</param>
            /// <returns></returns>
            public static string VectortoGps(Vector3D v, string name = "Position") => $"GPS:{name}:{v.X}:{v.Y}:{v.Z}:";
        }
    }
}
