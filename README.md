# todo-app-demo-st-live

Monorepo POC: a Todo-list system with a clean-architecture ASP.NET Web API backend and a React (Vite + TS) SPA frontend. **In-memory** persistence only — data resets on API restart.

## Layout

```
apps/
├─ api/        # ASP.NET 9 solution (Domain / Application / Infrastructure / Api + tests)
└─ web/        # Vite + React + TypeScript SPA
```

## Prerequisites

- .NET 9 SDK
- Node.js 20+ and pnpm 9+ (`npm i -g pnpm`)

## Install

```powershell
pnpm install
```

## Run (dev)

```powershell
pnpm dev
```

Runs API + web concurrently. Or split:

- API: `pnpm dev:api` → http://localhost:5080 (Swagger at `/swagger`)
- Web: `pnpm dev:web` → http://localhost:5173 (proxies `/api` to the backend)

The API seeds sample tasks/projects in Development when `Seed=true` (set in `launchSettings.json`).

## Test / Build

```powershell
pnpm test            # dotnet test on the solution
pnpm build           # builds API (Release) and web (Vite production bundle)
```

## Features (Phase 1 — implemented)

- Tasks: CRUD, status (`Todo|InProgress|Done`), priority (`Low|Medium|High|Urgent`), projects, labels, due dates, reminders, recurring (daily/weekly/monthly).
- Projects + labels CRUD.
- Smart filters: Today, Overdue, Upcoming (`/api/tasks/filters/{today|overdue|upcoming}`).
- Activity log (`/api/activity`) — actor is `"system"` (no auth in POC).
- Reminder background service polls every 30s and logs `ReminderFired` activity entries.
- Completing a recurring task auto-creates the next occurrence.

## Planned for Phase 2 (not implemented)

Subtasks / checklists · Attachments (with `IAttachmentStorage` abstraction) · Import / export · Auth / multi-user · Persistent database (EF Core swap-in).

## Architecture notes

- Clean layers: `Domain` (entities, value objects, `IClock`) → `Application` (DTOs, repository **interfaces**, services) → `Infrastructure` (in-memory repos, `SystemClock`, `ReminderBackgroundService`) → `Api` (controllers, DI, Swagger, CORS).
- Repositories live behind interfaces — swap `InMemory*Repository` for an EF Core implementation without touching application code.
- All datetimes are UTC.