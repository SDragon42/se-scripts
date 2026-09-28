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
        // Added the INI key/value to the MyIni object if it doesn't already exist, and returns the MyIniValue instance.
        public static MyIniValue Add<T>(this MyIni ini, string section, string name, T value, string comment = null) where T : struct => ini.Add(new MyIniKey(section, name), value.ToString(), comment);
        // Added the INI key/value to the MyIni object if it doesn't already exist, and returns the MyIniValue instance.
        public static MyIniValue Add(this MyIni ini, string section, string name, string value, string comment = null) => ini.Add(new MyIniKey(section, name), value, comment);
        // Added the INI key/value to the MyIni object if it doesn't already exist, and returns the MyIniValue instance.
        public static MyIniValue Add<T>(this MyIni ini, MyIniKey key, T value, string comment = null) where T : struct => ini.Add(key, value.ToString(), comment);
        // Added the INI key/value to the MyIni object if it doesn't already exist, and returns the MyIniValue instance.
        public static MyIniValue Add(this MyIni ini, MyIniKey key, string value, string comment = null) {
            if (!ini.ContainsKey(key)) ini.Set(key, value);
            ini.SetComment(key, comment);
            return ini.Get(key);
        }

        // Migrate the value from the old key to the new key, and delete the old key.
        public static void Migrate(this MyIni ini, string oldSection, string newSection, string name) => ini.Migrate(new MyIniKey(oldSection, name), new MyIniKey(newSection, name));
        // Migrate the value from the old key to the new key, and delete the old key.
        public static void Migrate(this MyIni ini, string oldSection, string oldName, string newSection, string newName) => ini.Migrate(new MyIniKey(oldSection, oldName), new MyIniKey(newSection, newName));
        // Migrate the value from the old key to the new key, and delete the old key.
        public static void Migrate(this MyIni ini, string oldSection, string oldName, MyIniKey newkey) => ini.Migrate(new MyIniKey(oldSection, oldName), newkey);
        // Migrate the value from the old key to the new key, and delete the old key.
        public static void Migrate(this MyIni ini, MyIniKey oldKey, string newSection, string newName) => ini.Migrate(oldKey, new MyIniKey(newSection, newName));
        // Migrate the value from the old key to the new key, and delete the old key.
        public static void Migrate(this MyIni ini, MyIniKey oldKey, MyIniKey newkey) {
            if (!ini.ContainsKey(oldKey)) return;
            var oldValue = ini.Get(oldKey);
            if (!ini.ContainsKey(newkey)) ini.Set(newkey, oldValue.ToString());
            ini.Delete(oldKey);
        }
    }
}
