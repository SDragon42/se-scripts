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
    partial class Program {
        static class CompassHelper {
            const double rad2deg = 180 / Math.PI; //constant to convert radians to degrees
            const string compassString = "N.W>-----<"
                + "N>-----<N.E>-----<E>-----<S.E>-----<S>-----<S.W>-----<W>-----<N.W>-----<"
                + "N>-----<N.E>-----<E>-----<S.E>-----<S>-----<S.W>-----<W>-----<N.W>-----<";
            static readonly Vector3D absoluteNorthVec = new Vector3D(0.342063708833718, -0.704407897782847, -0.621934025954579); //this was determined via Keen's code

            public static double GetBearing(IMyShipController reference) {
                var gravityVec = reference.GetNaturalGravity();

                //check if grav vector exists
                var gravMag = gravityVec.LengthSquared();
                if (double.IsNaN(gravMag) || gravMag == 0)
                    return double.NaN;

                var forwardVec = reference.WorldMatrix.Forward;
                var relativeEastVec = gravityVec.Cross(absoluteNorthVec);
                var relativeNorthVec = relativeEastVec.Cross(gravityVec);

                //project forward vector onto a plane comprised of the north and east vectors
                var forwardProjNorthVec = VectorProjection(forwardVec, relativeNorthVec);
                var forwardProjEastVec = VectorProjection(forwardVec, relativeEastVec);
                var forwardProjPlaneVec = forwardProjEastVec + forwardProjNorthVec;

                //find angle from abs north to projected forward vector measured clockwise
                var bearingAngle = Math.Acos(forwardProjPlaneVec.Dot(relativeNorthVec) / forwardProjPlaneVec.Length() / relativeNorthVec.Length()) * rad2deg;

                //check direction of angle
                if (forwardVec.Dot(relativeEastVec) < 0)
                    bearingAngle = 360 - bearingAngle; //because of how the angle is measured

                return bearingAngle;
            }
            private static Vector3D VectorProjection(Vector3D a, Vector3D b) {
                var projection = a.Dot(b) / b.Length() / b.Length() * b;
                return projection;
            }

            public static string GetCompassText(double bearingAngle) {
                if (double.IsNaN(bearingAngle)) return string.Empty;
                var startIdx = (int)MathHelper.Clamp(Math.Round(bearingAngle / 5), 0, 359);
                return compassString.Substring(startIdx, 21) + "\n" + "          ^";
            }
            public static string GetBearingText(double bearingAngle) {
                if (double.IsNaN(bearingAngle)) return string.Empty;

                var strCardinalDirection = "";
                //get cardinal direction
                if (bearingAngle < 22.5 || bearingAngle >= 337.5)
                    strCardinalDirection = "N";
                else if (bearingAngle < 67.5)
                    strCardinalDirection = "NE";
                else if (bearingAngle < 112.5)
                    strCardinalDirection = "E";
                else if (bearingAngle < 157.5)
                    strCardinalDirection = "SE";
                else if (bearingAngle < 202.5)
                    strCardinalDirection = "S";
                else if (bearingAngle < 247.5)
                    strCardinalDirection = "SW";
                else if (bearingAngle < 292.5)
                    strCardinalDirection = "W";
                else if (bearingAngle < 337.5)
                    strCardinalDirection = "NW";

                return string.Format("Bearing: {0:000} {1}", Math.Round(bearingAngle), strCardinalDirection);
            }

        }
    }
}
