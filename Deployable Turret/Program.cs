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

        readonly List<IMyTerminalBlock> FoundBlocks = new List<IMyTerminalBlock>();
        readonly List<IMyParachute> Parachutes = new List<IMyParachute>();
        readonly List<IMyInteriorLight> Lights = new List<IMyInteriorLight>();
        readonly List<IMyDecoy> Decoys = new List<IMyDecoy>();

        IMyLargeTurretBase Turret;
        IMyBatteryBlock Battery;
        IMyRadioAntenna Antenna;

        double timeLastBlockLoad = BLOCK_RELOAD_TIME;

        public Program() {
            //Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save() {
        }

        public void Main(string argument, UpdateType updateSource) {
            timeLastBlockLoad += Runtime.TimeSinceLastRun.TotalSeconds;
            var isTerminalRun = updateSource.HasFlag(UpdateType.Terminal);

            if (timeLastBlockLoad >= BLOCK_RELOAD_TIME || isTerminalRun) {
                timeLastBlockLoad = 0;
                ClearBlocks();
                LoadBlocks();
                InitBlocks();
                if (isTerminalRun)
                    EchoFoundBlocks();
            }
            /*
             * Beacon shows ammo level
             * Ammo Below threshold - strobe beacon
             * Check Parachutes
             * Not enough parachutes? turn on strobing lights
             */
        }

        void ClearBlocks() {
            Lights.Clear();
            Decoys.Clear();
            Parachutes.Clear();

            Antenna = null;
            Battery = null;
            Turret = null;
        }
        void LoadBlocks() {
            GridTerminalSystem.GetBlocksOfType(FoundBlocks, block => block.CubeGrid == Me.CubeGrid);
            var sb = new StringBuilder();
            foreach (var b in FoundBlocks) {
                b.ShowInTerminal = true;

                if (b is IMyDecoy) {
                    Decoys.Add((IMyDecoy)b);
                } else if (b is IMyParachute) {
                    Parachutes.Add((IMyParachute)b);
                } else if (b is IMyInteriorLight) {
                    Lights.Add((IMyInteriorLight)b);
                } else if (b is IMyBatteryBlock) {
                    if (Battery == null) Battery = (IMyBatteryBlock)b;
                } else if (b is IMyLargeTurretBase) {
                    if (Turret == null) Turret = (IMyLargeTurretBase)b;
                } else if (b is IMyRadioAntenna) {
                    if (Antenna == null) Antenna = (IMyRadioAntenna)b;
                } else {

                }
            }
            Me.CustomData = sb.ToString();
        }
        void EchoFoundBlocks() {
            Echo($"Decoys: {Decoys.Count}");
            Echo($"Parachutes: {Parachutes.Count}");
            Echo($"lights: {Lights.Count}");
            Echo("Turret: " + (Turret != null));
            Echo("Battery: " + (Battery != null));
            Echo("Antenna: " + (Antenna != null));
        }
        void InitBlocks() {
            var blinkOffsetInterval = 100f / Lights.Count;
            var blinkOff = 0f;
            foreach (var light in Lights) {
                light.Color = Color.Red;
                light.Radius = 2f;
                light.BlinkIntervalSeconds = 1f;
                light.BlinkLength = blinkOffsetInterval;
                light.BlinkOffset = blinkOff;
                light.Enabled = false;
                light.ShowInTerminal = false;
                blinkOff += blinkOffsetInterval;
            }
        }

    }
}
