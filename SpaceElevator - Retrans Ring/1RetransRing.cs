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
        //-------------------------------------------------------------------------------
        //  SCRIPT COMMANDS
        //-------------------------------------------------------------------------------
        const string CMD_DockCarriage = "dock";
        const string CMD_UndockCarriage = "undock";

        readonly CarriageVars _Maintenance;

        readonly DebugModule _debug;
        readonly LogModule _log;
        readonly COMMsModule _comms;
        readonly RunningSymbolModule _runSymbol;
        readonly TimeIntervalModule _executionInterval;
        readonly Queue<CommMessage> _messageQueue = new Queue<CommMessage>();

        public Program() {
            Echo = (t) => { }; // Disable Echo
            _debug = new DebugModule(this);
            //_debug.Enabled = false;
            _debug.EchoMessages = false;

            _log = new LogModule();

            //_custConfig = new CustomDataConfigModule();
            //_settings = new ScriptSettingsModule();
            //_settings.InitConfig(_custConfig);

            _Maintenance = new CarriageVars("Maint Carriage");
            if (!String.IsNullOrWhiteSpace(Storage)) {
                var gateStates = Storage.Split('\n');
                if (gateStates.Length == 5) {
                    _Maintenance.FromString(gateStates[4]);
                }
            }

            _lastCustomDataHash = -1;

            _runSymbol = new RunningSymbolModule();
            _executionInterval = new TimeIntervalModule(10);

            _comms = new COMMsModule(Me);
        }

        public void Save() {
        }

        public void Main(string argument) {
        }


        int _lastCustomDataHash;
        void LoadConfigSettings() {
            var hash = Me.CustomData.GetHashCode();
            if (hash == _lastCustomDataHash) return;
            //_custConfig.ReadFromCustomData(Me);
            //_settings.LoadFromSettingDict(_custConfig);
            //_custConfig.SaveToCustomData(Me);
            _lastCustomDataHash = hash;
        }
    }
}
