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

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        const string CMD_DOCK = "dock";
        const string CMD_UNDOCK = "undock";

        readonly RunningSymbolModule _runSymbol;
        readonly TimeIntervalModule _executionInterval;
        readonly DockSecureModule _dockSecure;
        readonly ScriptSettings _settings = new ScriptSettings();

        public Program()
        {
            //Echo = (t) => { }; // Disable Echo

            _runSymbol = new RunningSymbolModule();
            _executionInterval = new TimeIntervalModule();
            _dockSecure = new DockSecureModule();

            _settings.InitConfig(Me, _dockSecure, SetExecutionInterval);
        }

        public void Main(string argument)
        {
            Echo("Dock-Secure " + _runSymbol.GetSymbol(this.Runtime));
            _executionInterval.RecordTime(this.Runtime);

            if (!_executionInterval.AtNextInterval() && argument?.Length == 0) return;

            _settings.LoadConfig(Me, _dockSecure, SetExecutionInterval);

            _dockSecure.Init(this);

            if (argument?.Length > 0)
            {
                switch (argument.ToLower())
                {
                    case CMD_DOCK: _dockSecure.Dock(); return;
                    case CMD_UNDOCK: _dockSecure.UnDock(); return;
                    default: return;
                }
            }

            _dockSecure.AutoDockUndock();
        }

        void SetExecutionInterval()
        {
            _executionInterval.SetNumIntervalsPerSecond(_settings.RunInterval);
        }
    }
}