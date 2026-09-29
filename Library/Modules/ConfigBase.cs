// <mdk sortorder="1000" />
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
    partial class Program {

        abstract class ConfigBase {
            protected IMyTerminalBlock Block { get; private set; }
            protected IMyGridTerminalSystem GridTerminalSystem { get; private set; }
            protected readonly MyIni _ini = new MyIni();
            private int _lastConfigHash = 0;

            // Initialize the configuration with the block and grid terminal system.
            public void Initialize(IMyTerminalBlock block, IMyGridTerminalSystem gridTerminalSystem) {
                Block = block;
                GridTerminalSystem = gridTerminalSystem;
            }

            // Load the configuration from the block's CustomData.
            // Returns true if the configuration was loaded, false if it was already up to date.
            protected bool LoadIni(bool forceReload = false) {
                var tmpHashCode = Block.CustomData.GetHashCode();
                if (_lastConfigHash == tmpHashCode && !forceReload) return false;
                _lastConfigHash = tmpHashCode;
                _ini.Clear();
                return _ini.TryParse(Block.CustomData);
            }

            // Save the configuration to the block's CustomData.
            public virtual void Save() {
                Block.CustomData = _ini.ToString();
                _lastConfigHash = Block.CustomData.GetHashCode();
            }

        }

    }
}
