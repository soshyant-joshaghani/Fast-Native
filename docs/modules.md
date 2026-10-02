# Modules

Product features live in **app modules** — one ownership boundary per feature.

Before creating a new pattern, inspect the **canonical `sample` module** (notes CRUD). It is the reference implementation for how features are structured in Fast-Native.

## Locations

| Layer | Path |
|-------|------|
| Backend | `backend/app/modules/apps/<name>/` |
| Web client | Inside `frontend/web` after `web use` (`src/lib/modules/apps` for Svelte and Next, `lib/modules/apps` for Nuxt, `src/modules/apps` for Rio) |
| Web route | That kit's router under `frontend/web` |
| Windows page | `frontend/win/FastNative.Client/routes/<path>/Page.cs` when the path is shared |
| Android page | `frontend/android/app/src/routes/<path>/Page.kt` when the path is shared |
| Backend tests | `tests/backend/apps/<name>/` |
| Frontend tests | `tests/frontend/` (as needed) |

Platform code (not your product): `modules/base/` (auth), `modules/system/` (health).

Shared web shell: `frontend/web/.../modules/base/` after `web use` (design primitives at `base/ui/` on JS kits). There is no project `components/` folder — modules are the component home. Native chrome stays in `modules/global`.

## Backend layers

Use only what the feature needs:

```
Router → Service → Repository → Database
```

| File | When |
|------|------|
| `router.py` | Always — HTTP endpoints |
| `service.py` | Business rules, validation, orchestration |
| `repository.py` | Database queries |
| `models.py` | SQLModel tables |
| `schemas.py` | API input/output |

Register the router in `backend/app/modules/apps/router.py`.

Naming and cross-module rules: [conventions.md](conventions.md).

## Scaffold

```bat
__ctrl__\fast-native-ctrl.bat app create myfeature
```

Creates the backend module, a web client and route stub for the installed kit, and a backend test. Add native pages yourself when the route is shared. Extend using the sample module as reference.

## Canonical example: `sample`

The **notes** module is the reference implementation. Inspect before building anything new:

| Step | Location |
|------|----------|
| Model | `backend/app/modules/apps/sample/models.py` |
| Migration | `backend/app/alembics/core/versions/002_sample_notes.py` |
| Repository | `backend/app/modules/apps/sample/repository.py` |
| Service | `backend/app/modules/apps/sample/service.py` |
| Router | `backend/app/modules/apps/sample/router.py` |
| Web UI | Inside `frontend/web` (sample notes route of the installed kit) |
| API client | That kit's `modules/apps/sample/` |
| Windows | `frontend/win/FastNative.Client/routes/sample/notes/Page.cs` |
| Android | `frontend/android/app/src/routes/sample/notes/Page.kt` |
| Tests | `tests/backend/apps/sample/test_notes.py` |

UI: http://dashboard.localhost/sample/notes

## Web API clients

After `web use`, TypeScript clients live under that kit's `modules/apps/<name>/`:

- Svelte and Next: `frontend/web/src/lib/modules/apps/<name>/api.ts`
- Nuxt: `frontend/web/lib/modules/apps/<name>/api.ts`
- Rio: `frontend/web/src/modules/apps/<name>/api.py`

Import the API base URL from the kit's config. Use the kit's auth store for headers. Export typed functions (`listNotes`, `createNote`, and so on) — see the sample module inside `frontend/web`.

Keep routes thin: the page imports the module client and handles UI state. Native pages call the same HTTP API from C# or Kotlin.

## Rules

- Keep feature code in the feature module — avoid scattering helpers globally.
- Simple features stay simple — do not add empty layers for ceremony.
- Add a migration when the schema changes (see [database.md](database.md)).
- Add tests for meaningful behavior (see [testing.md](testing.md)).
- Use npm for frontend-only dependencies (3D libraries, charts, and so on) inside `frontend/web` after a JS kit is installed.
