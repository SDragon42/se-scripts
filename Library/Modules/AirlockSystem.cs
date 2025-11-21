// <mdk sortorder="1000" />
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
    partial class Program {
        /// <summary>
        /// Features:
        /// Any number of airlocks using single programmable block & timer
        /// Any number of doors per airlock(can be used with Hangar Doors)
        /// Any number of air vents per airlock(so you can use it for large airlocks)
        /// Automatic pressurization / depressurization with optional pressure detection in rooms behind doors
        /// Doors automatically closed & locked / unlocked & opened
        /// Lights changing colors based on state of airlock
        /// Play sound blocks when airlock is pressurized, depressurized or in both cases
        /// LCD display support to display state of any number of airlocks on any LCD
        /// Unlocks doors without opening when airlock can't be depressurized / pressurized
        /// 3 button actions support: transfer to inner doors, outer doors or switch between them
        /// Airlock can be also controlled by switching 'Depressurize' on air vents
        /// Supports all official Space Engineers languages
        /// </summary>
        class AirlockSystem {

            class AirlockGroup {

            }
        }
    }
}
