using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {

        int _configHashCode = 0;

        //const string SECTION_TIMER = "Countdown Timer";
        readonly MyIniKey Key_DisplayBlock = new MyIniKey("Block Names", "Display Block");
        readonly MyIniKey Key_TimerBlock = new MyIniKey("Block Names", "Timer Block");
        readonly MyIniKey Key_NumSeconds = new MyIniKey("Timer", "Countdown Time");
        readonly MyIniKey Key_DisplayClearSeconds = new MyIniKey("Timer", "Display Clear Time");

        void LoadConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;

            Ini.Clear();
            Ini.TryParse(Me.CustomData);

            Ini.Add(Key_DisplayBlock, string.Empty);
            Ini.Add(Key_TimerBlock, string.Empty);
            Ini.Add(Key_NumSeconds, 30);
            Ini.Add(Key_DisplayClearSeconds, 5);

            Me.CustomData = Ini.ToString();
            _configHashCode = Me.CustomData.GetHashCode();

        }

    }
}
