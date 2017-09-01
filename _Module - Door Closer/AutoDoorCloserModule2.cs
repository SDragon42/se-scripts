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
    public class AutoDoorCloserModule2
    {
        readonly Func<IMyDoor, bool> _doorCollectMethod;
        readonly Dictionary<IMyDoor, double> _openDoors = new Dictionary<IMyDoor, double>();
        readonly List<IMyDoor> _tmp = new List<IMyDoor>();

        double _numSecondsToLeaveDoorOpen = 2;
        public double GetNumSecondsToLeaveDoorOpen() { return _numSecondsToLeaveDoorOpen; }
        public void SetNumSecondsToLeaveDoorOpen(double value) { _numSecondsToLeaveDoorOpen = value; }

        public AutoDoorCloserModule2(Func<IMyDoor, bool> doorCollect = null, double secondsToLeaveOpen = 2)
        {
            _doorCollectMethod = doorCollect;
            SetNumSecondsToLeaveDoorOpen(secondsToLeaveOpen);
        }

        public void CloseOpenDoors(TimeSpan timeSinceLastCall, IMyGridTerminalSystem _gts)
        {
            _gts.GetBlocksOfType(_tmp, _doorCollectMethod);

            foreach (var door in _tmp)
                ProcessDoor(door, timeSinceLastCall);
        }
        void ProcessDoor(IMyDoor door, TimeSpan timeSinceLastCall)
        {
            switch (door.Status)
            {
                case DoorStatus.Closing: _openDoors.Remove(door); break;
                case DoorStatus.Closed: _openDoors.Remove(door); break;
                case DoorStatus.Open:
                    if (!_openDoors.ContainsKey(door))
                    {
                        _openDoors.Add(door, _numSecondsToLeaveDoorOpen);
                        break;
                    }

                    var doorTime = _openDoors[door] - timeSinceLastCall.TotalSeconds;
                    if (doorTime > 0.0)
                    {
                        _openDoors[door] = doorTime;
                        break;
                    }

                    _openDoors.Remove(door);
                    door.CloseDoor();
                    break;
            }

        }
    }
}
