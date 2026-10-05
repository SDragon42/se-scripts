// <mdk sortorder="1000" />
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
            public const double DEFAULT_SCAN_RANGE = 100;

            readonly Dictionary<Base6Directions.Direction, double?> _prox1 = new Dictionary<Base6Directions.Direction, double?>();
            readonly Dictionary<Base6Directions.Direction, double?> _prox2 = new Dictionary<Base6Directions.Direction, double?>();
            Dictionary<Base6Directions.Direction, double?> _currProx;
            Dictionary<Base6Directions.Direction, double?> _prevProx;
            readonly BlocksByOrientation _orientation = new BlocksByOrientation();
            IMyShipController _sc;

            public Proximity() {
                foreach (var dir in Base6Directions.EnumDirections) {
                    _prox1.Add(dir, null);
                    _prox2.Add(dir, null);
                }
                _currProx = _prox1;
                _prevProx = _prox2;
            }

            public double ScanRange { get; set; } = DEFAULT_SCAN_RANGE;


            public Action<string> Debug = (msg) => { };

            public double? GetClosestRange() => _currProx.Min(kv => kv.Value);
            public double? GetRange(Base6Directions.Direction dir) => _currProx[dir];
            public double? GetRangeDiff(Base6Directions.Direction dir) => _currProx[dir] - _prevProx[dir];


            public void Init(IMyShipController sc) {
                if (sc == _sc) return;
                _sc = sc;
                _orientation.Init(_sc);
            }
            public void RunScan(List<ProxCamera> cameras) {
                if (_sc == null || _orientation == null) return;
                SwapProxyLists();
                foreach (var k in Base6Directions.EnumDirections) { _currProx[k] = null; }

                _currProx[Base6Directions.Direction.Forward] = GetMinimumRange(cameras, _orientation.IsForward);
                _currProx[Base6Directions.Direction.Backward] = GetMinimumRange(cameras, _orientation.IsBackward);
                _currProx[Base6Directions.Direction.Left] = GetMinimumRange(cameras, _orientation.IsLeft);
                _currProx[Base6Directions.Direction.Right] = GetMinimumRange(cameras, _orientation.IsRight);
                _currProx[Base6Directions.Direction.Up] = GetMinimumRange(cameras, _orientation.IsUp);
                _currProx[Base6Directions.Direction.Down] = GetMinimumRange(cameras, _orientation.IsDown);
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

            double? GetMinimumRange(List<ProxCamera> cameras, Func<IMyTerminalBlock, bool> directionMethod) {
                var allRanges = cameras
                    .Where(c => directionMethod(c.Camera))
                    .Select(proxCamera => new { proxCamera.Camera, Range = GetRange(proxCamera) });
                var range = allRanges.Min(r => r.Range);
                return (range.HasValue && range <= ScanRange) ? range : null;
            }

            double? GetRange(ProxCamera proxCamera) {
                MyDetectedEntityInfo info;
                var success = RangeHelper.TryGetDetailedRange(proxCamera.Camera, ScanRange, out info);
                if (!success) return null;
                var range = Vector3D.Distance(proxCamera.Camera.GetPosition(), info.HitPosition ?? info.Position);
                return range - proxCamera.Offset;
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
