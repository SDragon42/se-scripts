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

        void Command_LockOnAll() {
            guidanceBlocks.ForEach(LockOn);
        }
        void LockOn(IMyRadioAntenna guidanceBlock) {
            guidanceBlock.Enabled = true;
            guidanceBlock.ApplyAction("Adn.ActionLockOnTarget");
        }
        void Command_TurnOffAll() {
            guidanceBlocks.ForEach(b => b.Enabled = false);
            TurnOffAllBeacons();
            RechargeAllPowerCells();
        }
        void Command_Launch() {
            var guidance = TorpedoSelection[selectionMode]?.Invoke();
            if (guidance == null) return;

            var beacon = SelectBlock(beaconBlocks, guidance, double.MaxValue, LessThan);
            if (beacon != null) {
                beacon.Enabled = true;
                beacon.Radius = 50000;
            }

            var powerCell = SelectBlock(powerCellBlocks, guidance, double.MaxValue, LessThan);
            if (powerCell != null) {
                powerCell.ChargeMode = ChargeMode.Discharge;
            }

            guidance.Enabled = true;
            guidance.ApplyAction("Adn.ActionLaunchMissile");
        }
        void Command_TargetRandomBlockOnAll() {
            guidanceBlocks.ForEach(b => SetTargetRandomBlock(b, true));
        }
        void Command_TargetRandomBlockOffAll() {
            guidanceBlocks.ForEach(b => SetTargetRandomBlock(b, false));
        }
        void SetTargetRandomBlock(IMyRadioAntenna b, bool random) {
            b.Enabled = true;
            b.SetValueBool("Adn.PropertyTargetRandomGridBlock", random);
            b.Enabled = true;
        }


        private void TurnOffAllBeacons() => beaconBlocks.ForEach(b => b.Enabled = false);
        private void RechargeAllPowerCells() => powerCellBlocks.ForEach(b => b.ChargeMode = ChargeMode.Recharge);




        IMyRadioAntenna SelectRandomTorpedo() {
            if (guidanceBlocks.Count == 0) return null;
            var rndIndex = randomGenerator.Next(guidanceBlocks.Count);
            return guidanceBlocks[rndIndex];
        }
        IMyRadioAntenna SelectClosestTorpedo() => SelectBlock(guidanceBlocks, referenceBlock, double.MaxValue, LessThan);
        IMyRadioAntenna SelectFurthestTorpedo() => SelectBlock(guidanceBlocks, referenceBlock, 0d, GreaterThan);

    }
}
