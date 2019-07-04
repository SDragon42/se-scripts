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
        class BasicBlockNames {

            Dictionary<string, string> NameDictionary = new Dictionary<string, string>();
            public BasicBlockNames() {

                var d = NameDictionary;
                d.Add("AirtightHangarDoor:", "Hangar Door");
                d.Add("AirtightSlideDoor:LargeBlockSlideDoor", "Door");
                d.Add("AirVent:", "Air Vent");
                d.Add("AirVent:SmallAirVent", "Air Vent");
                d.Add("Assembler:LargeAssembler", "Assembler");
                d.Add("BatteryBlock:LargeBlockBatteryBlock", "Battery");
                d.Add("BatteryBlock:SmallBlockBatteryBlock", "Battery");
                d.Add("BatteryBlock:SmallBlockSmallBatteryBlock", "Sm Battery");
                d.Add("Beacon:LargeBlockBeacon", "Beacon");
                d.Add("Beacon:SmallBlockBeacon", "Beacon");
                d.Add("ButtonPanel:ButtonPanelLarge", "Button Panel");
                d.Add("ButtonPanel:ButtonPanelSmall", "Button Panel");
                d.Add("CameraBlock:LargeCameraBlock", "Camera");
                d.Add("CameraBlock:SmallCameraBlock", "Camera");
                d.Add("CargoContainer:LargeBlockLargeContainer", "Lg Cargo");
                d.Add("CargoContainer:LargeBlockSmallContainer", "Sm Cargo");
                d.Add("CargoContainer:SmallBlockLargeContainer", "Lg Cargo");
                d.Add("CargoContainer:SmallBlockMediumContainer", "Med Cargo");
                d.Add("CargoContainer:SmallBlockSmallContainer", "Sm Cargo");
                d.Add("Cockpit:CockpitOpen", "Flight Seat");
                d.Add("Cockpit:DBSmallBlockFighterCockpit", "Fighter Cockpit");
                d.Add("Cockpit:LargeBlockCockpit", "Control Station");
                d.Add("Cockpit:LargeBlockCockpitSeat", "Cockpit");
                d.Add("Cockpit:PassengerSeatLarge", "Passenger Seat");
                d.Add("Cockpit:PassengerSeatSmall", "Passenger Seat");
                d.Add("Cockpit:SmallBlockCockpit", "Cockpit");
                d.Add("Collector:Collector", "Collector");
                d.Add("Collector:CollectorSmall", "Collector");
                d.Add("ConveyorSorter:LargeBlockConveyorSorter", "Sorter");
                d.Add("ConveyorSorter:MediumBlockConveyorSorter", "Sorter");
                d.Add("ConveyorSorter:SmallBlockConveyorSorter", "Sorter");
                d.Add("CryoChamber:LargeBlockCryoChamber", "Cryo Chamber");
                d.Add("Decoy:LargeDecoy", "Decoy");
                d.Add("Decoy:SmallDecoy", "Decoy");
                d.Add("Door:", "Door");
                d.Add("Drill:LargeBlockDrill", "Drill");
                d.Add("Drill:SmallBlockDrill", "Drill");
                d.Add("ExtendedPistonBase:LargePistonBase", "Piston");
                d.Add("ExtendedPistonBase:SmallPistonBase", "Piston");
                d.Add("GravityGenerator:", "Gravity Generator");
                d.Add("GravityGeneratorSphere:", "Sphere Gravity Generator");
                d.Add("Gyro:LargeBlockGyro", "Gyroscope");
                d.Add("Gyro:SmallBlockGyro", "Gyroscope");
                d.Add("InteriorLight:LargeBlockLight_1corner", "Corner Light");
                d.Add("InteriorLight:LargeBlockLight_2corner", "Corner Light 2x");
                d.Add("InteriorLight:SmallBlockLight_1corner", "Corner Light");
                d.Add("InteriorLight:SmallBlockLight_2corner", "Corner Light 2x");
                d.Add("InteriorLight:SmallBlockSmallLight", "Light");
                d.Add("InteriorLight:SmallLight", "Light");
                d.Add("InteriorTurret:LargeInteriorTurret", "Interior Turret");
                d.Add("JumpDrive:LargeJumpDrive", "Jump Drive");
                d.Add("LandingGear:LargeBlockLandingGear", "Landing Gear");
                d.Add("LandingGear:SmallBlockLandingGear", "Landing Gear");
                d.Add("LargeGatlingTurret:", "Gatling Turret");
                d.Add("LargeGatlingTurret:SmallGatlingTurret", "Gatling Turret");
                d.Add("LargeMissileTurret:", "Missile Turret");
                d.Add("LargeMissileTurret:SmallMissileTurret", "Missile Turret");
                d.Add("LaserAntenna:LargeBlockLaserAntenna", "Laser Antenna");
                d.Add("LaserAntenna:SmallBlockLaserAntenna", "Laser Antenna");
                d.Add("MedicalRoom:LargeMedicalRoom", "Medical");
                d.Add("MergeBlock:LargeShipMergeBlock", "Merge");
                d.Add("MergeBlock:SmallShipMergeBlock", "Merge");
                d.Add("MyObjectBuilder_Projector:LargeProjector", "Projector");
                d.Add("MyObjectBuilder_Projector:SmallProjector", "Projector");
                d.Add("MyProgrammableBlock:LargeProgrammableBlock", "Program");
                d.Add("MyProgrammableBlock:SmallProgrammableBlock", "Program");
                d.Add("OreDetector:LargeOreDetector", "Ore Detector");
                d.Add("OreDetector:SmallBlockOreDetector", "Ore Detector");
                d.Add("OxygenFarm:LargeBlockOxygenFarm", "Oxygen Farm");
                d.Add("OxygenGenerator:", "Oxygen Generator");
                d.Add("OxygenGenerator:OxygenGeneratorSmall", "Oxygen Generator");
                d.Add("OxygenTank:", "Oxygen Tank");
                d.Add("OxygenTank:LargeHydrogenTank", "Hydrogen Tank");
                d.Add("OxygenTank:OxygenTankSmall", "Oxygen Tank");
                d.Add("OxygenTank:SmallHydrogenTank", "Hydrogen Tank");
                d.Add("Parachute:LgParachute", "Parachute");
                d.Add("Parachute:SmParachute", "Parachute");
                d.Add("PistonBase:LargePistonBase", "Piston");
                d.Add("PistonBase:SmallPistonBase", "Piston");
                d.Add("RadioAntenna:LargeBlockRadioAntenna", "Antenna");
                d.Add("RadioAntenna:SmallBlockRadioAntenna", "Antenna");
                d.Add("Reactor:LargeBlockLargeGenerator", "Lg Reactor");
                d.Add("Reactor:LargeBlockSmallGenerator", "Sm Reactor");
                d.Add("Reactor:SmallBlockLargeGenerator", "Lg Reactor");
                d.Add("Reactor:SmallBlockSmallGenerator", "Sm Reactor");
                d.Add("Refinery:Blast Furnace", "Arc Furnace");
                d.Add("Refinery:LargeRefinery", "Refinery");
                d.Add("ReflectorLight:LargeBlockFrontLight", "Spotlight");
                d.Add("ReflectorLight:SmallBlockFrontLight", "Spotlight");
                d.Add("RemoteControl:LargeBlockRemoteControl", "Remote Control");
                d.Add("RemoteControl:SmallBlockRemoteControl", "Remote Control");
                d.Add("SensorBlock:LargeBlockSensor", "Sensor");
                d.Add("SensorBlock:SmallBlockSensor", "Sensor");
                d.Add("ShipConnector:Connector", "Connector");
                d.Add("ShipConnector:ConnectorMedium", "Connector");
                d.Add("ShipConnector:ConnectorSmall", "Ejector");
                d.Add("ShipGrinder:LargeShipGrinder", "Grinder");
                d.Add("ShipGrinder:SmallShipGrinder", "Grinder");
                d.Add("ShipWelder:LargeShipWelder", "Welder");
                d.Add("ShipWelder:SmallShipWelder", "Welder");
                d.Add("SmallGatlingGun:", "Gatling");
                d.Add("SmallMissileLauncher:", "Rocket Launcher");
                d.Add("SmallMissileLauncher:LargeMissileLauncher", "Rocket Launcher");
                d.Add("SmallMissileLauncherReload:SmallRocketLauncherReload", "Rocket Launcher");
                d.Add("SolarPanel:LargeBlockSolarPanel", "Solar Panel");
                d.Add("SolarPanel:SmallBlockSolarPanel", "Solar Panel");
                d.Add("SoundBlock:LargeBlockSoundBlock", "Sound");
                d.Add("SoundBlock:SmallBlockSoundBlock", "Sound");
                d.Add("SpaceBall:SpaceBallLarge", "Space Ball");
                d.Add("SpaceBall:SpaceBallSmall", "Space Ball");
                d.Add("TerminalBlock:ControlPanel", "Control Panel");
                d.Add("TerminalBlock:SmallControlPanel", "Control Panel");
                d.Add("TextPanel:LargeBlockCorner_LCD_1", "Corner LCD");
                d.Add("TextPanel:LargeBlockCorner_LCD_2", "Corner LCD");
                d.Add("TextPanel:LargeBlockCorner_LCD_Flat_1", "Flat LCD");
                d.Add("TextPanel:LargeBlockCorner_LCD_Flat_2", "Flat LCD");
                d.Add("TextPanel:LargeLCDPanel", "LCD");
                d.Add("TextPanel:LargeLCDPanelWide", "Wide LCD");
                d.Add("TextPanel:LargeTextPanel", "Text Panel");
                d.Add("TextPanel:SmallBlockCorner_LCD_1", "Corner LCD");
                d.Add("TextPanel:SmallBlockCorner_LCD_2", "Corner LCD");
                d.Add("TextPanel:SmallBlockCorner_LCD_Flat_1", "Flat LCD");
                d.Add("TextPanel:SmallBlockCorner_LCD_Flat_2", "Flat LCD");
                d.Add("TextPanel:SmallLCDPanel", "LCD");
                d.Add("TextPanel:SmallLCDPanelWide", "Wide LCD");
                d.Add("TextPanel:SmallTextPanel", "Text Panel");
                d.Add("Thrust:LargeBlockLargeAtmosphericThrust", "Lg Atmo Thruster");
                d.Add("Thrust:LargeBlockLargeHydrogenThrust", "Lg H2 Thruster");
                d.Add("Thrust:LargeBlockLargeThrust", "Lg Ion Thruster");
                d.Add("Thrust:LargeBlockSmallAtmosphericThrust", "Atmo Thruster");
                d.Add("Thrust:LargeBlockSmallHydrogenThrust", "H2 Thruster");
                d.Add("Thrust:LargeBlockSmallThrust", "Ion Thruster");
                d.Add("Thrust:SmallBlockLargeAtmosphericThrust", "Lg Atmo Thruster");
                d.Add("Thrust:SmallBlockLargeHydrogenThrust", "Lg H2 Thruster");
                d.Add("Thrust:SmallBlockLargeThrust", "Lg Ion Thruster");
                d.Add("Thrust:SmallBlockSmallAtmosphericThrust", "Atmo Thruster");
                d.Add("Thrust:SmallBlockSmallHydrogenThrust", "H2 Thruster");
                d.Add("Thrust:SmallBlockSmallThrust", "Ion Thruster");
                d.Add("TimerBlock:TimerBlockLarge", "Timer");
                d.Add("TimerBlock:TimerBlockSmall", "Timer");
                d.Add("UpgradeModule:LargeEffectivenessModule", "Yield Module");
                d.Add("UpgradeModule:LargeEnergyModule", "Power Module");
                d.Add("UpgradeModule:LargeProductivityModule", "Speed Module");
                d.Add("VirtualMass:VirtualMassLarge", "Mass");
                d.Add("VirtualMass:VirtualMassSmall", "Mass");
                d.Add("Warhead:LargeWarhead", "Warhead");
                d.Add("Warhead:SmallWarhead", "Warhead");
            }

            public string GetName(IMyTerminalBlock b) {
                var key = BlockHelper.GetKey(b);
                return GetName(key);
            }
            public string GetName(string key) {
                return HasTypeKey(key)
                    ? NameDictionary[key]
                    : string.Empty;
            }

            public bool HasTypeKey(IMyTerminalBlock b) {
                var key = BlockHelper.GetKey(b);
                return HasTypeKey(key);
            }

            public bool HasTypeKey(string key) {
                return NameDictionary.ContainsKey(key);
            }

        }
    }
}
