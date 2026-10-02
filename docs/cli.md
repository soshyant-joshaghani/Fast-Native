# CLI (`__ctrl__`)

`__ctrl__/` is the **control layer** for Fast-Native — the official interface for dev, test, deploy, and SSH ops. Prefer these commands over ad-hoc `docker compose` or manual process management.

Entry points:

```bat
__ctrl__\fast-native-ctrl.bat <command>
```

```bash
__ctrl__/fast-native-ctrl.sh <command>
```

Full reference: [`__ctrl__/README.md`](../__ctrl__/README.md)

## Local setup

```bat
fast-native-ctrl.bat setup-local
fast-native-ctrl.bat setup-local --force   # recreate .venv
```

Creates project `.venv`, installs `requirements.txt`, and runs `npm install` for the workspace.

## Development

```bat
fast-native-ctrl.bat dev run all
fast-native-ctrl.bat dev run all --slim
fast-native-ctrl.bat dev stop all
fast-native-ctrl.bat dev down all
fast-native-ctrl.bat dev purge infra
fast-native-ctrl.bat dev reset all
```

| Target | Meaning |
|--------|---------|
| `infra` | Docker: db, redis (full), proxy, adminer + migrations |
| `apps` | Host: uvicorn :8000, ARQ worker (full), installed web kit :5000 |
| `all` | Both (run order: infra → apps; stop: apps → infra) |

See [runtime-profiles.md](runtime-profiles.md) for `--slim`.

## Web kit

```bat
fast-native-ctrl.bat web list
fast-native-ctrl.bat web use svelte
fast-native-ctrl.bat web status
fast-native-ctrl.bat web update
```

`svelte` can be `next`, `nuxt`, or `rio`. Switching kits needs `--replace`. `web update --force` overwrites local edits in `frontend/web`.

## Native clients

```bat
fast-native-ctrl.bat native list
fast-native-ctrl.bat native run win
fast-native-ctrl.bat native run android
```

## Module scaffold

```bat
fast-native-ctrl.bat app create myfeature
```


## Logs

```bat
REM Production VM (SSH)
fast-native-ctrl.bat logs api
fast-native-ctrl.bat logs db --no-follow

REM Local development
fast-native-ctrl.bat dev logs api
fast-native-ctrl.bat dev logs db

REM Local compose.yml smoke
fast-native-ctrl.bat prod logs api
```

`logs` is production (SSH). `dev logs` is local. `prod logs` is local `compose.yml` smoke.


## Flatten / restore-flat (git history)

```bat
REM Rewrite history to a single __init__ commit + force-push (DANGER)
fast-native-ctrl.bat flatten --yes

REM Adopt rewritten history on this laptop
fast-native-ctrl.bat restore-flat
fast-native-ctrl.bat restore-flat --yes

REM Adopt on production VM (git only — then update/start to rebuild)
fast-native-ctrl.bat restore-flat --server fast-native --yes
```

Plain `git pull` fails after flatten; use `restore-flat` instead.

## Tests

```bat
fast-native-ctrl.bat test all
fast-native-ctrl.bat test backend
fast-native-ctrl.bat test frontend
fast-native-ctrl.bat test contract --base http://localhost:8000   # wire contract vs a running API
```

## Production (SSH from laptop)

```bat
fast-native-ctrl.bat setup
fast-native-ctrl.bat pubkey
fast-native-ctrl.bat clone
fast-native-ctrl.bat env
fast-native-ctrl.bat start
fast-native-ctrl.bat stop
fast-native-ctrl.bat update
fast-native-ctrl.bat status
fast-native-ctrl.bat connect
```

## Local prod smoke (Docker Desktop)

```bat
fast-native-ctrl.bat prod start
fast-native-ctrl.bat prod stop
fast-native-ctrl.bat prod reset
```

On-VM scripts: `__ctrl__/remote/` (invoked by SSH commands above).
