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
    interface ITestingBase {
        void Main(string argument);
    }

    class TestingBase {
        protected readonly MyGridProgram thisObj;
        protected Action<string> Echo;
        protected IMyGridTerminalSystem GridTerminalSystem { get { return thisObj.GridTerminalSystem; } }

        public TestingBase(MyGridProgram thisObj) {
            this.thisObj = thisObj;
            Echo = thisObj.Echo;
        }
    }
}
