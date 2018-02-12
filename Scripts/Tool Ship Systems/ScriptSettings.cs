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
    partial class Program {
        class ScriptSettings {
            const string KEY_AUTO_OFF = "Auto Turn OFF Systems";
            const string KEY_AUTO_ON = "Auto Turn ON Systems";
            const string KEY_ToggleThrusters = "Thrusters On/Off";
            const string KEY_ToggleGyros = "Gyros On/Off";
            const string KEY_ToggleLights = "Lights On/Off";
            const string KEY_ToggleBeacons = "Beacons On/Off";
            const string KEY_ToggleRadioAntennas = "Radio Antennas On/Off";
            const string KEY_ToggleSensors = "Sensors On/Off";
            const string KEY_ToggleOreDetectors = "Ore Detectors On/Off";
            const string KEY_TurnOffSpotLights = "Spotlights Off";

            const string KEY_ProximityTag = "Proximity Tag";
            const string KEY_ProximityRange = "Proximity Range (m)";
            const string KEY_ProximityAlert = "Proximity Alert On/Off";
            const string KEY_ProximityAlertRange = "Proximity Alert Range (m)";
            const string KEY_ProximityAlertSpeed = "Proximity Alert Speed (m/s)";

            const string KEY_ForwardTag = "Forward Range Tag";
            const string KEY_ForwardRange = "Forward Range (m)";
            const string KEY_ForwardClearTime = "Display Time (seconds)";

            readonly CustomDataConfig _config = new CustomDataConfig();
            int _configHashCode = 0;

            public void InitConfig(IMyProgrammableBlock me) {
                _config.AddKey(KEY_AUTO_OFF,
                    description: "This will turn on/off systems automatically when the ship undocks/docks \nvia a connector or landing gear.",
                    defaultValue: bool.TrueString);
                _config.AddKey(KEY_AUTO_ON,
                    defaultValue: bool.TrueString);

                _config.AddKey(KEY_ToggleThrusters,
                    description: "This are the block types to toggle On/Off.",
                    defaultValue: bool.TrueString);
                _config.AddKey(KEY_ToggleGyros, defaultValue: bool.TrueString);
                _config.AddKey(KEY_ToggleLights, defaultValue: bool.TrueString);
                _config.AddKey(KEY_ToggleBeacons, defaultValue: bool.TrueString);
                _config.AddKey(KEY_ToggleRadioAntennas, defaultValue: bool.TrueString);
                _config.AddKey(KEY_ToggleSensors, defaultValue: bool.TrueString);
                _config.AddKey(KEY_ToggleOreDetectors, defaultValue: bool.TrueString);
                _config.AddKey(KEY_TurnOffSpotLights,
                    description: "This are the block types to only turn off.",
                    defaultValue: bool.TrueString);

                _config.AddKey(KEY_ProximityTag,
                    description: "Proximity range settings",
                    defaultValue: "[proximity]");
                _config.AddKey(KEY_ProximityRange,
                    defaultValue: "50");
                _config.AddKey(KEY_ProximityAlert,
                    defaultValue: bool.FalseString);
                _config.AddKey(KEY_ProximityAlertRange,
                    defaultValue: "10");
                _config.AddKey(KEY_ProximityAlertSpeed,
                    defaultValue: "1.5");

                _config.AddKey(KEY_ForwardTag,
                    description: "Forward range settings.",
                    defaultValue: "[range]");
                _config.AddKey(KEY_ForwardRange,
                    defaultValue: "15000");
                _config.AddKey(KEY_ForwardClearTime,
                    defaultValue: "5");
            }
            public void LoadConfig(IMyProgrammableBlock me, DockSecure dsm) {
                if (_configHashCode == me.CustomData.GetHashCode())
                    return;
                InitConfig(me);
                _config.ReadFromCustomData(me);
                _config.SaveToCustomData(me);
                _configHashCode = me.CustomData.GetHashCode();

                dsm.Auto_On = _config.GetValue(KEY_AUTO_ON).ToBoolean();
                dsm.Auto_Off = _config.GetValue(KEY_AUTO_OFF).ToBoolean();
                dsm.Thrusters_OnOff = _config.GetValue(KEY_ToggleThrusters).ToBoolean();
                dsm.Gyros_OnOff = _config.GetValue(KEY_ToggleGyros).ToBoolean();
                dsm.Lights_OnOff = _config.GetValue(KEY_ToggleLights).ToBoolean();
                dsm.Beacons_OnOff = _config.GetValue(KEY_ToggleBeacons).ToBoolean();
                dsm.RadioAntennas_OnOff = _config.GetValue(KEY_ToggleRadioAntennas).ToBoolean();
                dsm.Sensors_OnOff = _config.GetValue(KEY_ToggleSensors).ToBoolean();
                dsm.OreDetectors_OnOff = _config.GetValue(KEY_ToggleOreDetectors).ToBoolean();
                dsm.Spotlights_Off = _config.GetValue(KEY_TurnOffSpotLights).ToBoolean();

                ProximityTag = _config.GetValue(KEY_ProximityTag);
                ProximityScanRange = _config.GetValue(KEY_ProximityRange).ToDouble();
                ProximityAlert = _config.GetValue(KEY_ProximityAlert).ToBoolean();
                ProximityAlertRange = _config.GetValue(KEY_ProximityAlertRange).ToDouble();
                ProximityAlertSpeed = _config.GetValue(KEY_ProximityAlertSpeed).ToDouble();

                ForwardScanTag = _config.GetValue(KEY_ForwardTag);
                ForwardScanRange = _config.GetValue(KEY_ForwardRange).ToDouble();
                ForwardDisplayClearTime = _config.GetValue(KEY_ForwardClearTime).ToDouble();
            }

            public string ProximityTag { get; private set; }
            public double ProximityScanRange { get; private set; }
            public bool ProximityAlert { get; private set; }
            public double ProximityAlertRange { get; private set; }
            public double ProximityAlertSpeed { get; private set; }

            public string ForwardScanTag { get; private set; }
            public double ForwardScanRange { get; private set; }
            public double ForwardDisplayClearTime { get; private set; }
        }
    }
}
