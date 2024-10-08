﻿// <mdk sortorder="1000" />
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    static class ToPrimativeExtensions {
        /// <summary>
        /// Tries to convert the string value to a boolean.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static bool ToBoolean(this string text, bool defValue = false) {
            if (text == null) return defValue;
            bool val;
            if (!bool.TryParse(text, out val)) return defValue;
            return val;
        }
    }
}
