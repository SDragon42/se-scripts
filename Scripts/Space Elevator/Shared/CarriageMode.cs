using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript {
    enum CarriageMode {
        Init,
        Manual_Control,
        Awaiting_DepartureClearance,
        Awaiting_CarriageReady2Depart,
        Transit_Powered,
        Transit_Coast,
        Transit_Slow2Approach,
        Transit_Docking,
        Docked
    }
}
