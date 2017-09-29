using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript {
    enum CarriageMode {
        Manual_Control,
        Awaiting_DepartureClearance,
        Awaiting_CarriageReady2Depart,
        Transit_Powered,
        Transit_Coast,
        Transit_Slow2Approach,
        Transit_Docking,
        Docked
    }
    static class CarriageModeHelper {
        public static bool IsValidModeValue(CarriageMode value) {
            return Enum.IsDefined(typeof(CarriageMode), value);
        }
        public static CarriageMode GetFromString(string value) {
            var outputVal = CarriageMode.Manual_Control;
            return (Enum.TryParse(value, out outputVal)) ? outputVal : CarriageMode.Manual_Control;
        }
    }
}
