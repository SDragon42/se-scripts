using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {

        static class ConfigSections {
            public const string UTILITY_SHIP = "Utility-Ship";
            public const string UTILITY_SHIP_SYSTEMS = "Utility-Ship-Systems";
            public const string PROXIMITY = "Proximity";
            public const string RANGE = "Range";
        }

        //================================================================================
        class ScriptConfiguration : ConfigBase {
            //public ScriptConfiguration(IMyTerminalBlock block, IMyGridTerminalSystem GridTerminalSystem) : base(block, GridTerminalSystem) { }
            public string ProximityTag { get; private set; } = "[proximity]";
            public bool ProximityAlert { get; private set; } = false;
            public double ProximityAlertRange { get; private set; } = 10;
            public double ProximityAlertSpeed { get; private set; } = 5;

            public string ForwardScanTag { get; private set; } = "[range]";
            public double ForwardScanRange { get; private set; } = 15000;
            public double ForwardScanRangeDisplayTime { get; private set; } = 5;

            public void Load(DockSecure dockSecure, Proximity proximity) {
                if (!LoadIni()) return;

                // Utility Ship settings
                dockSecure.Tag = _ini.Add(ConfigSections.UTILITY_SHIP, "Tag", "[utility]").ToString();
                dockSecure.IgnoreTag = _ini.Add(ConfigSections.UTILITY_SHIP, "Ignore-Tag", "[ignore]").ToString();

                // Migrate old settings to new section
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Auto Turn OFF Systems");
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Auto Turn ON Systems");
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Thrusters On/Off");
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Gyros On/Off");
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Lights On/Off");
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Beacons On/Off");
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Radio Antennas On/Off");
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Sensors On/Off");
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Ore Detectors On/Off");
                _ini.Migrate(ConfigSections.UTILITY_SHIP, ConfigSections.UTILITY_SHIP_SYSTEMS, "Spotlights On/Off");

                // Read/Create the new settings
                dockSecure.Auto_Off = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Auto Turn OFF Systems", dockSecure.Auto_Off).ToBoolean();
                dockSecure.Auto_On = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Auto Turn ON Systems", dockSecure.Auto_On).ToBoolean();
                dockSecure.Thrusters_OnOff = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Thrusters On/Off", dockSecure.Thrusters_OnOff).ToBoolean();
                dockSecure.Gyros_OnOff = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Gyros On/Off", dockSecure.Gyros_OnOff).ToBoolean();
                dockSecure.Lights_OnOff = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Lights On/Off", dockSecure.Lights_OnOff).ToBoolean();
                dockSecure.Beacons_OnOff = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Beacons On/Off", dockSecure.Beacons_OnOff).ToBoolean();
                dockSecure.RadioAntennas_OnOff = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Radio Antennas On/Off", dockSecure.RadioAntennas_OnOff).ToBoolean();
                dockSecure.Sensors_OnOff = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Sensors On/Off", dockSecure.Sensors_OnOff).ToBoolean();
                dockSecure.OreDetectors_OnOff = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Ore Detectors On/Off", dockSecure.OreDetectors_OnOff).ToBoolean();
                dockSecure.Spotlights_OnOff = _ini.Add(ConfigSections.UTILITY_SHIP_SYSTEMS, "Spotlights On/Off", dockSecure.Spotlights_OnOff).ToBoolean();

                // Proximity settings
                ProximityTag = _ini.Add(ConfigSections.PROXIMITY, "Tag", ProximityTag).ToString();
                proximity.ScanRange = _ini.Add(ConfigSections.PROXIMITY, "Range (m)", proximity.ScanRange).ToDouble();
                ProximityAlert = _ini.Add(ConfigSections.PROXIMITY, "Alert On/Off", ProximityAlert).ToBoolean();
                ProximityAlertRange = _ini.Add(ConfigSections.PROXIMITY, "Alert Range (m)", ProximityAlertRange).ToDouble();
                ProximityAlertSpeed = _ini.Add(ConfigSections.PROXIMITY, "Alert Speed (m/s)", ProximityAlertSpeed).ToDouble();

                // Range Scanning settings
                ForwardScanTag = _ini.Add(ConfigSections.RANGE, "Tag", ForwardScanTag).ToString();
                ForwardScanRange = _ini.Add(ConfigSections.RANGE, "Scan Range (m)", ForwardScanRange).ToDouble(); //TODO: Should this be a float?
                ForwardScanRangeDisplayTime = _ini.Add(ConfigSections.RANGE, "Display Time (seconds)", ForwardScanRangeDisplayTime).ToDouble(); //TODO: Should this be a float?

                Save();
            }
        }

        //================================================================================
        class CameraConfig : ConfigBase {
            //public CameraConfig(IMyTerminalBlock block, IMyGridTerminalSystem GridTerminalSystem) : base(block, GridTerminalSystem) { }
            public double RangeOffset { get; private set; } = 0;

            public void Load() {
                if (!LoadIni()) return;

                RangeOffset = _ini.Add(ConfigSections.PROXIMITY, "Range Offset", RangeOffset).ToDouble();
                Save();
            }
        }

        //================================================================================

        class DisplayConfig : ConfigBase {
            //public DisplayConfig(IMyTerminalBlock block, IMyGridTerminalSystem GridTerminalSystem) : base(block, GridTerminalSystem) { }
            public int ProximityScreenNumber { get; private set; } = 0;
            public int RangeScreenNumber { get; private set; } = 0;

            public void Load() {
                if (!LoadIni()) return;

                ProximityScreenNumber = _ini.Add(ConfigSections.PROXIMITY, "Screen Number", ProximityScreenNumber, null).ToInt32();
                RangeScreenNumber = _ini.Add(ConfigSections.RANGE, "Screen Number", RangeScreenNumber, null).ToInt32();
                Save();
            }
        }

    }
}
