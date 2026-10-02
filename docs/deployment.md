# Deployment

Path from local development to production on a single VM.

```text
Development  →  Tests  →  Production compose  →  Traefik + TLS  →  Running service
```

## Prerequisites

- Ubuntu VM with Docker
- DNS A records: `@`, `api`, optional `adminer` → VM IP
- External Docker network (once per VM): `docker network create traefik-public`

## Configure

1. Copy `.env.example` → `.env` — set secrets, DB password, superuser password
2. Edit `DOMAIN` and URLs in `compose.yml` (`x-prod-app-config`)
3. Upload prod env via `__ctrl__` safe folder for SSH deploy

## First deploy (SSH from laptop)

```bat
__ctrl__\fast-native-ctrl.bat setup
__ctrl__\fast-native-ctrl.bat pubkey
__ctrl__\fast-native-ctrl.bat clone
__ctrl__\fast-native-ctrl.bat env
__ctrl__\fast-native-ctrl.bat start
```

On VM (bootstrap):

```bash
bash __ctrl__/remote/setup-ubuntu.sh
bash __ctrl__/remote/start-prod.sh
```

## Production stack

`compose.yml` includes:

- Postgres, Redis, backend, worker (ARQ), frontend (the installed kit in `frontend/web`), Traefik (Let's Encrypt)
- Adminer (optional subdomain)

Traefik routes:

- `https://<domain>` → frontend
- `https://api.<domain>` → backend

## Day-2 operations

```bat
__ctrl__\fast-native-ctrl.bat update
__ctrl__\fast-native-ctrl.bat status
__ctrl__\fast-native-ctrl.bat stop
__ctrl__\fast-native-ctrl.bat backup-acme
```

Reset (wipes DB/Redis volumes, keeps SSL): see `__ctrl__/remote/reset-prod.sh`

## Local prod smoke

Windows Docker Desktop only:

```bat
__ctrl__\fast-native-ctrl.bat prod start
```

Scripts: [`__ctrl__/remote/`](../__ctrl__/remote/README.md)

## Web kit in production

`compose.yml` builds `frontend/web`. JS kits use `npm run build`. Set the API URL for the kit you committed:

| Kit | Build/runtime variable |
|-----|------------------------|
| Svelte, Rio | `PUBLIC_API_BASE_URL` |
| Next | `NEXT_PUBLIC_API_BASE_URL` |
| Nuxt | `NUXT_PUBLIC_API_BASE_URL` |

Production `clone` does not run `web use`. Commit `frontend/web` and `frontend/kit.lock.json` before you deploy.

Windows and Android are built on a developer machine (`native build win` / `native build android`). They are not part of the VM compose stack.
