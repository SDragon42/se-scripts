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

            readonly List<Direction> KeyList;
            readonly Dictionary<Direction, double?> _prox1 = new Dictionary<Direction, double?>();
            readonly Dictionary<Direction, double?> _prox2 = new Dictionary<Direction, double?>();

            Dictionary<Direction, double?> _currProx;
            Dictionary<Direction, double?> _prevProx;

            BlocksByOrientation _orientation;
            IMyShipController _sc;

            public double ScanRange { get; set; }



            public Proximity() {
                ScanRange = DEF_SCAN_RANGE;
                KeyList = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
                foreach (var key in KeyList) {
                    _prox1.Add(key, null);
                    _prox2.Add(key, null);
                }

                _currProx = _prox1;
                _prevProx = _prox2;
            }
            


            public double? GetRange(Direction dir) => _currProx[dir];
            public double? GetRangeDiff(Direction dir) => _currProx[dir] - _prevProx[dir];

            public void RunScan(MyGridProgram mgp, IMyShipController sc, List<IMyCameraBlock> cameras) {
                SwapProxyLists();
                KeyList.ForEach(k => _currProx[k] = null);

                if (_sc != sc) {
                    _sc = sc;
                    _orientation = new BlocksByOrientation(sc);
                }

                if (_orientation != null) {
                    _currProx[Direction.Forward] = GetMinimumRange(mgp, cameras, _orientation.IsForward);
                    _currProx[Direction.Backward] = GetMinimumRange(mgp, cameras, _orientation.IsBackward);
                    _currProx[Direction.Left] = GetMinimumRange(mgp, cameras, _orientation.IsLeft);
                    _currProx[Direction.Right] = GetMinimumRange(mgp, cameras, _orientation.IsRight);
                    _currProx[Direction.Up] = GetMinimumRange(mgp, cameras, _orientation.IsUp);
                    _currProx[Direction.Down] = GetMinimumRange(mgp, cameras, _orientation.IsDown);
                }
            }

            void SwapProxyLists() {
                if (_prevProx == _prox1) {
                    _currProx = _prox1;
                    _prevProx = _prox2;
                } else {
                    _currProx = _prox2;
                    _prevProx = _prox1;
                }
            }

            double? GetMinimumRange(MyGridProgram mpg, List<IMyCameraBlock> cameras, Func<IMyTerminalBlock, bool> directionMethod) {
                var range = cameras
                    .Where(c => directionMethod(c))
                    .Select(c => Ranger.GetDetailedRange(c, ScanRange))
                    .Min(r => r.Range);
                //return (range < ScanRange) ? range : null;
                if (!range.HasValue) return null;
                if (range > ScanRange) return null;
                if (range < 0.000001) return null;
                return range;
            }

        }
    }
}
