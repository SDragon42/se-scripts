---
name: space-engineers-script-change
description: Make or review a code change to a Space Engineers programmable-block script in this repository, preserving its project boundaries, in-game interfaces, and MDK² constraints.
---

# Space Engineers script changes

Use this skill when changing an existing script or shared helper in this repository.

## Workflow

1. Identify the owning script project and inspect its entry point, related files, and project imports before editing.
2. Reuse helpers from `Library\Library.projitems`. For Booster and Orbiter behavior shared by both projects, check `SDLS - Shared\SDLS - Shared.projitems` before adding project-local code.
3. Keep script code compatible with C# 6 and the programmable-block runtime. Do not add runtime dependencies that are unavailable in-game.
4. Preserve existing terminal command spelling, block tags, configuration keys, and other deployed in-game interfaces unless the requested change explicitly requires changing them.
5. When extending `Me.CustomData` configuration, follow the existing `MyIni` default/read/write pattern and preserve user values.
6. Preserve `partial Program` structure and existing `// <mdk sortorder="..."/>` directives. Add an appropriate sort order to any new split source file.
7. Keep changes limited to the relevant project. A change to `Library` affects every importing script, so use it only when the behavior is genuinely shared.
8. Build the affected project when the local MDK² and Space Engineers prerequisites are available. Report clearly if the environment prevents the build.

## Completion checks

- Confirm the change does not introduce incompatible language features or unavailable runtime dependencies.
- Review the diff for accidental changes to commands, tags, configuration defaults, or MDK ordering.
- Update project instructions or documentation when user-facing behavior or configuration changes.
