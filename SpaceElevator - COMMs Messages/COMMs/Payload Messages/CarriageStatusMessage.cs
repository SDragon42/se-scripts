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
    public class CarriageStatusMessage : BasePayloadMessage
    {
        public const string TYPE = "CarriageStats";
        public static CarriageStatusMessage CreateFromPayload(string message)
        {
            var obj = new CarriageStatusMessage();
            obj.LoadFromPayload(message);
            return obj;
        }

        private CarriageStatusMessage() : base(TYPE) { }
        public CarriageStatusMessage(string mode, Vector3D pos, double vertSpeed, float fuelLevel, double cargoMass, double rangeToGround, double rangeToDestination) : base(TYPE)
        {
            _msgParts = new string[] {
                mode,
                VectorHelper.VectortoGps(pos),
                vertSpeed.ToString(),
                fuelLevel.ToString(),
                cargoMass.ToString(),
                rangeToGround.ToString(),
                rangeToDestination.ToString()
            };
        }

        public string Mode { get { return _msgParts[0]; } }
        public Vector3D Position { get { return VectorHelper.GpsToVector(_msgParts[1]); } }
        public double VerticalSpeed { get { return _msgParts[2].ToDouble(); } }
        public float FuelLevel { get { return _msgParts[3].ToFloat(); } }
        public double CargoMass { get { return _msgParts[4].ToDouble(); } }
        public double RangeToGroundStation { get { return _msgParts[5].ToDouble(); } }
        public double RangeToDestination { get { return _msgParts[6].ToDouble(); } }
    }
}