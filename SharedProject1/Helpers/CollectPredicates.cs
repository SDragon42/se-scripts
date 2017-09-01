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
    static class CollectPredicates
    {
        public static bool IsOnSameGrid(IMyTerminalBlock sourceBlock, IMyTerminalBlock b) { return (b != null) ? (sourceBlock.CubeGrid == b.CubeGrid) : false; }

        public static bool IsAirVent(IMyTerminalBlock b) { return b is IMyAirVent; }
        public static bool IsArtificialMass(IMyTerminalBlock b) { return b is IMyArtificialMassBlock; }
        public static bool IsBeacon(IMyTerminalBlock b) { return b is IMyBeacon; }
        public static bool IsRadioAntenna(IMyTerminalBlock b) { return b is IMyRadioAntenna; }
        public static bool IsShipMerge(IMyTerminalBlock b) { return b is IMyShipMergeBlock; }
        public static bool IsOrientedForward(IMyTerminalBlock b)
        {
            if (b == null)
                return false;
            var i = b.Orientation.TransformDirectionInverse(b.Orientation.Forward);
            var j = VRageMath.Base6Directions.Direction.Forward;
            return (i == j);
        }

        public static bool IsConnector(IMyTerminalBlock b) { return b is IMyShipConnector; }
        public static bool IsConnectorConnectable(IMyTerminalBlock b) { return IsConnectorConnectable(b as IMyShipConnector); }
        public static bool IsConnectorConnectable(IMyShipConnector b)
        {
            if (b == null) return false;
            return (b.Status == MyShipConnectorStatus.Connectable);
        }
        public static bool IsConnectorConnected(IMyTerminalBlock b) { return IsConnectorConnected(b as IMyShipConnector); }
        public static bool IsConnectorConnected(IMyShipConnector b)
        {
            if (b == null) return false;
            return (b.Status == MyShipConnectorStatus.Connected);
        }
        public static bool IsConnectorUnconnected(IMyTerminalBlock b) { return IsConnectorUnconnected(b as IMyShipConnector); }
        public static bool IsConnectorUnconnected(IMyShipConnector b)
        {
            if (b == null) return false;
            return (b.Status == MyShipConnectorStatus.Unconnected);
        }

        public static bool IsDoor(IMyTerminalBlock b) { return b is IMyDoor; }
        public static bool IsBasicDoor_depricated(IMyTerminalBlock b)
        {
            return (IsDoor(b) && b.BlockDefinition.TypeIdString.Contains("_Door"));
        }
        public static bool IsBasicDoor(IMyTerminalBlock b)
        {
            if (b is IMyAirtightSlideDoor) return false;
            if (b is IMyAirtightHangarDoor) return false;
            return true;
        }
        public static bool IsHangarDoor_depricated(IMyTerminalBlock b)
        {
            return (IsDoor(b) && b.BlockDefinition.TypeIdString.Contains("_AirtightHangarDoor"));
        }
        public static bool IsHangarDoor(IMyTerminalBlock b) { return (b is IMyAirtightHangarDoor); }
        public static bool IsSlidingDoor_depricated(IMyTerminalBlock b)
        {
            return (IsDoor(b) && b.BlockDefinition.TypeIdString.Contains("_AirtightSlideDoor"));
        }
        public static bool IsSlidingDoor(IMyTerminalBlock b) { return (b is IMyAirtightSlideDoor); }
        public static bool IsHumanDoor(IMyTerminalBlock b) { return (IsDoor(b) && !IsHangarDoor(b)); }

        public static bool IsLandingGear(IMyTerminalBlock b) { return b is IMyLandingGear; }
        public static bool IsLandingGearUnlocked(IMyTerminalBlock b) { return IsLandingGearUnlocked(b as IMyLandingGear); }
        public static bool IsLandingGearUnlocked(IMyLandingGear b)
        {
            if (b == null) return false;
            return ((int)b.LockMode == 0); //TODO: Unlocked - Workaround this this is fixed
        }
        public static bool IsLandingGearReadyToLock(IMyTerminalBlock b) { return IsLandingGearReadyToLock(b as IMyLandingGear); }
        public static bool IsLandingGearReadyToLock(IMyLandingGear b)
        {
            if (b == null) return false;
            return ((int)b.LockMode == 1); //TODO: ReadyToLock - Workaround this this is fixed
        }
        public static bool IsLandingGearLocked(IMyTerminalBlock b) { return IsLandingGearLocked(b as IMyLandingGear); }
        public static bool IsLandingGearLocked(IMyLandingGear b)
        {
            if (b == null) return false;
            return ((int)b.LockMode == 2); //TODO: Locked - Workaround this this is fixed
        }

        public static bool IsCargoContainer(IMyTerminalBlock b) { return b is IMyCargoContainer; }

        //public static bool IsGasTank(IMyTerminalBlock b) { return b is IMyGasTank; }
        //public static bool IsOxygenTank(IMyTerminalBlock b) { return (IsGasTank(b) && !b.BlockDefinition.SubtypeId.Contains("Hydro")); }
        //public static bool IsHydrogenTank(IMyTerminalBlock b) { return (IsGasTank(b) && b.BlockDefinition.SubtypeId.Contains("Hydro")); }

        //public static bool IsThruster(IMyTerminalBlock b) { return b is IMyThrust; }
        //public static bool IsThrusterIon(IMyTerminalBlock b) { return (IsThruster(b) && !IsThrusterHydrogen(b) && !IsThrusterAtmospheric(b)); }
        //public static bool IsThrusterHydrogen(IMyTerminalBlock b) { return (IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Hydro")); }
        //public static bool IsThrusterAtmospheric(IMyTerminalBlock b) { return (IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Atmo")); }

        public static bool IsGyro(IMyTerminalBlock b) { return b is IMyGyro; }
        public static bool IsInteriorLight(IMyTerminalBlock b) { return b is IMyInteriorLight; }
        public static bool IsSensor(IMyTerminalBlock b) { return b is IMySensorBlock; }
        public static bool IsOreDetector(IMyTerminalBlock b) { return b is IMyOreDetector; }
    }
}
