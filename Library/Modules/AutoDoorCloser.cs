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
        /// <summary>Automatically will close doors after a set amount of time
        /// </summary>
        /// <remarks>
        /// Whiplash's : Whip's Auto Door and Airlock Script
        /// http://steamcommunity.com/sharedfiles/filedetails/?id=416932930
        /// </remarks>
        class AutoDoorCloser {
            readonly Dictionary<IMyDoor, double> _openDoors = new Dictionary<IMyDoor, double>();

            public AutoDoorCloser(double secondsToLeaveOpen = 4) {
                _secondsToLeaveOpen = secondsToLeaveOpen > 0.0 ? secondsToLeaveOpen : 0.0;
            }

            double _secondsToLeaveOpen;

            public void CloseOpenDoors(IMyGridProgramRuntimeInfo runtime, List<IMyDoor> doorList) {
                var timeSeconds = runtime.TimeSinceLastRun.TotalSeconds;
                foreach (var door in doorList)
                    ProcessDoor(door, timeSeconds);
            }

            void ProcessDoor(IMyDoor door, double timeSeconds) {
                if (door == null) return;
                if (_openDoors.ContainsKey(door)) {
                    switch (door.Status) {
                        case DoorStatus.Closed: _openDoors.Remove(door); break;
                        case DoorStatus.Closing: _openDoors.Remove(door); break;
                        case DoorStatus.Open:
                            double doorTime;
                            _openDoors.TryGetValue(door, out doorTime);
                            doorTime -= timeSeconds;
                            _openDoors.Remove(door);

                            if (doorTime > 0.0)
                                _openDoors.Add(door, doorTime);
                            else
                                door.CloseDoor();
                            break;
                    }
                } else {
                    if (door.Status == DoorStatus.Open)
                        _openDoors.Add(door, _secondsToLeaveOpen);
                }
            }

        }
    }
}
