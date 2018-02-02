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
        class Proximity {
            const double DEF_SCAN_RANGE = 100.0;

            readonly List<IMyCameraBlock> _cameras = new List<IMyCameraBlock>();
            readonly List<Direction> KeyList;
            readonly Dictionary<Direction, double?> _currProx = new Dictionary<Direction, double?>();
            readonly Dictionary<Direction, double?> _prevProx = new Dictionary<Direction, double?>();

            public Proximity() {
                ScanRange = DEF_SCAN_RANGE;
                KeyList = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
                foreach (var key in KeyList) {
                    _currProx.Add(key, null);
                    _prevProx.Add(key, null);
                }
            }

            BlocksByOrientation _orientation;
            IMyShipController _sc;

            string _tag = string.Empty;
            public string Tag { get { return _tag; } set { _tag = value?.Trim() ?? string.Empty; } }

            public double ScanRange { get; set; }

            public double? GetRange(Direction dir) => _currProx[dir];
            public double? GetRangeDiff(Direction dir) => _currProx[dir] - _prevProx[dir];

            public void RunScan(MyGridProgram mgp, IMyShipController sc) {
                foreach (var key in KeyList) {
                    _prevProx[key] = _currProx[key];
                    _currProx[key] = null;
                }

                if (_sc != sc) {
                    _sc = sc;
                    _orientation = new BlocksByOrientation(sc);
                }

                if (_orientation != null) {
                    _currProx[Direction.Forward] = GetMinimumRange(mgp, _orientation.IsForward);
                    _currProx[Direction.Backward] = GetMinimumRange(mgp, _orientation.IsBackward);
                    _currProx[Direction.Left] = GetMinimumRange(mgp, _orientation.IsLeft);
                    _currProx[Direction.Right] = GetMinimumRange(mgp, _orientation.IsRight);
                    _currProx[Direction.Up] = GetMinimumRange(mgp, _orientation.IsUp);
                    _currProx[Direction.Down] = GetMinimumRange(mgp, _orientation.IsDown);
                }
            }

            double? GetMinimumRange(MyGridProgram mpg, Func<IMyTerminalBlock, bool> directionMethod) {
                mpg.GridTerminalSystem.GetBlocksOfType(_cameras, b => IsTaggedBlock(b) && directionMethod(b));
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
}
