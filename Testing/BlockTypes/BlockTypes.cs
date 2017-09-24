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
    class BlockTypes : TestingBase, ITestingBase
    {
        public BlockTypes(MyGridProgram thisObj) : base(thisObj) { }

        readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

        public void Main(string argument)
        {
            var output = GridTerminalSystem.GetBlockWithName("DEBUG") as IMyTextPanel;
            GridTerminalSystem.GetBlocks(blocks);

            var q = blocks
                .Where(b => b.CustomName != "DEBUG")
                .Select(b => new { block = b, key = b.BlockDefinition.TypeIdString + ":" + b.BlockDefinition.SubtypeId })
                .OrderBy(i => i.key);

            var sb = new StringBuilder();
            foreach (var item in q)
                sb.AppendLine(item.key);

            Echo(sb.ToString());
            output.WritePublicText(sb.ToString());
            output.ShowPublicTextOnScreen();
        }
    }
}
