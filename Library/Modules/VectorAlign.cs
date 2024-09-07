// <mdk sortorder="1000" />
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

        // Modified from Wicorel's AutoGyro code
        // https://forums.keenswh.com/threads/gyro-alignment-code.7392762/
        // https://github.com/Wicorel/WicoSpaceEngineers/tree/master/SharedWicoGyro
        // Originally from: http://forums.keenswh.com/threads/aligning-ship-to-planet-gravity.7373513/#post-1286885461
        class VectorAlign {

            const float MinimumAngleRadians = 0.01f; // How tight to maintain aim. Smaller is tighter.
            readonly double ctrlCoeff;

            public VectorAlign(double controlCoeff = 0.8) {
                ctrlCoeff = controlCoeff;
            }

            public bool AlignWithGravity(IMyShipController sc, Base6Directions.Direction dir, IList<IMyGyro> gyroList, bool keepOverrideOn = false) {
                if (sc == null) return false;

                var gravityVec = sc.GetNaturalGravity();
                return AlignWithVector(gravityVec, sc, dir, gyroList, keepOverrideOn);
            }

            public bool AlignWithVector(Vector3D targetVec, IMyShipController sc, Base6Directions.Direction dir, IList<IMyGyro> gyroList, bool keepOverrideOn = false) {
                if (sc == null) return false;

                Matrix orient;
                sc.Orientation.GetMatrix(out orient);

                var currentVec = orient.GetDirectionVector(dir);

                currentVec.Normalize();
                targetVec.Normalize();

                var aligned = true;
                for (var i = 0; i < gyroList.Count; ++i) {
                    var rotationVec = GetRotationVector(gyroList[i], currentVec, targetVec);

                    aligned &= (rotationVec == Vector3D.Zero);
                    SetGyro(gyroList[i],
                        (float)rotationVec.GetDim(0),
                        -(float)rotationVec.GetDim(1),
                        -(float)rotationVec.GetDim(2),
                        !aligned || keepOverrideOn);
                }
                return aligned;
            }

            private Vector3D GetRotationVector(IMyGyro g, Vector3D currentVec, Vector3D targetVec) {
                // Transform vectors
                Matrix orient;
                g.Orientation.GetMatrix(out orient);
                currentVec = Vector3D.Transform(currentVec, MatrixD.Transpose(orient));
                targetVec = Vector3D.Transform(targetVec, MatrixD.Transpose(g.WorldMatrix.GetOrientation()));

                // Since the gyro UI lies, we are not trying to control yaw,pitch,roll but rather we need a rotation vector (axis around which to rotate) 
                var rotationVector = Vector3D.Cross(currentVec, targetVec);
                var dotVector = Vector3D.Dot(currentVec, targetVec);

                var ang = rotationVector.Length();
                ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));
                if (dotVector < 0) ang += Math.PI; // compensate for >+/-90

                if (ang < MinimumAngleRadians) return Vector3D.Zero; // close enough 

                var yawMax = g.GetMaximum<float>("Yaw");
                var ctrl_vel = yawMax * (ang / Math.PI) * ctrlCoeff;
                ctrl_vel = Math.Min(yawMax, ctrl_vel);
                ctrl_vel = Math.Max(0.01, ctrl_vel);

                rotationVector.Normalize();
                rotationVector *= ctrl_vel;
                return rotationVector;
            }

            public static void SetGyrosOff(List<IMyGyro> gyroList) => gyroList?.ForEach(g => SetGyroOff(g, false));
            public static void SetGyroOff(IMyGyro g, bool gyroOverride) => SetGyro(g, 0F, 0F, 0F, gyroOverride);
            public static void SetGyro(IMyGyro g, float pitch, float yaw, float roll, bool gyroOverride) {
                // Goes wonky if the properties are used.
                g.SetValueFloat("Pitch", pitch);
                g.SetValueFloat("Yaw", yaw);
                g.SetValueFloat("Roll", roll);
                g.GyroOverride = gyroOverride;
            }

        }
    }
}
