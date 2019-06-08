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
        public class Wheel {
            IMyMotorSuspension Block;
            Vector3I blockPos;
            IMyCubeGrid blockGrid;
            bool leftSide;
            bool front;
            bool rear;
            float AngleLeft;
            float AngleRight;
            readonly float AngleMax;
            MatrixD MatrixLocal;
            //MatrixD MatrixLocalInv;
            float SteerSpeedLeft, SteerSpeedRight;
            float ReturnSpeedLeft, ReturnSpeedRight;
            float LastAngle;
            float LastDirection;
            readonly float frictionInside;
            readonly float frictionOutside;
            readonly float frictionBrakInside;
            readonly float frictionBrakOutside;
            readonly float slideSpeedLimit;

            public Wheel(IMyMotorSuspension block, MatrixD matrix, float angleMax) {
                Block = block;
                blockPos = Block.Position;
                blockGrid = Block.CubeGrid;
                MatrixLocal = matrix;
                AngleMax = angleMax;
                frictionInside = FrictionInside;
                frictionOutside = FrictionOutside;
                frictionBrakInside = FrictionBrakInside;
                frictionBrakOutside = FrictionBrakOutside;
                slideSpeedLimit = SlideSpeedLimit;
            }

            public bool BlockExists() {
                return blockGrid.CubeExists(blockPos);
            }

            public bool IsFunctional() {
                return BlockExists() && Block.IsWorking;
            }

            public bool IsWorking() {
                return Block.IsWorking;
            }

            public void SetValueFloat(string prop, float value) {
                if (Block.GetValueFloat(prop) != value) Block.SetValueFloat(prop, value);
            }

            public bool Update(float forwSpd, float leftSpd) {
                if ((Block == null) || !Block.IsWorking)
                    return false;

                var outside = false;

                var CurrentAngle = Block.SteerAngle;
                var CurrentDirection = CurrentAngle - LastAngle;

                if (Block.SteerAngle > 0.001f) {
                    outside = !leftSide;
                    SetValueFloat("MaxSteerAngle", AngleLeft);
                    SetValueFloat("SteerSpeed", SteerSpeedLeft);
                    SetValueFloat("SteerReturnSpeed", ReturnSpeedLeft);
                } else if (Block.SteerAngle < -0.001f) {
                    outside = leftSide;
                    SetValueFloat("MaxSteerAngle", AngleRight);
                    SetValueFloat("SteerSpeed", SteerSpeedRight);
                    SetValueFloat("SteerReturnSpeed", ReturnSpeedRight);
                }

                if (((leftSpd > slideSpeedLimit) && leftSide) || ((leftSpd < -slideSpeedLimit) && !leftSide)) {
                    if (Block.Brake && ((rear && (forwSpd > 1.0f)) || (front && (forwSpd < -1.0f)))) {
                        SetValueFloat("Friction", frictionBrakOutside);
                    } else {
                        SetValueFloat("Friction", frictionOutside);
                    }
                } else {
                    if (Block.Brake) {
                        SetValueFloat("Friction", frictionBrakInside);
                    } else {
                        if (front && outside) {
                            SetValueFloat("Friction", frictionBrakOutside);
                        } else {
                            SetValueFloat("Friction", frictionInside);
                        }
                    }
                }

                // Final sets for the next run.
                LastAngle = CurrentAngle;
                LastDirection = CurrentDirection;
                return true;
            }

            public void SetFocus(float focusLeft, float focusRight, float steerSpeedFactor, float returnSpeedFactor) {
                var X = (float)MatrixLocal.Translation.GetDim(0);
                var Z = (float)MatrixLocal.Translation.GetDim(2);
                AngleRight = (float)Math.Abs(Math.Atan(Z / ((Z > 0 ? focusLeft : focusRight) - X)));
                AngleLeft = (float)Math.Abs(Math.Atan(Z / ((Z > 0 ? focusRight : focusLeft) - X)));
                front = Z < 0.0f;
                rear = Z > 0.0f;
                SteerSpeedLeft = AngleLeft * steerSpeedFactor;
                SteerSpeedRight = AngleRight * steerSpeedFactor;
                leftSide = X < 0.0f;
                ReturnSpeedLeft = AngleLeft * returnSpeedFactor;
                ReturnSpeedRight = AngleRight * returnSpeedFactor;
                if (debug) {
                    Block.CustomName = Block.CustomName + (leftSide ? " (left)" : " (right)") +
                        (front ? " (front)" : "") + (rear ? " (rear)" : "");
                }
            }
        }
    }
}
