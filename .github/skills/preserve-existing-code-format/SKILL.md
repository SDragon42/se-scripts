---
name: preserve-existing-code-format
description: Generate or add code that matches the formatting, naming, and structural conventions already used by the surrounding files.
---

# Preserve existing code format

Use this skill when generating new code for an existing repository or adding code to an existing file, especially when consistency with nearby code matters.

## Workflow

1. Identify the target file and inspect the surrounding code. For a new file, inspect the closest comparable files in the same project or directory.
2. Derive the local conventions from those examples, including indentation, line breaks, braces, spacing, naming, member ordering, comments, and common implementation patterns.
3. Match the closest relevant examples. Use repository-wide formatting guidance such as `.editorconfig` as the baseline, while preserving intentional, established local variations.
4. Make the smallest change that fits the existing structure. Do not reformat unrelated code or apply broad cleanup as part of code generation.
5. Review the diff to confirm the new code is consistent with its neighbors and that surrounding code was not changed unnecessarily. Run the relevant formatter, build, or tests when available and appropriate.

## Priority

Prefer the established style in the target file or its nearest comparable files over personal preference. When local examples conflict, follow the pattern used in the same construct or feature; consult repository-wide guidance if the conflict remains unresolved.
