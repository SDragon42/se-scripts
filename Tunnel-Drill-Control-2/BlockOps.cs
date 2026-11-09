// <mdk sortorder="10" />
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
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
    public partial class Program : MyGridProgram {

        void LoadBlocks() {
            // In Operations On/Off blocks
            GridTerminalSystem.GetBlocksOfType(_OnOffBlocks, b => IsMine(b) && !(b is IMyPistonBase));

            // Piston Blocks
            _pistons.Clear();
            IMyPistonBase previousPiston = null;
            var piston = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyPistonBase>(IsMine);
            if (piston != null) {
                _pistons.Add(piston);
                var rootPistonName = piston.CustomName.Replace(_config.TagName, "").Trim();
                var i = 0;
                // Traverse the piston chain and rename them sequentially
                while (piston != null) {
                    previousPiston = piston;
                    piston = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyPistonBase>(b => piston.TopGrid == b.CubeGrid);
                    if (piston != null) {
                        i++;
                        piston.CustomName = $"{rootPistonName} {i:00}";
                        _pistons.Add(piston);
                    }
                }
            }

            // Drill Blocks
            _drills.Clear();
            if (previousPiston != null)
                GridTerminalSystem.GetBlocksOfType(_drills, b => b.CubeGrid == previousPiston.TopGrid);

            // Cargo Container Blocks
            GridTerminalSystem.GetBlocksOfType(_cargoContainers, IsMine);
        }


        bool IsMine(IMyTerminalBlock b) => b.IsSameConstructAs(Me) && IsTagged(b, _config.TagName);
        bool IsRetracted(IMyPistonBase b) => b.CurrentPosition <= b.MinLimit + 0.01;
        bool IsExtended(IMyPistonBase b) => b.CurrentPosition >= b.MaxLimit - 0.01;

        void StopAllPistons() => _pistons.ForEach(b => b.Velocity = 0);
        void StopAllDrills() => _drills.ForEach(b => b.Enabled = false);
        void StartAllDrills() => _drills.ForEach(b => b.Enabled = true);

        bool ExtendPistonChain() {
            var i = _pistons.Count - 1;
            while (i >= 0) {
                var piston = _pistons[i];
                if (!IsExtended(piston)) {
                    if (piston.Velocity != _config.ExtensionVelocity) Debug($"Extending piston: {piston.CustomName}");
                    // Debug($"{piston.CustomName}: {piston.CurrentPosition} / {piston.MaxLimit}");
                    piston.Velocity = _config.ExtensionVelocity;
                    return true;
                }
                i--;
            }
            Debug("All pistons fully extended.");
            return false;
        }

    }
}