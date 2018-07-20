using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript {
    static class ConfigKeys {
        public const string COMM_GROUP_NAME = "COMM Group Name";
        public const string STEALTH_MODE = "Stealth Mode Enabled";
        public const string STATUS_LIGHTS = "Use Status Lights";
        public const string STATUS_ANTENNA = "Use Status Antenna";
        public const string STATUS_COMMS = "Use Status COMMs";
    }
    static class Commands {
        public const string TURRET_ON = "arm";
        public const string TURRET_OFF = "disarm";

        public const string PARACHUTES_ON = "parachutes-on";
        public const string PARACHUTES_OFF = "parachutes-off";
    }
}
