# NN Pension Planner

A pension/retirement planning application built for the **GitHub Copilot Workshop** at Nationale-Nederlanden.

## Getting Started

**No install required** — `run.bat` automatically downloads .NET 10 if it's not on your machine (first run only, ~300 MB, no admin rights needed).

### Run the App

1. Double-click **`run.bat`**
2. Your browser opens to **http://localhost:5000**
3. Press `Ctrl+C` in the terminal to stop

### After Making Code Changes

Double-click **`run.bat`** again — it rebuilds automatically before running.

To build without running, use **`build.bat`**.

> **Note:** Windows Firewall may prompt you on first run since the app opens a network port. Click **Allow**.

## Architecture

```
┌──────────────────────────────────────────────────┐
│                    Frontend                       │
│              (HTML / CSS / JavaScript)            │
└──────────────────┬───────────────────────────────┘
                   │ HTTP / JSON
┌──────────────────▼───────────────────────────────┐
│                  Middleware                        │
│         RequestLogging · ErrorHandling            │
└──────────────────┬───────────────────────────────┘
                   │
┌──────────────────▼───────────────────────────────┐
│              API Endpoints                        │
│   /participants · /plans · /enrollments           │
│   /contributions · /projections                   │
└──────────────────┬───────────────────────────────┘
                   │
┌──────────────────▼───────────────────────────────┐
│               Services                            │
│   ParticipantService · EnrollmentService          │
│   ContributionService · ProjectionService         │
└─────────┬────────────────────┬───────────────────┘
          │                    │
┌─────────▼─────────┐  ┌──────▼──────────────────┐
│     Event Bus      │  │   Data Access            │
│  (Pub/Sub Events)  │  │  InMemoryRepository<T>   │
└───────────────────┘  └──────────────────────────┘
```

## Domain Model

| Entity | Description |
|---|---|
| **Participant** | Person enrolled in a pension plan (name, DOB, salary, employer) |
| **PensionPlan** | Plan types: NN Basis, NN Flex, NN Premium, NN Starter (type, employer match, vesting) |
| **Enrollment** | Links participant to plan (status: Active/Suspended/Closed) |
| **Contribution** | Monthly contributions with auto-calculated employer match |
| **Projection** | Retirement projections with Conservative/Expected/Optimistic scenarios |

## Project Structure

```
NNPensionPlanner/
├── Program.cs                  # Entry point, DI, middleware, routes
├── Models/                     # Domain entities (5 classes)
├── Data/                       # IRepository<T>, InMemoryRepository, SeedData
├── Services/                   # Business logic (4 services)
├── Endpoints/                  # Minimal API route groups (5 files)
├── Events/                     # EventBus + domain event records
├── Middleware/                  # Request logging, error handling
└── wwwroot/                    # Frontend (HTML/CSS/JS)
```

## API Endpoints

### Participants
- `GET /api/participants` — List all
- `GET /api/participants/{id}` — Get by ID
- `POST /api/participants` — Create
- `PUT /api/participants/{id}` — Update
- `DELETE /api/participants/{id}` — Delete

### Plans
- `GET /api/plans` — List all plans
- `GET /api/plans/{id}` — Get plan details
- `GET /api/plans/{id}/enrollments` — Enrollments in a plan

### Enrollments
- `GET /api/enrollments` — List all
- `GET /api/enrollments/{id}` — Get by ID
- `GET /api/enrollments/participant/{participantId}` — By participant
- `POST /api/enrollments` — Create enrollment
- `PUT /api/enrollments/{id}/status` — Update status

### Contributions
- `GET /api/contributions/enrollment/{id}` — By enrollment
- `GET /api/contributions/enrollment/{id}/balance` — Total balance
- `GET /api/contributions/enrollment/{id}/summary` — Monthly summary
- `POST /api/contributions` — Add contribution

### Projections
- `GET /api/projections/enrollment/{id}` — By enrollment
- `GET /api/projections/enrollment/{id}/latest` — Latest projection
- `POST /api/projections/calculate` — Calculate new projection

## Workshop Exercises

This app is designed with deliberate feature gaps for workshop exercises:

### Exercise 1: Custom Instructions & Prompt Files

Learn how custom instructions change Copilot's behavior by comparing output **before** and **after** adding them.

#### Part A — Without Instructions (Baseline)
1. Open Copilot Chat and ask: *"Add a method to `ProjectionService.cs` that calculates the break-even age for a participant"*
2. Note the response — the coding style, comments, naming conventions, and approach
3. Now ask: *"Write a new API endpoint that returns a summary report for a participant"*
4. Save both responses somewhere (or just remember them) — you'll compare later

#### Part B — Add a `.github/copilot-instructions.md` File
Create a file `.github/copilot-instructions.md` in the root of this repo with project-wide instructions. For example:

```markdown
# Project Instructions

- This is a .NET 10 Minimal API project for Nationale-Nederlanden (NN)
- Use Dutch-language XML doc comments on all public methods
- Follow the existing pattern: Services handle business logic, Endpoints only map routes
- Always use decimal for monetary values, never double or float
- Method names should follow the pattern: {Verb}{Entity} (e.g., CalculateBreakEvenAge, GetParticipantSummary)
- Throw ArgumentException for validation errors with descriptive messages
- Publish domain events via EventBus for any state-changing operations
- Use ILogger for structured logging with meaningful context (include IDs, amounts)
```

5. Save the file, then ask Copilot the **same two questions** from Part A
6. Compare the responses — notice how Copilot now follows your conventions (Dutch doc comments, naming patterns, event publishing, etc.)

#### Part C — Add a `.prompt.md` Reusable Prompt
Create a file `prompts/add-endpoint.prompt.md` to define a reusable prompt template:

```markdown
---
mode: 'agent'
description: 'Generate a new API endpoint following NN project conventions'
---

Create a new Minimal API endpoint for this project. Follow these rules:

1. Create the endpoint method in the appropriate file under `Endpoints/`
2. The endpoint should delegate to a Service method — no business logic in the endpoint
3. If the Service method doesn't exist yet, create it too
4. Add Dutch XML doc comments to the Service method
5. Use proper HTTP status codes (200, 201, 404, 400)
6. Include structured logging with `ILogger`
7. If the operation changes state, publish a domain event via `EventBus`
8. Register any new endpoint mapping in `Program.cs`

The feature to implement: {{input}}
```

7. Open the prompt by typing `/` in chat, or run it from the Command Palette: **Chat: Run Prompt**
8. Try it with: *"Get a participant's total contributions across all their enrollments"*
9. Compare this to the baseline from Part A — the output should be much more consistent and complete

#### Bonus: File-Scoped Instructions (`.instructions.md` files)

`.instructions.md` files let you apply instructions conditionally based on file patterns or task descriptions. They live in the `.github/instructions/` folder (not alongside your source code). You can organize them in subdirectories by team, language, or module.

Create a file `.github/instructions/services.instructions.md`:

```markdown
---
name: 'Service Conventions'
description: 'Coding conventions for service classes in the Services folder'
applyTo: 'Services/**/*.cs'
---
- All service methods must validate input parameters before processing
- Log entry and exit of every public method
- Include the enrollment ID or participant ID in all log messages
```

> **Note:** The `applyTo` glob pattern is relative to the workspace root. The `name` and `description` fields are optional — `name` is shown in the UI (defaults to the file name), and `description` appears on hover in the Chat view.

10. Ask Copilot to add a method to any service and see if the scoped instructions are followed

> **Tip:** Type `/instructions` in the chat input to quickly open the Configure Instructions and Rules menu. You can also use `/create-instruction` followed by a description (e.g., *"always use tabs and single quotes"*) to let AI generate an `.instructions.md` file for you. Use `/init` to auto-generate workspace-wide instructions based on your project structure.

#### Discussion
- What difference did the instructions make?
- Which instructions were most impactful?
- How could you use this in your own projects?

---

### Exercise 2: Custom Mermaid Diagram Agent
The architecture has enough layers and relationships for interesting diagrams:
- Component diagrams (services, repos, event bus)
- Sequence diagrams (contribution flow with employer match calculation)
- Class diagrams (entity relationships)
- Flowcharts (projection calculation logic)

1. Create a Custom Agent that creates the above architecture diagrams
2. Make the Agent output the diagrams using Mermaid
3. (Bonus) Make the Custom Agent use subagents that can only be run by Agents, not humans 
4. You can view Mermaid digrams in VS Code with extensions like these: Markdown Preview Mermaid Support https://marketplace.visualstudio.com/items?itemName=bierner.markdown-mermaid
5. The best diagram wins! Make it look good and easy to understand


### Exercise 3: OpenSpec Feature Addition

Build a feature using **Spec Driven Development** — go from a plain-English description to working code using OpenSpec commands. Each feature below includes the exact commands to run at every step.

#### How to start
1. Remove the `copilot-instructions.md` and other instruction files you created in Exercise 1
2. OpenSpec is already initialized in this repository — the Copilot slash commands live in `.github/prompts/` and run entirely inside Copilot Chat, so there is **nothing to install**
3. Restart VS Code so the `/opsx-*` commands appear when you type `/` in Copilot Chat
4. Pick a feature below (start with an **Easy** one if this is your first time)
5. Follow the numbered steps inside each feature card
6. Have fun, and show us what you made!

> **Tip:** The OpenSpec lifecycle is: **propose → apply → archive**. `/opsx-propose` writes a proposal, design, and task list into `openspec/changes/`; `/opsx-apply` implements the tasks; `/opsx-archive` folds the result into `openspec/specs/`. `/opsx-explore` and `/opsx-sync` are optional helpers.
>
> **Heads-up:** OpenSpec's own docs call these commands `/opsx:propose`, `/opsx:apply`, and so on. In VS Code Copilot Chat they appear with a hyphen — `/opsx-propose`, `/opsx-apply` — same workflow.

---

#### Feature 1: Participant Search · 🟢 Easy

> Add a search endpoint so users can find participants by name or employer — no new models or files needed, just extend what's already there.

**What changes**
- Add a `Search(string query)` method to `Services/ParticipantService.cs`
- Add a `GET /api/participants/search?q={query}` endpoint to `Endpoints/ParticipantEndpoints.cs`
- Add a search box to the participants page in the frontend

**Business rules**
- Search is case-insensitive and matches against `FirstName`, `LastName`, or `EmployerName`
- Empty query returns all participants
- No new model, no events, no seed data changes

**Step-by-step with OpenSpec**

1. **Propose** — Open Copilot Chat and type:

   ```
   /opsx-propose Add a search endpoint to find participants by name or employer.
   It should add a Search(string query) method to ParticipantService that filters
   participants by FirstName, LastName, or EmployerName (case-insensitive).
   Add a GET /api/participants/search?q={query} endpoint. Empty query returns all.
   Add a search box to the participants page in the frontend.
   ```

   OpenSpec creates a change folder under `openspec/changes/` with a proposal, design, and task list — and asks you clarifying questions if anything is unclear.

2. **Apply** — Implement the tasks in the change:

   ```
   /opsx-apply
   ```

3. **Archive** — Once it works, fold the change into your specs:

   ```
   /opsx-archive
   ```

4. **Test it** — Double-click `run.bat` and try `http://localhost:5000/api/participants/search?q=jan`

---

#### Feature 2: Enrollment Statistics · 🟢 Easy

> Add a statistics endpoint that returns aggregate numbers about enrollments — total count, active count, and average contribution percentage. Show these on the dashboard.

**What changes**
- Add a `GetStatistics()` method to `Services/EnrollmentService.cs`
- Add a `GET /api/enrollments/statistics` endpoint to `Endpoints/EnrollmentEndpoints.cs`
- Display the stats as cards on the dashboard page

**Business rules**
- Returns: `totalEnrollments`, `activeEnrollments`, `averageContributionPercentage`
- Read-only — no validation, no events, no new models
- Average should only include Active enrollments

**Step-by-step with OpenSpec**

1. **Propose**:

   ```
   /opsx-propose Add an enrollment statistics endpoint. Add a GetStatistics()
   method to EnrollmentService that returns totalEnrollments (int),
   activeEnrollments (int), and averageContributionPercentage (decimal, average of
   Active enrollments only). Add a GET /api/enrollments/statistics endpoint.
   Display these as stat cards on the dashboard page in the frontend.
   ```

2. **Apply**: `/opsx-apply`
3. **Archive**: `/opsx-archive`
4. **Test it** — Double-click `run.bat` and try `http://localhost:5000/api/enrollments/statistics`

---

#### Feature 3: Participant Notes · 🟡 Medium

> Let users add simple text notes to a participant (like a CRM note). This creates a brand-new entity end-to-end: model, repository, service, endpoints, seed data, DI registration, and frontend.

**What to build**

| Layer | File to create/modify |
|---|---|
| Model | `Models/Note.cs` (new) |
| Service | `Services/NoteService.cs` (new) |
| Endpoints | `Endpoints/NoteEndpoints.cs` (new) |
| Data | Add seed data in `Data/SeedData.cs` |
| DI | Register in `Program.cs` |
| Frontend | Add a "Notes" section to the participant detail view |

**Domain model — `Note`**

| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | Auto-generated |
| `ParticipantId` | `Guid` | FK → `Participant.Id` |
| `Content` | `string` | Required, the note text |
| `CreatedAt` | `DateTime` | Auto-set to `DateTime.UtcNow` |

**API endpoints**

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/notes/participant/{participantId}` | List notes for a participant (newest first) |
| `POST` | `/api/notes` | Add a note to a participant |
| `DELETE` | `/api/notes/{id}` | Delete a note |

**Business rules**
- `Content` is required and cannot be empty
- Validate that `ParticipantId` exists
- Notes are returned newest-first by `CreatedAt`

**Step-by-step with OpenSpec**

1. **Propose**:

   ```
   /opsx-propose Add participant notes. Create a Note model with Id (Guid),
   ParticipantId (Guid), Content (string, required), and CreatedAt (DateTime).
   Create NoteService with methods to list notes for a participant (newest first),
   add a note, and delete a note. Validate that ParticipantId exists and Content
   is not empty. Create API endpoints: GET /api/notes/participant/{participantId},
   POST /api/notes, DELETE /api/notes/{id}. Register in DI, add seed data for
   existing participants, and add a Notes section to the frontend participant view.
   Follow the existing Repository → Service → Endpoint pattern.
   ```

2. **Apply**: `/opsx-apply`
3. **Archive**: `/opsx-archive`
4. **Test it** — Double-click `run.bat`, open a participant, and try adding a note

---

#### Feature 4: Contribution Goal Tracker · 🟡 Medium

> Let participants set a yearly contribution goal for an enrollment and see their progress toward it. This adds a new entity with a simple calculation (current balance vs goal).

**What to build**

| Layer | File to create/modify |
|---|---|
| Model | `Models/ContributionGoal.cs` (new) |
| Service | `Services/ContributionGoalService.cs` (new) |
| Endpoints | `Endpoints/ContributionGoalEndpoints.cs` (new) |
| Data | Add seed data in `Data/SeedData.cs` |
| DI | Register in `Program.cs` |
| Frontend | Add a progress bar to the contributions page |

**Domain model — `ContributionGoal`**

| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | Auto-generated |
| `EnrollmentId` | `Guid` | FK → `Enrollment.Id` |
| `Year` | `int` | The target year (e.g. 2026) |
| `TargetAmount` | `decimal` | Goal amount in euros, must be > 0 |

**API endpoints**

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/goals/enrollment/{enrollmentId}` | Get goals for an enrollment |
| `POST` | `/api/goals` | Set a contribution goal |
| `GET` | `/api/goals/{id}/progress` | Get progress: returns `{ targetAmount, currentAmount, percentComplete }` |

**Business rules**
- `TargetAmount` must be > 0
- Validate that `EnrollmentId` exists
- One goal per enrollment per year
- Progress = sum of contributions for that enrollment in that year vs `TargetAmount`
- `percentComplete` is capped at 100

**Step-by-step with OpenSpec**

1. **Propose**:

   ```
   /opsx-propose Add a contribution goal tracker. Create a ContributionGoal
   model with Id (Guid), EnrollmentId (Guid), Year (int), and TargetAmount
   (decimal, must be > 0). Create ContributionGoalService that can set a goal
   (one per enrollment per year), list goals for an enrollment, and calculate
   progress by summing contributions for that enrollment in that year. Progress
   returns targetAmount, currentAmount, and percentComplete (capped at 100).
   Create endpoints: GET /api/goals/enrollment/{enrollmentId}, POST /api/goals,
   GET /api/goals/{id}/progress. Register in DI, add seed data, and show a
   progress bar on the contributions page in the frontend. Follow the existing
   Repository → Service → Endpoint pattern.
   ```

2. **Apply**: `/opsx-apply`
3. **Archive**: `/opsx-archive`
4. **Test it** — Double-click `run.bat`, set a goal via the API, and check the progress bar

---

#### Quick Reference: OpenSpec Commands

| Step | Command | What it does |
|---|---|---|
| 1 | `/opsx-propose <description>` | Creates a change folder (proposal, design, tasks) from your description |
| 2 | `/opsx-apply` | Implements the tasks in the active change |
| 3 | `/opsx-archive` | Folds the change's specs into `openspec/specs/` and archives it |
| — | `/opsx-explore <topic>` | (Optional) Research the codebase before you propose |
| — | `/opsx-sync` | (Optional) Reconcile specs with the current code |


