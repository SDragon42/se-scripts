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
        //TestingBase _testingCode;
        readonly List<IMyTerminalBlock> _buffer = new List<IMyTerminalBlock>();
        readonly BlocksByOrientation _orientation;

        public Program()
        {
            //_testingCode = new BlockTypes();
            //_testingCode = new LCDTesting();
            //_testingCode = new CameraRanging();
            _orientation = new BlocksByOrientation();
        }

        public void Save()
        {
        }

        public void Main(string argument)
        {
            //_testingCode.Init(this);
            //_testingCode.Main(argument);

            GridTerminalSystem.GetBlocksOfType<IMyCockpit>(_buffer);
            var sc = _buffer.FirstOrDefault() as IMyShipController;
            _orientation.Init(sc);

            GridTerminalSystem.GetBlocksOfType(_cameras, _orientation.IsUp);
            Echo("UP");
            _cameras.ForEach(b => Echo(b.CustomName));
            Echo("");

            GridTerminalSystem.GetBlocksOfType(_cameras, _orientation.IsDown);
            Echo("Down");
            _cameras.ForEach(b => Echo(b.CustomName));
            Echo("");

            GridTerminalSystem.GetBlocksOfType(_cameras, _orientation.IsLeft);
            Echo("Left");
            _cameras.ForEach(b => Echo(b.CustomName));
            Echo("");

            GridTerminalSystem.GetBlocksOfType(_cameras, _orientation.IsRight);
            Echo("Right");
            _cameras.ForEach(b => Echo(b.CustomName));
        }


        readonly List<IMyCameraBlock> _cameras = new List<IMyCameraBlock>();



    }
}