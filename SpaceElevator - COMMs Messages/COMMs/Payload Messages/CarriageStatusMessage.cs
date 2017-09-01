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
            _msgParts = new string[]
            {
            mode,
            Vector3DtoGPS(pos),
            vertSpeed.ToString(),
            fuelLevel.ToString(),
            cargoMass.ToString(),
            rangeToGround.ToString(),
            rangeToDestination.ToString()
            };
        }

        public string GetMode() { return _msgParts[0]; }
        public Vector3D GetPosition() { return GetVector3D(1); }
        public double GetVerticalSpeed() { return GetDouble(2); }
        public float GetFuelLevel() { return GetFloat(3); }
        public double GetCargoMass() { return GetDouble(4); }
        public double GetRangeToGroundStation() { return GetDouble(5); }
        public double GetRangeToDestination() { return GetDouble(6); }
    }
}