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

        public void Main(string argument, UpdateType updateSource) {
            Echo(ScriptTitle);
            string command;
            ProcessArgument(argument, out command);
            ProcessConfig();
            LoadBlocks();
            if (guidanceBlocks.Count == 0) {
                Echo("No torpedo guidance blocks found");
                Echo($"Tag: {torpedoPrimaryTag}");
                command = string.Empty;
            }
            RechargeAllPowerCells();
            RunCommand(command);
            Echo(Instructions);
        }


        void ProcessArgument(string argument, out string command) {
            torpedoSecondaryTag = string.Empty;
            var cmdParts = argument.ToLower().Split(new char[] { ' ' }, 2);
            command = cmdParts[0];
            if (cmdParts.Length >= 2) torpedoSecondaryTag = cmdParts[1];
        }

        void LoadBlocks() {
            referenceBlock = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => b.IsSameConstructAs(Me) && b.CustomName.ToLower().Contains(referenceTag),
                b => b.IsSameConstructAs(Me) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => b.IsSameConstructAs(Me) && b is IMyCockpit && ((IMyCockpit)b).IsUnderControl,
                b => b.IsSameConstructAs(Me) && b is IMyCockpit,
                b => b.IsSameConstructAs(Me) && b is IMyRemoteControl);
            if (referenceBlock == null) referenceBlock = Me;
            Debug($"FRef: {referenceBlock.CustomName}");

            GridTerminalSystem.GetBlocksOfType(guidanceBlocks, IsTorpedoGuidance);
            Debug($"# Found Torps: {guidanceBlocks.Count}");

            GridTerminalSystem.GetBlocksOfType(beaconBlocks, b => b.IsSameConstructAs(Me) && Collect.IsTagged(b, torpedoBeaconTag));
            Debug($"# Found Torp Beacons: {beaconBlocks.Count}");

            powerCellBlocks.Clear();
            if (torpedoPowerCellTag.Length > 0) {
                GridTerminalSystem.GetBlocksOfType(powerCellBlocks, b => b.IsSameConstructAs(Me) && Collect.IsTagged(b, torpedoPowerCellTag));
                Debug($"# Found Torp P.Cells: {powerCellBlocks.Count}");
            }
        }
        bool IsTorpedoGuidance(IMyTerminalBlock b) {
            if (!b.IsSameConstructAs(Me)) return false;
            if (!Collect.IsTagged(b, torpedoPrimaryTag)) return false;
            if (torpedoSecondaryTag.Length > 0)
                return Collect.IsTagged(b, torpedoSecondaryTag);
            return true;
        }


        void RunCommand(string command) {
            if (Commands.ContainsKey(command)) Commands[command]?.Invoke();
        }


        static T SelectBlock<T>(List<T> blockList, IMyTerminalBlock referenceBlock, double initalDist, Func<double, double, bool> compareFunc) where T : IMyTerminalBlock {
            T selected = default(T);
            var lastDist = initalDist;
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
