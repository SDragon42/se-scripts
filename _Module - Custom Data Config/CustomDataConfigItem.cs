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
    class CustomDataConfigItem
    {
        public CustomDataConfigItem(string description, string val)
        {
            Description = description ?? string.Empty;
            Value = val;
        }

        public string Description { get; private set; }

        string _value;
        public string Value { get { return _value; } set { _value = value ?? string.Empty; } }
    }
}
