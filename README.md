[![](./FoxG-Kit.png)](./FoxG-Kit.png)

# Fast-Native

**One backend. A web kit you choose. Native Windows and Android shells.**

Structured for developers. Guided for AI. From idea to deployable service.

Fast-Native is a **product-agnostic launchpad** — not a finished product and not a fifth UI framework. Use the same foundation for dashboards, CRUD apps, APIs, SaaS products, 3D/WebGL experiences, background-processing services, or AI-powered applications. The difference is the **modules you add** and **which web kit you install**, not a different project architecture.

`frontend/web` stays empty until the CLI downloads Fast-Svelte, Fast-Next, Fast-Nuxt, or Fast-Rio from GitHub. Windows and Android ship in this repo as native clients of the same API. They are not a window around the website.

**Documentation:** [AGENTS.md](AGENTS.md) · [ROADMAP.md](ROADMAP.md) · [docs/](docs/) · [frontends](frontend/README.md)

---

## What Fast-Native provides

Fast-Native answers the same architectural questions as the other FoxG kits, then adds a switchable web slot and native shells:

| Pillar | What you get |
|--------|--------------|
| **Architecture** | Modular FastAPI backend, a web UI installed from one of the four kits, and native Windows and Android clients |
| **Modularity** | App modules with a canonical `sample` notes implementation |
| **Web kit slot** | `web use` copies that kit's `frontend/` into `frontend/web` and writes `frontend/kit.lock.json` |
| **Native shells** | WinUI 3 and Jetpack Compose apps that share [frontend/client-routes.json](frontend/client-routes.json) |
| **Project control** | `__ctrl__` CLI — web kit, native build, dev, test, and deploy |
| **AI guardrails** | [AGENTS.md](AGENTS.md) — conventions agents follow instead of reinventing structure |
| **Development workflow** | Hot reload, scaffolding, local tooling |
| **Testing** | pytest + Vitest layouts mirroring modules |
| **Deployment** | SSH/VM workflow from laptop to production. Production `clone` does not fetch the kit again |

You provide product requirements, features, business rules, UI needs, and integrations. Fast-Native provides structure, conventions, lifecycle control, background processing, testing, deployment, and the native clients.

> **Python backend + a chosen web kit = the dashboard.** FastAPI for API. PostgreSQL for data. Redis + ARQ for background jobs. WinUI 3 and Jetpack Compose for the native shells. `__ctrl__` for the workflow.

---

## Why it exists

The four UI kits are the same foundation with different frontends. A product that also needs Windows and Android should not fork four stacks, and it should not wrap the website in a WebView and call that the app.

```text
Idea
  ↓
Fast-Native (backend + web slot + native shells)
  ↓
web use svelte | next | nuxt | rio
  ↓
AI-assisted development inside that kit and the native routes
  ↓
Testing
  ↓
Deployment
  ↓
Working service + native clients
```

Fast-Native does not build products for you. It provides the runway; AI helps implement features **inside** the existing architecture.

---

## Architecture at a glance

The web folder is the only variant. Backend, infra, control, and the native shells stay in this repo.

```text
Fast-Native
│
├── Application Layer
│   ├── Web (SvelteKit | Next.js | Nuxt | Rio)   frontend/web/   ← web use
│   ├── Windows (WinUI 3)                        frontend/win/
│   ├── Android (Jetpack Compose)                frontend/android/
│   └── Backend (FastAPI)                        backend/app/modules/
│
├── Infrastructure Layer
│   ├── PostgreSQL + Alembic
│   ├── Redis + ARQ workers
│   ├── Traefik (routing / TLS)
│   └── Docker Compose
│
└── Control Layer
    └── __ctrl__/               web · native · dev · test · deploy · scaffold
```

**Backend flow:** Router → Service → Repository → Database

**Canonical example:** the `sample` notes module — inspect it before creating new patterns. UI: http://dashboard.localhost/sample/notes

Details: [docs/architecture.md](docs/architecture.md) · [docs/modules.md](docs/modules.md) · [frontend/README.md](frontend/README.md)

---

## Stack

| Layer | Tech | Dev URL |
|-------|------|---------|
| Web | SvelteKit, Next.js, Nuxt, or Rio, installed into `frontend/web` | http://dashboard.localhost |
| Windows | WinUI 3, Windows App SDK 1.6 | `native run win` |
| Android | Jetpack Compose | `native run android` |
| Backend | FastAPI + SQLModel + Alembic | http://api.localhost/docs · http://api.localhost/sdoc |
| Database | Postgres 18 | localhost:5432 |
| Jobs | ARQ + Redis 8 | localhost:6379 · worker on host (full runtime) |
| Proxy | Traefik 3.6 | http://localhost:8080 |
| Adminer | Adminer (via Traefik) | http://adminer.localhost |

`mac`, `linux`, and `ios` under `frontend/` are reserved client folders. They are not built yet.

---

## Runtime profiles

Both are **official** supported modes — not “full vs broken.”

| Profile | Command | Includes |
|---------|---------|----------|
| **Full** | `dev run all` | Postgres, Redis, ARQ worker, Traefik, Adminer, uvicorn, the installed web kit |
| **Slim** | `dev run all --slim` | Postgres, Traefik, Adminer, uvicorn, the installed web kit (no Redis / no worker) |

- **Full** — background jobs, queues, async work, or when you want the complete stack locally.
- **Slim** — CRUD, auth, simple APIs, faster startup, lower resource use. A valid lightweight profile, not a workaround.

Production always runs the full stack (Postgres, Redis, worker). See [docs/runtime-profiles.md](docs/runtime-profiles.md).

`setup-local`, `dev run`, and `test frontend` stop when no kit is installed. They do not pick Svelte for you.

---

## Quick start (dev)

From `fast-native/`:

```bat
__ctrl__\fast-native-ctrl.bat web list
__ctrl__\fast-native-ctrl.bat web use svelte
__ctrl__\fast-native-ctrl.bat setup-local
__ctrl__\fast-native-ctrl.bat dev run all
```

`svelte` can be `next`, `nuxt`, or `rio`. `web use` copies that repo's `frontend/` into `frontend/web` and writes `frontend/kit.lock.json`.

`setup-local` creates `.venv`, installs Python deps, and runs `npm install` for the workspace (`frontend/web`). Rio does not need the npm workspace.

| Service | URL |
|---------|-----|
| Dashboard | http://dashboard.localhost |
| Sample Notes (canonical example) | http://dashboard.localhost/sample/notes |
| API (Swagger) | http://api.localhost/docs |
| API (Scalar) | http://api.localhost/sdoc |
| Adminer | http://adminer.localhost |
| Traefik | http://localhost:8080 |
| Direct web | http://localhost:5000 |
| Direct API | http://localhost:8000/docs |

Linux/mac:

```bash
chmod +x __ctrl__/fast-native-ctrl.sh
__ctrl__/fast-native-ctrl.sh web use svelte
__ctrl__/fast-native-ctrl.sh setup-local
__ctrl__/fast-native-ctrl.sh dev run all
```

Stop: `__ctrl__\fast-native-ctrl.bat dev stop all`

**Port 80/443 conflict:** only one Traefik-on-`:80` stack at a time. Fast-Native claims the same `dashboard.localhost` and `api.localhost` hosts as the other kits. Stop the other proxy, or run apps only: `dev run apps` (direct `http://localhost:5000` / `http://localhost:8000/docs`).

### Native clients

```bat
__ctrl__\fast-native-ctrl.bat native run win
__ctrl__\fast-native-ctrl.bat native run android
```

Windows builds `FastNative.Client` without an MSIX package and launches the exe. Android runs `gradlew assembleDebug` and, when adb sees a device, installs and launches `fastnative.client`.

---

## Web commands

| Command | Effect |
|---------|--------|
| `web list` | Show Svelte, Next, Nuxt, and Rio |
| `web use <kit>` | Download that repo's `frontend/` into `frontend/web` |
| `web use <kit> --replace` | Switch kits (required when a kit is already installed) |
| `web status` | Show the lock and whether `frontend/web` was edited |
| `web update` | Re-fetch the locked branch; `--force` overwrites local edits |

Commit `frontend/web` and `frontend/kit.lock.json` when a product should ship the chosen UI. Production `clone` does not fetch the kit again.

---

## AI-assisted development

1. Install a kit (`web use svelte`, `next`, `nuxt`, or `rio`)
2. Start the project (`dev run all` or `--slim` as appropriate)
3. Point the AI at [AGENTS.md](AGENTS.md), the **sample** module, and [frontend/client-routes.json](frontend/client-routes.json)
4. AI implements inside existing layers — backend, the installed kit, and native route pages for shared paths
5. Run `test all` before deploy

Do not paste a kit's React, Svelte, Vue, or Rio UI into the Windows or Android apps. Re-implement the same route in that shell.

See [docs/development.md](docs/development.md).

---

## Adding a feature

1. Inspect the **canonical sample module** (`sample` — notes CRUD)
2. Scaffold (recommended): `__ctrl__\fast-native-ctrl.bat app create myfeature`
3. Backend: `backend/app/modules/apps/<name>/`
4. Web client: under `frontend/web` after `web use` (path depends on the kit — see the table below)
5. Native pages, when the path is shared: `frontend/win/.../routes/` and `frontend/android/.../routes/`, and add the path to `frontend/client-routes.json`
6. Migration if schema changes; tests under `tests/backend/` and `tests/frontend/`

Use the smallest appropriate implementation. Not every feature needs every layer — match the sample module's depth for similar CRUD features.

### Frontend modules (mandatory)

The web modules root is the one that kit uses, under `frontend/web`:

| Kit | Modules root after `web use` | Route home |
|-----|------------------------------|------------|
| Svelte | `frontend/web/src/lib/modules/` | `frontend/web/src/routes/` |
| Next | `frontend/web/src/lib/modules/` | `frontend/web/src/app/` |
| Nuxt | `frontend/web/lib/modules/` | `frontend/web/pages/` |
| Rio | `frontend/web/src/modules/` | `frontend/web/src/pages/` |

Under that modules root there are **only**:

- `base/` — kit/platform (auth, users, shell, stores) + design primitives at `base/ui/` when the kit uses shadcn
- `apps/<domain>/` — product domains (API clients + UI), mirroring `backend/app/modules/apps/<domain>/`

There is **no** project `components/` folder as the app UI home. Modules are the component home.
Do not add `global/`, `shell/`, `layout/`, or a top-level `modules/ui/` peer of `base`/`apps` inside the web kit.

Native shells are different on purpose. Shell chrome lives in `modules/global`. Pages live in `routes/` and follow [frontend/client-routes.json](frontend/client-routes.json).

---

## Project layout

```text
fast-native/
├── AGENTS.md                 # AI development contract
├── ROADMAP.md                # Vision and principles
├── package.json              # npm workspace; workspaces: frontend/web
├── __ctrl__/                 # Control layer — web, native, dev, test, deploy
├── compose.dev.yml           # Dev infra (db, redis, Traefik, adminer)
├── compose.yml               # Production stack (builds frontend/web)
├── backend/app/
│   ├── core/                 # config, db, security, arq
│   ├── worker/               # ARQ WorkerSettings + tasks
│   └── modules/
│       ├── base/             # auth, users
│       ├── system/           # health, private dev routes
│       └── apps/             # your product modules (+ sample/)
├── frontend/
│   ├── client-routes.json    # shared paths: /, /login, /sample/notes, /admin
│   ├── kit.lock.json         # written by web use
│   ├── web/                  # empty until web use
│   ├── win/                  # WinUI 3
│   ├── android/              # Jetpack Compose
│   ├── mac/ · linux/ · ios/  # reserved, not implemented
└── tests/
    ├── backend/              # pytest (mirrors backend module paths)
    └── frontend/             # vitest
```

---

## Tests

```bat
__ctrl__\fast-native-ctrl.bat test all
```

Backend needs the dev DB (`dev run infra` → `localhost:5432`). Frontend Vitest needs a kit installed. See [docs/testing.md](docs/testing.md).

Wire contract: tier STARTER. The HTTP API follows [CONTRACT.md](../../../CONTRACT.md). With the API running, `__ctrl__\fast-native-ctrl.bat test contract [--base URL]` runs `tests/contract/contract_test.py` (not part of `test all`).

After Alembic or Postgres volume changes: `dev purge infra`, then `dev run infra`.

---

## Production

From laptop (SSH):

```bat
__ctrl__\fast-native-ctrl.bat setup
__ctrl__\fast-native-ctrl.bat clone
__ctrl__\fast-native-ctrl.bat env
__ctrl__\fast-native-ctrl.bat start
```

On VM: `bash __ctrl__/remote/setup-ubuntu.sh` then `bash __ctrl__/remote/start-prod.sh`

Commit the installed `frontend/web` before `clone`. The server does not run `web use`.

See [docs/deployment.md](docs/deployment.md) · [`__ctrl__/README.md`](__ctrl__/README.md)

---

## Relationship to the other FoxG kits

| Kit | Role |
|-----|------|
| [Fast-Next](https://github.com/soshyant-joshaghani/Fast-Next) | General-purpose foundation (Next.js UI) |
| [Fast-Svelte](https://github.com/soshyant-joshaghani/Fast-Svelte) | General-purpose foundation (SvelteKit UI) |
| [Fast-Nuxt](https://github.com/soshyant-joshaghani/Fast-Nuxt) | General-purpose foundation (Nuxt UI) |
| [Fast-Rio](https://github.com/soshyant-joshaghani/Fast-Rio) | General-purpose Python full-stack foundation (Rio UI) |
| **Fast-Native** (this repo) | Same backend and control idea, plus `web use` and the Windows and Android shells |

Shared-layer changes (backend, Alembic, compose, `__ctrl__` lifecycle, dashboard route contract) still belong in Fast-Next, Fast-Svelte, Fast-Nuxt, and Fast-Rio, and they belong here too. Web UI changes belong in the kit you installed, then `web update` pulls them in. Windows and Android live only here. Do not paste React/Svelte/Vue/Rio UI into the native shells.

Workspace index: [fast-template/README.md](../README.md)

---

## Adminer & database

| Context | Server | Port |
|---------|--------|------|
| Adminer (browser) | `db` | `5432` |
| Host / IDE / pytest | `localhost` | `5432` |

Credentials from `.env` (`POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`).

**Troubleshooting:** [docs/database.md](docs/database.md)

---

## Environment

Copy `.env.example` → `.env`. URLs and CORS in `compose.*.yml`; secrets in `.env`.

Default superuser: `admin@example.com` / `FIRST_SUPERUSER_PASSWORD` from `.env`.

The web kit's API URL depends on the stack you installed:

| Kit | Variable |
|-----|----------|
| Svelte, Rio | `PUBLIC_API_BASE_URL` |
| Next | `NEXT_PUBLIC_API_BASE_URL` |
| Nuxt | `NUXT_PUBLIC_API_BASE_URL` |

---

## Documentation index

| Doc | Answers |
|-----|---------|
| [AGENTS.md](AGENTS.md) | Rules for AI coding agents |
| [ROADMAP.md](ROADMAP.md) | Vision, principles, long-term goals |
| [frontend/README.md](frontend/README.md) | Web slot, Windows, Android, reserved clients |
| [docs/architecture.md](docs/architecture.md) | Layers, boundaries, core vs apps |
| [docs/modules.md](docs/modules.md) | How to build a feature module |
| [docs/cli.md](docs/cli.md) | `__ctrl__` commands |
| [docs/runtime-profiles.md](docs/runtime-profiles.md) | Full vs Slim |
| [docs/background-jobs.md](docs/background-jobs.md) | Redis, ARQ, adding tasks |
| [docs/development.md](docs/development.md) | Local workflow, AI-assisted dev |
| [docs/testing.md](docs/testing.md) | pytest, Vitest, test layout |
| [docs/database.md](docs/database.md) | Migrations, Adminer, volumes |
| [docs/deployment.md](docs/deployment.md) | Dev → production path |
| [docs/dashboard-ui.md](docs/dashboard-ui.md) | Shared dashboard UX across FoxG kits |
| [docs/conventions.md](docs/conventions.md) | Naming, API responses, cross-module rules |
