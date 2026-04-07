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
3. (Bonus) Make the Custom Agent use subagents that can only be run by Agents, not humans 
4. You can view Mermaid digrams in VS Code with extensions like these: Markdown Preview Mermaid Support https://marketplace.visualstudio.com/items?itemName=bierner.markdown-mermaid
5. The best diagram wins! Make it look good and easy to understand


### Exercise 3: Spec Kit Feature Addition

Pick **one or two** features from the list below and build them using the Spec Driven Development lifecycle. Each feature card tells you exactly what to build, which existing code it connects to, and what the business rules are — so you can feed the description straight into `/specify`.

#### How to start
1. Remove the `copilot-instructions.md` and other instruction files you created in Exercise 1
2. Spec Kit is already initialized in this repository — no need to download or initialize it
3. Pick a feature below (start with an **Easy** one if this is your first time)
4. Walk through the Spec Driven Development lifecycle (see bottom of this section)
5. Have fun, and show us what you made!

---

#### Feature 1: Beneficiary Management · 🟢 Easy

> Let participants designate who receives their pension benefits (spouse, children, etc.) and what percentage each beneficiary gets.

**What to build**

| Layer | File(s) to create |
|---|---|
| Model | `Models/Beneficiary.cs` |
| Service | `Services/BeneficiaryService.cs` |
| Endpoints | `Endpoints/BeneficiaryEndpoints.cs` |
| Events | Add to `Events/DomainEvents.cs` |
| Frontend | Add a "Beneficiaries" tab or section in a participant's detail view |

**Domain model — `Beneficiary`**

| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | Auto-generated |
| `ParticipantId` | `Guid` | FK → `Participant.Id` |
| `FirstName` | `string` | Required |
| `LastName` | `string` | Required |
| `Relationship` | `string` | e.g. "Spouse", "Child", "Parent", "Other" |
| `DateOfBirth` | `DateTime` | Must be in the past |
| `AllocationPercentage` | `decimal` | 1–100, represents share of pension benefits |

**API endpoints**

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/beneficiaries/participant/{participantId}` | List beneficiaries for a participant |
| `GET` | `/api/beneficiaries/{id}` | Get a single beneficiary |
| `POST` | `/api/beneficiaries` | Add a beneficiary to a participant |
| `PUT` | `/api/beneficiaries/{id}` | Update beneficiary details |
| `DELETE` | `/api/beneficiaries/{id}` | Remove a beneficiary |

**Integration points**
- Validate that `ParticipantId` exists via `IRepository<Participant>`
- Register `BeneficiaryService` in DI and `MapBeneficiaryEndpoints()` in `Program.cs`
- Add seed data in `Data/SeedData.cs` for the existing participants

**Business rules**
- The sum of `AllocationPercentage` across all beneficiaries for one participant must be ≤ 100%
- A participant can have 0–10 beneficiaries
- `FirstName` and `LastName` are required
- `DateOfBirth` must be in the past

**Events**: `BeneficiaryAdded(beneficiaryId, participantId, timestamp)`, `BeneficiaryRemoved(beneficiaryId, participantId, timestamp)`

**Bonus**: Add a "remaining allocation" indicator in the UI that shows how much percentage is still unassigned.

---

#### Feature 2: Dashboard Charts · 🟡 Medium

> Add visual charts to the dashboard showing contribution trends over time, employee vs employer splits, and projection scenario comparisons.

**What to build**

| Layer | File(s) to create/modify |
|---|---|
| Frontend | Modify `wwwroot/js/ui.js` (chart rendering), optionally add `wwwroot/js/charts.js` |
| CSS | Add chart styles to `wwwroot/css/styles.css` |
| Backend | No new models or endpoints needed — uses existing API data |

**Charts to implement**

1. **Contribution Trend** — Bar or line chart showing monthly totals over time
   - Data source: `GET /api/contributions/enrollment/{id}/summary` → array of `{ year, month, total, employeeTotal, employerTotal }`
2. **Employee vs Employer Split** — Pie or stacked bar chart
   - Data source: Same monthly summary, aggregate `employeeTotal` and `employerTotal`
3. **Projection Scenarios** — Grouped bar chart comparing Conservative / Expected / Optimistic
   - Data source: `POST /api/projections/calculate` → `scenarios[]` with `estimatedMonthlyPension` and `estimatedLumpSum`

**Integration points**
- The existing `App.loadDashboard()` in `wwwroot/js/app.js` already fetches enrollments and contributions — extend it to pass data to chart renderers
- The existing `UI.renderDashboard()` in `wwwroot/js/ui.js` is where chart HTML should be added

**Implementation approach**
- Use pure HTML5 `<canvas>` or CSS-based bar charts (no external libraries needed)
- Or use a lightweight library like Chart.js via CDN `<script>` tag in `index.html`

**Bonus**: Make the charts interactive — click a bar to see that month's individual contributions.

---

#### Feature 3: Participant Summary Report · 🟡 Medium

> Generate a comprehensive summary report for a participant that aggregates all their data: personal info, enrollments, contributions, and projections — returned as structured JSON that the frontend renders as a printable report page.

**What to build**

| Layer | File(s) to create |
|---|---|
| Service | `Services/ReportService.cs` |
| Endpoints | `Endpoints/ReportEndpoints.cs` |
| Frontend | Add a "View Report" button per participant that opens a print-friendly page |

**API endpoints**

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/reports/participant/{participantId}` | Full summary report for a participant |

**Response shape** (no new model file needed — return an anonymous/record type)
```
{
  participant: { fullName, age, email, employer, salary },
  enrollments: [
    {
      planName, status, startDate, contributionPercentage,
      totalBalance,
      latestProjection: { retirementAge, scenarios: [...] }
    }
  ],
  totalAcrossAllPlans: decimal
}
```

**Integration points**
- `ReportService` depends on: `ParticipantService`, `EnrollmentService`, `ContributionService`, `ProjectionService`
- For each enrollment, call `ContributionService.GetTotalBalance()` and `ProjectionService.GetLatest()`
- Look up plan names via `IRepository<PensionPlan>`

**Business rules**
- Return `404` if participant not found
- Include all enrollments regardless of status (Active, Suspended, Closed)
- `totalAcrossAllPlans` = sum of all enrollment balances

**Bonus**: Add a "Print Report" button that uses `window.print()` with a print-friendly CSS stylesheet.

---

#### Feature 4: Pension Transfer · 🟡 Medium

> Allow participants to transfer pension savings from a previous provider into their NN enrollment. Transfers go through a review workflow (Pending → Approved/Rejected → Completed).

**What to build**

| Layer | File(s) to create |
|---|---|
| Model | `Models/PensionTransfer.cs` |
| Service | `Services/PensionTransferService.cs` |
| Endpoints | `Endpoints/TransferEndpoints.cs` |
| Events | Add to `Events/DomainEvents.cs` |
| Frontend | Add a "Transfer In" section on the contributions page |

**Domain model — `PensionTransfer`**

| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | Auto-generated |
| `ParticipantId` | `Guid` | FK → `Participant.Id` |
| `EnrollmentId` | `Guid` | FK → `Enrollment.Id` — target enrollment |
| `PreviousProvider` | `string` | Name of the previous pension provider, required |
| `TransferAmount` | `decimal` | Must be > 0, represents the amount in euros |
| `RequestDate` | `DateTime` | Auto-set to `DateTime.UtcNow` |
| `CompletionDate` | `DateTime?` | Set when status becomes Completed |
| `Status` | `TransferStatus` | Enum: `Pending`, `Approved`, `Rejected`, `Completed` |
| `Notes` | `string` | Optional notes or reason for rejection |

**API endpoints**

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/transfers/participant/{participantId}` | List all transfers for a participant |
| `GET` | `/api/transfers/{id}` | Get a single transfer |
| `POST` | `/api/transfers` | Request a new transfer (starts as `Pending`) |
| `PUT` | `/api/transfers/{id}/approve` | Move from Pending → Approved |
| `PUT` | `/api/transfers/{id}/reject` | Move from Pending → Rejected (include reason in body) |
| `PUT` | `/api/transfers/{id}/complete` | Move from Approved → Completed (adds funds) |

**Integration points**
- Validate `ParticipantId` and `EnrollmentId` exist; enrollment must be `Active`
- On **completion**, create a `Contribution` with `Type = ContributionType.CatchUp` and `EmployeeAmount = TransferAmount` (employer amount = 0) via `ContributionService.AddContribution()` — or create the `Contribution` directly via the repository to skip employer match calculation

**Business rules**
- Can only approve/reject a `Pending` transfer
- Can only complete an `Approved` transfer
- `TransferAmount` must be > 0
- A participant can have multiple transfers but only one `Pending` transfer per enrollment at a time

**Events**: `TransferRequested(transferId, participantId, amount, timestamp)`, `TransferCompleted(transferId, enrollmentId, amount, timestamp)`

**Bonus**: Show a transfer timeline/history in the UI with status badges (Pending=yellow, Approved=blue, Completed=green, Rejected=red).

---

#### Feature 5: Notifications · 🟠 Medium-Hard

> Automatically notify participants when things happen — contributions are received, enrollment status changes, or a projection is calculated. Notifications appear in the UI as a bell icon with an unread count.

**What to build**

| Layer | File(s) to create |
|---|---|
| Model | `Models/Notification.cs` |
| Service | `Services/NotificationService.cs` |
| Endpoints | `Endpoints/NotificationEndpoints.cs` |
| Events | Subscribe to existing events in `Events/EventBus.cs` |
| Frontend | Add notification bell in navbar + notification dropdown/panel |

**Domain model — `Notification`**

| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | Auto-generated |
| `ParticipantId` | `Guid` | FK → `Participant.Id` |
| `Title` | `string` | Short title, e.g. "Contribution Received" |
| `Message` | `string` | Detail message, e.g. "€450.00 was added to your NN Flex plan" |
| `Type` | `NotificationType` | Enum: `ContributionReceived`, `EnrollmentUpdate`, `ProjectionReady`, `SystemAlert` |
| `IsRead` | `bool` | Default `false` |
| `CreatedAt` | `DateTime` | Auto-set |

**API endpoints**

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/notifications/participant/{participantId}` | All notifications (newest first) |
| `GET` | `/api/notifications/participant/{participantId}/unread` | Unread count |
| `PUT` | `/api/notifications/{id}/read` | Mark one as read |
| `PUT` | `/api/notifications/participant/{participantId}/read-all` | Mark all as read |

**Integration points — this is the interesting part!**
- The app already publishes events via `EventBus`: `ContributionAdded`, `EnrollmentCreated`, `EnrollmentStatusChanged`, `ProjectionCalculated`
- In `Program.cs` (or in `NotificationService`), subscribe to these events and auto-create notifications:
  - `ContributionAdded` → look up enrollment → get `ParticipantId` → create notification "Contribution of €{amount} received"
  - `EnrollmentStatusChanged` → create notification "Your enrollment status changed to {newStatus}"
  - `ProjectionCalculated` → create notification "New retirement projection available"

**Business rules**
- Notifications are never deleted, only marked as read
- Newest first ordering
- Unread count only counts `IsRead == false`

**Bonus**: Add a polling mechanism in the frontend that checks for new notifications every 30 seconds and updates the bell badge.

---

#### Feature 6: Document Uploads · 🔴 Hard

> Let participants upload pension-related documents (pension statements from previous employers, ID copies, tax forms) and attach them to their profile.

**What to build**

| Layer | File(s) to create |
|---|---|
| Model | `Models/Document.cs` |
| Service | `Services/DocumentService.cs` |
| Endpoints | `Endpoints/DocumentEndpoints.cs` |
| Frontend | File upload form + document list per participant |

**Domain model — `Document`**

| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | Auto-generated |
| `ParticipantId` | `Guid` | FK → `Participant.Id` |
| `FileName` | `string` | Original file name |
| `ContentType` | `string` | MIME type, e.g. `application/pdf` |
| `Content` | `byte[]` | File content stored in memory |
| `FileSize` | `long` | Size in bytes |
| `Category` | `DocumentCategory` | Enum: `PensionStatement`, `IdDocument`, `TaxForm`, `Other` |
| `UploadedAt` | `DateTime` | Auto-set |

**API endpoints**

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/documents/participant/{participantId}` | List documents (metadata only, no content) |
| `GET` | `/api/documents/{id}` | Get document metadata |
| `GET` | `/api/documents/{id}/download` | Download file content |
| `POST` | `/api/documents` | Upload a document (multipart/form-data) |
| `DELETE` | `/api/documents/{id}` | Delete a document |

**Integration points**
- Validate `ParticipantId` exists via `IRepository<Participant>`
- The upload endpoint must handle `multipart/form-data` — in Minimal APIs, bind `IFormFile` from the request
- The download endpoint returns `Results.File(content, contentType, fileName)`

**Business rules**
- Maximum file size: 5 MB
- Allowed content types: `application/pdf`, `image/jpeg`, `image/png`
- Maximum 20 documents per participant
- `FileName` must be sanitized (no path traversal characters)

**Why this is hard**: Handling multipart file uploads in .NET Minimal APIs requires specific parameter binding (`IFormFile`), and you need to think about content-type validation, file size limits, and secure file name handling. The in-memory storage of `byte[]` is also different from the typical entity pattern.

**Bonus**: Add thumbnail previews for image documents and a file type icon for PDFs.

---

#### Feature 7: Authentication & Role-Based Access · 🔴 Hard

> Add user authentication so participants can only see their own data, employers can see their employees' data, and admins can see everything.

**What to build**

| Layer | File(s) to create |
|---|---|
| Model | `Models/User.cs` |
| Service | `Services/AuthService.cs` |
| Middleware | `Middleware/AuthMiddleware.cs` (or use built-in ASP.NET auth) |
| Endpoints | `Endpoints/AuthEndpoints.cs` |
| Frontend | Login page, role-based UI visibility |

**Domain model — `User`**

| Property | Type | Notes |
|---|---|---|
| `Id` | `Guid` | Auto-generated |
| `Email` | `string` | Unique, used as username |
| `PasswordHash` | `string` | Hashed password (never store plaintext!) |
| `Role` | `UserRole` | Enum: `Participant`, `Employer`, `Admin` |
| `ParticipantId` | `Guid?` | Nullable — links to `Participant.Id` for Participant role |
| `CreatedAt` | `DateTime` | Auto-set |

**API endpoints**

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/auth/register` | Create a new user account |
| `POST` | `/api/auth/login` | Authenticate and return a token/cookie |
| `POST` | `/api/auth/logout` | End the session |
| `GET` | `/api/auth/me` | Get current user info |

**Integration points**
- Add authorization checks to **all existing endpoints**:
  - `Participant` role: can only access their own data (filter by their `ParticipantId`)
  - `Employer` role: can access data for participants with matching `EmployerName`
  - `Admin` role: unrestricted access
- Add seed users in `SeedData.cs` linked to existing participants
- Use ASP.NET built-in cookie auth or JWT — cookie auth is simpler for this app

**Business rules**
- Passwords must be hashed (use `BCrypt` or ASP.NET's `PasswordHasher<T>`)
- Email must be unique
- Registration can optionally link to an existing participant by email match
- Failed login attempts should not reveal whether the email exists

**Why this is hard**: This touches every layer of the application. Every existing endpoint needs authorization logic, the frontend needs a login flow, and you need to handle password security correctly.

**Bonus**: Add a "Switch User" dropdown in the UI (for demo purposes) that lets you quickly test different roles.

---

#### Spec Driven Development Lifecycle

Use these Spec Kit commands to go from feature description to working code:

**Phase 1: Foundation**
- **Constitution** — Establish project principles

  `/constitution Focus on code quality, testing, UX consistency`

- **Specification** — Define requirements clearly (paste a feature description from above!)

  `/specify Add beneficiary management so participants can designate who receives their pension benefits...`

- **Clarification** — Resolve ambiguities

  `/clarify`


**Phase 2: Implementation**
- **Planning** — Choose tech stack and architecture

  `/plan`

- **Tasks** — Break down into actionable items

  `/tasks`

- **Implementation** — Generate working code

  `/implement`


