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

        enum MissileSelectionMode { Random = 0, Closest = 1, Furthest = 2 }

        // Blocks
        readonly List<IMyRadioAntenna> gdblocks = new List<IMyRadioAntenna>();
        IMyTerminalBlock referenceBlock = null;

        // Command vars
        readonly IDictionary<MissileSelectionMode, Func<IMyRadioAntenna>> MissileSelectionAction = new Dictionary<MissileSelectionMode, Func<IMyRadioAntenna>>();
        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();
        readonly string Instructions;
        readonly Random randomGenerator = new Random();

        // Config Settings
        string referenceBlockName = string.Empty;
        string missilePrimaryTag = "Torpedo Payload Guidance";
        string missileSecondaryTag = "";
        MissileSelectionMode selectionMode = MissileSelectionMode.Random;

        // config vars
        int _configHashCode = 0;

        const string SECTION_MISSILE_GUIDANCE = "Missile Guidance";
        readonly MyIniKey Key_ReferanceBlock = new MyIniKey(SECTION_MISSILE_GUIDANCE, "Reference Block Name");
        readonly MyIniKey Key_GuidanceTag = new MyIniKey(SECTION_MISSILE_GUIDANCE, "Guidance Tag");
        readonly MyIniKey Key_LaunchMode = new MyIniKey(SECTION_MISSILE_GUIDANCE, "Launch Mode");

        public Program() {
            Commands.Add("lock", LockOnAll);
            Commands.Add("off", TurnOffAll);
            Commands.Add("launch", Launch);
            Commands.Add("trdm-on", TargetRandomBlockOnAll);
            Commands.Add("trdm-off", TargetRandomBlockOffAll);

            MissileSelectionAction.Add(MissileSelectionMode.Random, SelectRandomMissile);
            MissileSelectionAction.Add(MissileSelectionMode.Closest, SelectClosestMissile);
            MissileSelectionAction.Add(MissileSelectionMode.Furthest, SelectFurthestMissile);

            // Instructions
            var sb = new StringBuilder();
            sb.AppendLine("Script Commands");
            foreach (var c in Commands.Keys) sb.AppendLine(c);
            Instructions = sb.ToString();

            Echo(Instructions);

            ProcessConfig();
        }

        public void Main(string argument, UpdateType updateSource) {
            ProcessConfig();
            
            var cmdParts = argument.ToLower().Split(new char[] { ' ' }, 2);

            missileSecondaryTag = cmdParts.Length == 2 ? cmdParts[1] : string.Empty;

            LoadBlocks();

            if (gdblocks.Count == 0) {
                Echo("No missile guidance blocks found");
                Echo($"Tag: {missilePrimaryTag}");
            }

            if (Commands.ContainsKey(cmdParts[0])) Commands[cmdParts[0]]?.Invoke();

            Echo(Instructions);
        }

        void ProcessConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;
            _configHashCode = tmpHashCode;

            var ini = new MyIni();

            // Create Default Config
            ini.Add(Key_ReferanceBlock, referenceBlockName);
            ini.Add(Key_GuidanceTag, missilePrimaryTag);
            ini.Add(Key_LaunchMode, (int)selectionMode, "Modes: 0 = Random, 1 = Closest, 2 = Furthest");

            referenceBlockName = ini.Get(Key_ReferanceBlock).ToString();
            missilePrimaryTag = ini.Get(Key_GuidanceTag).ToString();

            var mode = ini.Get(Key_LaunchMode).ToInt32();
            if (Enum.IsDefined(typeof(MissileSelectionMode), mode))
                selectionMode = (MissileSelectionMode)mode;


            Me.CustomData = ini.ToString();
            _configHashCode = Me.CustomData.GetHashCode();
        }

        void LoadBlocks() {
            referenceBlock = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => b.IsSameConstructAs(Me) && b.CustomName.Equals(referenceBlockName),
                b => b.IsSameConstructAs(Me) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => b.IsSameConstructAs(Me) && b is IMyCockpit && ((IMyCockpit)b).IsUnderControl,
                b => b.IsSameConstructAs(Me) && b is IMyCockpit,
                b => b.IsSameConstructAs(Me) && b is IMyRemoteControl);
            if (referenceBlock == null) referenceBlock = Me;

            GridTerminalSystem.GetBlocksOfType(gdblocks, IsMissleGuidance);
        }

        bool IsMissleGuidance(IMyTerminalBlock b) {
            if (!b.IsSameConstructAs(Me)) return false;
            var customName = b.CustomName.ToLower();
            if (!customName.Contains(missilePrimaryTag)) return false;
            if (missileSecondaryTag.Length > 0)
                if (!customName.Contains(missileSecondaryTag)) return false;
            return true;
        }




        void LockOnAll() {
            gdblocks.ForEach(LockOn);
        }
        void LockOn(IMyRadioAntenna b) {
            //b.ApplyAction("OnOff_On");
            b.Enabled = true;
            b.ApplyAction("Adn.ActionLockOnTarget");
        }
        void TurnOffAll() {
            gdblocks.ForEach(b => b.Enabled = false);
            //gdblocks.ForEach(b => b.ApplyAction("OnOff_Off"));
        }
        void Launch() {
            var b = MissileSelectionAction[selectionMode]?.Invoke();
            if (b == null) return;
            //b.ApplyAction("OnOff_On");
            b.Enabled = true;
            b.ApplyAction("Adn.ActionLaunchMissile");
        }
        void TargetRandomBlockOnAll() {
            gdblocks.ForEach(b => SetTargetRandomBlock(b, true));
        }
        void TargetRandomBlockOffAll() {
            gdblocks.ForEach(b => SetTargetRandomBlock(b, false));
        }
        void SetTargetRandomBlock(IMyRadioAntenna b, bool random) {
            //b.ApplyAction("OnOff_On");
            b.Enabled = true;
            b.SetValueBool("Adn.PropertyTargetRandomGridBlock", random);
            //b.ApplyAction("OnOff_Off");
            b.Enabled = true;
        }



        IMyRadioAntenna SelectRandomMissile() {
            var rndIndex = randomGenerator.Next(gdblocks.Count);
            return gdblocks[rndIndex];
        }
        IMyRadioAntenna SelectClosestMissile() => SelectMissile(double.MaxValue, LessThan);
        IMyRadioAntenna SelectFurthestMissile() => SelectMissile(0d, GreaterThan);

        IMyRadioAntenna SelectMissile(double initalDist, Func<double, double, bool> compareFunc) {
            IMyRadioAntenna selected = null;
            var lastDist = initalDist;
            var refPosition = referenceBlock.GetPosition();

            foreach (var b in gdblocks) {
                var currDist = (b.GetPosition() - refPosition).Length();
                if (compareFunc(currDist, lastDist)) {
                    lastDist = currDist;
                    selected = b;
                }
            }

            return selected;
        }
        bool GreaterThan(double a, double b) => a > b;
        bool LessThan(double a, double b) => a < b;

    }
}
