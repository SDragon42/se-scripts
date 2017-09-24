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
    /// <summary>
    /// </summary>
    /// <remarks>
    /// Code here is taken from "Wico craft controller" by Wicorel and modified for my use.
    /// http://steamcommunity.com/sharedfiles/filedetails/?id=571364878
    /// </remarks>
    static class ThrusterHelper
    {
        public static void SetThrusterOverride(IMyTerminalBlock thruster, double powerPercentage) { SetThrusterOverride(thruster as IMyThrust, Convert.ToSingle(powerPercentage)); }
        public static void SetThrusterOverride(IMyTerminalBlock thruster, float powerPercentage) { SetThrusterOverride(thruster as IMyThrust, powerPercentage); }
        public static void SetThrusterOverride(IMyThrust thruster, double powerPercentage) { SetThrusterOverride(thruster, Convert.ToSingle(powerPercentage)); }
        public static void SetThrusterOverride(IMyThrust thruster, float powerPercentage)
        {
            if (thruster == null) return;
            powerPercentage = MathHelper.Clamp(powerPercentage, 0f, 100f);
            thruster.SetValue("Override", powerPercentage);
        }

        public static float GetThrusterOveridePercentage(IMyTerminalBlock thruster) { return GetThrusterOveridePercentage(thruster as IMyThrust); }
        public static float GetThrusterOveridePercentage(IMyThrust thruster)
        {
            if (thruster == null) return 0f;
            var maxThrust = GetMaxThrust(thruster);
            return (maxThrust > 0)
                ? (thruster.ThrustOverride / maxThrust) * 100f
                : 0f;
        }


        public static double ConvertMass2Newtons(int mass_kg) { return (mass_kg / 0.101971621); }

        public static float GetMaxThrust(IMyTerminalBlock b) { return GetMaxThrust(b as IMyThrust); }
        public static float GetMaxThrust(IMyThrust thruster)
        {
            if (thruster == null) return 0f;
            if (!IsThrusterWorking(thruster)) return 0f;
            return thruster.MaxThrust;
        }

        public static float GetMaxEffectiveThrust(IMyTerminalBlock b) { return GetMaxEffectiveThrust(b as IMyThrust); }
        public static float GetMaxEffectiveThrust(IMyThrust thruster)
        {
            if (thruster == null) return 0f;
            if (!IsThrusterWorking(thruster)) return 0f;
            return thruster.MaxEffectiveThrust;
        }

        public static float GetCurrentThrust(IMyTerminalBlock b) { return GetCurrentThrust(b as IMyThrust); }
        public static float GetCurrentThrust(IMyThrust thruster)
        {
            if (thruster == null) return 0f;
            if (!IsThrusterWorking(thruster)) return 0f;
            return thruster.CurrentThrust;
        }

        public static bool IsThrusterWorking(IMyTerminalBlock thruster) { return IsThrusterWorking(thruster as IMyThrust); }
        public static bool IsThrusterWorking(IMyThrust thruster)
        {
            if (thruster == null) return false;
            if (!thruster.Enabled) return false;
            return !(thruster.ThrustOverride != 0 && thruster.CurrentThrust == 0);
        }

    }
}
