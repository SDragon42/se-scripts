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
    partial class Program {
        abstract class ConfigBase<K> {
            protected readonly static string[] SepBlankLine = new string[] { "\n\n" };
            protected readonly static char[] SepNewLine = new char[] { '\n' };
            protected readonly static char[] SepEquals = new char[] { '=' };

            protected readonly Dictionary<string, K> Items = new Dictionary<string, K>();

            public void Clear() => Items.Clear();

            public bool ContainsKey(string key) => Items.ContainsKey(key);

            public abstract void Load(IMyTerminalBlock block, bool addIfMissing = false);
            public abstract void Save(IMyTerminalBlock block);
        }
    }
}
