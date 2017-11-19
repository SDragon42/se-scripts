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
    class ScriptSettings {
        public int DockSecureInterval { get; private set; }
        public int ProximityInterval { get; private set; }


        //const string KEY_DockSecureInterval = "Dock Secure Interval";
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

        //const string KEY_ProximityInterval = "Proximity Interval";
        const string KEY_ProximityRange = "Proximity Range";
        const string KEY_ProximityTag = "Proximity Camera Tag";

        readonly CustomDataConfigModule _config = new CustomDataConfigModule();
        int _configHashCode = 0;

        public void InitConfig(IMyProgrammableBlock me, DockSecureModule dsm, ProximityModule pm, Action postLoadAction = null) {
            //_config.AddKey(KEY_DockSecureInterval,
            //    description: "The number of times/second to check if docked.",
            //    defaultValue: "4");
            _config.AddKey(KEY_AUTO_OFF,
                description: "This will turn off systems automactically when the ship docks via a\nconnector or landing gear.",
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

            //_config.AddKey(KEY_ProximityInterval,
            //    description: "The number of times/second to get the proximity ranges.",
            //    defaultValue: "2");
            _config.AddKey(KEY_ProximityRange,
                description: "The range in meters to scan.",
                defaultValue: "99");
            _config.AddKey(KEY_ProximityTag,
                defaultValue: "");

            LoadConfig(me, dsm, pm, postLoadAction);
        }
        public void LoadConfig(IMyProgrammableBlock me, DockSecureModule dsm, ProximityModule pm, Action postLoadAction = null) {
            if (_configHashCode == me.CustomData.GetHashCode())
                return;
            _config.ReadFromCustomData(me, true);
            _config.SaveToCustomData(me);
            _configHashCode = me.CustomData.GetHashCode();

            //DockSecureInterval = _config.GetValue(KEY_DockSecureInterval).ToInt();
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

            //ProximityInterval = _config.GetValue(KEY_ProximityInterval).ToInt();
            pm.ProximityTag = _config.GetValue(KEY_ProximityTag);
            pm.ScanRange = _config.GetValue(KEY_ProximityRange).ToDouble();

            postLoadAction?.Invoke();
        }

    }
}
