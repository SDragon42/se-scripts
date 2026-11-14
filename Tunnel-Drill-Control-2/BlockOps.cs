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
            // Piston Blocks
            _pistons.Clear();
            IMyPistonBase previousPiston = null;
            var piston = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyPistonBase>(IsMine);
            if (piston != null) {
                _pistons.Add(piston);
                var rootPistonName = piston.CustomName.Replace(_config.TagName, "").Trim();
                var i = 1;
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

            // In-Operation blocks
            GridTerminalSystem.GetBlocksOfType(_warningLights, b => IsMine(b));
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
                    piston.Velocity = _config.ExtensionVelocity;
                    return true;
                }
                i--;
            }
            Debug("All pistons fully extended.");
            return false;
        }
        bool RetractPistonChain(int numberOfPistons = int.MaxValue) {
            var i = 0;
            while (i < _pistons.Count) {
                var piston = _pistons[i];
                if (!IsRetracted(piston)) {
                    if (piston.Velocity != -_config.RetractionVelocity) Debug($"Retracting piston: {piston.CustomName}");
                    piston.Velocity = -_config.RetractionVelocity;
                    return true;
                }
                i++;
                if (i >= numberOfPistons) break;
            }
            Debug("All pistons fully retracted.");
            return false;
        }

        const float PISTON_BASE_LENGTH = 5.16f;

        float GetCurrentPistonExtension() => _pistons.Sum(p => p.CurrentPosition + PISTON_BASE_LENGTH);

        float GetMinPistonExtension() => _pistons.Count * PISTON_BASE_LENGTH;

        float GetMaxPistonExtension() => _pistons.Count * (PISTON_BASE_LENGTH + 10f);

    }
}