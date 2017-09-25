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
            SenderGridEntityId = me.CubeGrid.EntityId;
            SenderGridName = me.CubeGrid.DisplayName ?? string.Empty;
            TargetGridName = targetGridName ?? string.Empty;
            PayloadType = payloadType ?? string.Empty;
            Payload = payload ?? string.Empty;
        }

        public long SenderGridEntityId { get; private set; }
        public string SenderGridName { get; private set; }
        public string TargetGridName { get; private set; }
        public string PayloadType { get; private set; }
        public string Payload { get; private set; }

        public bool IsValid()
        {
            if (SenderGridEntityId <= 0) return false;
            if (string.IsNullOrWhiteSpace(SenderGridName)) return false;
            return true;
        }

        const char DELIMITER = '\n';
        const int NUM_HEADER_PARTS = 6;
        public const string HEADER_START = "MSG";

        public override string ToString()
        {
            return HEADER_START + DELIMITER +
                SenderGridEntityId.ToString() + DELIMITER +
                SenderGridName + DELIMITER +
                TargetGridName + DELIMITER +
                PayloadType + DELIMITER +
                Payload;
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
            long senderId;
            if (!long.TryParse(parts[1], out senderId)) return false;
            tmpMsg.SenderGridEntityId = senderId;
            tmpMsg.SenderGridName = parts[2] ?? string.Empty;
            tmpMsg.TargetGridName = parts[3] ?? string.Empty;
            tmpMsg.PayloadType = parts[4] ?? string.Empty;
            tmpMsg.Payload = parts[5] ?? string.Empty;

            message = tmpMsg;
            return true;
        }

    }
}