# Space Engineers Scripts

A collection of independent programmable-block scripts for Space Engineers. Each script has its own project directory; `SE-Scripts.sln` groups the projects for building in Visual Studio.

## Prerequisites

- Install [Malware's Development Kit (MDK²-SE)](https://github.com/malforge/mdk2) to build and package the scripts.
- Configure MDK² to locate your Space Engineers installation.

## Projects

| Project | Description |
| --- | --- |
| [Axel - Drop GPS Recorder](Axel%20-%20Drop%20GPS%20Recorder/) | Watches tagged merge blocks and records the programmable block's GPS position to LCDs when a merge block disconnects. |
| [Block Report](Block%20Report/) | Counts blocks by type and writes the inventory report to the programmable block's Custom Data. |
| [BlockInfo](BlockInfo/) | Reports information about the grid connected to a connector tagged `[starting]`, including its name, entity ID, and most common owner ID. |
| [COMM Commands](COMM%20Commands/) | Prototype for queueing and processing communications commands. |
| [Countdown Trigger](Countdown%20Trigger/) | Displays a configurable countdown on tagged LCDs, triggers tagged timer blocks, and supports starting or aborting a countdown. |
| [Deployable Turret](Deployable%20Turret/) | Monitors a turret and its deployment support blocks, updates warning lights and antenna status, and listens for IGC updates. |
| [Grav Curve Map](Grav%20Curve%20Map/) | Runs a vertical ascent while recording natural gravity at different elevations, alongside a calculated gravity curve; supports `go` and `abort`. |
| [GridOS](GridOS/) | Provides automatic door closing with configurable exclusions and a starting point for hangar and airlock controls. |
| [Group Renamer](Group%20Renamer/) | Applies rename, numbering, prefix, suffix, removal, and replacement operations to blocks in groups named with supported rename rules. |
| [Guidance Block Launch Control](Guidance%20Block%20Launch%20Control/) | Selects and controls torpedo guidance blocks, including target lock, launch, and beacon or power-cell management. |
| [Initialize Blocks](Initialize%20Blocks/) | Applies basic names to supported block types and reports block types that lack configured names. |
| [Malhavoc - WelderFactory Init](Malhavoc%20-%20WelderFactory%20Init/) | Extends or retracts a welder factory by moving configured piston groups through their travel positions. |
| [Sapphire-Ship Systems](Sapphire-Ship%20Systems/) | Ship utilities for ship IDs, airlock pressurization, merge-block decoupling, door closing, connector locking, and oxygen-generator control. |
| [Sapphire Mk2 - Ship Systems](Sapphire%20Mk2%20-%20Ship%20Systems/) | Manages connected train grids, grid IDs and names, antenna and thruster state, automatic door closing, and tagged disconnect sequences. |
| [SDLS - Booster](SDLS%20-%20Booster/) | Empty MDK² starter project reserved for an SDLS booster script. |
| [SDLS - GrassHopper](SDLS%20-%20GrassHopper/) | Runs a tagged vertical flight test with gravity alignment, ascent, hover, descent, and abort operations. |
| [SDLS - Launch Center](SDLS%20-%20Launch%20Center/) | Controls launch-pad boom connection and retraction operations, including the launch command. |
| [SDLS - Orbiter](SDLS%20-%20Orbiter/) | SDLS orbiter-side flight-control project with gravity-alignment sequences and shared launch-system integration. |
| [SDLS Rocket (Merges)](SDLS%20Rocket%20%28Merges%29/) | Coordinates a multi-stage SDLS rocket using merge-block-connected stages, including launch sequencing and stage handoff. |
| [SDLS Rocket (Rotors)](SDLS%20Rocket%20%28Rotors%29/) | Coordinates SDLS rocket flight and stage sequences for rotor-connected assemblies. |
| [SpotRacing](SpotRacing/) | Tracks racing checkpoints, laps, and lap times, with LCD displays for race status, speed, lap history, and an artificial horizon. |
| [TIM Cargo Switcher](TIM%20Cargo%20Switcher/) | Saves and switches named block-name configurations used by Taleden's Inventory Manager (TIM). |
| [TWR Calculator](TWR%20Calculator/) | Calculates directional thrust-to-weight ratios and liftable cargo estimates for a ship. |
| [Utility Ship Systems](Utility%20Ship%20Systems/) | Automates ship docking behavior and provides tool toggles, forward camera scanning, proximity alerts, and related displays. |

## Shared code

|Project|Description|
| --- | --- |
| [Library](Library/) | Contains helpers and modules imported by multiple script projects. |
| [SDLS - Shared](SDLS%20-%20Shared/) | Shared library of code unique to the SDLS rocket projects. |


## Build

Build all projects from the repository root:

```powershell
dotnet build .\SE-Scripts.sln --configuration Debug
```

MDK² packaging and programmable-block analyzers run as part of the build.
