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
        readonly List<IMyCameraBlock> _camFront = new List<IMyCameraBlock>();
        readonly List<IMyCameraBlock> _camBack = new List<IMyCameraBlock>();
        readonly List<IMyCameraBlock> _camLeft = new List<IMyCameraBlock>();
        readonly List<IMyCameraBlock> _camRight = new List<IMyCameraBlock>();
        readonly List<IMyCameraBlock> _camUp = new List<IMyCameraBlock>();
        readonly List<IMyCameraBlock> _camDown = new List<IMyCameraBlock>();

        readonly Dictionary<int, double?> _ranges = new Dictionary<int, double?>();

        BlocksByOrientation _orientation;
        IMyShipController _sc;
        string _proximityTag = string.Empty;



        public void RunScan(MyGridProgram thisObj, IMyShipController sc)
        {
            if (_sc != sc)
            {
                _sc = sc;
                _orientation = new BlocksByOrientation(sc);
            }

            if (_orientation != null)
            {
                thisObj.GridTerminalSystem.GetBlocksOfType(_camFront, b => IsTaggedBlock(b) && _orientation.IsForward(b));
                thisObj.GridTerminalSystem.GetBlocksOfType(_camBack, b => IsTaggedBlock(b) && _orientation.IsBackward(b));
                thisObj.GridTerminalSystem.GetBlocksOfType(_camLeft, b => IsTaggedBlock(b) && _orientation.IsLeft(b));
                thisObj.GridTerminalSystem.GetBlocksOfType(_camRight, b => IsTaggedBlock(b) && _orientation.IsRight(b));
                thisObj.GridTerminalSystem.GetBlocksOfType(_camUp, b => IsTaggedBlock(b) && _orientation.IsUp(b));
                thisObj.GridTerminalSystem.GetBlocksOfType(_camDown, b => IsTaggedBlock(b) && _orientation.IsDown(b));
            }
        }


        bool IsTaggedBlock(IMyTerminalBlock b)
        {
            if (_proximityTag.Length == 0) return true;
            return b.CustomName.ToLower().Contains(_proximityTag);
        }
    }
}
