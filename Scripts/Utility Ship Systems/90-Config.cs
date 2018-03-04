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

        const string KEY_ProximityTag = "Tag";
        const string KEY_ProximityRange = "Range (m)";
        const string KEY_ProximityAlert = "Alert On/Off";
        const string KEY_ProximityAlertRange = "Alert Range (m)";
        const string KEY_ProximityAlertSpeed = "Alert Speed (m/s)";

        const string KEY_RANGE_OFFSET = "Range Offset";

        const string KEY_ForwardTag = "Tag";
        const string KEY_ForwardRange = "Scan Range (m)";
        const string KEY_ForwardClearTime = "Display Time (seconds)";

        void InitConfigs() {
            _scriptCfg.AddKey(KEY_AUTO_OFF, true);
            _scriptCfg.AddKey(KEY_AUTO_ON, true);

            _scriptCfg.AddKey(KEY_ToggleThrusters, true);
            _scriptCfg.AddKey(KEY_ToggleGyros, true);
            _scriptCfg.AddKey(KEY_ToggleLights, true);
            _scriptCfg.AddKey(KEY_ToggleBeacons, true);
            _scriptCfg.AddKey(KEY_ToggleRadioAntennas, true);
            _scriptCfg.AddKey(KEY_ToggleSensors, true);
            _scriptCfg.AddKey(KEY_ToggleOreDetectors, true);

            _scriptCfg.AddKey(KEY_TurnOffSpotLights, true);

            _scriptProxCfg.AddKey(KEY_ProximityTag, "[proximity]");
            _scriptProxCfg.AddKey(KEY_ProximityRange, 50.0);
            _scriptProxCfg.AddKey(KEY_ProximityAlert, true);
            _scriptProxCfg.AddKey(KEY_ProximityAlertRange, 10.0);
            _scriptProxCfg.AddKey(KEY_ProximityAlertSpeed, 5.0);

            _scriptRangeCfg.AddKey(KEY_ForwardTag, "[range]");
            _scriptRangeCfg.AddKey(KEY_ForwardRange, 15000.0);
            _scriptRangeCfg.AddKey(KEY_ForwardClearTime, 5.0);

            _cameraProxCfg.AddKey(KEY_RANGE_OFFSET, 0.0);
        }

        int _configHashCode = 0;
        void LoadConfigs() {
            if (_configHashCode == Me.CustomData.GetHashCode()) return;
            _scriptCfg.Load(Me);
            _scriptProxCfg.Load(Me);
            _scriptRangeCfg.Load(Me);

            _dockSecure.Auto_On = _scriptCfg.GetValue(KEY_AUTO_ON).ToBoolean();
            _dockSecure.Auto_Off = _scriptCfg.GetValue(KEY_AUTO_OFF).ToBoolean();
            _dockSecure.Thrusters_OnOff = _scriptCfg.GetValue(KEY_ToggleThrusters).ToBoolean();
            _dockSecure.Gyros_OnOff = _scriptCfg.GetValue(KEY_ToggleGyros).ToBoolean();
            _dockSecure.Lights_OnOff = _scriptCfg.GetValue(KEY_ToggleLights).ToBoolean();
            _dockSecure.Beacons_OnOff = _scriptCfg.GetValue(KEY_ToggleBeacons).ToBoolean();
            _dockSecure.RadioAntennas_OnOff = _scriptCfg.GetValue(KEY_ToggleRadioAntennas).ToBoolean();
            _dockSecure.Sensors_OnOff = _scriptCfg.GetValue(KEY_ToggleSensors).ToBoolean();
            _dockSecure.OreDetectors_OnOff = _scriptCfg.GetValue(KEY_ToggleOreDetectors).ToBoolean();
            _dockSecure.Spotlights_Off = _scriptCfg.GetValue(KEY_TurnOffSpotLights).ToBoolean();

            ProximityTag = _scriptProxCfg.GetValue(KEY_ProximityTag);
            _proximity.ScanRange = _scriptProxCfg.GetValue(KEY_ProximityRange).ToDouble();
            ProximityAlert = _scriptProxCfg.GetValue(KEY_ProximityAlert).ToBoolean();
            ProximityAlertRange = _scriptProxCfg.GetValue(KEY_ProximityAlertRange).ToDouble();
            ProximityAlertSpeed = _scriptProxCfg.GetValue(KEY_ProximityAlertSpeed).ToDouble();

            ForwardScanTag = _scriptRangeCfg.GetValue(KEY_ForwardTag);
            ForwardScanRange = _scriptRangeCfg.GetValue(KEY_ForwardRange).ToDouble();
            ForwardDisplayClearTime = _scriptRangeCfg.GetValue(KEY_ForwardClearTime).ToDouble();

            _scriptCfg.Save(Me);
            _scriptProxCfg.Save(Me);
            _scriptRangeCfg.Save(Me);
        }
    }
}
