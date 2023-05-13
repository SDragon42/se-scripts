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

        public Action<string> Debug = (msg) => { };

        const double BLOCK_RELOAD_TIME = 10.0;

        const string ListenerTagName = "DeployableTurret";
        //const string IGC_Update = "IGC_Update";

        readonly RunningSymbol runningSymbol = new RunningSymbol();
        readonly StateMachineQueue ActionQueue = new StateMachineQueue();

        //Blocks
        IMyLargeTurretBase turret;
        IMyBatteryBlock battery;
        IMyRadioAntenna antenna;
        readonly List<IMyParachute> parachutes = new List<IMyParachute>();
        readonly List<IMyDecoy> decoys = new List<IMyDecoy>();
        readonly List<IMyLandingGear> landingGears = new List<IMyLandingGear>();
        readonly List<IMyInteriorLight> parachuteLights = new List<IMyInteriorLight>();
        readonly List<IMyInteriorLight> disarmedLights = new List<IMyInteriorLight>();
        readonly List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();

        // Config Values
        string CommGroupName { get; set; } = "";
        string TurretId { get; set; } = "";
        bool StealthMode { get; set; } = false;
        bool ShowStatusOnAntenna { get; set; } = false;
        bool ReportStatusOnCOMMs { get; set; } = false;

        //
        readonly IDictionary<string, Action> MainCommands = new Dictionary<string, Action>();
        readonly IDictionary<string, Action> IgcCommands = new Dictionary<string, Action>();

        // Script Vars
        readonly IMyBroadcastListener Listener;
        double timeLastBlockLoad = BLOCK_RELOAD_TIME;
        long ammoAmount = 0;
        bool hasAllParachutes = false;


        readonly static char[] CMD_SPLIT = new char[] { ' ' };

        public Program() {
            Debug = Echo;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            Listener = IGC.RegisterBroadcastListener(ListenerTagName);
            Listener.SetMessageCallback("IGC_Update");

            IgcCommands.Add("arm", ArmTurret);
            IgcCommands.Add("disarm", DisarmTurret);
            //IgcCommands.Add("parachutes-on", TurnOnParachutes);
            //IgcCommands.Add("parachutes-off", TurnOffParachutes);

            IgcCommands.Add("deploy", null);

            IgcCommands.Add("stealth-on", null);
            IgcCommands.Add("stealth-off", null);

            foreach (var cmd in IgcCommands) MainCommands.Add(cmd.Key, cmd.Value);
            MainCommands.Add("init", InitializeBlocks);
            MainCommands.Add("IGC_Update", IgcUpdate);

        }

        public void Save() {
        }
    }
}
