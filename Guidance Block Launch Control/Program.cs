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

        const string ScriptTitle = "SDragons Missile Guidance Launcher";

        enum MissileSelectionMode { Random = 0, Closest = 1, Furthest = 2 }

        // Blocks
        readonly List<IMyRadioAntenna> guidanceBlocks = new List<IMyRadioAntenna>();
        readonly List<IMyBeacon> beaconBlocks = new List<IMyBeacon>();
        IMyTerminalBlock referenceBlock = null;

        // Command vars
        readonly IDictionary<MissileSelectionMode, Func<IMyRadioAntenna>> MissileSelectionAction = new Dictionary<MissileSelectionMode, Func<IMyRadioAntenna>>();
        readonly IDictionary<string, Action> Commands = new Dictionary<string, Action>();
        readonly string Instructions;
        readonly Random randomGenerator = new Random();

        // Config Settings
        string referenceTag = "Cockpit";
        string missilePrimaryTag = "Torpedo Payload Guidance";
        string missileSecondaryTag = string.Empty;
        string missileBeaconTag = "Torpedo Payload Beacon";
        MissileSelectionMode selectionMode = MissileSelectionMode.Random;


        Action<string> Debug = (text) => { };

        public Program() {
            //Debug = Echo;

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

            Echo(ScriptTitle);
            Echo(Instructions);

            ProcessConfig();
        }

        public void Main(string argument, UpdateType updateSource) {
            Echo(ScriptTitle);
            missileSecondaryTag = string.Empty;

            ProcessConfig();

            var cmdParts = argument.ToLower().Split(new char[] { ' ' }, 2);
            missileSecondaryTag = cmdParts.Length == 2 ? cmdParts[1].ToLower() : string.Empty;

            LoadBlocks();

            if (guidanceBlocks.Count == 0) {
                Echo("No missile guidance blocks found");
                Echo($"Tag: {missilePrimaryTag}");
            }

            if (Commands.ContainsKey(cmdParts[0])) Commands[cmdParts[0]]?.Invoke();

            Echo(Instructions);
        }


        // config vars
        int _configHashCode = 0;
        const string SEC_MissileGuidanceTags = "Missile Guidance Tags";
        readonly MyIniKey Key_ReferanceBlock = new MyIniKey(SEC_MissileGuidanceTags, "Reference Block Tag");
        readonly MyIniKey Key_GuidanceTag = new MyIniKey(SEC_MissileGuidanceTags, "Guidance Tag");
        readonly MyIniKey Key_BeaconTag = new MyIniKey(SEC_MissileGuidanceTags, "Beacon Tag");

        const string SEC_MissileLaunch = "Missile Launch";
        readonly MyIniKey Key_LaunchMode = new MyIniKey(SEC_MissileLaunch, "Launch Mode");

        void ProcessConfig() {
            Debug("ProcessConfig()");
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;
            _configHashCode = tmpHashCode;

            var ini = new MyIni();

            // Create Default Config
            ini.Add(Key_ReferanceBlock, referenceTag);
            ini.Add(Key_GuidanceTag, missilePrimaryTag);
            ini.Add(Key_BeaconTag, missileBeaconTag);

            ini.Add(Key_LaunchMode, (int)selectionMode, "Modes: 0 = Random, 1 = Closest, 2 = Furthest");


            referenceTag = ini.Get(Key_ReferanceBlock).ToString().ToLower();
            missilePrimaryTag = ini.Get(Key_GuidanceTag).ToString().ToLower();
            missileBeaconTag = ini.Get(Key_BeaconTag).ToString().ToLower();

            var mode = ini.Get(Key_LaunchMode).ToInt32();
            if (Enum.IsDefined(typeof(MissileSelectionMode), mode))
                selectionMode = (MissileSelectionMode)mode;


            Me.CustomData = ini.ToString();
            _configHashCode = Me.CustomData.GetHashCode();

            Debug($"Ref: {referenceTag}");
            Debug($"GTag: {missilePrimaryTag}");
            Debug($"BTag: {missileBeaconTag}");
            Debug($"Smode: {selectionMode}");
        }

        void LoadBlocks() {
            referenceBlock = GridTerminalSystem.GetBlockOfTypeWithFirst<IMyShipController>(
                b => b.IsSameConstructAs(Me) && b.CustomName.ToLower().Contains(referenceTag),
                b => b.IsSameConstructAs(Me) && b is IMyCockpit && ((IMyCockpit)b).IsMainCockpit,
                b => b.IsSameConstructAs(Me) && b is IMyCockpit && ((IMyCockpit)b).IsUnderControl,
                b => b.IsSameConstructAs(Me) && b is IMyCockpit,
                b => b.IsSameConstructAs(Me) && b is IMyRemoteControl);
            if (referenceBlock == null) referenceBlock = Me;
            Debug($"FRef: {referenceBlock.CustomName}");

            GridTerminalSystem.GetBlocksOfType(guidanceBlocks, IsMissleGuidance);
            Debug($"# Found Torps: {guidanceBlocks.Count}");

            GridTerminalSystem.GetBlocksOfType(beaconBlocks, IsMissleBeacon);
            Debug($"# Found Torps Beacons: {guidanceBlocks.Count}");
        }

        bool IsMissleGuidance(IMyTerminalBlock b) {
            if (!b.IsSameConstructAs(Me)) return false;
            var customName = b.CustomName.ToLower();
            if (!customName.Contains(missilePrimaryTag)) return false;
            if (missileSecondaryTag.Length > 0)
                if (!customName.Contains(missileSecondaryTag)) return false;
            return true;
        }
        //
        bool IsMissleBeacon(IMyTerminalBlock b) {
            if (!b.IsSameConstructAs(Me)) return false;
            var customName = b.CustomName.ToLower();
            if (!customName.Contains(missileBeaconTag)) return false;
            return true;
        }



        void LockOnAll() {
            guidanceBlocks.ForEach(LockOn);
        }
        void LockOn(IMyRadioAntenna b) {
            b.Enabled = true;
            b.ApplyAction("Adn.ActionLockOnTarget");
        }
        void TurnOffAll() {
            guidanceBlocks.ForEach(b => b.Enabled = false);
            beaconBlocks.ForEach(b => b.Enabled = false);
        }
        void Launch() {
            var guidance = MissileSelectionAction[selectionMode]?.Invoke();
            if (guidance == null) return;

            var beacon = SelectBlock(beaconBlocks, guidance, double.MaxValue, LessThan);
            if (beacon != null) {
                beacon.Enabled = true;
                beacon.Radius = 50000;
            }

            guidance.Enabled = true;
            guidance.ApplyAction("Adn.ActionLaunchMissile");
        }
        void TargetRandomBlockOnAll() {
            guidanceBlocks.ForEach(b => SetTargetRandomBlock(b, true));
        }
        void TargetRandomBlockOffAll() {
            guidanceBlocks.ForEach(b => SetTargetRandomBlock(b, false));
        }
        void SetTargetRandomBlock(IMyRadioAntenna b, bool random) {
            b.Enabled = true;
            b.SetValueBool("Adn.PropertyTargetRandomGridBlock", random);
            b.Enabled = true;
        }



        IMyRadioAntenna SelectRandomMissile() {
            if (guidanceBlocks.Count == 0) return null;
            var rndIndex = randomGenerator.Next(guidanceBlocks.Count);
            return guidanceBlocks[rndIndex];
        }
        IMyRadioAntenna SelectClosestMissile() => SelectBlock(guidanceBlocks, referenceBlock, double.MaxValue, LessThan);
        IMyRadioAntenna SelectFurthestMissile() => SelectBlock(guidanceBlocks, referenceBlock, 0d, GreaterThan);


        static T SelectBlock<T>(List<T> blockList, IMyTerminalBlock referenceBlock, double initalDist, Func<double, double, bool> compareFunc) where T : IMyTerminalBlock {
            T selected = default(T);
            var lastDist = initalDist;
            var refPosition = referenceBlock.GetPosition();

            foreach (var b in blockList) {
                var currDist = (b.GetPosition() - refPosition).Length();
                if (compareFunc(currDist, lastDist)) {
                    lastDist = currDist;
                    selected = b;
                }
            }

            return selected;
        }

        static bool GreaterThan(double a, double b) => a > b;
        static bool LessThan(double a, double b) => a < b;

    }
}
