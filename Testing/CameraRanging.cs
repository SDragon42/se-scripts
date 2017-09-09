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
    class CameraRanging : TestingBase
    {
        const double SCAN_DISTANCE = 25000;
        const float PITCH = 0;
        const float YAW = 0;


        readonly TimeIntervalModule _interval;
        readonly RunningSymbolModule _running;
        readonly List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();

        public CameraRanging() : base()
        {
            _interval = new TimeIntervalModule(10);
            _running = new RunningSymbolModule();
        }


        public override void Main(string argument)
        {
            Echo("Scanner " + _running.GetSymbol(thisObj.Runtime));
            _interval.RecordTime(thisObj.Runtime);

            if (!_interval.AtNextInterval()) return;

            UpdateRange("Camera - Forward", "Flat LCD - Left");
            UpdateRange("Camera - Docking", "Flat LCD - Right");
        }

        void UpdateRange(string cameraName, string lcdName)
        {
            Echo("Scanning: " + cameraName);
            GridTerminalSystem.GetBlocksOfType(_blocks, b => IsNamedType<IMyCameraBlock>(b, cameraName));
            var camera = _blocks.FirstOrDefault() as IMyCameraBlock;
            if (camera == null) return;

            GridTerminalSystem.GetBlocksOfType(_blocks, b => IsNamedType<IMyTextPanel>(b, lcdName));
            var lcd = _blocks.FirstOrDefault() as IMyTextPanel;
            if (lcd == null) return;

            camera.EnableRaycast = true;

            var info = default(MyDetectedEntityInfo);
            if (camera.CanScan(SCAN_DISTANCE))
            {
                info = camera.Raycast(SCAN_DISTANCE, PITCH, YAW);

                var range = -1.0;
                if (info.HitPosition.HasValue)
                    //range = (camera.GetPosition() - de.HitPosition.Value).Length();
                    //range = Vector3D.Distance(camera.GetPosition(), de.HitPosition.Value);
                    range = Vector3D.Distance(camera.GetPosition(), info.HitPosition.Value);

                var rangeText = range >= 0 ? $"{range:N2} m" : "---";
                lcd.WritePublicText($"Range: {rangeText}");
                lcd.ShowPublicTextOnScreen();
            }
        }

        bool IsNamedType<T>(IMyTerminalBlock b, string name) where T : IMyTerminalBlock
        {
            if (b.CubeGrid.EntityId != thisObj.Me.CubeGrid.EntityId) return false;
            if (!(b is T)) return false;
            return (b.CustomName == name);
        }
    }
}
