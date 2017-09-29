using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript {
    class CarriageVars {
        public CarriageVars(string gridName) {
            _gridName = gridName ?? string.Empty;
        }


        private string _gridName = string.Empty;
        public string GetGridName() { return _gridName; }

        private bool _connect = false;
        public bool GetConnect() { return _connect; }
        public void SetConnect(bool value) { _connect = value; }

        private bool _sendResponseMsg = false;
        public bool GetSendResponseMsg() { return _sendResponseMsg; }
        public void SetSendResponseMsg(bool value) { _sendResponseMsg = value; }

        private int _gateState = HookupState.Disconnecting;
        public int GetGateState() { return _gateState; }
        public void SetGateState(int value) { _gateState = value; }


        public override string ToString() {
            return _connect.ToString() + ":" +
                _sendResponseMsg.ToString() + ":" +
                _gateState.ToString();
        }

        public void FromString(string stateData) {
            if (string.IsNullOrWhiteSpace(stateData)) return;
            var parts = stateData.Split(':');

            bool boolVal;
            if (bool.TryParse(parts[0], out boolVal))
                _connect = boolVal;

            if (bool.TryParse(parts[1], out boolVal))
                _sendResponseMsg = boolVal;

            int intVal;
            if (int.TryParse(parts[2], out intVal))
                _gateState = intVal;
        }
    }
}
