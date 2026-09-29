# Contributing

Thanks for contributing to this collection of Space Engineers programmable-block scripts.

## Changes

- Keep changes focused on the affected script. Shared helpers in `Library` are compiled into importing projects, so change them only when the behavior should be shared.
- Preserve established in-game command names, block tags, and configuration keys unless a change to that interface is intentional.
- Keep code compatible with C# 6 and the programmable-block runtime. Do not add runtime dependencies that are unavailable in-game.
- Preserve MDK² source ordering directives when adding or reorganizing split source files.
- Include relevant updates to the script's `Instructions.readme` or other documentation when behavior, commands, tags, or configuration change.

## Build and validation

Install MDK²-SE and configure it to locate the Space Engineers game binaries before building.

Build the affected project:

```powershell
dotnet build ".\GridOS\GridOS.csproj" --configuration Debug
```

Replace `GridOS` with the affected project. To build the full solution:

```powershell
dotnet build .\SE-Scripts.sln --configuration Debug
```

MDK² analyzers run as part of the build. This repository does not currently have an automated test project, so test behavior in Space Engineers when feasible and report any validation that could not be performed.

## Issues and pull requests

Use the issue templates to report a reproducible bug or propose a feature. In pull requests, summarize the change, identify affected scripts, and state which builds or in-game checks you performed.
