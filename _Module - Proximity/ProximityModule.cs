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

// IngameScript
namespace IngameScript
{
    class ProximityModule
    {
        readonly List<IMyCameraBlock> _cameras = new List<IMyCameraBlock>();
        readonly Dictionary<int, double?> _ranges = new Dictionary<int, double?>();

        MyGridProgram thisObj;
        IMyShipController _sc;

        public ProximityModule()
        {

        }

        public string ProximityTag { get; set; }
        //public string ForwardTag { get; set; }

        

        public void Init(MyGridProgram thisObj, IMyShipController sc)
        {
            this.thisObj = thisObj;
            this._sc = sc;
        }

        //public Func<IMyTerminalBlock, bool>

        public void RunScan()
        {

        }
    }
}
