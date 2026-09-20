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
        static class VectorHelper {
            // Gets the Vector3D from a GPS coordinate string.
            public static Vector3D GpsToVector(string gpsCoordinate) {
                string name;
                Vector3D position;
                GpsToVector(gpsCoordinate, out name, out position);
                return position;
            }

            // Gets the Vector3D and name from a GPS coordinate string.
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

            // Creates a GPS location format.
            // param: "v" - Location of the GPS coordinates.
            // param: "name" - label for the GPS coordinates.
            // returns: The GPS coordinate string.
            public static string VectorToGps(Vector3D v, string name = "Position") => $"GPS:{name}:{v.X}:{v.Y}:{v.Z}:";
        }
    }
}
