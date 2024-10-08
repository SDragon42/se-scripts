﻿using System;
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

        // config vars
        int _configHashCode = 0;
        const string SEC_TorpedoGuidanceTags = "Torpedo Guidance Tags";
        readonly MyIniKey Key_ReferanceBlock = new MyIniKey(SEC_TorpedoGuidanceTags, "Reference Block Tag");
        readonly MyIniKey Key_GuidanceTag = new MyIniKey(SEC_TorpedoGuidanceTags, "Guidance Tag");
        readonly MyIniKey Key_BeaconTag = new MyIniKey(SEC_TorpedoGuidanceTags, "Beacon Tag");
        readonly MyIniKey Key_PowerCellTag = new MyIniKey(SEC_TorpedoGuidanceTags, "Battery Tag");

        const string SEC_TorpedoLaunch = "Torpedo Launch";
        readonly MyIniKey Key_LaunchMode = new MyIniKey(SEC_TorpedoLaunch, "Launch Mode");

        void ProcessConfig() {
            Debug("ProcessConfig()");
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;
            _configHashCode = tmpHashCode;

            var ini = new MyIni();

            // Create Default Config
            referenceTag = ini.Add(Key_ReferanceBlock, referenceTag).ToString().ToLower();
            torpedoPrimaryTag = ini.Add(Key_GuidanceTag, torpedoPrimaryTag).ToString().ToLower();
            torpedoBeaconTag = ini.Add(Key_BeaconTag, torpedoBeaconTag).ToString().ToLower();
            torpedoPowerCellTag = ini.Add(Key_PowerCellTag, torpedoPowerCellTag).ToString().ToLower();

            var mode = ini.Add(Key_LaunchMode, (int)selectionMode, "Modes: 0 = Random, 1 = Closest, 2 = Furthest").ToInt32();
            if (Enum.IsDefined(typeof(TorpedoSelectionMode), mode))
                selectionMode = (TorpedoSelectionMode)mode;

            Me.CustomData = ini.ToString();
            _configHashCode = Me.CustomData.GetHashCode();

            Debug($"Ref: {referenceTag}");
            Debug($"GTag: {torpedoPrimaryTag}");
            Debug($"BTag: {torpedoBeaconTag}");
            Debug($"Smode: {selectionMode}");
        }

    }
}
