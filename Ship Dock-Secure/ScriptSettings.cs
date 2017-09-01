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
    class ScriptSettings
    {
        public bool Auto_On { get; private set; }
        public bool Auto_Off { get; private set; }

        public bool Thrusters_OnOff { get; private set; }
        public bool Gyros_OnOff { get; private set; }
        public bool Lights_OnOff { get; private set; }
        public bool Beacons_OnOff { get; private set; }
        public bool RadioAntennas_OnOff { get; private set; }
        public bool Sensors_OnOff { get; private set; }
        public bool OreDetectors_OnOff { get; private set; }
        public bool Spotlights_Off { get; private set; }

        public int RunInterval { get; private set; }


        const string KEY_AUTO_OFF = "Auto Turn OFF Systems";
        const string KEY_AUTO_ON = "Auto Turn ON Systems";
        const string KEY_RunInterval = "Runs Per Second";
        const string KEY_ToggleThrusters = "Thrusters On/Off";
        const string KEY_ToggleGyros = "Gyros On/Off";
        const string KEY_ToggleLights = "Lights On/Off";
        const string KEY_ToggleBeacons = "Beacons On/Off";
        const string KEY_ToggleRadioAntennas = "Radio Antennas On/Off";
        const string KEY_ToggleSensors = "Sensors On/Off";
        const string KEY_ToggleOreDetectors = "Ore Detectors On/Off";
        const string KEY_TurnOffSpotLights = "Spotlights Off";

        readonly CustomDataConfigModule _config = new CustomDataConfigModule();
        int _configHashCode = 0;

        public void InitConfig(IMyProgrammableBlock me, Action postLoadAction = null)
        {
            _config.AddKey(KEY_AUTO_OFF,
                description: "This will turn off systems automactically when the ship docks via a\nconnector or landing gear.",
                defaultValue: bool.TrueString);
            _config.AddKey(KEY_AUTO_ON,
                //description: "This will turn on systems automactically when the ship undocks via a connector or landing gear.",
                defaultValue: bool.TrueString);

            _config.AddKey(KEY_RunInterval,
                description: "This is the number of times per second the script will run.",
                defaultValue: "4");

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

            LoadConfig(me, postLoadAction);
        }
        public void LoadConfig(IMyProgrammableBlock me, Action postLoadAction = null)
        {
            if (_configHashCode == me.CustomData.GetHashCode())
                return;
            _config.ReadFromCustomData(me, true);
            _config.SaveToCustomData(me);
            _configHashCode = me.CustomData.GetHashCode();

            Auto_On = _config.GetBoolean(KEY_AUTO_ON);
            Auto_Off = _config.GetBoolean(KEY_AUTO_OFF);
            Thrusters_OnOff = _config.GetBoolean(KEY_ToggleThrusters);
            Gyros_OnOff = _config.GetBoolean(KEY_ToggleGyros);
            Lights_OnOff = _config.GetBoolean(KEY_ToggleLights);
            Beacons_OnOff = _config.GetBoolean(KEY_ToggleBeacons);
            RadioAntennas_OnOff = _config.GetBoolean(KEY_ToggleRadioAntennas);
            Sensors_OnOff = _config.GetBoolean(KEY_ToggleSensors);
            OreDetectors_OnOff = _config.GetBoolean(KEY_ToggleOreDetectors);
            Spotlights_Off = _config.GetBoolean(KEY_TurnOffSpotLights);
            RunInterval = _config.GetInt(KEY_RunInterval);

            postLoadAction?.Invoke();
        }

    }
}
