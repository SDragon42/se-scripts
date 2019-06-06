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
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {
        /// <summary>
        /// I acquired some of this code from Whiplash's compass script (I think. I've had it a while)
        /// https://steamcommunity.com/sharedfiles/filedetails/?id=616627882&searchtext=Compass
        /// </summary>
        static class CompassHelper {
            const double rad2deg = 180 / Math.PI; //constant to convert radians to degrees

            const string compassLineM = " N.W ═════ N ═════ N.E ═════ E ═════ S.E ═════ S ═════ S.W ═════ W ═════ N.W ═════ N ═════ N.E ";
            const string compassLineO = "  │        │        │        │        │        │        │        │        │        │        │  ";
            const string compassFooter = "           ▲           ";
            static readonly string[] cardinals = new string[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

            static readonly Vector3D absoluteNorthVec = new Vector3D(0.342063708833718, -0.704407897782847, -0.621934025954579); //this was determined via Keen's code

            public static double GetBearing(IMyShipController sc) {
                var gravityVec = sc.GetNaturalGravity();

                var gravMag = gravityVec.LengthSquared();
                if (double.IsNaN(gravMag) || gravMag == 0)
                    return double.NaN;

                var forwardVec = sc.WorldMatrix.Forward;
                var relativeEastVec = gravityVec.Cross(absoluteNorthVec);
                var relativeNorthVec = relativeEastVec.Cross(gravityVec);

                //project forward vector onto a plane comprised of the north and east vectors
                var forwardProjEastVec = VectorProjection(forwardVec, relativeEastVec);
                var forwardProjNorthVec = VectorProjection(forwardVec, relativeNorthVec);
                var forwardProjPlaneVec = forwardProjEastVec + forwardProjNorthVec;

                //find angle from abs north to projected forward vector measured clockwise
                var bearing = Math.Acos(forwardProjPlaneVec.Dot(relativeNorthVec) / forwardProjPlaneVec.Length() / relativeNorthVec.Length()) * rad2deg;

                //check direction of angle
                if (forwardVec.Dot(relativeEastVec) < 0)
                    bearing = 360 - bearing; //because of how the angle is measured

                return bearing;
            }
            private static Vector3D VectorProjection(Vector3D a, Vector3D b) {
                var projection = a.Dot(b) / b.Length() / b.Length() * b;
                return projection;
            }

            public static void InitDisplay(IMyTextSurface d) {
                d.ContentType = ContentType.TEXT_AND_IMAGE;
                d.Font = LCDFonts.MONOSPACE;
                d.FontSize = 1.144f;
                d.Alignment = TextAlignment.CENTER;
                d.TextPadding = 0f;
            }

            public static string GetDisplayText(double bearing) {
                if (double.IsNaN(bearing)) return string.Empty;

                var startIdx = (int)MathHelper.Clamp(Math.Round(bearing / 5), 0, 359);
                var sb = new StringBuilder();
                var headfoot = compassLineO.Substring(startIdx, 23);
                sb.AppendLine($"           ▼    {bearing,3:N0}°{GetCardinalDir(bearing),-2} ");
                sb.AppendLine(headfoot);
                sb.AppendLine(compassLineM.Substring(startIdx, 23));
                sb.AppendLine(headfoot);
                sb.Append(compassFooter);
                return sb.ToString();
            }

            private static string GetCardinalDir(double bearing) {
                var idx = (int)Math.Round(bearing / 45);
                if (idx >= cardinals.Length) idx = 0;
                return cardinals[idx];
            }
        }
    }
}
