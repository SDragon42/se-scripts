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
        class GpsInfo {
            public GpsInfo(string rawGPS) {
                string name;
                Vector3D loc;
                VectorHelper.GpsToVector(rawGPS, out name, out loc);
                RawGPS = rawGPS;
                Location = loc;
                if (name.StartsWith("*")) {
                    NeedsClearance = false;
                    Name = name.Substring(1);
                } else {
                    Name = name;
                    NeedsClearance = true;
                }
            }

            public string Name { get; private set; }
            public Vector3D Location { get; private set; }
            public bool NeedsClearance { get; private set; }
            public string RawGPS { get; private set; }
        }
    }
}
