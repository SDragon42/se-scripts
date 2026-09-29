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

        void Command_LockOnAll() {
            _guidanceBlocks.ForEach(LockOn);
        }
        void LockOn(IMyRadioAntenna guidanceBlock) {
            guidanceBlock.Enabled = true;
            guidanceBlock.ApplyAction("Adn.ActionLockOnTarget");
        }
        void Command_TurnOffAll() {
            _guidanceBlocks.ForEach(b => b.Enabled = false);
            TurnOffAllBeacons();
            RechargeAllPowerCells();
        }
        void Command_Launch() {
            var guidance = _torpedoSelection[_config.selectionMode]?.Invoke();
            if (guidance == null) return;

            var beacon = SelectBlock(_beaconBlocks, guidance, double.MaxValue, LessThan);
            if (beacon != null) {
                beacon.Enabled = true;
                beacon.Radius = 50000;
            }

            var powerCell = SelectBlock(_powerCellBlocks, guidance, double.MaxValue, LessThan);
            if (powerCell != null) {
                powerCell.ChargeMode = ChargeMode.Discharge;
            }

            guidance.Enabled = true;
            guidance.ApplyAction("Adn.ActionLaunchMissile");
        }
        void Command_TargetRandomBlockOnAll() {
            _guidanceBlocks.ForEach(b => SetTargetRandomBlock(b, true));
        }
        void Command_TargetRandomBlockOffAll() {
            _guidanceBlocks.ForEach(b => SetTargetRandomBlock(b, false));
        }
        void SetTargetRandomBlock(IMyRadioAntenna b, bool random) {
            b.Enabled = true;
            b.SetValueBool("Adn.PropertyTargetRandomGridBlock", random);
            b.Enabled = true;
        }


        private void TurnOffAllBeacons() => _beaconBlocks.ForEach(b => b.Enabled = false);
        private void RechargeAllPowerCells() => _powerCellBlocks.ForEach(b => b.ChargeMode = ChargeMode.Recharge);




        IMyRadioAntenna SelectRandomTorpedo() {
            if (_guidanceBlocks.Count == 0) return null;
            var rndIndex = _randomGenerator.Next(_guidanceBlocks.Count);
            return _guidanceBlocks[rndIndex];
        }
        IMyRadioAntenna SelectClosestTorpedo() => SelectBlock(_guidanceBlocks, _referenceBlock, double.MaxValue, LessThan);
        IMyRadioAntenna SelectFurthestTorpedo() => SelectBlock(_guidanceBlocks, _referenceBlock, 0d, GreaterThan);

    }
}
