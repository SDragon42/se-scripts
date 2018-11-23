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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public class DockSecure2 {
            readonly List<IMyFunctionalBlock> ToggleBlocks = new List<IMyFunctionalBlock>();
            readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();
            readonly List<IMyShipConnector> Connectors = new List<IMyShipConnector>();

            MyGridProgram _gridProgram;
            bool _wasDockedLastRun = false;
            bool _isDocked = false;

            public void Init(MyGridProgram gridProgram, bool findBlocks = true) {
                _gridProgram = gridProgram;
                if (!findBlocks) return;
                _gridProgram.GridTerminalSystem.GetBlocksOfType(LandingGears, IsNotIgnored);
                _gridProgram.GridTerminalSystem.GetBlocksOfType(Connectors, IsNotIgnored);
                
            }
            bool IsNotIgnored(IMyTerminalBlock b) => Program.OnSameGrid(_gridProgram.Me, b) && !Collect.IsTagged(b, "IgnoreTag");



        }
    }
}
