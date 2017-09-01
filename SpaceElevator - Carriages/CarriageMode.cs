using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    static class CarriageMode
    {
        public const string Manual_Control = "Manual Control";
        public const string Awaiting_DepartureClearance = "Awaiting Departure Clearance";
        public const string Awaiting_CarriageReady2Depart = "Awaiting Carriage Ready to Depart";
        public const string Transit_Powered = "Transit Powered";
        public const string Transit_Coast = "Coasting";
        public const string Transit_Slow2Approach = "Slow to Approach";
        public const string Transit_Docking = "Docking";
        public const string Docked = "Docked";

        public static bool IsValidModeValue(string value)
        {
            if (value == Manual_Control) return true;
            if (value == Awaiting_DepartureClearance) return true;
            if (value == Awaiting_CarriageReady2Depart) return true;
            if (value == Transit_Powered) return true;
            if (value == Transit_Coast) return true;
            if (value == Transit_Slow2Approach) return true;
            if (value == Transit_Docking) return true;
            if (value == Docked) return true;
            return false;
        }
    }
}
