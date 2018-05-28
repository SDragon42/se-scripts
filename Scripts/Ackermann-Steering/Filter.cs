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
        class Filter {
            readonly float[] values;
            readonly int numValues;
            int index;

            public Filter(int num) {
                var index = 0;
                numValues = (num > 0) ? num : 1;
                values = new float[numValues];
                Array.Clear(values, index, numValues);
            }

            public void Add(float value) {
                if (index >= numValues) index = 0;
                values[index] = value;
                index++;
            }

            public float Get() {
                var sum = 0.0f;
                for (var ix = 0; ix < numValues; ix++)
                    sum += values[ix];
                return (sum / numValues);
            }
        }
    }
}
