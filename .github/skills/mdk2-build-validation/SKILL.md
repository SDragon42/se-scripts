---
name: mdk2-build-validation
description: Build and diagnose one or more Space Engineers script projects with MDK², and accurately report environment limitations and validation results.
---

# MDK² build validation

Use this skill when validating script changes or investigating a build failure.

## Prerequisites

- MDK²-SE must be installed and available to the project.
- MDK² must be able to locate the Space Engineers game binaries referenced by the project.
- This repository has no automated test project or separate lint command; MDK² analyzers run during `dotnet build`.

## Workflow

1. Prefer the affected project for a focused change:

   ```powershell
   dotnet build ".\GridOS\GridOS.csproj" --configuration Debug
   ```

   Replace `GridOS` with the actual project directory and project name.
2. For changes to shared library code or solution-wide validation, build the solution:

   ```powershell
   dotnet build .\SE-Scripts.sln --configuration Debug
   ```
3. Read the full build output. Distinguish source/analyzer errors from missing MDK² or game-installation prerequisites; do not report an unrun or blocked build as passing.
4. After a successful build, inspect the generated programmable-block script when relevant. For in-game behavior changes, state that game testing is still needed unless it was actually performed.

## Reporting

State the exact command run, whether it succeeded, and any blocking prerequisite. Do not claim automated tests or in-game validation that were not run.
