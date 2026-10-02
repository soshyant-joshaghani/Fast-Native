# fast-native `__ctrl__`

The **control layer** for Fast-Native — one CLI for the web kit, native shells, and the project lifecycle.

```bat
fast-native-ctrl.bat web use svelte
fast-native-ctrl.bat setup-local
fast-native-ctrl.bat dev run all
fast-native-ctrl.bat native run win
fast-native-ctrl.bat native run android
```

## Command map

| Area | Commands |
|------|----------|
| Web kit | `web list` · `web use {svelte\|next\|nuxt\|rio}` · `web status` · `web update` |
| Native clients | `native list` · `native build {android\|win\|all}` · `native run {android\|win}` · `native clean {android\|win\|all}` |
| Local tooling | `setup-local [--force]` |
| Dev stack | `dev run\|stop\|down\|purge\|reset {infra,apps,all}` · `--slim` for lightweight runtime |
| App scaffold | `app create <name>` |
| Tests | `test {all,backend,frontend}` |
| Local prod smoke | `prod start\|stop\|reset\|backup-acme\|…` |
| SSH / VM | `setup`, `pubkey`, `clone`, `env`, `start`, `stop`, `update`, `reset`, `backup-acme`, `connect`, … |

`web use` downloads that kit's `frontend/` from GitHub into `frontend/web` and writes `frontend/kit.lock.json`. `setup-local`, `dev run`, and `test frontend` stop until a kit is installed. Switching kits needs `web use <kit> --replace`. `web update` re-fetches the locked branch and stops when `frontend/web` has local edits unless you pass `--force`.

## Layout

| Path | Role |
|------|------|
| `kits.json` | Svelte, Next, Nuxt, and Rio download profiles |
| `platforms.json` | Windows and Android build paths |
| `servers.json` | Single VM entry |
| `safe/` | PEM, address, prod `.env` |
| `static/gpg` | Docker Ubuntu GPG (Iran bootstrap) |
| `remote/` | On-VM / local-prod compose scripts |
| `fast-native-ctrl.bat` / `.sh` | CLI entry |

## Local dev

`dev run` starts the backend on :8000 and the installed web kit on :5000. Svelte and Next use `npm run dev -w frontend`. Nuxt runs inside `frontend/web`. Rio runs `python -m rio run --port 5000 --public`.

Production compose builds `frontend/web` (`context: frontend/web`). `web use` rewrites the extracted Dockerfiles for that context.

## Native clients

```bat
fast-native-ctrl.bat native list
fast-native-ctrl.bat native build win
fast-native-ctrl.bat native run android
```

Windows is `dotnet build` of `FastNative.Client` without an MSIX package, then launch of the exe. Android is `gradlew assembleDebug`, then install and launch when adb sees a device.

## Quick start (Windows)

From `fast-native/__ctrl__/`:

```bat
fast-native-ctrl.bat
```

Interactive prompt, or one-shot:

```bat
fast-native-ctrl.bat setup-local
fast-native-ctrl.bat dev run all
fast-native-ctrl.bat test all
fast-native-ctrl.bat list
fast-native-ctrl.bat connect
```

Linux/mac:

```bash
chmod +x fast-native-ctrl.sh
./fast-native-ctrl.sh status
```

## Command map

| Area | Commands |
|------|----------|
| Local tooling | `setup-local [--force]` |
| Dev stack | `dev run\|stop\|down\|purge\|reset {infra,apps,all}` · `--slim` for lightweight runtime |
| App scaffold | `app create <name>` |
| Tests | `test {all,backend,frontend}` |
| Local prod smoke | `prod start\|stop\|reset\|backup-acme\|…` |
| SSH / VM | `setup`, `pubkey`, `clone`, `env`, `start`, `stop`, `update`, `reset`, `backup-acme`, `connect`, … |

On-VM bash/bat scripts (what SSH `start`/`stop` invoke) live in [`remote/`](remote/README.md).

## Layout

| Path | Role |
|------|------|
| `servers.json` | Single VM entry (`fast-svelte`) |
| `safe/` | PEM, address, prod `.env` |
| `static/gpg` | Docker Ubuntu GPG (Iran bootstrap) |
| `remote/` | On-VM / local-prod compose scripts |
| `fast-native-ctrl.bat` / `.sh` | CLI entry |

## Typical first deploy (SSH)

```bat
fast-native-ctrl.bat setup
fast-native-ctrl.bat pubkey
REM add VM pubkey to GitHub
fast-native-ctrl.bat clone
fast-native-ctrl.bat env
fast-native-ctrl.bat start
```

Day-2:

```bat
fast-native-ctrl.bat update
fast-native-ctrl.bat status
fast-native-ctrl.bat backup-acme
```

## Local dev (Docker Desktop / host apps)

```bat
fast-native-ctrl.bat setup-local
fast-native-ctrl.bat dev run all
fast-native-ctrl.bat dev stop all
fast-native-ctrl.bat dev down all
fast-native-ctrl.bat dev purge infra
fast-native-ctrl.bat dev reset all
```

| Action | Infra (compose.dev.yml) | Apps (host) |
|--------|-------------------------|-------------|
| `run` / `start` | `up -d` db, redis (full), proxy, adminer + migrate | uvicorn :8000, arq worker (full), installed web kit :5000 |
| `stop` | `compose stop` — containers kept | kill host processes |
| `down` | `compose down` — volumes kept | kill host processes |
| `purge` | `compose down -v` — wipe data, stay down | kill host processes |
| `reset` | wipe then `run` | stop then run |

| Target | Notes |
|--------|-------|
| `infra` | Docker only + Alembic / initial_data |
| `apps` | host processes (needs infra already up) |
| `all` | run: infra→apps · stop/down/purge/reset: apps→infra |

Opens browser tabs for Adminer / Traefik / dashboard / API docs after a successful run.

**Runtime profiles:** `dev run all` (full — includes Redis + worker) · `dev run all --slim` (no Redis/worker). See [docs/runtime-profiles.md](../docs/runtime-profiles.md).

## Tests

```bat
fast-native-ctrl.bat test all
fast-native-ctrl.bat test backend
fast-native-ctrl.bat test frontend
```

Backend needs the dev DB (`dev run infra` → `localhost:5432`).

## Local production smoke

```bat
fast-native-ctrl.bat prod start
fast-native-ctrl.bat prod stop
fast-native-ctrl.bat prod reset
fast-native-ctrl.bat prod backup-acme
```

Same scripts SSH uses under `remote/`. Prefer SSH `start`/`stop` when operating the real VM from your laptop.

## Setup (ctrl tool itself)

```bat
python -m venv .venv
.venv\Scripts\pip install -r requirements.txt
```

`setup-local` also runs `npm install` for the frontend workspace.

On first `setup-local` / `dev run all`, the ctrl entry installs system **Python 3.10+** (via winget / Homebrew / apt) if missing, then `_setup_local` installs **Node.js LTS + npm** the same way before creating the project `.venv` and running `npm install`.

Iran VMs (`iran_setup: true`) keep provider DNS, rewrite apt to Arvan `apt_mirror`, and use Arvan Docker `registry_mirror`. `clone` routes GitHub SSH via `ssh.github.com:443`.

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

## Flatten / restore-flat

```bat
fast-native-ctrl.bat flatten --yes
fast-native-ctrl.bat restore-flat
fast-native-ctrl.bat restore-flat --server <id> --yes
```

