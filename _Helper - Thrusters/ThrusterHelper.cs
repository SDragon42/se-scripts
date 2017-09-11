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
        public static bool IsThruster(IMyTerminalBlock b) { return b is IMyThrust; }
        public static bool IsThrusterIon(IMyTerminalBlock b) { return (IsThruster(b) && !IsThrusterHydrogen(b) && !IsThrusterAtmospheric(b)); }
        public static bool IsThrusterHydrogen(IMyTerminalBlock b) { return (IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Hydro")); }
        public static bool IsThrusterAtmospheric(IMyTerminalBlock b) { return (IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Atmo")); }

        public static void SetThrusterOverride(IMyTerminalBlock thruster, double powerPercentage)
        {
            SetThrusterOverride(thruster as IMyThrust, Convert.ToSingle(powerPercentage));
        }
        public static void SetThrusterOverride(IMyTerminalBlock thruster, float powerPercentage)
        {
            SetThrusterOverride(thruster as IMyThrust, powerPercentage);
        }
        public static void SetThrusterOverride(IMyThrust thruster, double powerPercentage)
        {
            SetThrusterOverride(thruster, Convert.ToSingle(powerPercentage));
        }
        public static void SetThrusterOverride(IMyThrust thruster, float powerPercentage)
        {
            if (thruster == null) return;
            powerPercentage = MathHelper.Clamp(powerPercentage, 0f, 100f);
            thruster.SetValue("Override", powerPercentage);
        }

        public static void SetThrusterOverride2All(List<IMyTerminalBlock> thrusterList, double powerPercentage)
        {
            SetThrusterOverride2All(thrusterList, Convert.ToSingle(powerPercentage));
        }
        public static void SetThrusterOverride2All(List<IMyTerminalBlock> thrusterList, float powerPercentage)
        {
            for (var i = 0; i < thrusterList.Count; i++)
            {
                var thruster = thrusterList[i] as IMyThrust;
                if (thruster != null)
                    SetThrusterOverride(thruster, powerPercentage);
            }
        }
        public static void SetThrusterOverride2All(List<IMyThrust> thrusterList, double powerPercentage)
        {
            SetThrusterOverride2All(thrusterList, Convert.ToSingle(powerPercentage));
        }
        public static void SetThrusterOverride2All(List<IMyThrust> thrusterList, float powerPercentage)
        {
            foreach (var thruster in thrusterList)
                SetThrusterOverride(thruster, powerPercentage);
        }

        public static float GetThrusterOveridePercentage(IMyTerminalBlock thruster)
        {
            return GetThrusterOveridePercentage(thruster as IMyThrust);
        }
        public static float GetThrusterOveridePercentage(IMyThrust thruster)
        {
            if (thruster == null) return 0f;
            var maxThrust = GetMaxThrust(thruster);
            return (maxThrust > 0)
                ? (thruster.ThrustOverride / maxThrust) * 100f
                : 0f;
        }


        public static void AdjustThrottle(
                List<IMyTerminalBlock> thrusterList,
                double lowerSpeedTarget, double upperSpeedTarget, double currSpeed, double currAccel,
                float overrideAjustAmount, float currOverride)
        {
            var newOverride = currOverride;
            if (currSpeed < lowerSpeedTarget) newOverride = newOverride + overrideAjustAmount;
            if (currSpeed > upperSpeedTarget && currAccel > 0) newOverride = newOverride - overrideAjustAmount;
            if (newOverride > 100) newOverride = 100;
            if (newOverride < 0) newOverride = 0;
            if (newOverride != currOverride) SetThrusterOverride2All(thrusterList, newOverride);
        }


        public static double ConvertMass2Newtons(int mass_kg) { return (mass_kg / 0.101971621); }

        //static readonly Matrix __ThrusterIdentityMatrix = new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        //public static Vector3 GetDirectionVector(int direction)
        //{
        //    switch (direction)
        //    {
        //        case DirectionConst.Forward: return __ThrusterIdentityMatrix.Forward;
        //        case DirectionConst.Backward: return __ThrusterIdentityMatrix.Backward;
        //        case DirectionConst.Up: return __ThrusterIdentityMatrix.Up;
        //        case DirectionConst.Down: return __ThrusterIdentityMatrix.Down;
        //        case DirectionConst.Left: return __ThrusterIdentityMatrix.Left;
        //        case DirectionConst.Right: return __ThrusterIdentityMatrix.Right;
        //        default: return Vector3.Invalid;
        //    }
        //}

        //public static void GetThrustersInDirection(IMyGridTerminalSystem gts, IMyShipController sc, List<IMyTerminalBlock> thrusters, int direction, Func<IMyTerminalBlock, bool> collect = null)
        //{
        //    if (sc == null || thrusters == null) return;
        //    var dirVector = GetDirectionVector(direction);
        //    if (dirVector == Vector3.Invalid) return;

        //    Matrix scMatrix;
        //    sc.Orientation.GetMatrix(out scMatrix);
        //    Matrix.Transpose(ref scMatrix, out scMatrix);

        //    gts.GetBlocksOfType<IMyThrust>(thrusters, b =>
        //    {
        //        if (collect != null && !collect(b)) return false;
        //        Matrix thrusterMatrix;
        //        b.Orientation.GetMatrix(out thrusterMatrix);
        //        var accelDir = Vector3.Transform(thrusterMatrix.Backward, scMatrix);
        //        return (accelDir == dirVector);
        //    });
        //}
        //public static void GetThrustersInDirection(IMyGridTerminalSystem gts, IMyShipController sc, List<IMyThrust> thrusters, int direction, Func<IMyTerminalBlock, bool> collect = null)
        //{
        //    if (sc == null || thrusters == null) return;
        //    var dirVector = GetDirectionVector(direction);
        //    if (dirVector == Vector3.Invalid) return;

        //    Matrix scMatrix;
        //    sc.Orientation.GetMatrix(out scMatrix);
        //    Matrix.Transpose(ref scMatrix, out scMatrix);

        //    gts.GetBlocksOfType(thrusters, b =>
        //    {
        //        if (collect != null && !collect(b)) return false;
        //        Matrix thrusterMatrix;
        //        b.Orientation.GetMatrix(out thrusterMatrix);
        //        var accelDir = Vector3.Transform(thrusterMatrix.Backward, scMatrix);
        //        return (accelDir == dirVector);
        //    });
        //}
        //public static List<IMyThrust> GetThrustersInDirection(IMyGridTerminalSystem gts, IMyShipController sc, int direction, Func<IMyTerminalBlock, bool> collect = null)
        //{
        //    var thrusters = new List<IMyThrust>();
        //    GetThrustersInDirection(gts, sc, thrusters, direction, collect);
        //    return thrusters;
        //}


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
