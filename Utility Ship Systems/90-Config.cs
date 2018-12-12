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

        const string SECTION_WEIGHT = "Weight";
        readonly MyIniKey KEY_MinimumTWR = new MyIniKey(SECTION_WEIGHT, "Minimum TWR");
        readonly MyIniKey KEY_WorldInvMulti = new MyIniKey(SECTION_WEIGHT, "Inventory Multiplier");
        readonly MyIniKey KEY_MaxCargoMass = new MyIniKey(SECTION_WEIGHT, "Max Operational Cargo Mass");

        void LoadConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;
            _configHashCode = tmpHashCode;
            LoadINI(Ini, Me.CustomData);

            Ini.Add(Key_IgnoreTag, "[ignore]");
            Ini.Add(Key_AutoOff, true);
            Ini.Add(Key_AutoOn, true);
            Ini.Add(Key_ToggleThrusters, true);
            Ini.Add(Key_ToggleGyros, true);
            Ini.Add(Key_ToggleLights, true);
            Ini.Add(Key_ToggleBeacons, true);
            Ini.Add(Key_ToggleRadioAntennas, true);
            Ini.Add(Key_ToggleSensors, true);
            Ini.Add(Key_ToggleOreDetectors, true);
            Ini.Add(Key_TurnOffSpotLights, true);
            Ini.SetSectionComment(SECTION_UTILITY_SHIP, null);

            Ini.Add(KEY_MaxCargoMass, 0.0);
            Ini.Add(KEY_MinimumTWR, 1.5, comment: "The minimum TWR to use for calc maximum cargo capacity.");
            Ini.Add(KEY_WorldInvMulti, 0, comment: "The World setting for Inventory Multiplier");
            Ini.SetSectionComment(SECTION_WEIGHT, "This is used to determine that maximum (operation) cargo mass amount.");

            Ini.Add(Key_ProxTag, "[proximity]");
            Ini.Add(Key_ProxRange, 50.0);
            Ini.Add(Key_ProxAlert, false);
            Ini.Add(Key_ProxAlertRange, 10.0);
            Ini.Add(Key_ProxAlertSpeed, 5.0);
            Ini.SetSectionComment(SECTION_PROXIMITY, null);

            Ini.Add(Key_RangeTag, "[range]");
            Ini.Add(Key_RangeDistance, 15000.0);
            Ini.Add(Key_RangeDisplayTime, 5.0);
            Ini.SetSectionComment(SECTION_RANGE, null);



            DockSecureModule.IgnoreTag = Ini.Get(Key_IgnoreTag).ToString();
            DockSecureModule.Auto_Off = Ini.Get(Key_AutoOff).ToBoolean();
            DockSecureModule.Auto_On = Ini.Get(Key_AutoOn).ToBoolean();
            DockSecureModule.Thrusters_OnOff = Ini.Get(Key_ToggleThrusters).ToBoolean();
            DockSecureModule.Gyros_OnOff = Ini.Get(Key_ToggleGyros).ToBoolean();
            DockSecureModule.Lights_OnOff = Ini.Get(Key_ToggleLights).ToBoolean();
            DockSecureModule.Beacons_OnOff = Ini.Get(Key_ToggleBeacons).ToBoolean();
            DockSecureModule.RadioAntennas_OnOff = Ini.Get(Key_ToggleRadioAntennas).ToBoolean();
            DockSecureModule.Sensors_OnOff = Ini.Get(Key_ToggleSensors).ToBoolean();
            DockSecureModule.OreDetectors_OnOff = Ini.Get(Key_ToggleOreDetectors).ToBoolean();
            DockSecureModule.Spotlights_Off = Ini.Get(Key_TurnOffSpotLights).ToBoolean();

            ProximityTag = Ini.Get(Key_ProxTag).ToString();
            ProximityModule.ScanRange = Ini.Get(Key_ProxRange).ToDouble();
            ProximityAlert = Ini.Get(Key_ProxAlert).ToBoolean();
            ProximityAlertRange = Ini.Get(Key_ProxAlertRange).ToDouble();
            ProximityAlertSpeed = Ini.Get(Key_ProxAlertSpeed).ToDouble();

            ForwardScanTag = Ini.Get(Key_RangeTag).ToString();
            ForwardScanRange = Ini.Get(Key_RangeDistance).ToDouble();
            ForwardDisplayClearTime = Ini.Get(Key_RangeDisplayTime).ToDouble();

            MinimumTWR = Ini.Get(KEY_MinimumTWR).ToSingle();
            InventoryMultiplier = Ini.Get(KEY_WorldInvMulti).ToInt32();
            MaxOperationalCargoMass = Ini.Get(KEY_MaxCargoMass).ToDouble();
            if (MaxOperationalCargoMass == 0)
                MaxOperationalCargoMass = null;

            Flag_SaveConfig = true;
        }
        void SaveConfig() {
            if (!Flag_SaveConfig) return;
            Ini.Set(KEY_WorldInvMulti, InventoryMultiplier);
            Ini.Set(KEY_MaxCargoMass, MaxOperationalCargoMass?.ToString() ?? string.Empty);

            var text = Ini.ToString();
            _configHashCode = text.GetHashCode();
            Me.CustomData = text;
        }

        readonly List<IMyThrust> LiftThrusters = new List<IMyThrust>();

        void LoadCameraProximityConfig(IMyCameraBlock b) {
            LoadINI(CameraIni, b.CustomData);
            CameraIni.Add(KEY_RangeOffset, 0.0);
            b.CustomData = CameraIni.ToString();
            ProxCameraList.Add(new ProxCamera(b, Ini.Get(KEY_RangeOffset).ToDouble()));
        }

        static void LoadINI(MyIni ini, string text) {
            ini.Clear();
            if (!ini.TryParse(text)) {
                var tmp = text.Replace('<', '[').Replace('>', ']').Replace(':', '=');
                if (!ini.TryParse(tmp))
                    ini.EndContent = text;
            }
        }

    }
}
