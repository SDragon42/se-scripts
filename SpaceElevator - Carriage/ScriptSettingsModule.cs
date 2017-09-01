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
    class ScriptSettingsModule
    {
        const string DEFAULT_BlockTag = "[carriage]";
        const int DEFAULT_WorldInventoryMultiplier = 1;
        const double DEFAULT_TravelSpeed = 99.0;
        const double DEFAULT_DockSpeed = 2.0;
        const double DEFAULT_ConnectorLockDelay = 2.0;
        const double DEFAULT_ApproachDistence = 25.0;
        const bool DEFAULT_GravityDescelEnabled = false;
        const double DEFAULT_DoorCloseDelay = 4.0;
        const bool DEFAULT_SendStatusMessages = true;

        const string KEY_BlockTag = "Block Tag";
        const string KEY_InvMultiplier = "Inventory Multiplier";
        const string KEY_TravelSpeed = "Travel Speed";
        const string KEY_DockSpeed = "Dock Speed";
        const string KEY_LockDelay = "Lock Delay";
        const string KEY_ApproachDist = "Approach Distance";
        const string KEY_GravityDescelEnabled = "Gravity Decel Enabled";
        const string KEY_TimeToLeaveDoorOpen = "Time to Leave Doors Open";
        const string KEY_SendStatusMessages = "Transmit Status";
        const string KEY_GpsPoint = "GPS Point ";

        public void InitializeConfig(CustomDataConfigModule config)
        {
            config.Clear();
            config.AddKey(KEY_BlockTag,
                description: "This is the name tag to add to the blocks so that the script can\ncontrol them.",
                defaultValue: DEFAULT_BlockTag);
            config.AddKey(KEY_InvMultiplier,
                description: "This is the inventory multiplier from World Settings.",
                defaultValue: DEFAULT_WorldInventoryMultiplier.ToString());
            config.AddKey(KEY_TravelSpeed,
                description: "The is the maxium speed that should be traveled.\nDock Speed is the velocity to approach when docking.",
                defaultValue: DEFAULT_TravelSpeed.ToString());
            config.AddKey(KEY_DockSpeed,
                defaultValue: DEFAULT_DockSpeed.ToString());
            config.AddKey(KEY_LockDelay,
                description: "The delay (in seconds) to wait to lock a connector once it is able to lock.",
                defaultValue: DEFAULT_ConnectorLockDelay.ToString());
            config.AddKey(KEY_ApproachDist,
                description: "This is the distance as which will be approaching at dock speed.",
                defaultValue: DEFAULT_ApproachDistence.ToString());
            config.AddKey(KEY_GravityDescelEnabled,
                description: "This determines of the carrage will use gavity to slow it's ascent.",
                defaultValue: DEFAULT_GravityDescelEnabled.ToString());
            config.AddKey(KEY_TimeToLeaveDoorOpen,
                description: "This the the amount of time (in seconds) to leave\ninternal doors open before closing them.",
                defaultValue: DEFAULT_DoorCloseDelay.ToString());
            config.AddKey(KEY_SendStatusMessages,
                description: "Toggles the sending of the carriage status messages.",
                defaultValue: DEFAULT_SendStatusMessages.ToString());

            config.AddKey(KEY_GpsPoint + "1",
                description: "These are the GPS points for the docking (as taken from the RC block).\n" +
                             "The \"name\" of the GPS point must be the name of the grid docking to\n" +
                             "at that point. Also, \"" + KEY_GpsPoint + "1\" MUST be the Lowest point.\n" +
                             "Additional GPS points can be made by just adding to the key list here.");
            config.AddKey(KEY_GpsPoint + "2");
            config.AddKey(KEY_GpsPoint + "3");
        }
        public void LoadFromSettingDict(CustomDataConfigModule config)
        {
            _inventoryMultiplier = config.GetInt(KEY_InvMultiplier, DEFAULT_WorldInventoryMultiplier);
            _travelSpeed = config.GetDouble(KEY_TravelSpeed, DEFAULT_TravelSpeed);
            _DockSpeed = config.GetDouble(KEY_DockSpeed, DEFAULT_DockSpeed);
            _ConnectorLockDelay = config.GetDouble(KEY_LockDelay, DEFAULT_ConnectorLockDelay);
            _ApproachDistance = config.GetDouble(KEY_ApproachDist, DEFAULT_ApproachDistence);
            _GravityDescelEnabled = config.GetBoolean(KEY_GravityDescelEnabled, DEFAULT_GravityDescelEnabled);
            _BlockTag = config.GetString(KEY_BlockTag, DEFAULT_BlockTag);
            _DoorCloseDelay = config.GetDouble(KEY_TimeToLeaveDoorOpen, DEFAULT_DoorCloseDelay);
            _SendStatusMessages = config.GetBoolean(KEY_SendStatusMessages, DEFAULT_SendStatusMessages);

            _gpsPoints.Clear();
            var i = 1;
            while (true)
            {
                var key = KEY_GpsPoint + i.ToString();
                i++;
                if (!config.ContainsKey(key)) break;
                _gpsPoints.Add(new GpsInfo(config.GetString(key)));
            }
        }
        public void BuidSettingDict(CustomDataConfigModule config)
        {
            config.SetValue(KEY_InvMultiplier, _inventoryMultiplier.ToString());
            config.SetValue(KEY_TravelSpeed, _travelSpeed.ToString());
            config.SetValue(KEY_DockSpeed, _DockSpeed.ToString());
            config.SetValue(KEY_ApproachDist, _ApproachDistance.ToString());
            config.SetValue(KEY_GravityDescelEnabled, _GravityDescelEnabled.ToString());
            config.SetValue(KEY_BlockTag, _BlockTag);
            config.SetValue(KEY_TimeToLeaveDoorOpen, _DoorCloseDelay.ToString());
            config.SetValue(KEY_SendStatusMessages, _SendStatusMessages.ToString());

            for (var i = 0; i < _gpsPoints.Count; i++)
                config.SetValue(KEY_GpsPoint + i.ToString(), _gpsPoints[i].GetRawGPS());
        }


        int _inventoryMultiplier;
        public int GetInventoryMultiplier() { return _inventoryMultiplier; }

        double _travelSpeed;
        public double GetTravelSpeed() { return _travelSpeed; }

        double _DockSpeed;
        public double GetDockSpeed() { return _DockSpeed; }

        double _ConnectorLockDelay;
        public double GetConnectorLockDelay() { return _ConnectorLockDelay; }

        double _ApproachDistance;
        public double GetApproachDistance() { return _ApproachDistance; }

        bool _GravityDescelEnabled;
        public bool GetGravityDescelEnabled() { return _GravityDescelEnabled; }


        string _BlockTag;
        public string GetBlockTag() { return _BlockTag; }


        double _DoorCloseDelay;
        public double GetDoorCloseDelay() { return _DoorCloseDelay; }


        bool _SendStatusMessages;
        public bool GetSendStatusMessages() { return _SendStatusMessages; }

        readonly List<GpsInfo> _gpsPoints = new List<GpsInfo>();
        public IList<GpsInfo> GetGpsPointList() { return _gpsPoints; }

        public Vector3D GetBottomPoint()
        {
            return (_gpsPoints.Count > 0)
                ? _gpsPoints[0].GetLocation()
                : Vector3D.Zero;
        }
        public Vector3D GetGpsPoint(string name)
        {
            foreach (var point in _gpsPoints)
                if (string.Compare(point.GetName(), name, true) == 0)
                    return point.GetLocation();
            return Vector3D.Zero;
        }
        public GpsInfo GetGpsInfo(string name)
        {
            foreach (var point in _gpsPoints)
                if (string.Compare(point.GetName(), name, true) == 0)
                    return point;
            return null;
        }

    }
}
