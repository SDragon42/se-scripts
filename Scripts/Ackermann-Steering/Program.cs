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
    partial class Program : MyGridProgram {
        /* Modify the following values as needed: */

        /* To move the focus point, middle value is forward(+)/backward(-) */
        Vector3D AnchorOffset = new Vector3D(0.0, -5.0, 0.0);
        /* Friction when going straight and of inside wheels when sliding */
        const float FrictionInside = 50.0f;
        /* Friction of outside wheels when sliding */
        const float FrictionOutside = 8.0f;
        /* Friction in case of braking when going straight and of inside wheels when sliding */
        const float FrictionBrakInside = 70.0f;
        /* Friction in case of braking of rear outside wheel(s) when sliding */
        const float FrictionBrakOutside = 15.0f;
        /* Above this sideways speed [m/s] the vehicle is considered sliding */
        const float SlideSpeedLimit = 5.0f;

        /* Don't modify lines below (or at own risk) */
        const bool debug = false;
        const double secondsElapsedInv = 60.0;

        WheelController wheelController;
        LinearSpeed speedInfo;

        static MyGridProgram GP;

        public void Main(string argument) {
            List<IMyTerminalBlock> wheels = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyMotorSuspension>(wheels,
                x => (x.CubeGrid == Me.CubeGrid) && (x.IsWorking));
            if (wheelController == null || argument == "reset" || wheelController.WheelAdded(wheels.Count)) {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                GP = this;
                GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(blocks, x => (x.CubeGrid == Me.CubeGrid));

                // Lets initialize an example wheel controller
                wheelController = new WheelController(
                        blocks.Count == 0 ? Me : blocks[0],
                        wheels,
                        2f,
                        4f,
                        AnchorOffset
                    );
            }
            if (speedInfo == null) speedInfo = new LinearSpeed(wheelController.Anchor);
            speedInfo.Update();
            wheelController.Update(speedInfo.curForwardSpd, speedInfo.curLeftSpd);
        }

        //###################################################################

        public class LinearSpeed {
            Vector3D speedVector;
            float prevForwardSpd;
            float prevLeftSpd;
            float prevUpSpd;
            Vector3D? prevVehiclePos;
            Filter accFilter;
            IMyTerminalBlock refBlock;
            public float curForwardSpd;
            public float curLeftSpd;
            public float curUpSpd;
            public bool reverse;
            public float vehSpeed;
            public float absForwardSpd;
            public float signForwardSpd;

            public float acceleration;
            public float accLeft;
            public float accUp;

            class Filter {
                float[] values;
                int numValues;
                int index;

                public Filter(int num) {
                    index = 0;
                    if (num > 0) numValues = num; else numValues = 1;
                    values = new float[numValues];
                    Array.Clear(values, index, numValues);
                }

                public void Add(float value) {
                    if (index >= numValues) index = 0;
                    values[index] = value;
                    index++;
                }

                public float Get() {
                    float sum = 0.0f;
                    for (int ix = 0; ix < numValues; ix++) {
                        sum += values[ix];
                    }
                    return (float)(sum / numValues);
                }
            }

            public LinearSpeed(IMyTerminalBlock refB) {
                prevForwardSpd = float.NaN;
                prevLeftSpd = float.NaN;
                prevUpSpd = float.NaN;
                prevVehiclePos = null;
                accFilter = new Filter(20);
                refBlock = refB;
            }

            public void Update() {
                MatrixD refBase = MatrixD.Normalize(refBlock.WorldMatrix);
                Vector3D? pos = refBlock.GetPosition();
                /*** Linear speed and acceleration ***/
                speedVector = new Vector3D(0, 0, 0);

                if (pos.HasValue) {
                    if (prevVehiclePos.HasValue) speedVector = (pos.Value - prevVehiclePos.Value) * secondsElapsedInv;
                    prevVehiclePos = pos.Value;
                }
                curForwardSpd = (float)Vector3D.Dot(speedVector, refBase.Forward);
                curLeftSpd = (float)Vector3D.Dot(speedVector, refBase.Left);
                curUpSpd = (float)Vector3D.Dot(speedVector, refBase.Up);
                reverse = curForwardSpd < -0.1f;
                vehSpeed = (float)speedVector.Length();
                absForwardSpd = Math.Abs(curForwardSpd);
                signForwardSpd = Math.Sign(curForwardSpd);

                acceleration = 0.0f;
                accLeft = 0.0f;
                accUp = 0.0f;

                if (prevForwardSpd != float.NaN) acceleration = (curForwardSpd - prevForwardSpd) * (float)secondsElapsedInv;
                if (prevLeftSpd != float.NaN) accLeft = (curLeftSpd - prevLeftSpd) * (float)secondsElapsedInv;
                if (prevUpSpd != float.NaN) accUp = (curUpSpd - prevUpSpd) * (float)secondsElapsedInv;
                accFilter.Add(accUp);
                prevForwardSpd = curForwardSpd;
                prevLeftSpd = curLeftSpd;
                prevUpSpd = curUpSpd;
            }

        }

        //###################################################################

        public class WheelController {
            static readonly Dictionary<string, Vector3D> WheelOffsetLookup = new Dictionary<string, Vector3D>
                {
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

            public class Wheel {
                IMyMotorSuspension Block;
                Vector3I blockPos;
                IMyCubeGrid blockGrid;
                bool leftSide;
                bool front;
                bool rear;
                float AngleLeft;
                float AngleRight;
                float AngleMax;
                MatrixD MatrixLocal;
                MatrixD MatrixLocalInv;
                float SteerSpeedLeft, SteerSpeedRight;
                float ReturnSpeedLeft, ReturnSpeedRight;
                float LastAngle;
                float LastDirection;
                float frictionInside;
                float frictionOutside;
                float frictionBrakInside;
                float frictionBrakOutside;
                float slideSpeedLimit;

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

                    bool outside = false;

                    float CurrentAngle = Block.SteerAngle;
                    float CurrentDirection = CurrentAngle - LastAngle;

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
                    float X = (float)MatrixLocal.Translation.GetDim(0);
                    float Z = (float)MatrixLocal.Translation.GetDim(2);
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
                        Block.SetCustomName(Block.CustomName + (leftSide ? " (left)" : " (right)") +
                            (front ? " (front)" : "") + (rear ? " (rear)" : ""));
                    }
                }
            }
        }

    }
}
