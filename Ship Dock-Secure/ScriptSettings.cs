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
            const string KEY_TurnOffSorters = "Sorters Off";

            readonly ConfigCustom _config = new ConfigCustom();
            int _configHashCode = 0;

            public void InitConfig(IMyProgrammableBlock me) {
                _config.Clear();
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
                _config.AddKey(KEY_TurnOffSorters, defaultValue: bool.TrueString);
            }
            public void LoadConfig(IMyProgrammableBlock me, DockSecure dsm, Action postLoadAction = null) {
                if (_configHashCode == me.CustomData.GetHashCode())
                    return;
                InitConfig(me);
                _config.Load(me);
                _config.Save(me);
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
                dsm.Sorters_Off = _config.GetValue(KEY_TurnOffSorters).ToBoolean();

                postLoadAction?.Invoke();
            }

        }
    }
}
