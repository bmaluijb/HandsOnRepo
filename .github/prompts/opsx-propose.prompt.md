---
description: Propose a new OpenSpec change - create the change folder and all artifacts (no CLI, no install)
mode: agent
---

Create a new OpenSpec change and generate all of its artifacts directly as Markdown files. You do everything yourself with your file-editing tools - this command does **not** use the `openspec` CLI or any other external tool, so nothing needs to be installed.

**Input**: the text after `/opsx-propose` is either a kebab-case change name OR a description of what to build. If nothing was provided, ask the user what they want to build, then continue.

## Steps

1. **Pick a change name.** Derive a short kebab-case name from the request (e.g. "add user authentication" -> `add-user-auth`). If `openspec/changes/<name>/` already exists, ask whether to continue that change or choose a new name.

2. **Understand the project first.** Skim the relevant code and any existing specs under `openspec/specs/` so the proposal matches the project's conventions.

3. **Create the change folder** `openspec/changes/<name>/` and write these files.

   `proposal.md`:

   ```markdown
   # Proposal: <Title>

   ## Why
   <1-3 sentences: the problem or opportunity.>

   ## What Changes
   - <bullet list of the behavior being added or changed>

   ## Impact
   - Affected specs: <capability folders, e.g. participants, enrollments>
   - Affected code: <files or areas, if known>
   ```

   `design.md`:

   ```markdown
   # Design: <Title>

   ## Approach
   <How you'll implement it: key decisions, data shapes, endpoints, files to touch.>

   ## Notes / Trade-offs
   <Anything non-obvious, alternatives considered, or open questions. Write "N/A" if none.>
   ```

   `tasks.md`:

   ```markdown
   # Tasks

   ## 1. <Group>
   - [ ] 1.1 <task>
   - [ ] 1.2 <task>

   ## 2. <Group>
   - [ ] 2.1 <task>
   ```

   One delta spec per affected capability at `openspec/changes/<name>/specs/<capability>/spec.md`:

   ```markdown
   # <Capability> (delta)

   ## ADDED Requirements

   ### Requirement: <name>
   The system SHALL <behavior>.

   #### Scenario: <name>
   - GIVEN <precondition>
   - WHEN <action>
   - THEN <expected result>

   ## MODIFIED Requirements

   ### Requirement: <name>
   The system SHALL <new behavior>. (Previously: <old behavior>)

   ## REMOVED Requirements

   ### Requirement: <name>
   (Reason for removal.)
   ```

   Only include the ADDED / MODIFIED / REMOVED sections that actually apply. Every requirement must have at least one `#### Scenario:` with GIVEN / WHEN / THEN.

4. **Keep momentum.** Prefer sensible defaults; only ask the user when a decision materially changes the spec.

5. **Summarize**: list the files you created, then finish with - "Ready to build. Run `/opsx-apply` to implement the tasks."

## Guardrails
- Create and edit the files yourself. Do **not** run `openspec ...` or any other CLI command.
- `proposal.md` = why/what, `design.md` = how, `tasks.md` = ordered steps, delta specs = requirements + scenarios.
- Do not write any application code yet - that happens in `/opsx-apply`.
