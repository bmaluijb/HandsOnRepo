---
description: Sync OpenSpec specs with the actual code so they describe current behavior (no CLI, no install)
mode: agent
---

Reconcile the specs under `openspec/specs/` with what the code actually does. Use this when the implementation has drifted from the specs (or specs are missing for existing behavior). You do this yourself with file edits - no `openspec` CLI.

**Input**: the text after `/opsx-sync` is an optional capability or area to focus on. If omitted, review the whole `openspec/specs/` tree.

## Steps

1. **Scope it.** Pick the capability/capabilities to sync (the requested one, or all of `openspec/specs/`).

2. **Compare specs to code.** For each capability, read its `openspec/specs/<capability>/spec.md` and the corresponding implementation. Note where they disagree:
   - behavior in the code that no requirement covers,
   - requirements/scenarios that no longer match the code,
   - requirements for behavior that was removed.

3. **Report the drift** as a short list before editing, so the user can see what will change.

4. **Update the specs** to match current behavior: add missing requirements/scenarios, correct outdated ones, and remove requirements for behavior that no longer exists. Keep the format - `# <Capability>` heading, `### Requirement:` sections with SHALL/MUST language, each containing `#### Scenario:` blocks with GIVEN / WHEN / THEN.

5. **Summarize** which spec files changed and how.

## Guardrails
- Do **not** run `openspec ...` or any other external CLI.
- Sync edits the **specs**, not the code. If you find a real bug in the code, point it out and suggest a `/opsx-propose` to fix it rather than fixing it here.
- Only document behavior you actually verified in the code.
