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
    class GpsInfo {
        public GpsInfo(string rawGPS) {
            _rawGPS = rawGPS;
            VectorHelper.GpsToVector(rawGPS, out _name, out _location);
            if (_name.StartsWith("*")) {
                _needsClearance = false;
                _name = _name.Substring(1);
            }
        }

        private string _name;
        public string GetName() { return _name; }

        private Vector3D _location;
        public Vector3D GetLocation() { return _location; }

        private bool _needsClearance = true;
        public bool GetNeedsClearance() { return _needsClearance; }

        private string _rawGPS;
        public string GetRawGPS() { return _rawGPS; }
    }
}
