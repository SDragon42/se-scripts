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
        readonly List<IMyTerminalBlock> _tmp = new List<IMyTerminalBlock>();

        readonly Dictionary<string, Action> _renameMethods = new Dictionary<string, Action>();
        //readonly Dictionary<string, string> _basicBlockNames = new Dictionary<string, string>();

        public Program()
        {
            _renameMethods.Add("basic", BasicRename);
            _renameMethods.Add("test", TestMethod);
        }

        public void Main(string argument)
        {
            Echo(argument);
            var key = argument.ToLower();
            if (_renameMethods.ContainsKey(key))
                _renameMethods[key]?.Invoke();
        }

        void BasicRename()
        {
            Echo("BasicRename()");
            GridTerminalSystem.GetBlocks(_tmp);
            _tmp.ForEach(b => b.ShowInTerminal = true);

            GridTerminalSystem.GetBlocksOfType(_tmp, b => BasicBlockNames.GetName(b).Length > 0);
            //GridTerminalSystem.GetBlocksOfType(_tmp);
            Echo($"Num Blocks: {_tmp.Count:N0}");
            _tmp.ForEach(b =>
            {
                //if (b.Name == null) return;
                //Echo($"{b.CustomName} -- {b.Name ?? "NULL"}");
                //b.CustomName = b.Name;
                //b.CustomName = b?.Name ?? string.Empty;
                b.ShowOnHUD = false;
                b.CustomName = BasicBlockNames.GetName(b);
            });

            //// Un-Numbered Blocks
            //GridTerminalSystem.GetBlocksOfType<IMyThrust>(_tmp, b => b.BlockDefinition.SubtypeId.EndsWith("LargeThrust"));
            //RenameMethods.RenameTo(_tmp, "Lg Ion Thruster");
            //_tmp.ForEach(NoTerminalNoToolbar);
            //GridTerminalSystem.GetBlocksOfType<IMyThrust>(_tmp, b => b.BlockDefinition.SubtypeId.EndsWith("SmallThrust"));
            //RenameMethods.RenameTo(_tmp, "Sm Ion Thruster");
            //_tmp.ForEach(NoTerminalNoToolbar);

            //GridTerminalSystem.GetBlocksOfType<IMyThrust>(_tmp, b => b.BlockDefinition.SubtypeId.EndsWith("LargeHydrogenThrust"));
            //RenameMethods.RenameTo(_tmp, "Lg Hydrogen Thruster");
            //_tmp.ForEach(NoTerminalNoToolbar);
            //GridTerminalSystem.GetBlocksOfType<IMyThrust>(_tmp, b => b.BlockDefinition.SubtypeId.EndsWith("SmallHydrogenThrust"));
            //RenameMethods.RenameTo(_tmp, "Sm Hydrogen Thruster");
            //_tmp.ForEach(NoTerminalNoToolbar);

            //GridTerminalSystem.GetBlocksOfType<IMyThrust>(_tmp, b => b.BlockDefinition.SubtypeId.EndsWith("LargeAtmosphericThrust"));
            //RenameMethods.RenameTo(_tmp, "Lg Atmo Thruster");
            //_tmp.ForEach(NoTerminalNoToolbar);
            //GridTerminalSystem.GetBlocksOfType<IMyThrust>(_tmp, b => b.BlockDefinition.SubtypeId.EndsWith("SmallAtmosphericThrust"));
            //RenameMethods.RenameTo(_tmp, "Sm Atmo Thruster");
            //_tmp.ForEach(NoTerminalNoToolbar);




            //// Numbered Blocks
            //GridTerminalSystem.GetBlocksOfType<IMyReactor>(_tmp, b => b.BlockDefinition.SubtypeId.EndsWith("LargeGenerator"));
            //RenameMethods.NumberRenameTo(_tmp, "Lg Reactor");
            //RenameMethods.SuffixWith(_tmp, " [TIM Uranium:P1:10]");
            //_tmp.ForEach(NoTerminalNoToolbar);
            //GridTerminalSystem.GetBlocksOfType<IMyReactor>(_tmp, b => b.BlockDefinition.SubtypeId.EndsWith("SmallGenerator"));
            //RenameMethods.NumberRenameTo(_tmp, "Sm Reactor");
            //RenameMethods.SuffixWith(_tmp, " [TIM Uranium:P1:2]");
            //_tmp.ForEach(NoTerminalNoToolbar);

            //GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(_tmp);
            //RenameMethods.NumberRenameTo(_tmp, "Battery");
            //_tmp.ForEach(NoTerminalNoToolbar);

            //GridTerminalSystem.GetBlocksOfType<IMyAirtightHangarDoor>(_tmp);
            //RenameMethods.NumberRenameTo(_tmp, "Hangar Door");
            //_tmp.ForEach(NoTerminalNoToolbar);

            //GridTerminalSystem.GetBlocksOfType<IMyGyro>(_tmp);
            //RenameMethods.NumberRenameTo(_tmp, "Gyroscope");
            //_tmp.ForEach(NoTerminalNoToolbar);

            //GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(_tmp, b => !IsNavLight(b));
            //RenameMethods.NumberRenameTo(_tmp, "Light");
            //_tmp.ForEach(b => { NoTerminalNoToolbar(b); SetInteriorLight(b); });

            //GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(_tmp, IsNavLight);
            //_tmp.ForEach(b =>
            //{
            //    NoTerminalNoToolbar(b);
            //    if (IsNavLightPort(b)) SetNavLightPort(b);
            //    else if (IsNavLightStarboard(b)) SetNavLightStarboard(b);
            //    else if (IsNavLightTop(b)) SetNavLightTop(b);
            //    else if (IsNavLightBottom(b)) SetNavLightBottom(b);
            //});

            //GridTerminalSystem.GetBlocksOfType<IMyReflectorLight>(_tmp);
            //RenameMethods.NumberRenameTo(_tmp, "Spotlight");
            //_tmp.ForEach(b => { NoTerminalNoToolbar(b); SetSpotlight(b); });

            //GridTerminalSystem.GetBlocksOfType<IMyLandingGear>(_tmp);
            //RenameMethods.NumberRenameTo(_tmp, "Landing Gear");
            //_tmp.ForEach(b => { NoTerminalNoToolbar(b); ((IMyLandingGear)b).AutoLock = false; });

            //GridTerminalSystem.GetBlocksOfType<IMyAirVent>(_tmp);
            //RenameMethods.NumberRenameTo(_tmp, "Air Vent");
            //_tmp.ForEach(NoTerminalNoToolbar);

            //GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(_tmp);
            //RenameMethods.NumberRenameTo(_tmp, "Drill");
            //_tmp.ForEach(NoTerminalNoToolbar);

            //GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(_tmp);
            //RenameMethods.NumberRenameTo(_tmp, "Drill");
            //_tmp.ForEach(NoTerminalNoToolbar);
        }

        void NoTerminalNoToolbar(IMyTerminalBlock block)
        {
            block.ShowInTerminal = false;
            block.ShowInToolbarConfig = false;
        }
        void SetInteriorLight(IMyTerminalBlock b)
        {
            var light = b as IMyInteriorLight;
            if (light == null) return;
            light.Color = new Color(100, 100, 100);
            light.Radius = light.GetMaximum<float>("Radius");
            //light.Intensity = 
            //light.Falloff = 
            light.BlinkIntervalSeconds = 0f;
            light.BlinkLength = 0.1f;
            light.BlinkOffset = 0f;
        }

        bool IsNavLight(IMyTerminalBlock b)
        {
            if (!(b is IMyInteriorLight)) return false;
            return (b.CustomName.ToLower().Contains("nav light"));
        }
        bool IsNavLightPort(IMyTerminalBlock b)
        {
            if (!IsNavLight(b)) return false;
            return (b.CustomName.ToLower().Contains("- port"));
        }
        bool IsNavLightStarboard(IMyTerminalBlock b)
        {
            if (!IsNavLight(b)) return false;
            return (b.CustomName.ToLower().Contains("- starboard"));
        }
        bool IsNavLightTop(IMyTerminalBlock b)
        {
            if (!IsNavLight(b)) return false;
            return (b.CustomName.ToLower().Contains("- top"));
        }
        bool IsNavLightBottom(IMyTerminalBlock b)
        {
            if (!IsNavLight(b)) return false;
            return (b.CustomName.ToLower().Contains("- bottom"));
        }

        void SetNavLightPort(IMyTerminalBlock b)
        {
            var light = b as IMyInteriorLight;
            if (light == null) return;
            SetNavLight_Core(light);
            light.Color = new Color(150, 0, 0);
            light.BlinkIntervalSeconds = 1.5f;
            light.BlinkLength = 75f;
        }
        void SetNavLightStarboard(IMyTerminalBlock b)
        {
            SetNavLightPort(b);
            var light = b as IMyInteriorLight;
            if (light == null) return;
            light.Color = new Color(0, 150, 0);
        }
        void SetNavLightTop(IMyTerminalBlock b)
        {
            var light = b as IMyInteriorLight;
            if (light == null) return;
            SetNavLight_Core(light);
            light.Color = new Color(150, 150, 150);
            light.BlinkIntervalSeconds = 1f;
            light.BlinkLength = 10f;
        }
        void SetNavLightBottom(IMyTerminalBlock b)
        {
            SetNavLightTop(b);
            var light = b as IMyInteriorLight;
            if (light == null) return;
            light.Color = new Color(150, 150, 0);
        }
        void SetNavLight_Core(IMyInteriorLight light)
        {
            light.Intensity = light.GetMaximum<float>("Intensity");
            light.BlinkOffset = 0f;
            if (light.BlockDefinition.SubtypeId.StartsWith("LargeBlock"))
            {

            }
            else
            {
                light.Radius = 2f;
                light.Intensity = 1.5f;
            }
        }

        void SetSpotlight(IMyTerminalBlock b)
        {
            var light = b as IMyReflectorLight;
            if (light == null) return;
            light.Color = new Color(100, 100, 100);
            light.Radius = light.GetMaximum<float>("Radius");
        }


        //static void InitBlock(List<IMyTerminalBlock> blocks, Func<List<IMyTerminalBlock>, string, int> renameMethod)
        //{
        //    renameMethod?.Invoke(blocks, )
        //}


        //bool IsThrusterIon(IMyTerminalBlock b) { return (b is IMyThrust && !IsThrusterHydrogen(b) && !IsThrusterAtmospheric(b)); }
        //bool IsThrusterHydrogen(IMyTerminalBlock b) { return (b is IMyThrust && b.BlockDefinition.SubtypeId.Contains("Hydro")); }
        //bool IsThrusterAtmospheric(IMyTerminalBlock b) { return (b is IMyThrust && b.BlockDefinition.SubtypeId.Contains("Atmo")); }





        void TestMethod()
        {
            GridTerminalSystem.GetBlocks(_tmp);
            _tmp.ForEach(b =>
            {
                Echo(b.CustomName);
                Echo(b.BlockDefinition.SubtypeId);
                Echo("");
            });
        }
    }
}