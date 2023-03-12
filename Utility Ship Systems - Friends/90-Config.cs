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

        int _configHashCode = 0;


        const string SECTION_UTILITY_SHIP = "Utility-Ship";
        readonly MyIniKey Key_Tag = new MyIniKey(SECTION_UTILITY_SHIP, "Tag");
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
        readonly MyIniKey Key_ToggleSpotLights = new MyIniKey(SECTION_UTILITY_SHIP, "Spotlights On/Off");

        const string SECTION_PROXIMITY = "Proximity";
        readonly MyIniKey Key_ProxTag = new MyIniKey(SECTION_PROXIMITY, "Tag");
        readonly MyIniKey Key_ProxRange = new MyIniKey(SECTION_PROXIMITY, "Range (m)");
        readonly MyIniKey Key_ProxAlert = new MyIniKey(SECTION_PROXIMITY, "Alert On/Off");
        readonly MyIniKey Key_ProxAlertRange = new MyIniKey(SECTION_PROXIMITY, "Alert Range (m)");
        readonly MyIniKey Key_ProxAlertSpeed = new MyIniKey(SECTION_PROXIMITY, "Alert Speed (m/s)");
        readonly MyIniKey KEY_ProxRangeOffset = new MyIniKey(SECTION_PROXIMITY, "Range Offset");
        readonly MyIniKey KEY_ProxScreenNumber = new MyIniKey(SECTION_PROXIMITY, "Screen Number");

        const string SECTION_RANGE = "Range";
        readonly MyIniKey Key_RangeTag = new MyIniKey(SECTION_RANGE, "Tag");
        readonly MyIniKey Key_RangeDistance = new MyIniKey(SECTION_RANGE, "Scan Range (m)");
        readonly MyIniKey Key_RangeDisplayTime = new MyIniKey(SECTION_RANGE, "Display Time (seconds)");
        readonly MyIniKey KEY_RangeScreenNumber = new MyIniKey(SECTION_RANGE, "Screen Number");

        const string SECTION_WEIGHT = "Weight";
        readonly MyIniKey KEY_MinimumTWR = new MyIniKey(SECTION_WEIGHT, "Minimum TWR");
        readonly MyIniKey KEY_WorldInvMulti = new MyIniKey(SECTION_WEIGHT, "Inventory Multiplier");
        readonly MyIniKey KEY_MaxCargoMass = new MyIniKey(SECTION_WEIGHT, "Max Operational Cargo Mass");

        void LoadConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;
            _configHashCode = tmpHashCode;
            LoadINI(Ini, Me.CustomData);

            // Dock Secure module settings
            DockSecureModule.Tag = Ini.Add(Key_Tag, "[utility]").ToString();
            DockSecureModule.IgnoreTag = Ini.Add(Key_IgnoreTag, "[ignore]").ToString();
            DockSecureModule.Auto_Off = Ini.Add(Key_AutoOff, true).ToBoolean();
            DockSecureModule.Auto_On = Ini.Add(Key_AutoOn, true).ToBoolean();
            DockSecureModule.Thrusters_OnOff = Ini.Add(Key_ToggleThrusters, true).ToBoolean();
            DockSecureModule.Gyros_OnOff = Ini.Add(Key_ToggleGyros, true).ToBoolean();
            DockSecureModule.Lights_OnOff = Ini.Add(Key_ToggleLights, true).ToBoolean();
            DockSecureModule.Beacons_OnOff = Ini.Add(Key_ToggleBeacons, true).ToBoolean();
            DockSecureModule.RadioAntennas_OnOff = Ini.Add(Key_ToggleRadioAntennas, true).ToBoolean();
            DockSecureModule.Sensors_OnOff = Ini.Add(Key_ToggleSensors, true).ToBoolean();
            DockSecureModule.OreDetectors_OnOff = Ini.Add(Key_ToggleOreDetectors, true).ToBoolean();
            DockSecureModule.Spotlights_OnOff = Ini.Add(Key_ToggleSpotLights, true).ToBoolean();
            Ini.SetSectionComment(SECTION_UTILITY_SHIP, null);

            // Mass settings
            MaxOperationalCargoMass = Ini.Add(KEY_MaxCargoMass, 0.0).ToDouble();
            MinimumTWR = Ini.Add(KEY_MinimumTWR, 1.5f, comment: "The minimum TWR to use for calc maximum cargo capacity.").ToSingle();
            InventoryMultiplier = Ini.Add(KEY_WorldInvMulti, 0, comment: "The World setting for Inventory Multiplier").ToInt32();
            Ini.SetSectionComment(SECTION_WEIGHT, "This is used to determine that maximum (operation) cargo mass amount.");
            if (MaxOperationalCargoMass == 0)
                MaxOperationalCargoMass = null;

            // Proximity settings
            ProximityTag = Ini.Add(Key_ProxTag, "[proximity]").ToString();
            ProximityModule.ScanRange = Ini.Add(Key_ProxRange, 50.0).ToDouble();
            ProximityAlert = Ini.Add(Key_ProxAlert, false).ToBoolean();
            ProximityAlertRange = Ini.Add(Key_ProxAlertRange, 10.0).ToDouble();
            ProximityAlertSpeed = Ini.Add(Key_ProxAlertSpeed, 5.0).ToDouble();
            Ini.SetSectionComment(SECTION_PROXIMITY, null);

            // Range Scanning settings
            ForwardScanTag = Ini.Add(Key_RangeTag, "[range]").ToString();
            ForwardScanRange = Ini.Add(Key_RangeDistance, 15000.0).ToDouble();
            ForwardDisplayClearTime = Ini.Add(Key_RangeDisplayTime, 5.0).ToDouble();
            Ini.SetSectionComment(SECTION_RANGE, null);

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
            CameraIni.Add(KEY_ProxRangeOffset, 0.0);
            b.CustomData = CameraIni.ToString();
            ProxCameraList.Add(new ProxCamera(b, CameraIni.Get(KEY_ProxRangeOffset).ToDouble()));
        }

        void LoadTextScreenProviderConfig(IMyTerminalBlock b, MyIni ini) {
            LoadINI(ini, b.CustomData);
            ini.Add(KEY_ProxScreenNumber, -1);
            ini.Add(KEY_RangeScreenNumber, -1);
            b.CustomData = ini.ToString();
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
