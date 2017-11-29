using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript {
    class UpdateDisplayMessage : BasePayloadMessage {
        public const string TYPE = "UpdateDisplayMessage";
        public static UpdateDisplayMessage CreateFromPayload(string message) {
            var obj = new UpdateDisplayMessage();
            obj.LoadFromPayload(message);
            return obj;
        }

        private UpdateDisplayMessage() : base(TYPE) { }
        public UpdateDisplayMessage(string carriageKey, string displayKey, string text) : this() {
            _msgParts = new string[] { carriageKey, displayKey, text };
        }

        public string CarriageKey => _msgParts[0];
        public string DisplayKey => _msgParts[1];
        public string Text => _msgParts[2];
    }
}
