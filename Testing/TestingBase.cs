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

namespace IngameScript
{
    class TestingBase
    {
        public MyGridProgram thisObj { get; private set; }

        public IMyGridTerminalSystem GridTerminalSystem { get { return thisObj.GridTerminalSystem; } }
        public Action<string> Echo { get { return this.thisObj.Echo; } }

        public void Init(MyGridProgram thisObj)
        {
            this.thisObj = thisObj;
        }

        public virtual void Main(string argument)
        {

        }
    }
}
