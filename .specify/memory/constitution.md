# NN Pension Planner Constitution

## Core Principles

### I. Repository → Service → Endpoint Pattern
All business logic flows through a strict layering model: **Data Layer** (Repository) → **Business Logic** (Service) → **HTTP Interface** (Endpoint). Each layer has a single responsibility. Repositories handle entity persistence only; Services orchestrate validation and domain logic; Endpoints expose REST routes. Violations create tight coupling, testing debt, and mixed concerns.

**Non-negotiable**: No direct repository access from endpoints; no business logic in repository or endpoint layers; each service has dedicated tests; all responses come from service results.

### II. Test-First Development
Tests and implementation co-evolve: Write tests first (they may fail), get stakeholder approval, then implement. This ensures testability, prevents over-engineering, and creates living documentation. Unit tests validate service layer logic; integration tests validate repository/service contracts; endpoint tests verify API contracts.

**Non-negotiable**: Critical paths (enrollment, contributions, projections) require unit + integration tests before merge; red-green-refactor cycle respected.

### III. Event-Driven Architecture & Pub/Sub
Domain events (EnrollmentCreatedEvent, ContributionAddedEvent, etc.) decouple services and enable extensibility. The EventBus singleton coordinates subscriptions. Services publish after successful operations; other services (or middleware) subscribe. This allows features like audit trails, notifications, or cascading calculations without modifying core services.

**Non-negotiable**: State changes publish events; event handlers are idempotent; no direct service-to-service calls for side effects; new flows must use event subscriptions not event parameters.

## Technology & Architecture Standards

- **Language**: C# with nullable reference types and implicit usings enabled  
- **Framework**: .NET 10 Minimal APIs  
- **Data Access**: In-memory (no external database) via generic `InMemoryRepository<T>`  
- **Persistence**: `ConcurrentDictionary` for thread-safe in-memory storage  
- **Serialization**: JSON (camelCase, enum as strings via `JsonStringEnumConverter`)  
- **Logging**: `ILogger<T>` injected; structured logging for diagnostics  
- **Errors**: Business errors throw `ArgumentException` (mapped to HTTP 400 by middleware)

## Development Workflow

- **Code Organization**: One entity → one service class; one endpoint mapper per entity group (`/api/participants`, `/api/enrollments`, etc.)  
- **Dependency Injection**: Constructor injection in all services; registered in `Program.cs`  
- **Middleware**: RequestLoggingMiddleware adds correlation IDs; ErrorHandlingMiddleware catches business errors  
- **Seed Data**: Domain seeded in-memory on app start (SeedData.cs)  
- **Frontend**: React-free vanilla JS; API calls via `/js/api.js`; UI updates in `/js/ui.js`

## Governance

**Principle-Led Review**: All PRs verify the three core principles are not violated. Code review checklist includes:
- [ ] Repository layer contains only entity access, no business logic  
- [ ] Service layer handles validation and orchestration; endpoints delegate to services  
- [ ] No direct repository calls from endpoints  
- [ ] Critical paths have unit + integration tests  
- [ ] State changes publish relevant events  
- [ ] New features use events, not direct service dependencies

**Amendments**: This constitution may be evolved to reflect learnings. Changes require:
1. Documented rationale (why current principle is insufficient)  
2. Updated principle text or new principle  
3. Migration path for existing code  
4. Version bump (MAJOR for breaking changes, MINOR for new principles, PATCH for clarifications)

**Tooling**: Use `.github/copilot-instructions.md` as the runtime development guide; this constitution defines **governance**; copilot-instructions.md defines **how-to**.

**Version**: 1.0.0 | **Ratified**: 2026-03-23 | **Last Amended**: 2026-03-23
