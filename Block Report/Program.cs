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

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        
        public Program() {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var text = new StringBuilder();

            text.Append($"Run at: {DateTime.Now:hh:mm:ss}\n");
            text.Append("--------------------\n");

            var allBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType(allBlocks);//, b => !(b is IMyThrust) && !(b is IMyShipConnector));

            allBlocks
                .Select(b => b.GetType().ToString())
                .Distinct()
                .Select(s => new {
                    typeName = s,
                    count = allBlocks.Where(b => b.GetType().ToString().Equals(s)).Count()
                })
                .Select(t => new {
                    typeName = t.typeName.Substring(t.typeName.LastIndexOf('.') + 1),
                    t.count
                })
                .OrderBy(bt => bt.typeName)
                .ToList()
                .ForEach(bt => text.Append($"{bt.count,2:N0}  {bt.typeName}\n"))
                ;

            Me.CustomData = text.ToString();
            Echo(Me.CustomData);
        }
    }
}
