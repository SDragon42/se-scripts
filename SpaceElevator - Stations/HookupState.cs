using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript {
    static class HookupState {
        public const int Connected = 1;
        public const int Connecting = 2;
        public const int Disconnected = 3;
        public const int Disconnecting = 4;
    }
}
