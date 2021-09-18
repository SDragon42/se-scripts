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
        // Automatically will close doors after a set amount of time
        // Whiplash's : Whip's Auto Door and Airlock Script
        // http://steamcommunity.com/sharedfiles/filedetails/?id=416932930
        class AutoDoorCloser {
            readonly Dictionary<IMyDoor, double> OpenDoors = new Dictionary<IMyDoor, double>();

            public IMyProgrammableBlock Me { get; set; }

            double delay = 4.0;
            public double CloseDelay {
                get { return delay; }
                set { delay = Math.Max(value, 0.0); }
            }

            public void CloseOpenDoors(IMyGridProgramRuntimeInfo runtime, List<IMyDoor> doorList) {
                var timeSeconds = runtime.TimeSinceLastRun.TotalSeconds;
                foreach (var door in doorList) {
                    if (!door.Closed && Me != null && door.IsSameConstructAs(Me)) {
                        var isOpen = door.Status == DoorStatus.Open;
                        if (!OpenDoors.ContainsKey(door)) {
                            if (isOpen) OpenDoors.Add(door, delay);
                            continue;
                        }

                        if (isOpen) {
                            OpenDoors[door] -= timeSeconds;
                            if (OpenDoors[door] > 0.0) continue;
                            door.CloseDoor();
                        }
                    }

                    if (OpenDoors.ContainsKey(door)) OpenDoors.Remove(door);
                }
            }

        }

    }
}
