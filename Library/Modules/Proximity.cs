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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
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

            public Action<string> Debug = (msg) => { };



            public double? GetRange(Direction dir) => _currProx[dir];
            public double? GetRangeDiff(Direction dir) => _currProx[dir] - _prevProx[dir];

            public void RunScan(MyGridProgram mgp, IMyShipController sc, List<ProxCamera> cameras) {
                SwapProxyLists();
                KeyList.ForEach(k => _currProx[k] = null);

                if (_sc != sc) {
                    _sc = sc;
                    _orientation = new BlocksByOrientation(sc);
                }

                if (_orientation != null) {
                    Debug("Forward");
                    _currProx[Direction.Forward] = GetMinimumRange(mgp, cameras, _orientation.IsForward);
                    Debug("Backward");
                    _currProx[Direction.Backward] = GetMinimumRange(mgp, cameras, _orientation.IsBackward);
                    Debug("Left");
                    _currProx[Direction.Left] = GetMinimumRange(mgp, cameras, _orientation.IsLeft);
                    Debug("Right");
                    _currProx[Direction.Right] = GetMinimumRange(mgp, cameras, _orientation.IsRight);
                    Debug("Up");
                    _currProx[Direction.Up] = GetMinimumRange(mgp, cameras, _orientation.IsUp);
                    Debug("Down");
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

            double? GetMinimumRange(MyGridProgram mpg, List<ProxCamera> cameras, Func<IMyTerminalBlock, bool> directionMethod) {
                var allRanges = cameras
                    .Where(c => directionMethod(c.Camera))
                    .Select(c => new { c.Camera, Ranger.GetDetailedRange(c.Camera, ScanRange, c.Offset).Range });
                foreach (var r in allRanges) Debug($"  {r.Range,4:N2} - {r.Camera.CustomName}");
                var range = allRanges.Min(r => r.Range);
                if (!range.HasValue) return null;
                if (range > ScanRange) return null;
                //if (range < 0.000001) return null;
                return range;
            }

        }

        class ProxCamera {
            public ProxCamera(IMyCameraBlock camera, double offset) {
                Camera = camera;
                Offset = offset;
            }
            public IMyCameraBlock Camera { get; private set; }
            public double Offset { get; private set; }
        }
    }
}
