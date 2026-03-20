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

## Workshop Activities

This app is designed with deliberate feature gaps for workshop exercises:

### Activity: Custom Mermaid Diagram Agent
The architecture has enough layers and relationships for interesting diagrams:
- Component diagrams (services, repos, event bus)
- Sequence diagrams (contribution flow with employer match calculation)
- Class diagrams (entity relationships)
- Flowcharts (projection calculation logic)

### Activity: Spec Kit Feature Addition
Ideas for features to add via Spec Kit:
- **Beneficiary Management** — Add beneficiaries per participant
- **Authentication** — User login and role-based access
- **Document Uploads** — Pension statement attachments
- **Notifications** — Email alerts for contributions
- **PDF Export** — Export projection reports
- **Pension Transfer** — Transfer from another provider
- **Dashboard Charts** — Visual contribution trends
