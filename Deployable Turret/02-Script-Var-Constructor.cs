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
        const long INV_ITEM_COUNT_MODIFIER = 1000000L;
        const string ListenerTagName = "DeployableTurret";
        const string IGC_Update = "IGC_Update";

        readonly RunningSymbol _running = new RunningSymbol();
        readonly StateMachineQueue _actionQueue = new StateMachineQueue();

        //Blocks
        IMyLargeTurretBase _turret;
        IMyBatteryBlock _battery;
        IMyRadioAntenna _antenna;
        readonly List<IMyParachute> _parachutes = new List<IMyParachute>();
        readonly List<IMyDecoy> _decoys = new List<IMyDecoy>();
        readonly List<IMyLandingGear> _landingGears = new List<IMyLandingGear>();
        readonly List<IMyInteriorLight> _parachuteLights = new List<IMyInteriorLight>();
        readonly List<IMyInteriorLight> _disarmedLights = new List<IMyInteriorLight>();
        readonly List<MyInventoryItem> _inventoryItems = new List<MyInventoryItem>();

        // Config Values
        Config _config = new Config();

        //
        readonly IDictionary<string, Action> _mainCommands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
        readonly IDictionary<string, Action> _igcCommands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);

        // Script Vars
        readonly IMyBroadcastListener _listener;
        double _timeLastBlockLoad = BLOCK_RELOAD_TIME;
        long _ammoAmount = 0;
        bool _hasAllParachutes = false;


        readonly static char[] CMD_SPLIT = new char[] { ' ' };

        public Program() {
            Debug = Echo;
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            _listener = IGC.RegisterBroadcastListener(ListenerTagName);
            _listener.SetMessageCallback("IGC_Update");

            _igcCommands.Add("arm", ArmTurret);
            _igcCommands.Add("disarm", DisarmTurret);
            //IgcCommands.Add("parachutes-on", TurnOnParachutes);
            //IgcCommands.Add("parachutes-off", TurnOffParachutes);

            // IgcCommands.Add("deploy", null);

            // IgcCommands.Add("stealth-on", null);
            // IgcCommands.Add("stealth-off", null);

            foreach (var cmd in _igcCommands) _mainCommands.Add(cmd.Key, cmd.Value);
            _mainCommands.Add("init", InitializeBlocks);
            _mainCommands.Add("IGC_Update", IgcUpdate);

            _config.Initialize(Me, GridTerminalSystem);
            _config.Load();
        }

        public void Save() {
        }
    }
}
