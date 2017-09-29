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
        ITestingBase _testingCode;
        readonly List<IMyTerminalBlock> _buffer = new List<IMyTerminalBlock>();

        public Program() {
            _testingCode = new MonospaceChars(this);
        }

        public void Save() {
        }

        public void Main(string argument) {
            _testingCode.Main(argument);
        }


    }
}
