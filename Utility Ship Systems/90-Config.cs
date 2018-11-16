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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        int _configHashCode = 0;


        const string SECTION_UTILITY_SHIP = "Utility-Ship";
        readonly MyIniKey Key_IgnoreTag = new MyIniKey(SECTION_UTILITY_SHIP, "Ignore-Tag");
        readonly MyIniKey Key_AutoOff = new MyIniKey(SECTION_UTILITY_SHIP, "Auto Turn OFF Systems");
        readonly MyIniKey Key_AutoOn = new MyIniKey(SECTION_UTILITY_SHIP, "Auto Turn ON Systems");
        readonly MyIniKey Key_ToggleThrusters = new MyIniKey(SECTION_UTILITY_SHIP, "Thrusters On/Off");
        readonly MyIniKey Key_ToggleGyros = new MyIniKey(SECTION_UTILITY_SHIP, "Gyros On/Off");
        readonly MyIniKey Key_ToggleLights = new MyIniKey(SECTION_UTILITY_SHIP, "Lights On/Off");
        readonly MyIniKey Key_ToggleBeacons = new MyIniKey(SECTION_UTILITY_SHIP, "Beacons On/Off");
        readonly MyIniKey Key_ToggleRadioAntennas = new MyIniKey(SECTION_UTILITY_SHIP, "Radio Antennas On/Off");
        readonly MyIniKey Key_ToggleSensors = new MyIniKey(SECTION_UTILITY_SHIP, "Sensors On/Off");
        readonly MyIniKey Key_ToggleOreDetectors = new MyIniKey(SECTION_UTILITY_SHIP, "Ore Detectors On/Off");
        readonly MyIniKey Key_TurnOffSpotLights = new MyIniKey(SECTION_UTILITY_SHIP, "Spotlights Off");

        const string SECTION_PROXIMITY = "Proximity";
        readonly MyIniKey Key_ProxTag = new MyIniKey(SECTION_PROXIMITY, "Tag");
        readonly MyIniKey Key_ProxRange = new MyIniKey(SECTION_PROXIMITY, "Range (m)");
        readonly MyIniKey Key_ProxAlert = new MyIniKey(SECTION_PROXIMITY, "Alert On/Off");
        readonly MyIniKey Key_ProxAlertRange = new MyIniKey(SECTION_PROXIMITY, "Alert Range (m)");
        readonly MyIniKey Key_ProxAlertSpeed = new MyIniKey(SECTION_PROXIMITY, "Alert Speed (m/s)");

        readonly MyIniKey KEY_RangeOffset = new MyIniKey(SECTION_PROXIMITY, "Range Offset");

        const string SECTION_RANGE = "Range";
        readonly MyIniKey Key_RangeTag = new MyIniKey(SECTION_RANGE, "Tag");
        readonly MyIniKey Key_RangeDistance = new MyIniKey(SECTION_RANGE, "Scan Range (m)");
        readonly MyIniKey Key_RangeDisplayTime = new MyIniKey(SECTION_RANGE, "Display Time (seconds)");

        void LoadConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;
            _configHashCode = tmpHashCode;
            LoadINI(Me.CustomData);

            _ini.Add(Key_IgnoreTag, "[ignore]");
            _ini.Add(Key_AutoOff, true);
            _ini.Add(Key_AutoOn, true);
            _ini.Add(Key_ToggleThrusters, true);
            _ini.Add(Key_ToggleGyros, true);
            _ini.Add(Key_ToggleLights, true);
            _ini.Add(Key_ToggleBeacons, true);
            _ini.Add(Key_ToggleRadioAntennas, true);
            _ini.Add(Key_ToggleSensors, true);
            _ini.Add(Key_ToggleOreDetectors, true);
            _ini.Add(Key_TurnOffSpotLights, true);
            _ini.SetSectionComment(SECTION_UTILITY_SHIP, null);

            _ini.Add(Key_ProxTag, "[proximity]");
            _ini.Add(Key_ProxRange, 50.0);
            _ini.Add(Key_ProxAlert, true);
            _ini.Add(Key_ProxAlertRange, 10.0);
            _ini.Add(Key_ProxAlertSpeed, 5.0);
            _ini.SetSectionComment(SECTION_PROXIMITY, null);

            _ini.Add(Key_RangeTag, "[range]");
            _ini.Add(Key_RangeDistance, 15000.0);
            _ini.Add(Key_RangeDisplayTime, 5.0);
            _ini.SetSectionComment(SECTION_RANGE, null);

            Me.CustomData = _ini.ToString();

            _dockSecure.IgnoreTag = _ini.Get(Key_IgnoreTag).ToString();
            _dockSecure.Auto_Off = _ini.Get(Key_AutoOff).ToBoolean();
            _dockSecure.Auto_On = _ini.Get(Key_AutoOn).ToBoolean();
            _dockSecure.Thrusters_OnOff = _ini.Get(Key_ToggleThrusters).ToBoolean();
            _dockSecure.Gyros_OnOff = _ini.Get(Key_ToggleGyros).ToBoolean();
            _dockSecure.Lights_OnOff = _ini.Get(Key_ToggleLights).ToBoolean();
            _dockSecure.Beacons_OnOff = _ini.Get(Key_ToggleBeacons).ToBoolean();
            _dockSecure.RadioAntennas_OnOff = _ini.Get(Key_ToggleRadioAntennas).ToBoolean();
            _dockSecure.Sensors_OnOff = _ini.Get(Key_ToggleSensors).ToBoolean();
            _dockSecure.OreDetectors_OnOff = _ini.Get(Key_ToggleOreDetectors).ToBoolean();
            _dockSecure.Spotlights_Off = _ini.Get(Key_TurnOffSpotLights).ToBoolean();

            ProximityTag = _ini.Get(Key_ProxTag).ToString();
            _proximity.ScanRange = _ini.Get(Key_ProxRange).ToDouble();
            ProximityAlert = _ini.Get(Key_ProxAlert).ToBoolean();
            ProximityAlertRange = _ini.Get(Key_ProxAlertRange).ToDouble();
            ProximityAlertSpeed = _ini.Get(Key_ProxAlertSpeed).ToDouble();

            ForwardScanTag = _ini.Get(Key_RangeTag).ToString();
            ForwardScanRange = _ini.Get(Key_RangeDistance).ToDouble();
            ForwardDisplayClearTime = _ini.Get(Key_RangeDisplayTime).ToDouble();
        }

        void LoadCameraProximityConfig(IMyTerminalBlock b) {
            LoadINI(b.CustomData);
            _ini.Add(KEY_RangeOffset, 0.0);
            b.CustomData = _ini.ToString();
            _proxCameraList.Add(new ProxCamera((IMyCameraBlock)b, _ini.Get(KEY_RangeOffset).ToDouble()));
        }

        void LoadINI(string text) {
            MyIniParseResult result;
            _ini.Clear();
            if (!_ini.TryParse(text, out result)) {
                var tmp = text.Replace('<', '[').Replace('>', ']').Replace(':', '=');
                if (!_ini.TryParse(tmp, out result))
                    _ini.EndContent = text;
            }
        }

    }
}
