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
        const UpdateFrequency UPDATE_RATE = UpdateFrequency.Update100;

        //const string KEY_USE_COMMS = "Use COMMs";

        //Modules
        readonly ConfigINI Config = new ConfigINI("Sandbag Turret Defense");
        readonly Settings settings = new Settings();

        //Blocks
        IMyLargeTurretBase Turret;
        IMyBatteryBlock Battery;
        IMyRadioAntenna Antenna;
        readonly List<IMyParachute> Parachutes = new List<IMyParachute>();
        readonly List<IMyDecoy> Decoys = new List<IMyDecoy>();
        readonly List<IMyLandingGear> LandingGears = new List<IMyLandingGear>();
        readonly List<IMyInteriorLight> ParachuteLights = new List<IMyInteriorLight>();
        readonly List<IMyInteriorLight> DisarmedLights = new List<IMyInteriorLight>();


        double timeLastBlockLoad = BLOCK_RELOAD_TIME;
        int configHashCode = 0;

        public Program() {
            Config.AddKey(ConfigKeys.COMM_GROUP_NAME, "Sandbag");
            Config.AddKey(ConfigKeys.STEALTH_MODE, false);
            Config.AddKey(ConfigKeys.STATUS_LIGHTS, true);
            Config.AddKey(ConfigKeys.STATUS_ANTENNA, true);
            Config.AddKey(ConfigKeys.STATUS_COMMS, true);

            Runtime.UpdateFrequency = UPDATE_RATE;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;

            UpdateConfig();

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

            }


            // Get info
            var disarmedLightsEnabled = false;
            var ammoAmount = 0L;
            if (Turret != null) {
                ammoAmount = GetInventoryItemCount(Turret.GetInventory()) / 1000000L;
                if (ammoAmount == 0)
                    Turret.Enabled = false;
                disarmedLightsEnabled |= !Turret.Enabled;
            }

            var canvasAmount = 0L;
            var emptyParachutes = false;
            foreach (var para in Parachutes) {
                var tmp = GetInventoryItemCount(para.GetInventory()) / 1000000L;
                canvasAmount += tmp;
                Echo($"{para.CustomName}: {tmp}");
                if (tmp == 0)
                    emptyParachutes = true;
            }


            Echo($"Disarmed: {disarmedLightsEnabled}");
            Echo($"Ammo: {ammoAmount}");

            // Disarmed Lights
            SetLights(DisarmedLights, disarmedLightsEnabled);
            SetLights(ParachuteLights, emptyParachutes);




            if (settings.ShowStatusAntenna) {
                Antenna.CustomName = "";
            } else {
                Antenna.CustomName = "";
            }

            /*
             * Beacon shows ammo level
             * Ammo Below threshold - strobe beacon
             * Check Parachutes
             * Not enough parachutes? turn on strobing lights
             */

            Antenna.Enabled = !settings.StealthMode;
            if (Antenna.EnableBroadcasting) {
            }
        }

        long GetInventoryItemCount(IMyInventory inven) {
            var amount = 0L;
            foreach (var item in inven.GetItems())
                amount += item.Amount.RawValue;
            return amount;
        }

        void SetLights(List<IMyInteriorLight> lights, bool enabled) {
            if (!settings.ShowStatusLights) enabled = false;
            foreach (var b in lights) b.Enabled = enabled;
        }

        void UpdateConfig() {
            var x = Me.CustomData.GetHashCode();
            if (x == configHashCode) return;
            Config.Load(Me);
            Config.Save(Me);
            configHashCode = Me.CustomData.GetHashCode();
            settings.Load(Config);
        }

        void ClearBlocks() {
            ParachuteLights.Clear();
            Decoys.Clear();
            Parachutes.Clear();

            Antenna = null;
            Battery = null;
            Turret = null;
        }
        void LoadBlocks() {
            Battery = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyBatteryBlock>(OnSameGrid);
            Turret = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyLargeTurretBase>(OnSameGrid);
            Antenna = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRadioAntenna>(OnSameGrid);

            GridTerminalSystem.GetBlocksOfType(Decoys, OnSameGrid);
            GridTerminalSystem.GetBlocksOfType(Parachutes, OnSameGrid);
            GridTerminalSystem.GetBlocksOfType(LandingGears, OnSameGrid);

            GridTerminalSystem.GetBlocksOfType(ParachuteLights, OnParachuteBlock);
            GridTerminalSystem.GetBlocksOfType(DisarmedLights, b => !OnParachuteBlock(b));
        }
        bool OnSameGrid(IMyTerminalBlock b) => b.CubeGrid == Me.CubeGrid;
        bool OnParachuteBlock(IMyTerminalBlock b) {
            foreach (var para in Parachutes) {
                var blockDist = (b.Position - para.Position).Length();
                if (blockDist == 1) return true;
            }
            return false;
        }

        void InitBlocks() {
            var blinkOffsetInterval = 100f / ParachuteLights.Count;
            var blinkOff = 0f;
            foreach (var b in ParachuteLights) {
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

            foreach (var b in DisarmedLights) {
                b.Color = Color.Red;
                b.Radius = 5f;
                b.BlinkIntervalSeconds = 0f;
                b.BlinkLength = 0f; ;
                b.BlinkOffset = 0f;
                b.Enabled = false;
                b.ShowInTerminal = false;
                b.CustomName = "Light - Disarmed";
            }

            foreach (var b in Decoys) {
                b.ShowInTerminal = false;
                b.CustomName = "Decoy";
            }

            foreach (var b in Parachutes) {
                b.ShowInTerminal = true;
                b.CustomName = "Parachute";
            }

            foreach (var b in LandingGears) {
                b.ShowInTerminal = true;
                b.CustomName = "Parachute";
            }

            RenameMethods.NumberRenameTo(LandingGears, "LandingGear");
        }

    }
}
