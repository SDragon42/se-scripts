# Copilot instructions

## Project model

This repository is a collection of independent Space Engineers programmable-block scripts. Each script directory is a C# project; `SE-Scripts.sln` groups them for Visual Studio. Projects target .NET Framework 4.8, use C# 6, and compile against the Space Engineers in-game API through MDK² (`Mal.Mdk2.*` packages). Keep new language features compatible with C# 6.

MDK² packages each project into a single deployable script in the `IngameScript` namespace. The script entry point is normally a `partial class Program : MyGridProgram`; a project may split that class over ordered source files. `// <mdk sortorder="..."/>` controls the order in the generated script, so retain it and add an appropriate sort order to new split files.

`Instructions.readme` and `thumb.png` are MDK additional files. Text in `Instructions.readme` is injected at the top of the generated script. Each project's checked-in `.mdk.ini` configures a programmable-block package with `stripcomments` minification. Do not add runtime dependencies that cannot exist in the Space Engineers programmable-block environment.

## Shared code and script boundaries

`Library\Library.projitems` is imported by most script projects and compiles its helpers, extensions, and modules into every importing script. Reuse its `Collect`, block-selection helpers, configuration extensions, logging, state-machine, and display utilities rather than duplicating them in individual scripts. A `Library` change has a broad impact across importing projects.

The SDLS launch-system projects additionally import `SDLS - Shared\SDLS - Shared.projitems`; its partial `Program` files provide the shared SDLS constants, variables, collection logic, sequencing, and miscellaneous behavior. Keep SDLS-specific additions there when they are shared by the Booster and Orbiter projects, and preserve the `partial Program` structure.

Individual script projects own their block tags, commands, and configuration. Blocks are commonly selected with `GridTerminalSystem.GetBlocksOfType` plus `IsSameConstructAs(Me)`, project-specific grid predicates, and bracketed custom-name tags such as `[main]` or `[airlock]`. Reuse the existing tag constants and `Collect.IsTagged` so block naming remains compatible with deployed ships.

## Runtime and configuration conventions

`Program()` initializes command dictionaries, dependent modules, and `Runtime.UpdateFrequency`; `Main(string argument, UpdateType updateSource)` dispatches terminal arguments and scheduled updates. Treat `updateSource` as a flags value, and keep recurring work aligned with the project’s chosen `Update1`, `Update10`, `Update100`, `Once`, or `None` scheduling.

Configuration is stored in programmable-block `Me.CustomData` using `MyIni`. Existing config loaders preserve defaults by adding keys and writing the resulting INI back, often avoiding repeat parsing with a custom-data hash. Follow that read/default/write pattern when extending an existing script’s config. User-visible commands are generally lower-case dictionary keys; preserve established command spelling and tag values because they are an external in-game interface.

## Build, analysis, and tests

Install MDK²-SE and make the Space Engineers installation discoverable to MDK² before building. The reference package locates the game binaries during build.

```powershell
# Restore packages and build every solution project
dotnet build .\SE-Scripts.sln --configuration Debug

# Restore packages and build one script project
dotnet build ".\GridOS\GridOS.csproj" --configuration Debug
```

MDK² programmable-block analyzers run as part of `dotnet build`; there is no separate lint command. The repository has no automated test project or test runner, so there is no full-suite or single-test command.

## Formatting

Follow `.editorconfig`: four-space indentation, trailing whitespace removed, final newlines, braces on the same line as declarations/control statements, and the existing C# 6-compatible `var` and expression-bodied-member preferences.
