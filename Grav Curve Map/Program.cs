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

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save() {
        }

        readonly RunningSymbol runningSym = new RunningSymbol();
        readonly StateMachineSets sequenceSets = new StateMachineSets();
        readonly Logging log = new Logging();

        IMyShipController shipController = null;
        IMyTextSurface outputSurface = null;
        IMyTextSurface calcSurface = null;

        Vector3D planetCenter = new Vector3D(0.0, 0.0, 0.0);
        double maxR = 0;

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

                sequenceSets.RunAll();
            } catch (Exception ex) {
                log.AppendLine("ERROR");
                log.AppendLine(ex.Message);
            }

            Echo(log.GetLogText());
        }

        void Abort() {
            log.AppendLine("Abort()");
            LoadBlocks();
            upThrusters.ForEach(t => t.ThrustOverride = 0f);
            Runtime.UpdateFrequency = UpdateFrequency.None;
            sequenceSets.Clear();
        }

        void RunAscent() {
            log.AppendLine("RunAscent()");
            LoadBlocks();
            outputSurface.WriteText(string.Empty);
            calcSurface.WriteText(string.Empty);
            sequenceSets.Add("accent", SEQ_RunAscent());
        }

        private void LoadBlocks() {
            shipController = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyRemoteControl>();
            if (shipController == null) throw new Exception("No RC block found");

            outputSurface = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyTextPanel>(b => Collect.IsTagged(b, "[log]"));
            if (outputSurface == null) throw new Exception("No display with [log]");
            calcSurface = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyTextPanel>(b => Collect.IsTagged(b, "[grav]"));
            if (calcSurface == null) throw new Exception("No display with [grav]");
            InitDisplay();

            var orient = new BlocksByOrientation(shipController);
            GridTerminalSystem.GetBlocksOfType(upThrusters, b => orient.IsDown(b));
            if (upThrusters.Count == 0) throw new Exception("No up thrusters");

            GridTerminalSystem.GetBlocksOfType(gears);
        }


        IEnumerator<bool> SEQ_RunAscent() {
            log.AppendLine("SEQ_RunAscent()");

            upThrusters.ForEach(t => t.Enabled = true);
            yield return true;

            upThrusters.ForEach(t => t.ThrustOverridePercentage = 1f);
            gears.ForEach(g => g.Unlock());

            var lastElevation = 0.0;
            shipController.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out lastElevation);

            var seaLevelElevation = 0.0;
            var lastGAccel = 0.0;
            var hasElevation = true;
            var points = new List<double>();
            do {
                yield return true;

                hasElevation = shipController.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out seaLevelElevation);
                if (!hasElevation)
                    break;

                var gAccel = GetGravityAccel();
                var coreAltitude = GetCoreAltitude();
                var calcGAccel = GetCalculatedGravityAccel(coreAltitude);

                calcSurface.WriteText($"Real: {gAccel:N2} m/s\nCalc: {calcGAccel:N2} m/s");

                if (!double.IsNaN(gAccel)) {
                    gAccel = Math.Round(gAccel, 2);
                    if (gAccel != lastGAccel && lastGAccel != 0.0) {
                        var midElevation = lastElevation + (seaLevelElevation - lastElevation);
                        points.Add(midElevation);
                        outputSurface.WriteText($"{midElevation:N2}  {gAccel,3:N2}\n", true);
                    }
                } else {
                    var midElevation = points.Average();
                    outputSurface.WriteText($"{midElevation:N2}  {0.0,3:N2}\n", true);
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

        double GetCoreAltitude() {
            var loc = shipController.GetPosition();
            var coreAltitude = (loc - planetCenter).Length();
            return coreAltitude;
        }

        //67,214.56
        double GetCalculatedGravityAccel(double coreAltitude) {
            if (double.IsNaN(coreAltitude))
                return double.NaN;
            /*
            g  = the current acceleration due to gravity felt at r (the current altitude away from the core) in m/s^2 (on the surface it should be 9.81 m/s^2 or 1 g)
            b  = base g in m/s^2. For planets: 9.81. For moons: 2.45 m/s^2 (a quarter of 9.81 or 0.25 g)
            r  = current altitude/distance from planet 'center' or core in meters.
            MinR  = the minimum Radius of the planet in meters - it's found in the planet's sbc file.
            MaxR = the maximum Radius of the planet in meters - it's found in the planet's sbc file, 

            if r > MaxR           g = b x (MaxR/r )^7 [Gravity after maximum Radius decreases exponentially to the power of 7]
            if MinR <= r <= MaxR  g = b               [Gravity remains base g until, your distance from planet 'center' is greater than the maximum Radius]
            if r < MinR           g = b x (r/MinR)    [Typically radius less than minimum Radius under the 'surface' of the planet]
             */

            const double maxR = 67244;
            // 67214.56  67205.27  67244.17  67244.34  67244.09

            if (coreAltitude < maxR)
                return 9.81;

            var gAccel = 9.81 * Math.Pow(maxR / coreAltitude, 7);

            return gAccel;
        }

        void InitDisplay() {
            if (outputSurface != null) {
                outputSurface.ContentType = ContentType.TEXT_AND_IMAGE;
                outputSurface.TextPadding = 0f;
                outputSurface.Alignment = TextAlignment.RIGHT;
            }

            if (calcSurface != null) {
                calcSurface.ContentType = ContentType.TEXT_AND_IMAGE;
                calcSurface.TextPadding = 8f;
                calcSurface.Alignment = TextAlignment.CENTER;
                calcSurface.FontSize = 3.6f;
                calcSurface.Font = LCDFonts.MONOSPACE;
            }
        }

    }
}
