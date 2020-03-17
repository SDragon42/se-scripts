﻿using Sandbox.Game.EntityComponents;
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

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save() {
        }

        readonly RunningSymbol runningSym = new RunningSymbol();
        readonly StateMachineSets sequenceSets = new StateMachineSets();
        readonly VectorAlign vectorAlign = new VectorAlign();
        readonly Logging log = new Logging();

        IMyShipController shipController = null;
        IMyTextSurface outputSurface = null;
        readonly List<IMyThrust> upThrusters = new List<IMyThrust>();
        readonly List<IMyLandingGear> gears = new List<IMyLandingGear>();

        public void Main(string argument, UpdateType updateSource) {
            try {
                Echo("Grav Map " + runningSym.GetSymbol(Runtime));
                argument = argument?.ToLower() ?? string.Empty;

                switch (argument) {
                    case "abort":
                        Abort();
                        break;
                    case "go":
                        RunAscent();
                        break;
                    default: break;
                }

                sequenceSets.RunAllTasks();
            } catch (Exception ex) {
                log.AppendLine("ERROR");
                log.AppendLine(ex.Message);
                //Runtime.UpdateFrequency = UpdateFrequency.None;
            }

            Echo(log.GetLogText());
        }

        void Abort() {
            log.AppendLine("Abort()");
            LoadBlocks();
            upThrusters.ForEach(t => t.ThrustOverride = 0f);
            //Runtime.UpdateFrequency = UpdateFrequency.None;
            sequenceSets.Clear();
        }

        void RunAscent() {
            log.AppendLine("RunAscent()");
            LoadBlocks();
            outputSurface.WriteText(string.Empty);
            //Runtime.UpdateFrequency = UpdateFrequency.Update10;
            sequenceSets.Add("accent", SEQ_RunAscent());
        }

        private void LoadBlocks() {
            shipController = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>();
            if (shipController == null) throw new Exception("No RC block found");

            outputSurface = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyTextPanel>(b => Collect.IsTagged(b, "[log]")); //b.CustomName.Equals("[log]", StringComparison.OrdinalIgnoreCase));
            if (outputSurface == null) throw new Exception("No display with [log]");
            InitDisplay();

            var orient = new BlocksByOrientation(shipController);
            GridTerminalSystem.GetBlocksOfType(upThrusters, b => orient.IsDown(b));
            if (upThrusters.Count == 0) throw new Exception("No up thrusters");

            GridTerminalSystem.GetBlocksOfType(gears);
        }

        IEnumerator<bool> SEQ_RunAscent() {
            log.AppendLine("SEQ_RunAscent()");
            yield return true;
            upThrusters.ForEach(t => t.ThrustOverridePercentage = 1f);
            gears.ForEach(g => g.Unlock());

            var lastElevation = 0.0;
            shipController.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out lastElevation);

            var seaLevelElevation = 0.0;
            var lastGAccel = 0.0;
            var hasElevation = true;
            do {
                yield return true;

                hasElevation = shipController.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out seaLevelElevation);
                if (!hasElevation)
                    break;

                var gAccel = GetGravityAccel();
                gAccel = Math.Round(gAccel, 1);

                if (gAccel != lastGAccel) {
                    //var elevation = hasElevation
                    //    ? seaLevelElevation + ((seaLevelElevation - lastElevation) / 2)
                    //    : lastElevation;
                    //outputSurface.WriteText($"{elevation:N2}  {gAccel,3:N1}\n", true);
                    outputSurface.WriteText($"{lastElevation:N2}  {lastGAccel,3:N1}\n", true);
                    outputSurface.WriteText($"{seaLevelElevation:N2}  {gAccel,3:N1}\n", true);
                }

                lastGAccel = gAccel;
                lastElevation = seaLevelElevation;
            } while (hasElevation);

            upThrusters.ForEach(t => t.ThrustOverride = 0f);

            yield return false;
        }

        double GetGravityAccel() {
            var gVec = shipController.GetNaturalGravity();
            var gAccel = Math.Sqrt(
                Math.Pow(gVec.X, 2) +
                Math.Pow(gVec.Y, 2) +
                Math.Pow(gVec.Z, 2));
            return gAccel;
        }

        void InitDisplay() {
            if (outputSurface == null) return;
            outputSurface.ContentType = ContentType.TEXT_AND_IMAGE;
            outputSurface.TextPadding = 0f;
            outputSurface.Alignment = TextAlignment.RIGHT;
        }

    }
}
