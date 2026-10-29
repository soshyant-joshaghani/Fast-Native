# Development

## Launchpad workflow

Fast-Native is designed so you focus on product requirements, not repeated architecture decisions:

```text
Clone → web use <kit> → setup-local → dev run all
    → give AI your feature requirements
    → AI reads AGENTS.md + inspects sample module
    → AI extends modules inside existing architecture
    → test all → deploy
```

You provide: product rules, features, UI requirements, integrations.
Fast-Native provides: structure, conventions, control CLI, testing, deployment path.
AI helps implement the product inside the guardrails. See [AGENTS.md](../AGENTS.md).

## First run

```bat
copy .env.example .env
__ctrl__\fast-native-ctrl.bat web use svelte
__ctrl__\fast-native-ctrl.bat setup-local
__ctrl__\fast-native-ctrl.bat dev run all
```

`web use` accepts `svelte`, `next`, `nuxt`, or `rio`. `setup-local` creates `.venv`, installs Python deps from `requirements.txt`, and runs `npm install` for the `frontend/web` workspace when that kit is JavaScript.

Choose Full or Slim: [runtime-profiles.md](runtime-profiles.md)

## Hot reload

- **API:** uvicorn `--reload` on port 8000 (host)
- **Web UI:** the installed kit on port 5000 (host). Svelte and Next use Vite or the Next dev server via `npm run dev -w frontend`. Nuxt runs inside `frontend/web`. Rio runs `python -m rio run --port 5000 --public`.
- **Infra:** Docker Compose (`compose.dev.yml`)

Edit Python and the installed kit's source. Services restart from the dev servers. Native shells rebuild with `native run win` or `native run android`.

## Manual run (without `__ctrl__`)

Prefer `__ctrl__` for normal development. Use manual commands only when debugging individual services.

Infra only (full — includes Redis):

```bat
docker compose -f compose.dev.yml up -d db redis proxy adminer
```

Infra only (slim — no Redis):

```bat
docker compose -f compose.dev.yml up -d db proxy adminer
```

API only:

```bat
cd backend
set PYTHONPATH=.
..\.venv\Scripts\python.exe -m uvicorn app.main:app --reload --port 8000
```

Frontend only (after `web use`; Svelte or Next):

```bat
npm run dev
```

Worker (full stack):

```bat
cd backend
..\.venv\Scripts\python.exe -m arq app.worker.worker.WorkerSettings
```

## npm workspace

Root `package.json` defines an npm workspace whose only member is `frontend/web`:

```bat
npm run dev       # installed JS kit dev server
npm run build     # production build of that kit
npm run test      # Vitest (tests/frontend/)
```

Install dependencies after `web use`: `__ctrl__\fast-native-ctrl.bat setup-local` or `npm install` from repo root.

## Configuration

| What | Where |
|------|-------|
| Secrets, DB/Redis credentials | `.env` |
| App settings | `backend/app/core/config.py` |
| CORS, hosts, domain | `compose.dev.yml` / `compose.yml` |
| Frontend → API URL | `PUBLIC_API_BASE_URL`, `NEXT_PUBLIC_API_BASE_URL`, or `NUXT_PUBLIC_API_BASE_URL` |
| Dev API proxy | `API_PROXY_TARGET` (default `http://localhost:8000`) on kits that proxy `/api` |

Do not read `os.environ` scattered across app code — use `settings` from `core/config.py`.

## Private dev routes

When `ENVIRONMENT=local`, FastAPI exposes `/api/v1/private/*` (signup without auth, job ping test). Not available in production.

## Web kit

`frontend/web` is whatever `web use` installed. JS kits are ordinary TypeScript apps in that folder. Install npm packages there for:

- 3D/WebGL: Three.js, Babylon.js, Threlte (Svelte) or the equivalent for that kit
- Charts: Chart.js, D3
- UI: that kit's component libraries

Rio has no npm frontend. Keep backend logic in Python either way. Native shells stay in `frontend/win` and `frontend/android` and do not import the web kit.
