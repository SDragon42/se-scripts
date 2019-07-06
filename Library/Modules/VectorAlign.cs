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

            const float MIN_ANGLE_RADIANS = 0.01f; // how tight to maintain aim Lower is tighter.

            readonly double ctrlCoeff;

            public VectorAlign(double controlCoeff = 0.8) {
                ctrlCoeff = controlCoeff;
            }

            public bool AlignWithGravity(IMyShipController sc, Direction dir, IList<IMyGyro> gyroList, bool keepOverrideOn = false) {
                if (sc == null) return false;
                var grav = sc.GetNaturalGravity();
                return AlignWithVector(grav, sc, dir, gyroList, keepOverrideOn);
            }

            public bool AlignWithVector(Vector3D vDirection, IMyShipController sc, Direction dir, IList<IMyGyro> gyroList, bool keepOverrideOn = false) {
                if (sc == null) return false;
                var bAligned = true;

                Matrix or;
                sc.Orientation.GetMatrix(out or);

                var down = GetAlignmentVector(or, dir);
                vDirection.Normalize();

                for (var i = 0; i < gyroList.Count; ++i) {
                    var g = gyroList[i];
                    g.Orientation.GetMatrix(out or);
                    var localDown = Vector3D.Transform(down, MatrixD.Transpose(or));
                    var localGrav = Vector3D.Transform(vDirection, MatrixD.Transpose(g.WorldMatrix.GetOrientation()));

                    // Since the gyro UI lies, we are not trying to control yaw,pitch,roll but rather we need a rotation vector (axis around which to rotate) 
                    var rot = Vector3D.Cross(localDown, localGrav);
                    var dot2 = Vector3D.Dot(localDown, localGrav);
                    var ang = rot.Length();
                    ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));
                    if (dot2 < 0) ang += Math.PI; // compensate for >+/-90
                    if (ang < MIN_ANGLE_RADIANS) { // close enough 
                        SetGyroOff(g, keepOverrideOn);
                        continue;
                    }

                    var yawMax = g.GetMaximum<float>("Yaw");
                    var ctrl_vel = yawMax * (ang / Math.PI) * ctrlCoeff;
                    ctrl_vel = Math.Min(yawMax, ctrl_vel);
                    ctrl_vel = Math.Max(0.01, ctrl_vel);
                    rot.Normalize();
                    rot *= ctrl_vel;

                    SetGyro(g,
                        (float)rot.GetDim(0),
                        -(float)rot.GetDim(1),
                        -(float)rot.GetDim(2),
                        true);

                    bAligned = false;
                }
                return bAligned;
            }

            public void gyrosOff(List<IMyGyro> gyroList) {
                if (gyroList == null) return;
                gyroList.ForEach(g => SetGyroOff(g, false));
            }

            private void SetGyroOff(IMyGyro g, bool gyroOverride) {
                SetGyro(g, 0F, 0F, 0F, gyroOverride);
            }
            private void SetGyro(IMyGyro g, float pitch, float yaw, float roll, bool gyroOverride) {
                // Goes wonky if the properties are used.
                g.SetValueFloat("Pitch", pitch);
                g.SetValueFloat("Yaw", yaw);
                g.SetValueFloat("Roll", roll);
                g.GyroOverride = gyroOverride;
            }

            Vector3D GetAlignmentVector(Matrix craftOrient, Direction dir) {
                switch (dir) {
                    case Direction.Forward: return craftOrient.Forward;
                    case Direction.Backward: return craftOrient.Backward;
                    case Direction.Left: return craftOrient.Left;
                    case Direction.Right: return craftOrient.Right;
                    case Direction.Up: return craftOrient.Up;
                    default: return craftOrient.Down;
                }
            }
        }
    }
}
