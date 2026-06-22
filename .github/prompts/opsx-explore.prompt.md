---
description: Explore an idea and turn it into a clear OpenSpec proposal direction (no CLI, no install)
mode: agent
---

Help the user think through an idea or problem *before* committing to a change. This is a read-and-discuss command: investigate the codebase and existing specs, then recommend a direction. No files are required and no `openspec` CLI is used.

**Input**: the text after `/opsx-explore` is the idea, question, or problem to explore.

## Steps

1. **Understand the request.** If it is vague, ask one or two focused questions to scope it.

2. **Investigate.** Read the relevant application code and any existing specs under `openspec/specs/` and active changes under `openspec/changes/`. Identify what already exists, what is missing, and any constraints or conventions to follow.

3. **Lay out options.** Present 1-3 viable approaches with their trade-offs. Recommend one and say why. Keep it concise and concrete (mention the actual files, endpoints, or capabilities involved).

4. **Define scope.** Summarize what a change would and would not include, the capabilities it would touch, and the main requirements/scenarios you would expect.

5. **Hand off.** End with a suggested kebab-case change name and - "Run `/opsx-propose <name>` to turn this into a change."

## Guardrails
- Do **not** run `openspec ...` or any other external CLI, and do **not** write application code here.
- It is fine to not create any files - exploration is about clarity. Only create a change when the user is ready (that is `/opsx-propose`).
