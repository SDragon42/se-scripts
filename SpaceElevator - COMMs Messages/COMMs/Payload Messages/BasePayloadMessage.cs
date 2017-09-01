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
    public abstract class BasePayloadMessage
    {
        protected const char DELIMITER = '\n';

        protected string[] _msgParts;


        protected BasePayloadMessage(string messageType)
        {
            _type = messageType;
        }

        public void LoadFromPayload(string message)
        {
            _msgParts = message.Split(DELIMITER);
        }


        readonly string _type;
        public string GetMessageType() { return _type; }


        protected int GetInt(int i)
        {
            if (i < 0 || i >= _msgParts.Length) return 0;
            int val;
            if (!int.TryParse(_msgParts[i], out val)) return 0;
            return val;
        }
        protected float GetFloat(int i)
        {
            if (i < 0 || i >= _msgParts.Length) return 0F;
            float val;
            if (!float.TryParse(_msgParts[i], out val)) return 0F;
            return val;
        }
        protected double GetDouble(int i)
        {
            if (i < 0 || i >= _msgParts.Length) return 0.0;
            double val;
            if (!double.TryParse(_msgParts[i], out val)) return 0.0;
            return val;
        }
        protected bool GetBoolean(int i)
        {
            if (i < 0 || i >= _msgParts.Length) return false;
            bool val;
            if (!bool.TryParse(_msgParts[i], out val)) return false;
            return val;
        }
        protected Vector3D GetVector3D(int i)
        {
            var gpsParts = _msgParts[i].Split(':');
            if (gpsParts == null || gpsParts.Length < 5) return Vector3D.Zero;
            return new Vector3D(
                double.Parse(gpsParts[2]),
                double.Parse(gpsParts[3]),
                double.Parse(gpsParts[4]));
        }
        protected string Vector3DtoGPS(Vector3D v)
        {
            return string.Format("GPS:Position:{0}:{1}:{2}:",
                v.X, v.Y, v.Z);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < _msgParts.Length; i++)
                sb.Append(DELIMITER + _msgParts[i]);
            sb.Remove(0, 1);
            return sb.ToString();
        }
    }
}