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
    partial class Program {

        public void Main(string argument, UpdateType updateSource) {
            Echo(SCRIPT_TITLE);
            string command;
            ProcessArgument(argument, out command);
            _config.Load();
            LoadBlocks();
            if (_guidanceBlocks.Count == 0) {
                Echo("No torpedo guidance blocks found");
                Echo($"Tag: {_config.torpedoPrimaryTag}");
                command = string.Empty;
            }
            RechargeAllPowerCells();
            
            if (_commands.ContainsKey(command)) _commands[command]?.Invoke();

            Echo(_instructions);
        }


        void ProcessArgument(string argument, out string command) {
            torpedoSecondaryTag = string.Empty;
            var cmdParts = argument.ToLower().Split(new char[] { ' ' }, 2);
            command = cmdParts[0];
            if (cmdParts.Length >= 2) torpedoSecondaryTag = cmdParts[1];
        }

        void LoadBlocks() {
            _referenceBlock = (IMyTerminalBlock)GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => Me.IsSameConstructAs(b) && IsTagged(b, _config.referenceTag),
                b => Me.IsSameConstructAs(b) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => Me.IsSameConstructAs(b) && b is IMyCockpit && ((IMyCockpit)b).IsUnderControl,
                b => Me.IsSameConstructAs(b) && b is IMyCockpit,
                b => Me.IsSameConstructAs(b) && b is IMyRemoteControl)
                ?? Me;
            Debug($"FRef: {_referenceBlock.CustomName}");

            GridTerminalSystem.GetBlocksOfType(_guidanceBlocks, IsTorpedoGuidance);
            Debug($"# Found Torps: {_guidanceBlocks.Count}");

            GridTerminalSystem.GetBlocksOfType(_beaconBlocks, b => Me.IsSameConstructAs(b) && IsTagged(b, _config.torpedoBeaconTag));
            Debug($"# Found Torp Beacons: {_beaconBlocks.Count}");

            _powerCellBlocks.Clear();
            if (_config.torpedoPowerCellTag.Length > 0) {
                GridTerminalSystem.GetBlocksOfType(_powerCellBlocks, b => Me.IsSameConstructAs(b) && IsTagged(b, _config.torpedoPowerCellTag));
                Debug($"# Found Torp P.Cells: {_powerCellBlocks.Count}");
            }
        }
        bool IsTorpedoGuidance(IMyTerminalBlock b) => Me.IsSameConstructAs(b) 
                                                    && IsTagged(b, _config.torpedoPrimaryTag)
                                                    && (torpedoSecondaryTag.Length == 0 || IsTagged(b, torpedoSecondaryTag));


        static T SelectBlock<T>(List<T> blockList, IMyTerminalBlock referenceBlock, double initialDist, Func<double, double, bool> compareFunc) where T : IMyTerminalBlock {
            T selected = default(T);
            var lastDist = initialDist;
            var refPosition = referenceBlock.GetPosition();

            foreach (var b in blockList) {
                var currDist = (b.GetPosition() - refPosition).Length();
                if (compareFunc(currDist, lastDist)) {
                    lastDist = currDist;
                    selected = b;
                }
            }

            return selected;
        }

        static bool GreaterThan(double a, double b) => a > b;
        static bool LessThan(double a, double b) => a < b;

    }
}
