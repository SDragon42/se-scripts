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
        public partial class Collect {

            public static bool IsOrientedForward(IMyTerminalBlock b) => b.Orientation.TransformDirectionInverse(b.Orientation.Forward) == Base6Directions.Direction.Forward;
            public static bool IsTagged(IMyTerminalBlock b, string tag) {
                if (tag == null || tag.Length == 0) return false;
                return b.CustomName.ToLower().Contains(tag.ToLower());
            }

            public static bool IsConnector(IMyTerminalBlock b) => b is IMyShipConnector;
            public static bool IsConnectorConnectable(IMyTerminalBlock b) => IsConnectorConnectable(b as IMyShipConnector);
            public static bool IsConnectorConnectable(IMyShipConnector b) => b?.Status == MyShipConnectorStatus.Connectable;
            public static bool IsConnectorConnected(IMyTerminalBlock b) => IsConnectorConnected(b as IMyShipConnector);
            public static bool IsConnectorConnected(IMyShipConnector b) => b?.Status == MyShipConnectorStatus.Connected;
            public static bool IsConnectorUnconnected(IMyTerminalBlock b) => IsConnectorUnconnected(b as IMyShipConnector);
            public static bool IsConnectorUnconnected(IMyShipConnector b) => b?.Status == MyShipConnectorStatus.Unconnected;

            public static bool IsDoor(IMyTerminalBlock b) => b is IMyDoor;
            public static bool IsBasicDoor(IMyTerminalBlock b) => !(IsSlidingDoor(b) || IsHangarDoor(b));
            public static bool IsHangarDoor(IMyTerminalBlock b) => b is IMyAirtightHangarDoor;
            public static bool IsSlidingDoor(IMyTerminalBlock b) => b is IMyAirtightSlideDoor;
            public static bool IsHumanDoor(IMyTerminalBlock b) => IsDoor(b) && !IsHangarDoor(b);

            public static bool IsGasTank(IMyTerminalBlock b) => b is IMyGasTank;
            public static bool IsOxygenTank(IMyTerminalBlock b) => IsGasTank(b) && !b.BlockDefinition.SubtypeId.Contains("Hydro");
            public static bool IsHydrogenTank(IMyTerminalBlock b) => IsGasTank(b) && b.BlockDefinition.SubtypeId.Contains("Hydro");

            public static bool IsLandingGear(IMyTerminalBlock b) => b is IMyLandingGear;
            public static bool IsLandingGearUnlocked(IMyTerminalBlock b) => IsLandingGearUnlocked(b as IMyLandingGear);
            public static bool IsLandingGearUnlocked(IMyLandingGear b) => (int)b?.LockMode == 0; //TODO: Unlocked - Workaround until this is fixed
            public static bool IsLandingGearReadyToLock(IMyTerminalBlock b) => IsLandingGearReadyToLock(b as IMyLandingGear);
            public static bool IsLandingGearReadyToLock(IMyLandingGear b) => (int)b?.LockMode == 1; //TODO: ReadyToLock - Workaround until this is fixed
            public static bool IsLandingGearLocked(IMyTerminalBlock b) => IsLandingGearLocked(b as IMyLandingGear);
            public static bool IsLandingGearLocked(IMyLandingGear b) => (int)b?.LockMode == 2; //TODO: Locked - Workaround until this is fixed

            public static bool IsTextPanel(IMyTerminalBlock b) => b is IMyTextPanel;
            public static bool IsSmTextPanel(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.TextPanelSM;
            public static bool IsLgTextPanel(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.TextPanelLG;
            public static bool IsLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmLcd(b) || IsLgLcd(b));
            public static bool IsSmLcd(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.LcdPanelSM;
            public static bool IsLgLcd(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.LcdPanelLG;
            public static bool IsWideLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmWideLcd(b) || IsLgWideLcd(b));
            public static bool IsSmWideLcd(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.WideLcdPanelSM;
            public static bool IsLgWideLcd(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.WideLcdPanelLG;
            public static bool IsAngledCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmAngledCornerLcd(b) || IsLgAngledCornerLcd(b));
            public static bool IsSmAngledCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (b.BlockDefinition.SubtypeId == SubTypeIDs.CornerLcdPanel1SM || b.BlockDefinition.SubtypeId == SubTypeIDs.CornerLcdPanel2SM);
            public static bool IsLgAngledCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (b.BlockDefinition.SubtypeId == SubTypeIDs.CornerLcdPanel1LG || b.BlockDefinition.SubtypeId == SubTypeIDs.CornerLcdPanel2LG);
            public static bool IsFlatCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmFlatCornerLcd(b) || IsLgFlatCornerLcd(b));
            public static bool IsSmFlatCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (b.BlockDefinition.SubtypeId == SubTypeIDs.FlatCornerLcdPanel1SM || b.BlockDefinition.SubtypeId == SubTypeIDs.FlatCornerLcdPanel2SM);
            public static bool IsLgFlatCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (b.BlockDefinition.SubtypeId == SubTypeIDs.FlatCornerLcdPanel1LG || b.BlockDefinition.SubtypeId == SubTypeIDs.FlatCornerLcdPanel2LG);
            public static bool IsCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsAngledCornerLcd(b) || IsFlatCornerLcd(b));
            public static bool IsSmCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmAngledCornerLcd(b) || IsSmFlatCornerLcd(b));
            public static bool IsLgCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsLgAngledCornerLcd(b) || IsLgFlatCornerLcd(b));

            public static bool IsThruster(IMyTerminalBlock b) => b is IMyThrust;
            public static bool IsThrusterIon(IMyTerminalBlock b) => IsThruster(b) && !IsThrusterHydrogen(b) && !IsThrusterAtmospheric(b);
            public static bool IsThrusterHydrogen(IMyTerminalBlock b) => IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Hydro");
            public static bool IsThrusterAtmospheric(IMyTerminalBlock b) => IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Atmo");

            public static bool IsCargoContainer(IMyTerminalBlock b) => b is IMyCargoContainer;
            public static bool IsSmallBlockSmallCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.SmBlock_SmContainer;
            public static bool IsSmallBlockMediumCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.SmBlock_MdContainer;
            public static bool IsSmallBlockLargeCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.SmBlock_LgContainer;
            public static bool IsLargeBlockSmallCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.LgBlock_SmContainer;
            public static bool IsLargeBlockLargeCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.LgBlock_LgContainer;
        }
        //static class Common {
        //    public static bool IsLessThen<T>(T value, T other) where T : IComparable { return value.CompareTo(other) < 0; }
        //    public static bool IsGreaterThen<T>(T value, T other) where T : IComparable { return value.CompareTo(other) > 0; }
        //}
    }
}
