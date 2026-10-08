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
    partial class Program : MyGridProgram {

        
        
        public void Main(string argument, UpdateType updateSource) {
            _timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - _timeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);
            Echo($"Deployable Turret 0.1 {_running.GetSymbol(Runtime)}");
            Echo($"Scanning for blocks in {timeTilUpdate:N0} seconds.\n");

            _config.Load();
            LoadBlocks();
            //Debug($"{decoys.Count} Decoys");
            //Debug($"{parachutes.Count} Parachutes");
            //Debug($"{landingGears.Count} LandingGears");
            //Debug($"{parachuteLights.Count} Para-Lights");
            //Debug($"{disarmedLights.Count} DisArm-Lights");

            // Get info
            GetCurrentStatus();
            //Debug($"Ammo: {ammoAmount}");
            //Debug($"Has Parachutes: {hasAllParachutes}");

            // Set Lights
            //SetLights(disarmedLights, disarmedLightsOn);
            SetLights(_parachuteLights, !_hasAllParachutes);
            SetAntenna();

            if (_mainCommands.ContainsKey(argument)) _mainCommands[argument]?.Invoke();

            _actionQueue.Run();

            Runtime.UpdateFrequency = _actionQueue.HasTasks ? UpdateFrequency.Update10 : UpdateFrequency.Update100;

            Echo("");
            if (_turret != null) {
                Echo(_turret.Enabled ? "* ARMED *" : "- Disarmed -");
            } else {
                Echo("Turret missing!");
            }
        }

        private void SetAntenna() {
            if (_antenna == null) return;
            _antenna.EnableBroadcasting = !_config.StealthMode;
            if (_config.StealthMode) return;

            var antennaMessage = _config.Id;

            if (_config.ShowStatusOnAntenna) {
                // Show Low power
                if (_battery != null && _battery.IsWorking) {
                    var remaining = _battery.CurrentStoredPower / _battery.MaxStoredPower;
                    if (remaining <= 0.25f && remaining > 0.1)
                        antennaMessage += "\nLOW POWER";
                    if (remaining <= 0.1f)
                        antennaMessage += "\nCRITICAL POWER";
                }

                // Ammo Level
                switch (_ammoAmount) {
                    case 2: antennaMessage += "\nLOW AMMO"; break;
                    case 1: antennaMessage += "\nCRITICAL AMMO"; break;
                    case 0: antennaMessage += "\nNO AMMO"; break;
                }

                // Show Damage
                if (_battery == null)
                    antennaMessage += "\nBATTERY DESTROYED";
            }

            _antenna.HudText = antennaMessage;
        }

        private void GetCurrentStatus() {
            _ammoAmount = 0L;
            if (_turret != null) {
                _ammoAmount = GetInventoryItemCount(_turret.GetInventory());
                if (_ammoAmount == 0)
                    _turret.Enabled = false;
            }

            var canvasAmount = 0L;
            //hasAllParachutes = true;
            foreach (var para in _parachutes) {
                var tmp = GetInventoryItemCount(para.GetInventory());
                canvasAmount += tmp;
                //if (tmp == 0)
                //    hasAllParachutes = false;
            }

            if (canvasAmount == 0) {
                _hasAllParachutes = false;
            } else {
                _hasAllParachutes = (_parachutes.Count / canvasAmount) == 1;
            }
        }

        long GetInventoryItemCount(IMyInventory inventory) {
            _inventoryItems.Clear();
            inventory.GetItems(_inventoryItems);
            var amount = 0L;
            foreach (var item in _inventoryItems)
                amount += item.Amount.RawValue;

            return amount / 1000000L; // Inventory Item Count Modifier
        }

        void SetLights(List<IMyInteriorLight> lights, bool enabled) {
            if (_config.StealthMode) enabled = false;
            foreach (var b in lights) b.Enabled = enabled;
        }


        void LoadBlocks() {
            if (_timeLastBlockLoad < BLOCK_RELOAD_TIME) return;
            _timeLastBlockLoad = 0;

            _antenna = null;
            _battery = null;
            _turret = null;

            // Load Blocks
            _battery = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyBatteryBlock>(Me.IsSameConstructAs);
            _turret = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyLargeTurretBase>(Me.IsSameConstructAs);
            _antenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(Me.IsSameConstructAs);

            GridTerminalSystem.GetBlocksOfType(_decoys, Me.IsSameConstructAs);
            GridTerminalSystem.GetBlocksOfType(_parachutes, Me.IsSameConstructAs);
            GridTerminalSystem.GetBlocksOfType(_landingGears, Me.IsSameConstructAs);

            GridTerminalSystem.GetBlocksOfType(_parachuteLights, OnParachuteBlock);
            GridTerminalSystem.GetBlocksOfType(_disarmedLights, b => !OnParachuteBlock(b));
        }
        bool OnParachuteBlock(IMyTerminalBlock b) => _parachutes.Any(p => (b.Position - p.Position).Length() == 1);

        void InitializeBlocks() {
            var blinkOffsetInterval = 100f / _parachuteLights.Count;
            var blinkOff = 0f;
            foreach (var b in _parachuteLights) {
                b.Color = Color.Orange;
                b.Radius = 2f;
                b.BlinkIntervalSeconds = 1f;
                b.BlinkLength = blinkOffsetInterval;
                b.BlinkOffset = blinkOff;
                b.Enabled = false;
                b.ShowInTerminal = false;
                b.CustomName = "Light - Parachute Warning";
                blinkOff += blinkOffsetInterval;
            }

            foreach (var b in _disarmedLights) {
                b.Color = Color.Black;
                b.Radius = 5f;
                b.BlinkIntervalSeconds = 0f;
                b.BlinkLength = 0f;
                b.BlinkOffset = 0f;
                b.Enabled = false;
                b.ShowInTerminal = false;
                b.CustomName = "Light - Disarmed";
            }

            foreach (var b in _decoys) {
                b.ShowInTerminal = false;
                b.CustomName = "Decoy";
            }

            //foreach (var b in parachutes) {
            //    b.ShowInTerminal = true;
            //    b.CustomName = "Parachute";
            //}

            //foreach (var b in landingGears) {
            //    b.ShowInTerminal = true;
            //    b.CustomName = "Landing Gear";
            //}
        }



        void IgcUpdate() {
            var msg = _listener.AcceptMessage();
            var data = msg.Data as string;
            if (string.IsNullOrWhiteSpace(data)) return;

            var cmdParts = data.Split(CMD_SPLIT, StringSplitOptions.RemoveEmptyEntries);
        }

        void ArmTurret() {
            if (_turret == null || _turret.Enabled) return;
            _actionQueue.Clear();
            _actionQueue.Add(ArmTurret_TurnOnLights_Sequence());
            _actionQueue.Add(DelayEnumerator(10));
            _actionQueue.Add(ArmTurret_TurnOffLights_Sequence());
            _actionQueue.Add(ArmTurret_Sequence(true));
            _actionQueue.Run();
        }
        void DisarmTurret() {
            _actionQueue.Clear();
            _actionQueue.Add(ArmTurret_Sequence(false));
            _actionQueue.Add(DisarmTurret_TurnOnLights_Sequence());
            _actionQueue.Run();
        }

        void TurnOnParachutes() => _parachutes.ForEach(p => p.Enabled = true);
        void TurnOffParachutes() => _parachutes.ForEach(p => p.Enabled = false);


        IEnumerator<bool> ArmTurret_TurnOnLights_Sequence() {
            foreach (var b in _disarmedLights) {
                b.Color = Color.Red;
                b.BlinkIntervalSeconds = 1f;
                b.BlinkLength = 50f;
                b.BlinkOffset = 0f;
                b.Enabled = true;
            }
            yield return true;
        }

        IEnumerator<bool> ArmTurret_TurnOffLights_Sequence() {
            foreach (var b in _disarmedLights) {
                b.Color = Color.Black;
                b.BlinkIntervalSeconds = 0f;
                b.BlinkLength = 0f;
                b.BlinkOffset = 0f;
                b.Enabled = false;
            }
            yield return true;
        }

        IEnumerator<bool> DisarmTurret_TurnOnLights_Sequence() {
            foreach (var b in _disarmedLights) {
                b.Color = Color.Green;
                b.BlinkIntervalSeconds = 0f;
                b.BlinkLength = 0f;
                b.BlinkOffset = 0f;
                b.Enabled = true;
            }
            yield return true;
        }

        IEnumerator<bool> ArmTurret_Sequence(bool enabled) {
            if (_turret != null)
                _turret.Enabled = enabled;
            yield return true;
        }

    }
}
