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
    }
}
