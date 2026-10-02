# Architecture

Fast-Native is a modular full-stack foundation. This document describes how the pieces fit together.

## Three layers

```text
Fast-Native
│
├── Application Layer
│   ├── Web (installed kit)     frontend/web/
│   ├── Windows (WinUI 3)       frontend/win/
│   ├── Android (Compose)       frontend/android/
│   └── Backend (FastAPI)       backend/app/modules/
│
├── Infrastructure Layer
│   ├── PostgreSQL + Alembic
│   ├── Redis + ARQ workers
│   ├── Traefik (routing / TLS)
│   └── Docker Compose
│
└── Control Layer
    └── __ctrl__/               dev · test · deploy · scaffold
```

## Stack

| Layer | Technology | Role |
|-------|------------|------|
| Web | SvelteKit, Next.js, Nuxt, or Rio, via `web use` | Dashboard UI in `frontend/web` |
| Windows | WinUI 3 | Native shell for `client-routes.json` |
| Android | Jetpack Compose | Native shell for `client-routes.json` |
| Backend | FastAPI + SQLModel | REST API and business logic |
| Database | PostgreSQL 18 | Persistent storage |
| Migrations | Alembic | Schema versioning |
| Jobs | ARQ + Redis 8 | Background processing |
| Proxy | Traefik 3.6 | Dev/prod routing and TLS |
| Control | `__ctrl__/` CLI | Dev, test, and deploy operations |

## Runtime profiles

Fast-Native supports two official dev/runtime modes:

| Profile | Command | Infrastructure | Apps |
|---------|---------|----------------|------|
| **Full** | `dev run all` | DB + Redis + Traefik + Adminer | uvicorn + ARQ worker + installed web kit |
| **Slim** | `dev run all --slim` | DB + Traefik + Adminer | uvicorn + installed web kit |

Slim is an official supported mode — not a fallback. Use it when the app does not need background jobs.

Production (`compose.yml`) always includes Redis + worker service.

## Request flow (backend)

```
HTTP Request
    ↓
FastAPI app (backend/app/main.py)
    ↓
API router (backend/app/api/main.py)
    ↓
Module router (backend/app/modules/.../router.py)
    ↓
Service (business logic)
    ↓
Repository (database access)
    ↓
PostgreSQL
```

### Layer responsibilities

**Router** — HTTP concerns only: path/method, dependency injection (`SessionDep`, `CurrentUser`), calling service functions, returning response models.

**Service** — Application logic: validation, authorization checks, coordinating repositories, raising `HTTPException` for business errors.

**Repository** — Persistence: queries, inserts, updates, deletes. No business rules.

**Models** — SQLModel table classes (`table=True`).

**Schemas** — Pydantic/SQLModel classes for API contracts (`NoteCreate`, `NotePublic`, etc.).

## Project layout

```
fast-native/
├── AGENTS.md                 # AI development contract
├── ROADMAP.md                # Long-term vision and goals
├── Readme.md                 # Quick start
├── package.json              # npm workspace root
├── __ctrl__/                 # CLI (dev run, test, deploy)
├── compose.dev.yml           # Dev infrastructure (db, redis, Traefik, adminer)
├── compose.yml               # Production stack
├── backend/app/
│   ├── main.py               # FastAPI entry
│   ├── api/                  # Router aggregation, shared deps
│   ├── core/                 # Config, db, security, arq
│   ├── alembics/core/        # Database migrations
│   └── modules/
│       ├── system/           # Health checks, private dev routes
│       ├── base/             # Auth, users (platform)
│       └── apps/             # Product features
├── frontend/
│   ├── client-routes.json    # /, /login, /sample/notes, /admin
│   ├── web/                  # empty until `web use` fills a kit's frontend/
│   ├── win/                  # WinUI 3 shell (modules/global + routes/)
│   └── android/              # Jetpack Compose shell
└── tests/
    ├── backend/              # pytest (mirrors backend modules)
    └── frontend/             # Vitest (config helpers, pure TS)
```

## Module types

### Platform modules (`modules/base/`)

Shared infrastructure used by every application: authentication, users. These ship with Fast-Native and are not product-specific.

Uses `crud.py` in the users submodule (predates the app-module convention).

### App modules (`modules/apps/<name>/`)

Product features you build on top of Fast-Native. Each module owns its backend and frontend code.

Standard app module structure (use only what you need):

```
backend/app/modules/apps/<name>/
├── models.py       # SQLModel tables
├── schemas.py      # API contracts
├── repository.py   # Database access
├── service.py      # Business logic
└── router.py       # HTTP endpoints
```

Register the router in `backend/app/modules/apps/router.py`.

Frontend counterpart (after `web use` — paths move under `frontend/web`):

```text
Svelte / Next   frontend/web/src/lib/modules/apps/<name>/api.ts
Nuxt            frontend/web/lib/modules/apps/<name>/api.ts
Rio             frontend/web/src/modules/apps/<name>/

Windows         frontend/win/FastNative.Client/routes/<path>/Page.cs
Android         frontend/android/app/src/routes/<path>/Page.kt
```

Add a shared path to `frontend/client-routes.json` when native clients should open it.

## Canonical example: sample notes

The `sample` module is the reference implementation. It demonstrates a complete feature lifecycle:

| Step | File |
|------|------|
| Model | `backend/app/modules/apps/sample/models.py` |
| Migration | `backend/app/alembics/core/versions/002_sample_notes.py` |
| Repository | `backend/app/modules/apps/sample/repository.py` |
| Service | `backend/app/modules/apps/sample/service.py` |
| Router | `backend/app/modules/apps/sample/router.py` |
| Web UI | Inside `frontend/web` after `web use` (sample notes route in that kit) |
| API client | Inside that kit's `modules/apps/sample/` |
| Windows page | `frontend/win/FastNative.Client/routes/sample/notes/Page.cs` |
| Android page | `frontend/android/app/src/routes/sample/notes/Page.kt` |
| Tests | `tests/backend/apps/sample/test_notes.py` |

Open `http://dashboard.localhost/sample/notes` after starting dev to see the UI.

## Frontend architecture

`frontend/web` is empty until `web use` downloads Fast-Svelte, Fast-Next, Fast-Nuxt, or Fast-Rio. That kit owns the dashboard: routes, shell, and API clients. JS kits can use the npm ecosystem (Three.js, Babylon.js, charts) inside `frontend/web`. Rio stays Python.

Windows and Android are separate clients. Shell chrome lives in `modules/global`. Pages live under `routes/` and match `frontend/client-routes.json`. They do not embed the website.

`mac`, `linux`, and `ios` are reserved folders. They do not ship a client yet.

## Database and migrations

- Connection settings: `backend/app/core/config.py` → `CORE_SQLALCHEMY_DATABASE_URI`
- Migrations run via `backend/scripts/prestart.sh` (Alembic upgrade head)
- Alembic env whitelists tables in `included_tables` — add new tables there
- Dev DB port: `localhost:5432` (published from Docker)

## Authentication

- OAuth2 password flow: `POST /api/v1/base/login/access-token`
- Current user: `GET /api/v1/base/login/me`
- Backend deps: `CurrentUser`, `SuperAdminUser` in `backend/app/api/deps.py`
- Frontend stores the token in the installed kit's auth module (Svelte/Next `stores/auth`, Nuxt `useAuth`, Rio session). Native shells keep their own session handling when they call the API.

App modules should use `CurrentUser` when endpoints require authentication.

## Configuration

| What | Where |
|------|-------|
| Secrets, DB credentials | `.env` (from `.env.example`) |
| App settings | `backend/app/core/config.py` |
| CORS, hosts | `compose.dev.yml` / `compose.yml` |
| Frontend API URL | `PUBLIC_API_BASE_URL`, `NEXT_PUBLIC_API_BASE_URL`, or `NUXT_PUBLIC_API_BASE_URL`, depending on the kit |

## Testing

Backend tests use FastAPI `TestClient` with a real dev database:

```bat
__ctrl__\fast-native-ctrl.bat test backend
```

Frontend tests use Vitest:

```bat
__ctrl__\fast-native-ctrl.bat test frontend
```

Test paths mirror module paths: `tests/backend/apps/sample/` tests `backend/app/modules/apps/sample/`.

## Background jobs (ARQ)

```
Enqueue (FastAPI)          Worker (ARQ)
      ↓                         ↓
create_arq_pool()         WorkerSettings
      ↓                         ↓
Redis ←────────────────── tasks.py
```

| File | Role |
|------|------|
| `backend/app/core/arq.py` | Redis connection + pool |
| `backend/app/worker/worker.py` | WorkerSettings (register functions) |
| `backend/app/worker/tasks.py` | Generic + app-specific tasks |

Dev: worker runs on host via `arq app.worker.worker.WorkerSettings` (full runtime only).
Prod: `worker` service in `compose.yml`.

Test enqueue (local only): `POST /api/v1/private/jobs/ping/`

## Module scaffolding

```bat
__ctrl__\fast-native-ctrl.bat app create myfeature
```

Creates the backend module, plus a web client and route stub inside `frontend/web` for the installed kit, and a backend test. It does not add Windows or Android pages. Add those, and a path in `frontend/client-routes.json`, when the feature should appear in the native shells. Extend using the sample module as reference.

## What belongs in core vs apps

**Core** (Fast-Native foundation): auth, database, migrations, CLI, deployment, testing infrastructure, modular conventions.

**Apps** (your product): any feature specific to what you are building — notes, orders, dashboards, AI workflows, 3D viewers, etc.

Before adding something to core, ask: *Could this be useful for fundamentally different applications?* If not, it belongs in `modules/apps/`.

## Logging and error handling

Fast-Native intentionally uses the **simple approach**: services raise `HTTPException` for business errors. No separate application-exception hierarchy.

| Context | Approach |
|---------|----------|
| Application code | `logging.getLogger(__name__)` |
| Business errors (not found, forbidden, validation) | `HTTPException` in **service** layer |
| Auth failures | `HTTPException` in deps/routers |
| Worker tasks | Log + ARQ retry via `max_tries` |
| Startup / prestart | `logging` in `backend_pre_start.py`, `initial_data.py` |
| Unexpected exceptions | FastAPI default 500 handling |

Do not add a separate error-handling framework or custom JSON error envelopes. See [conventions.md](conventions.md).

## File storage (extension point)

Fast-Native does **not** ship a storage abstraction yet. When applications need uploads or generated files:

- Implement storage access in the **feature module's service layer**
- Prefer an interface that could later swap local disk vs S3-compatible backends
- Do not hard-code AWS/Azure/GCP into core without a generality review

A future `backend/app/core/storage/` module may formalize this. Until then, keep module-local and documented.

## Documentation index

| Doc | Topic |
|-----|-------|
| [modules.md](modules.md) | Building features |
| [cli.md](cli.md) | `__ctrl__` commands |
| [runtime-profiles.md](runtime-profiles.md) | Full vs Slim |
| [background-jobs.md](background-jobs.md) | ARQ tasks |
| [development.md](development.md) | Local workflow |
| [testing.md](testing.md) | pytest, Vitest |
| [database.md](database.md) | Migrations, volumes |
| [deployment.md](deployment.md) | Production |
| [conventions.md](conventions.md) | Naming, responses, cross-module rules |
