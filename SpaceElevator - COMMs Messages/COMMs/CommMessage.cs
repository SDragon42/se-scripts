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
    public class CommMessage
    {
        private CommMessage() { }
        public CommMessage(IMyProgrammableBlock me, string targetGridName, string payloadType, string payload)
        {
            _senderGridEntityId = me.CubeGrid.EntityId;
            _senderGridName = me.CubeGrid.DisplayName ?? string.Empty;
            _targetGridName = targetGridName ?? string.Empty;
            _payloadType = payloadType ?? string.Empty;
            _payload = payload ?? string.Empty;
        }

        long _senderGridEntityId;
        string _senderGridName;
        string _targetGridName;
        string _payloadType;
        string _payload;

        public long GetSenderGridEntityId() { return _senderGridEntityId; }
        public string GetSenderGridName() { return _senderGridName; }
        public string GetTargetGridName() { return _targetGridName; }
        public string GetPayloadType() { return _payloadType; }
        public string GetPayload() { return _payload; }

        public bool IsValid()
        {
            if (GetSenderGridEntityId() <= 0) return false;
            if (string.IsNullOrWhiteSpace(GetSenderGridName())) return false;
            return true;
        }

        const char DELIMITER = '\n';
        const int NUM_HEADER_PARTS = 6;
        public const string HEADER_START = "MSG";

        public override string ToString()
        {
            return HEADER_START + DELIMITER +
                GetSenderGridEntityId().ToString() + DELIMITER +
                GetSenderGridName() + DELIMITER +
                GetTargetGridName() + DELIMITER +
                GetPayloadType() + DELIMITER +
                GetPayload();
        }
        public static bool TryParse(string messageText, out CommMessage message)
        {
            message = null;
            if (messageText == null) return false;
            var parts = messageText.Split(new char[] { DELIMITER }, NUM_HEADER_PARTS);
            if (parts == null) return false;
            if (parts.Length != NUM_HEADER_PARTS) return false;

            var tmpMsg = new CommMessage();

            if (parts[0] != HEADER_START) return false;
            if (!long.TryParse(parts[1], out tmpMsg._senderGridEntityId)) return false;
            tmpMsg._senderGridName = parts[2] ?? string.Empty;
            tmpMsg._targetGridName = parts[3] ?? string.Empty;
            tmpMsg._payloadType = parts[4] ?? string.Empty;
            tmpMsg._payload = parts[5] ?? string.Empty;

            message = tmpMsg;
            return true;
        }

    }
}