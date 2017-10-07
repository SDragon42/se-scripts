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
    class ScriptSettingsModule {
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


        public void InitializeConfig(CustomDataConfigModule config) {
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
        public void LoadFromSettingDict(CustomDataConfigModule config) {
            InventoryMultiplier = config.GetValue(KEY_InvMultiplier).ToInt(DEFAULT_WorldInventoryMultiplier);
            TravelSpeed = config.GetValue(KEY_TravelSpeed).ToDouble(DEFAULT_TravelSpeed);
            DockSpeed = config.GetValue(KEY_DockSpeed).ToDouble(DEFAULT_DockSpeed);
            ConnectorLockDelay = config.GetValue(KEY_LockDelay).ToDouble(DEFAULT_ConnectorLockDelay);
            ApproachDistance = config.GetValue(KEY_ApproachDist).ToDouble(DEFAULT_ApproachDistence);
            GravityDescelEnabled = config.GetValue(KEY_GravityDescelEnabled).ToBoolean(DEFAULT_GravityDescelEnabled);
            BlockTag = config.GetValue(KEY_BlockTag, DEFAULT_BlockTag);
            DoorCloseDelay = config.GetValue(KEY_TimeToLeaveDoorOpen).ToDouble(DEFAULT_DoorCloseDelay);
            SendStatusMessages = config.GetValue(KEY_SendStatusMessages).ToBoolean(DEFAULT_SendStatusMessages);

            GpsPoints.Clear();
            var i = 1;
            while (true) {
                var key = KEY_GpsPoint + i.ToString();
                i++;
                if (!config.ContainsKey(key)) break;
                var gps = new GpsInfo(config.GetValue(key));
                if (gps.Location != Vector3D.Zero)
                    GpsPoints.Add(gps);
            }
        }
        public void BuidSettingDict(CustomDataConfigModule config) {
            config.SetValue(KEY_InvMultiplier, InventoryMultiplier.ToString());
            config.SetValue(KEY_TravelSpeed, TravelSpeed.ToString());
            config.SetValue(KEY_DockSpeed, DockSpeed.ToString());
            config.SetValue(KEY_LockDelay, ConnectorLockDelay.ToString());
            config.SetValue(KEY_ApproachDist, ApproachDistance.ToString());
            config.SetValue(KEY_GravityDescelEnabled, GravityDescelEnabled.ToString());
            config.SetValue(KEY_BlockTag, BlockTag);
            config.SetValue(KEY_TimeToLeaveDoorOpen, DoorCloseDelay.ToString());
            config.SetValue(KEY_SendStatusMessages, SendStatusMessages.ToString());

            for (var i = 0; i < GpsPoints.Count; i++)
                config.SetValue(KEY_GpsPoint + i.ToString(), GpsPoints[i].RawGPS);
        }


        public int InventoryMultiplier { get; private set; }
        public double TravelSpeed { get; private set; }
        public double DockSpeed { get; private set; }
        public double ConnectorLockDelay { get; private set; }
        public double ApproachDistance { get; private set; }
        public bool GravityDescelEnabled { get; private set; }
        public string BlockTag { get; private set; }
        public double DoorCloseDelay { get; private set; }
        public bool SendStatusMessages { get; private set; }
        public readonly List<GpsInfo> GpsPoints = new List<GpsInfo>();


        public Vector3D GetBottomPoint() {
            return (GpsPoints.Count > 0)
                ? GpsPoints[0].Location
                : Vector3D.Zero;
        }
        public Vector3D GetTopPoint() {
            return (GpsPoints.Count > 0)
                ? GpsPoints[GpsPoints.Count - 1].Location
                : Vector3D.Zero;
        }
        public GpsInfo GetGpsInfo(string name) {
            foreach (var gps in GpsPoints)
                if (string.Compare(gps.Name, name, true) == 0)
                    return gps;
            return null;
        }

    }
}
