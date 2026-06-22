---
description: Archive a completed OpenSpec change and fold its specs into the source of truth (no CLI, no install)
mode: agent
---

Finalize a completed change: merge its delta specs into the main specs under `openspec/specs/`, then move the change folder into `openspec/changes/archive/`. You do this yourself with file edits and a plain file move - no `openspec` CLI.

**Input**: the text after `/opsx-archive` is an optional change name. If omitted, infer it or ask.

## Steps

1. **Select the change** from `openspec/changes/` (ignore `archive/`). If it is not obvious, list the active changes and ask the user to choose. Do not guess.

2. **Confirm it is done.** Check `tasks.md` for unchecked `- [ ]` items. If any remain, warn the user and ask whether to archive anyway.

3. **Merge each delta spec** in `openspec/changes/<name>/specs/<capability>/spec.md` into the matching main spec at `openspec/specs/<capability>/spec.md` (create the file/folder if it does not exist):
   - **ADDED Requirements** -> append the requirement (and its scenarios) to the main spec.
   - **MODIFIED Requirements** -> replace the existing requirement of the same name with the new text.
   - **REMOVED Requirements** -> delete that requirement from the main spec.
   Keep the main spec clean: a top-level `# <Capability>` heading followed by `### Requirement:` sections, each with its `#### Scenario:` blocks. Do not keep the ADDED/MODIFIED/REMOVED headings in the main spec - those only describe the delta.

4. **Move the change folder** to `openspec/changes/archive/<YYYY-MM-DD>-<name>/` using today's date. Use your file tools or a simple move (for example `git mv`); do **not** use the `openspec` CLI. If the target already exists, pick a different date suffix or ask the user.

5. **Summarize**: which capabilities were updated, where the change was archived, and any warnings (incomplete tasks, skipped merges).

## Guardrails
- Do **not** run `openspec ...` or any other external CLI.
- Only fold requirements into specs that are genuinely complete and reflected in the code.
- The main specs in `openspec/specs/` describe how the system behaves *now*; keep them coherent after merging.
