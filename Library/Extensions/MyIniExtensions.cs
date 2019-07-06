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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    static class MyIniExtensions {
        public static MyIniValue Add<T>(this MyIni ini, string section, string name, T value, string comment = null) where T : struct => ini.Add(new MyIniKey(section, name), value.ToString(), comment);
        public static MyIniValue Add(this MyIni ini, string section, string name, string value, string comment = null) => ini.Add(new MyIniKey(section, name), value, comment);

        public static MyIniValue Add<T>(this MyIni ini, MyIniKey key, T value, string comment = null) where T : struct => ini.Add(key, value.ToString(), comment);
        public static MyIniValue Add(this MyIni ini, MyIniKey key, string value, string comment = null) {
            if (!ini.ContainsKey(key)) {
                ini.Set(key, value);
                ini.SetComment(key, comment);
            }
            return ini.Get(key);
        }
    }
}
