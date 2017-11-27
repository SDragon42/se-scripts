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
    class Proximity {
        const double SCAN_RANGE = 100.0;

        readonly List<IMyCameraBlock> _cameras = new List<IMyCameraBlock>();

        BlocksByOrientation _orientation;
        IMyShipController _sc;

        string _tag = string.Empty;
        public string ProximityTag { get { return _tag; } set { _tag = value?.Trim()?.ToLower() ?? string.Empty; } }

        double _scanRange = SCAN_RANGE;
        public double ScanRange { get { return _scanRange; } set { _scanRange = value; } }


        public double? Forward { get; private set; }
        public double? Backward { get; private set; }
        public double? Left { get; private set; }
        public double? Right { get; private set; }
        public double? Up { get; private set; }
        public double? Down { get; private set; }

        public void RunScan(MyGridProgram thisObj, IMyShipController sc) {
            Forward = null;
            Backward = null;
            Left = null;
            Right = null;
            Up = null;
            Down = null;

            if (_sc != sc) {
                _sc = sc;
                _orientation = new BlocksByOrientation(sc);
            }

            if (_orientation != null) {
                Forward = GetMinimumRange(thisObj, _orientation.IsForward);
                Backward = GetMinimumRange(thisObj, _orientation.IsBackward);
                Left = GetMinimumRange(thisObj, _orientation.IsLeft);
                Right = GetMinimumRange(thisObj, _orientation.IsRight);
                Up = GetMinimumRange(thisObj, _orientation.IsUp);
                Down = GetMinimumRange(thisObj, _orientation.IsDown);
            }
        }

        double? GetMinimumRange(MyGridProgram thisObj, Func<IMyTerminalBlock, bool> directionMethod) {
            thisObj.GridTerminalSystem.GetBlocksOfType(_cameras, b => IsTaggedBlock(b) && directionMethod(b));
            var range = _cameras
                .Select(c => Ranger.GetDetailedRange(c, ScanRange))
                .Min(r => r.Range);
            return (range < ScanRange) ? range : null;
        }


        bool IsTaggedBlock(IMyTerminalBlock b) {
            if (_tag.Length == 0) return true;
            return b.CustomName.ToLower().Contains(_tag);
        }
    }
}
