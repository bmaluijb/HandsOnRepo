# NN Pension Planner

A pension/retirement planning application built for the **GitHub Copilot Workshop** at Nationale-Nederlanden.

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Run the App

```bash
dotnet run
```

Open your browser at **http://localhost:5000** (or the URL shown in the terminal).

That's it — no database, no Docker, no external packages. Just `dotnet run`.

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

#### Bonus: File-Scoped Instructions
Create a `.instructions.md` file that only applies to certain files, for example `Services/.instructions.md`:

```markdown
---
applyTo: "Services/**/*.cs"
---
- All service methods must validate input parameters before processing
- Log entry and exit of every public method
- Include the enrollment ID or participant ID in all log messages
```

10. Ask Copilot to add a method to any service and see if the scoped instructions are followed

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
3. You can view Mermaid digrams in VS Code with extensions like these: Markdown Preview Mermaid Support https://marketplace.visualstudio.com/items?itemName=bierner.markdown-mermaid
4. The best diagram wins! Make it look good and easy to understand


### Exercise 3: Spec Kit Feature Addition
Ideas for features to add via Spec Kit:
- **Beneficiary Management** — Add beneficiaries per participant
- **Authentication** — User login and role-based access
- **Document Uploads** — Pension statement attachments
- **Notifications** — Email alerts for contributions
- **PDF Export** — Export projection reports
- **Pension Transfer** — Transfer from another provider
- **Dashboard Charts** — Visual contribution trends

1. Remove the copilot-instructions.md and other instruction files
2. Initialize Spec Kit in this repo 
3. Pick one or 2 features to implement
4. Go through the Spec Driven Development lifecycle for each feature
5. Have fun, and show us what you made!

#### Phase 1: Foundation
Constitution: Establish project principles
Specification: Define requirements clearly
Clarification: Resolve ambiguities

/constitution Focus on code quality, testing, UX consistency

/specify Build a photo album organizer with drag-and-drop...

/clarify # AI asks targeted questions

#### Phase 2: Implementation
Planning: Choose tech stack and architecture
Tasks: Break down into actionable items
Implementation: Generate working code


/plan Use Vite, vanilla JS, SQLite for local storage

/tasks # Generate task breakdown

/implement # Execute implementation


