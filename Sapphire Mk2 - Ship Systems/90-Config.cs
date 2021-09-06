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

        readonly MyIni ProgramIni = new MyIni();
        int _configHashCode = 0;
        bool Flag_SaveConfig = false;

        const string SECTION_SDI = "SDI";
        //readonly MyIniKey Key_GridName = new MyIniKey(SECTION_SDI, "Grid Name");
        //readonly MyIniKey Key_TrainName = new MyIniKey(SECTION_SDI, "Train Name");

        readonly MyIniKey Key_IsEngine = new MyIniKey(SECTION_SDI, "IsEngine");
        readonly MyIniKey Key_GridId = new MyIniKey(SECTION_SDI, "Grid ID");

        string gridName;
        string trainName;

        bool isEngine;
        long gridId;

        void LoadConfig() {
            var tmpHashCode = Me.CustomData.GetHashCode();
            if (_configHashCode == tmpHashCode) return;
            _configHashCode = tmpHashCode;
            ProgramIni.Clear();
            if (!ProgramIni.TryParse(Me.CustomData)) ProgramIni.EndContent = Me.CustomData;

            gridName = ProgramIni.Add(SECTION_SDI, "Grid Name", "", " Name of the grid when not merged").ToString();
            trainName = ProgramIni.Add(SECTION_SDI, "Train Name", "", " Name of the grid when merged (Engine only)").ToString();
            isEngine = ProgramIni.Add(Key_IsEngine, false, " Set to true if this is an Engine grid.").ToBoolean();
            gridId = ProgramIni.Add(Key_GridId, Me.EntityId, " Unique ID for this grid").ToInt64();

            Flag_SaveConfig = true;

            var text = ProgramIni.ToString();
            _configHashCode = text.GetHashCode();
            Me.CustomData = text;
        }

        void SaveConfig() {
            if (!Flag_SaveConfig) return;
            //Ini.Set(KEY_WorldInvMulti, InventoryMultiplier);
            //Ini.Set(KEY_MaxCargoMass, MaxOperationalCargoMass?.ToString() ?? string.Empty);

            ProgramIni.Set(Key_IsEngine, isEngine);
            ProgramIni.Set(Key_GridId, gridId);

            var text = ProgramIni.ToString();
            _configHashCode = text.GetHashCode();
            Me.CustomData = text;
        }

    }
}
