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

namespace IngameScript {
    partial class Program : MyGridProgram {

        const double BLOCK_RELOAD_TIME = 10.0;
        const long INV_ITEM_COUNT_MODIFIER = 1000000L;

        // Modules

        //Blocks
        IMyLargeTurretBase turret;
        IMyBatteryBlock battery;
        IMyRadioAntenna antenna;
        readonly List<IMyParachute> parachutes = new List<IMyParachute>();
        readonly List<IMyDecoy> decoys = new List<IMyDecoy>();
        readonly List<IMyLandingGear> landingGears = new List<IMyLandingGear>();
        readonly List<IMyInteriorLight> parachuteLights = new List<IMyInteriorLight>();
        readonly List<IMyInteriorLight> disarmedLights = new List<IMyInteriorLight>();

        // Config Values
        string CommGroupName { get; set; } = "";
        bool StealthMode { get; set; } = false;
        bool ShowStatusLights { get; set; } = false;
        bool ShowStatusAntenna { get; set; } = false;
        //bool ReportStatusCOMMs { get; set; } = false;

        // Script Vars
        double timeLastBlockLoad = BLOCK_RELOAD_TIME;

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;

            LoadConfig();

            var isTerminalRun = updateSource.HasFlag(UpdateType.Terminal);
            var isCommRun = updateSource.HasFlag(UpdateType.Antenna);
            //var isTriggerRun = updateSource.HasFlag(UpdateType.Trigger);
            //var isAutoRun = updateSource.HasFlag(UpdateType.Update100);

            if (timeLastBlockLoad >= BLOCK_RELOAD_TIME || isTerminalRun) {
                timeLastBlockLoad = 0;
                ClearBlocks();
                LoadBlocks();
                InitBlocks();
            }

            if (isCommRun) {
                ProcessCommand(argument?.ToLower());
            }


            // Get info
            var disarmedLightsEnabled = false;
            var ammoAmount = 0L;
            if (turret != null) {
                ammoAmount = GetInventoryItemCount(turret.GetInventory()) / INV_ITEM_COUNT_MODIFIER;
                if (ammoAmount == 0)
                    turret.Enabled = false;
                disarmedLightsEnabled |= !turret.Enabled;
            }

            var canvasAmount = 0L;
            var emptyParachutes = false;
            foreach (var para in parachutes) {
                var tmp = GetInventoryItemCount(para.GetInventory()) / INV_ITEM_COUNT_MODIFIER;
                canvasAmount += tmp;
                if (tmp == 0)
                    emptyParachutes = true;
            }

            // Set Lights
            SetLights(disarmedLights, disarmedLightsEnabled);
            SetLights(parachuteLights, emptyParachutes);

            var antennaMessage = "Antenna";
            if (ShowStatusAntenna && (ammoAmount <= 1)) {
                antennaMessage += "\nLOW AMMO";
            }
            antenna.CustomName = antennaMessage;

            antenna.EnableBroadcasting = !StealthMode;
        }

        long GetInventoryItemCount(IMyInventory inven) {
            var amount = 0L;
            foreach (var item in inven.GetItems())
                amount += item.Amount.RawValue;
            return amount;
        }

        void SetLights(List<IMyInteriorLight> lights, bool enabled) {
            if (!ShowStatusLights) enabled = false;
            foreach (var b in lights) b.Enabled = enabled;
        }

        void ClearBlocks() {
            parachuteLights.Clear();
            decoys.Clear();
            parachutes.Clear();

            antenna = null;
            battery = null;
            turret = null;
        }
        void LoadBlocks() {
            battery = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyBatteryBlock>(OnSameGrid);
            turret = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyLargeTurretBase>(OnSameGrid);
            antenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(OnSameGrid);

            GridTerminalSystem.GetBlocksOfType(decoys, OnSameGrid);
            GridTerminalSystem.GetBlocksOfType(parachutes, OnSameGrid);
            GridTerminalSystem.GetBlocksOfType(landingGears, OnSameGrid);

            GridTerminalSystem.GetBlocksOfType(parachuteLights, OnParachuteBlock);
            GridTerminalSystem.GetBlocksOfType(disarmedLights, b => !OnParachuteBlock(b));
        }
        bool OnSameGrid(IMyTerminalBlock b) => b.CubeGrid == Me.CubeGrid;
        bool OnParachuteBlock(IMyTerminalBlock b) {
            foreach (var para in parachutes) {
                var blockDist = (b.Position - para.Position).Length();
                if (blockDist == 1) return true;
            }
            return false;
        }

        void InitBlocks() {
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
                b.Color = Color.Red;
                b.Radius = 5f;
                b.BlinkIntervalSeconds = 0f;
                b.BlinkLength = 0f; ;
                b.BlinkOffset = 0f;
                b.Enabled = false;
                b.ShowInTerminal = false;
                b.CustomName = "Light - Disarmed";
            }

            foreach (var b in decoys) {
                b.ShowInTerminal = false;
                b.CustomName = "Decoy";
            }

            foreach (var b in parachutes) {
                b.ShowInTerminal = true;
                b.CustomName = "Parachute";
            }

            foreach (var b in landingGears) {
                b.ShowInTerminal = true;
                b.CustomName = "Parachute";
            }

            RenameMethods.NumberRenameTo(landingGears, "LandingGear");
        }

        static char[] CMD_SPLIT = new char[] { ' ' };
        void ProcessCommand(string command) {
            if (string.IsNullOrWhiteSpace(command)) return;
            var cmdParts = command.Split(CMD_SPLIT, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
