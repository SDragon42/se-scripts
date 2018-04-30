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
        public class ProxCamera {
            public ProxCamera(IMyCameraBlock camera, double offset) {
                Camera = camera;
                Offset = offset;
            }
            public IMyCameraBlock Camera { get; private set; }
            public double Offset { get; private set; }
        }
    }
}
