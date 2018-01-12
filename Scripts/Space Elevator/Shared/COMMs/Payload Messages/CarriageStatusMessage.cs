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

namespace IngameScript {
    class CarriageStatusMessage : BasePayloadMessage {
        public const string TYPE = "CarriageStats";
        public static CarriageStatusMessage CreateFromPayload(string message) {
            var obj = new CarriageStatusMessage();
            obj.LoadFromPayload(message);
            return obj;
        }

        public CarriageStatusMessage() : base(TYPE) {
            _msgParts = new string[9];
            Position = Vector3D.Zero;
            VerticalSpeed = 0;
            FuelLevel = 0;
            CargoMass = 0;
            Range2Bottom = 0;
            Range2Top = 0;
            Destination = "";
            InTransit = false;
        }

        public Vector3D Position {
            get { return VectorHelper.GpsToVector(_msgParts[1]); }
            set { _msgParts[1] = VectorHelper.VectortoGps(value); }
        }
        public double VerticalSpeed {
            get { return _msgParts[2].ToDouble(); }
            set { _msgParts[2] = value.ToString(); }
        }
        public double FuelLevel {
            get { return _msgParts[3].ToDouble(); }
            set { _msgParts[3] = value.ToString(); }
        }
        public double CargoMass {
            get { return _msgParts[4].ToDouble(); }
            set { _msgParts[4] = value.ToString(); }
        }
        public double Range2Bottom {
            get { return _msgParts[5].ToDouble(); }
            set { _msgParts[5] = value.ToString(); }
        }
        public double Range2Top {
            get { return _msgParts[6].ToDouble(); }
            set { _msgParts[6] = value.ToString(); }
        }
        public string Destination {
            get { return _msgParts[0]; }
            set { _msgParts[0] = value; }
        }
        public bool InTransit {
            get { return _msgParts[7].ToBoolean(); }
            set { _msgParts[7] = value.ToString(); }
        }

        internal void SetTransit(string name, CarriageMode carriageMode) {
            switch (carriageMode) {
                case CarriageMode.Docked:
                    Destination = "Docked";
                    InTransit = false;
                    break;
                case CarriageMode.Manual_Control:
                    Destination = "Manual Control";
                    InTransit = true;
                    break;
                default:
                    Destination = name ?? "Unknown";
                    InTransit = true;
                    break;
            }
        }


    }
}
