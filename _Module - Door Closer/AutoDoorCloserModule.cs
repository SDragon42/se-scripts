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
    /// <summary>Automatically will close doors after a set amount of time
    /// </summary>
    /// <remarks>
    /// Whiplash's : Whip's Auto Door and Airlock Script
    /// http://steamcommunity.com/sharedfiles/filedetails/?id=416932930
    /// </remarks>
    class AutoDoorCloserModule
    {
        readonly Dictionary<IMyDoor, double> _openDoors = new Dictionary<IMyDoor, double>();

        double _numSecondsToLeaveDoorOpen = 2;
        public double GetNumSecondsToLeaveDoorOpen() { return _numSecondsToLeaveDoorOpen; }
        public void SetNumSecondsToLeaveDoorOpen(double value) { _numSecondsToLeaveDoorOpen = value; }

        public void CloseOpenDoors(TimeSpan timeSinceLastCall, IList<IMyTerminalBlock> doors)
        {
            for (int i = 0; i < doors.Count; i++)
            {
                var door = (IMyDoor)doors[i];

                if (_openDoors.ContainsKey(door))
                {
                    switch (door.Status)
                    {
                        case DoorStatus.Closed: _openDoors.Remove(door); break;
                        case DoorStatus.Closing: _openDoors.Remove(door); break;
                        case DoorStatus.Open:
                            double doorTime;
                            _openDoors.TryGetValue(door, out doorTime);
                            doorTime -= timeSinceLastCall.TotalSeconds;
                            _openDoors.Remove(door);

                            if (doorTime > 0.0)
                                _openDoors.Add(door, doorTime);
                            else
                                door.CloseDoor();
                            break;
                    }
                }
                else
                {
                    if (door.Status == DoorStatus.Open)
                        _openDoors.Add(door, _numSecondsToLeaveDoorOpen);
                }

            }
        }

    }
}
