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

        private CarriageStatusMessage() : base(TYPE) {
            _msgParts = new string[7];
        }
        public CarriageStatusMessage(CarriageMode mode, Vector3D pos, double vertSpeed, float fuelLevel, double cargoMass, double range2Bottom, double range2Top) : this() {
            Mode = mode;
            Position = pos;
            VerticalSpeed = vertSpeed;
            FuelLevel = fuelLevel;
            CargoMass = cargoMass;
            Range2Bottom = Range2Bottom;
            Range2Top = range2Top;
        }

        public CarriageMode Mode {
            get { return _msgParts[0].ToEnum(defValue: CarriageMode.Manual_Control); }
            set { _msgParts[0] = value.ToString(); }
        }
        public Vector3D Position {
            get { return VectorHelper.GpsToVector(_msgParts[1]); }
            set { _msgParts[1] = VectorHelper.VectortoGps(value); }
        }
        public double VerticalSpeed {
            get { return _msgParts[2].ToDouble(); }
            set { _msgParts[2] = value.ToString(); }
        }
        public float FuelLevel {
            get { return _msgParts[3].ToFloat(); }
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
    }
}
