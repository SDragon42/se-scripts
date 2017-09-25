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
    public class CarriageRequestMessage : BasePayloadMessage
    {
        public const string TYPE = "CarriageRequestMessage";
        public const string REQUEST_DOCK = "Dock";
        public const string REQUEST_DEPART = "Depart";
        public static CarriageRequestMessage CreateFromPayload(string message)
        {
            var obj = new CarriageRequestMessage();
            obj.LoadFromPayload(message);
            return obj;
        }

        private CarriageRequestMessage() : base(TYPE) { }
        public CarriageRequestMessage(string carriageName, string request) : base(TYPE)
        {
            _msgParts = new string[] { carriageName, request };
        }

        public string CarriageName { get { return _msgParts[0]; } }
        public string Request { get { return _msgParts[1]; } }
    }
}