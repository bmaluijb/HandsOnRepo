---
description: Implement the tasks from an OpenSpec change (no CLI, no install)
mode: agent
---

Implement the tasks defined in an OpenSpec change by writing the actual code. You do this yourself with your file-editing tools - no `openspec` CLI is involved.

**Input**: the text after `/opsx-apply` is an optional change name. If omitted, infer it from the conversation or the active change.

## Steps

1. **Select the change.** Look in `openspec/changes/` (ignore the `archive/` folder).
   - If a name was given, use it.
   - If exactly one active change exists, use it.
   - Otherwise, list the active changes and ask the user which to implement.
   Announce: "Implementing change: <name>".

2. **Read the change.** Read `proposal.md`, `design.md`, `tasks.md`, and every delta spec under `specs/` in that change folder so you understand the intent and the acceptance scenarios.

3. **Implement each task in order.** For every unchecked `- [ ]` item in `tasks.md`:
   - Make the code changes that task describes, following the project's existing patterns.
   - Keep changes minimal and focused on that task.
   - Mark it done by changing `- [ ]` to `- [x]` in `tasks.md` immediately after finishing it.

4. **Pause and ask** only if a task is ambiguous, the design needs to change, or you hit a blocker. Otherwise keep going until all tasks are checked.

5. **Verify.** Make sure the change satisfies the scenarios in the delta specs (build/run if practical). Report what you changed and the task progress (for example "7/7 tasks complete").

6. When everything is done, finish with - "All tasks complete. Run `/opsx-archive` to fold this change into your specs."

## Guardrails
- Do **not** run `openspec ...` or any other external CLI - implement the work directly.
- Update the checkbox in `tasks.md` as soon as each task is finished, not all at the end.
- If implementation reveals that the proposal, design, or specs are wrong, update those artifacts too and note it.
