// <mdk sortorder="900" />
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

        // Common Predicate Methods
        static bool IsSameGrid(IMyTerminalBlock a, IMyTerminalBlock b) => a.CubeGrid == b.CubeGrid;
        bool IsOnThisGrid(IMyTerminalBlock b) => IsSameGrid(Me, b);

        static bool IsOrientedForward(IMyTerminalBlock b) => b.Orientation.TransformDirectionInverse(b.Orientation.Forward) == Base6Directions.Direction.Forward;
        // static bool IsTagged(IMyTerminalBlock b, string tag) => b.CustomName.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0;
        static bool IsTagged(IMyTerminalBlock b, string tag) => tag.Length == 0 || b.CustomName.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0;
        static bool IsTaggedAny(IMyTerminalBlock b, params string[] tags) => tags.Any(t => IsTagged(b, t));

        static bool IsConnector(IMyTerminalBlock b) => b is IMyShipConnector;
        static bool IsConnectorConnectable(IMyTerminalBlock b) => IsConnectorConnectable(b as IMyShipConnector);
        static bool IsConnectorConnectable(IMyShipConnector b) => b?.Status == MyShipConnectorStatus.Connectable;
        static bool IsConnectorConnected(IMyTerminalBlock b) => IsConnectorConnected(b as IMyShipConnector);
        static bool IsConnectorConnected(IMyShipConnector b) => b?.Status == MyShipConnectorStatus.Connected;
        static bool IsConnectorUnconnected(IMyTerminalBlock b) => IsConnectorUnconnected(b as IMyShipConnector);
        static bool IsConnectorUnconnected(IMyShipConnector b) => b?.Status == MyShipConnectorStatus.Unconnected;

        static bool IsDoor(IMyTerminalBlock b) => b is IMyDoor;
        static bool IsBasicDoor(IMyTerminalBlock b) => !(IsSlidingDoor(b) || IsHangarDoor(b));
        static bool IsHangarDoor(IMyTerminalBlock b) => b is IMyAirtightHangarDoor;
        static bool IsSlidingDoor(IMyTerminalBlock b) => b is IMyAirtightSlideDoor;
        static bool IsHumanDoor(IMyTerminalBlock b) => IsDoor(b) && !IsHangarDoor(b);

        static bool IsGasTank(IMyTerminalBlock b) => b is IMyGasTank;
        static bool IsOxygenTank(IMyTerminalBlock b) => IsGasTank(b) && !b.BlockDefinition.SubtypeId.Contains("Hydro");
        static bool IsHydrogenTank(IMyTerminalBlock b) => IsGasTank(b) && b.BlockDefinition.SubtypeId.Contains("Hydro");

        static bool IsLandingGear(IMyTerminalBlock b) => b is IMyLandingGear;
        static bool IsLandingGearUnlocked(IMyTerminalBlock b) => IsLandingGearUnlocked(b as IMyLandingGear);
        static bool IsLandingGearUnlocked(IMyLandingGear b) => b.LockMode == LandingGearMode.Unlocked;
        static bool IsLandingGearReadyToLock(IMyTerminalBlock b) => IsLandingGearReadyToLock(b as IMyLandingGear);
        static bool IsLandingGearReadyToLock(IMyLandingGear b) => b.LockMode == LandingGearMode.ReadyToLock;
        static bool IsLandingGearLocked(IMyTerminalBlock b) => IsLandingGearLocked(b as IMyLandingGear);
        static bool IsLandingGearLocked(IMyLandingGear b) => b.LockMode == LandingGearMode.Locked;

        static bool IsTextPanel(IMyTerminalBlock b) => b is IMyTextPanel;
        static bool IsSmTextPanel(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.TextPanelSM;
        static bool IsLgTextPanel(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.TextPanelLG;
        static bool IsLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmLcd(b) || IsLgLcd(b));
        static bool IsSmLcd(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.LcdPanelSM;
        static bool IsLgLcd(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.LcdPanelLG;
        static bool IsWideLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmWideLcd(b) || IsLgWideLcd(b));
        static bool IsSmWideLcd(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.WideLcdPanelSM;
        static bool IsLgWideLcd(IMyTerminalBlock b) => IsTextPanel(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.WideLcdPanelLG;
        static bool IsAngledCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmAngledCornerLcd(b) || IsLgAngledCornerLcd(b));
        static bool IsSmAngledCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (b.BlockDefinition.SubtypeId == SubTypeIDs.CornerLcdPanel1SM || b.BlockDefinition.SubtypeId == SubTypeIDs.CornerLcdPanel2SM);
        static bool IsLgAngledCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (b.BlockDefinition.SubtypeId == SubTypeIDs.CornerLcdPanel1LG || b.BlockDefinition.SubtypeId == SubTypeIDs.CornerLcdPanel2LG);
        static bool IsFlatCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmFlatCornerLcd(b) || IsLgFlatCornerLcd(b));
        static bool IsSmFlatCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (b.BlockDefinition.SubtypeId == SubTypeIDs.FlatCornerLcdPanel1SM || b.BlockDefinition.SubtypeId == SubTypeIDs.FlatCornerLcdPanel2SM);
        static bool IsLgFlatCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (b.BlockDefinition.SubtypeId == SubTypeIDs.FlatCornerLcdPanel1LG || b.BlockDefinition.SubtypeId == SubTypeIDs.FlatCornerLcdPanel2LG);
        static bool IsCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsAngledCornerLcd(b) || IsFlatCornerLcd(b));
        static bool IsSmCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsSmAngledCornerLcd(b) || IsSmFlatCornerLcd(b));
        static bool IsLgCornerLcd(IMyTerminalBlock b) => IsTextPanel(b) && (IsLgAngledCornerLcd(b) || IsLgFlatCornerLcd(b));

        static bool IsThruster(IMyTerminalBlock b) => b is IMyThrust;
        static bool IsThrusterIon(IMyTerminalBlock b) => IsThruster(b) && !IsThrusterHydrogen(b) && !IsThrusterAtmospheric(b);
        static bool IsThrusterHydrogen(IMyTerminalBlock b) => IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Hydro");
        static bool IsThrusterAtmospheric(IMyTerminalBlock b) => IsThruster(b) && b.BlockDefinition.SubtypeId.Contains("Atmo");

        static bool IsCargoContainer(IMyTerminalBlock b) => b is IMyCargoContainer;
        static bool IsSmallBlockSmallCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.SmBlock_SmContainer;
        static bool IsSmallBlockMediumCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.SmBlock_MdContainer;
        static bool IsSmallBlockLargeCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.SmBlock_LgContainer;
        static bool IsLargeBlockSmallCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.LgBlock_SmContainer;
        static bool IsLargeBlockLargeCargoContainer(IMyTerminalBlock b) => IsCargoContainer(b) && b.BlockDefinition.SubtypeId == SubTypeIDs.LgBlock_LgContainer;

    }
}
