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
        public class WheelController {
            static readonly Dictionary<string, Vector3D> WheelOffsetLookup = new Dictionary<string, Vector3D> {
                { "Large", new Vector3D(0, 0, -3.1) },
                { "Small", new Vector3D(0, 0, -0.6) }
            };

            string EchoString;

            List<Wheel> Wheels = new List<Wheel>();
            int prevNumWheels;
            IMyTerminalBlock anchor;
            public Vector3D? center;
            float SteerSpeedFactor;
            float ReturnSpeedFactor;

            /*
                @anchor - The anchor block which gets used as reference for position and orientation.
                @wheels - The list of wheels with which we have to work.
                @steerSpeedFactor - This is a proportional factor for total steer speed over the max angle.
                @returnSpeedFactor - This is a proportional factor for total return speed over the max angle.
                @offset - The additional offset in forward and right direction.
            */
            public WheelController(IMyTerminalBlock refBlock, List<IMyTerminalBlock> wheels, float steerSpeedFactor,
                float steerReturnFactor, Vector3D offset) {
                Wheels.Clear();
                anchor = refBlock;
                MatrixD anchorMatrixInv = MatrixD.Invert(Anchor.WorldMatrix);
                center = null;

                if (debug) EchoString = "";
                SteerSpeedFactor = steerSpeedFactor;
                ReturnSpeedFactor = steerReturnFactor;

                double FocusLeft = double.MaxValue;
                double FocusRight = double.MinValue;

                Vector3I anchorPosGrid = anchor.Position;

                for (int i = 0; i < wheels.Count; i++) {
                    // Check if the wheel actually is a suspension wheel.
                    IMyMotorSuspension wheel = wheels[i] as IMyMotorSuspension;
                    if (wheel == null)
                        continue;

                    // Get the transformed local matrix in respect to the new anchor with offset.
                    MatrixD wheelMatrix = wheel.WorldMatrix * anchorMatrixInv;

                    MatrixD wheelMatrixInv = wheelMatrix;
                    wheelMatrixInv.Translation = new Vector3D(0, 0, 0);
                    wheelMatrixInv = MatrixD.Invert(wheelMatrixInv);
                    Vector3D wheelOffset = WheelOffsetLookup.ContainsKey(wheel.BlockDefinition.SubtypeId) ?
                        WheelOffsetLookup[wheel.BlockDefinition.SubtypeId] : WheelOffsetLookup[wheel.CubeGrid.GridSizeEnum.ToString()];
                    wheelOffset = Vector3D.Transform(wheelOffset - offset, wheelMatrixInv);
                    wheelMatrix.Translation += wheelOffset;

                    if (i > 0) center = center.Value + wheel.GetPosition();
                    else center = wheel.GetPosition();

                    float maxSteerAngle = wheel.GetMaximum<float>("MaxSteerAngle");
                    if (wheel.CustomName.Contains("MaxSteerAngle=")) {
                        string[] split = wheel.CustomName.Split(' ');
                        for (int j = 0; j < split.Length; j++) {
                            if (!split[j].StartsWith("MaxSteerAngle="))
                                continue;
                            split[j] = split[j].Replace("MaxSteerAngle=", "");
                            float angle = 0;
                            bool isDegree = split[j].EndsWith("°");
                            if (isDegree)
                                split[j] = split[j].Replace("°", "");
                            if (!float.TryParse(split[j], out angle))
                                continue;
                            if (isDegree)
                                angle = VRageMath.MathHelper.ToRadians(angle);
                            if (angle < 0)
                                angle *= -1;
                            if (angle < maxSteerAngle)
                                maxSteerAngle = angle;
                        }
                    }
                    double Z = wheelMatrix.Translation.GetDim(2);
                    double tan = Math.Tan(maxSteerAngle);
                    double deltaX = Math.Abs(Z / tan);
                    double X = wheelMatrix.Translation.GetDim(0);
                    double CurrentFocusLeft = X - deltaX;
                    double CurrentFocusRight = X + deltaX;
                    //DEBUG
                    if (debug) EchoString += string.Format("> {0}\nX:{1:0.00} - Z:{2:0.00}", wheel.CustomName, wheelMatrix.Translation.GetDim(0), wheelMatrix.Translation.GetDim(2));
                    //GP.Echo(string.Format("> {0}\nX:{3} - Z\nangle: {4:0.000} - tan: {5:0.000}\n{1}:{2}",wheel.CustomName, CurrentFocusLeft, CurrentFocusRight,X,Z,maxSteerAngle,tan));
                    if (CurrentFocusLeft < FocusLeft)
                        FocusLeft = CurrentFocusLeft;
                    if (CurrentFocusRight > FocusRight)
                        FocusRight = CurrentFocusRight;

                    // Add the new wheel with parameters to the list of wheels.
                    Wheels.Add(new Wheel(
                            wheel,
                            wheelMatrix,
                            maxSteerAngle
                        ));
                }
                if (debug) GP.Echo(EchoString);
                prevNumWheels = Wheels.Count;
                if (center.HasValue) center = center / prevNumWheels;
                for (int i = 0; i < Wheels.Count; i++) {
                    Wheels[i].SetFocus((float)FocusLeft, (float)FocusRight, SteerSpeedFactor, ReturnSpeedFactor);
                }
            }

            public void Update(float forwSpd, float leftSpd) {
                Wheels.RemoveAll(x => !x.IsFunctional());
                for (int i = 0; i < Wheels.Count; i++) {
                    Wheels[i].Update(forwSpd, leftSpd);
                }
                prevNumWheels = Wheels.Count;
            }

            public bool WheelAdded(int currentNumWheels) {
                return (currentNumWheels > prevNumWheels);
            }

            public IMyTerminalBlock Anchor {
                get {
                    return anchor;
                }
            }
            public Vector3D? Center {
                get {
                    return center;
                }
            }


        }
    }
}
