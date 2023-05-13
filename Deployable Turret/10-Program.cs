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
            timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            var timeTilUpdate = MathHelper.Clamp(Math.Truncate(BLOCK_RELOAD_TIME - timeLastBlockLoad) + 1, 0, BLOCK_RELOAD_TIME);
            Echo($"Deployable Turret 0.1 {runningSymbol.GetSymbol(Runtime)}");
            Echo($"Scanning for blocks in {timeTilUpdate:N0} seconds.\n");

            LoadConfig();
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
            SetLights(parachuteLights, !hasAllParachutes);
            SetAntenna();

            if (MainCommands.ContainsKey(argument)) MainCommands[argument]?.Invoke();

            ActionQueue.Run();

            Runtime.UpdateFrequency = ActionQueue.HasTasks ? UpdateFrequency.Update10 : UpdateFrequency.Update100;

            Echo("");
            Echo(turret.Enabled ? "* ARMED *" : "- Disarmed -");
        }

        private void SetAntenna() {
            antenna.EnableBroadcasting = !StealthMode;
            if (StealthMode) return;

            var antennaMessage = TurretId;

            if (ShowStatusOnAntenna) {
                // Show Low power
                if (battery != null && battery.IsWorking) {
                    var remaining = battery.MaxStoredPower / battery.MaxStoredPower;
                    if (remaining <= 0.25f && remaining > 0.1)
                        antennaMessage += "\nLOW POWER";
                    if (remaining <= 0.1f)
                        antennaMessage += "\nCRITICAL POWER";
                }

                // Ammo Level
                switch (ammoAmount) {
                    case 2: antennaMessage += "\nLOW AMMO"; break;
                    case 1: antennaMessage += "\nCRITICAL AMMO"; break;
                    case 0: antennaMessage += "\nNO AMMO"; break;
                }

                // Show Damage
                if (battery == null)
                    antennaMessage += "\nBATTERY DESTROYED";
            }

            antenna.HudText = antennaMessage;
        }

        private void GetCurrentStatus() {
            ammoAmount = 0L;
            if (turret != null) {
                ammoAmount = GetInventoryItemCount(turret.GetInventory());
                if (ammoAmount == 0)
                    turret.Enabled = false;
            }

            var canvasAmount = 0L;
            //hasAllParachutes = true;
            foreach (var para in parachutes) {
                var tmp = GetInventoryItemCount(para.GetInventory());
                canvasAmount += tmp;
                //if (tmp == 0)
                //    hasAllParachutes = false;
            }
            hasAllParachutes = (parachutes.Count / canvasAmount) == 1;
        }

        long GetInventoryItemCount(IMyInventory inven) {
            inventoryItems.Clear();
            inven.GetItems(inventoryItems);
            var amount = 0L;
            foreach (var item in inventoryItems)
                amount += item.Amount.RawValue;

            return amount / 1000000L; // Inventory Item Count Modifier
        }

        void SetLights(List<IMyInteriorLight> lights, bool enabled) {
            if (StealthMode) enabled = false;
            foreach (var b in lights) b.Enabled = enabled;
        }


        void LoadBlocks() {
            if (timeLastBlockLoad < BLOCK_RELOAD_TIME) return;
            timeLastBlockLoad = 0;

            antenna = null;
            battery = null;
            turret = null;

            // Load Blocks
            battery = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyBatteryBlock>(Me.IsSameConstructAs);
            turret = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyLargeTurretBase>(Me.IsSameConstructAs);
            antenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(Me.IsSameConstructAs);

            GridTerminalSystem.GetBlocksOfType(decoys, Me.IsSameConstructAs);
            GridTerminalSystem.GetBlocksOfType(decoys, Me.IsSameConstructAs);
            GridTerminalSystem.GetBlocksOfType(parachutes, Me.IsSameConstructAs);
            GridTerminalSystem.GetBlocksOfType(landingGears, Me.IsSameConstructAs);

            GridTerminalSystem.GetBlocksOfType(parachuteLights, OnParachuteBlock);
            GridTerminalSystem.GetBlocksOfType(disarmedLights, b => !OnParachuteBlock(b));
        }
        bool OnParachuteBlock(IMyTerminalBlock b) => parachutes.Any(p => (b.Position - p.Position).Length() == 1);

        void InitializeBlocks() {
            var blinkOffsetInterval = 100f / parachuteLights.Count;
            var blinkOff = 0f;
            foreach (var b in parachuteLights) {
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

            foreach (var b in disarmedLights) {
                b.Color = Color.Black;
                b.Radius = 5f;
                b.BlinkIntervalSeconds = 0f;
                b.BlinkLength = 0f;
                b.BlinkOffset = 0f;
                b.Enabled = false;
                b.ShowInTerminal = false;
                b.CustomName = "Light - Disarmed";
            }

            foreach (var b in decoys) {
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
            var msg = Listener.AcceptMessage();
            var data = msg.Data as string;
            if (string.IsNullOrWhiteSpace(data)) return;

            var cmdParts = data.Split(CMD_SPLIT, StringSplitOptions.RemoveEmptyEntries);
        }

        void ArmTurret() {
            if (turret.Enabled)
                return;
            ActionQueue.Clear();
            ActionQueue.Add(ArmTurret_TurnOnLights_Sequence());
            ActionQueue.Add(Delay(10000));
            ActionQueue.Add(ArmTurret_TurnOffLights_Sequence());
            ActionQueue.Add(ArmTurret_Sequence(true));

            ActionQueue.Run();
        }
        void DisarmTurret() {
            ActionQueue.Clear();
            ActionQueue.Add(ArmTurret_Sequence(false));
            ActionQueue.Add(DisarmTurret_TurnOnLights_Sequence());

            ActionQueue.Run();
        }

        void TurnOnParachutes() {
            foreach (var p in parachutes)
                p.Enabled = true;
        }
        void TurnOffParachutes() {
            foreach (var p in parachutes)
                p.Enabled = false;
        }


        IEnumerator<bool> ArmTurret_TurnOnLights_Sequence() {
            foreach (var b in disarmedLights) {
                b.Color = Color.Red;
                b.BlinkIntervalSeconds = 1f;
                b.BlinkLength = 50f;
                b.BlinkOffset = 0f;
                b.Enabled = true;
            }

            yield return true;
        }

        IEnumerator<bool> ArmTurret_TurnOffLights_Sequence() {
            foreach (var b in disarmedLights) {
                b.Color = Color.Black;
                b.BlinkIntervalSeconds = 0f;
                b.BlinkLength = 0f;
                b.BlinkOffset = 0f;
                b.Enabled = false;
            }

            yield return true;
        }

        IEnumerator<bool> DisarmTurret_TurnOnLights_Sequence() {
            foreach (var b in disarmedLights) {
                b.Color = Color.Green;
                b.BlinkIntervalSeconds = 0f;
                b.BlinkLength = 0f;
                b.BlinkOffset = 0f;
                b.Enabled = true;
            }

            yield return true;
        }

        IEnumerator<bool> ArmTurret_Sequence(bool enabled) {
            if (turret != null)
                turret.Enabled = enabled;

            yield return true;
        }

        IEnumerator<bool> Delay(double milliseconds) {
            var time = 0.0;
            do {
                yield return (time < milliseconds);
                time += Runtime.TimeSinceLastRun.TotalMilliseconds;
            } while (time < milliseconds);
        }

    }
}
