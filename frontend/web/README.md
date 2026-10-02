# Web slot

This folder is empty until you choose a web kit. Fast-Native does not ship a UI framework.

From the repo root:

```bat
__ctrl__\fast-native-ctrl.bat web list
__ctrl__\fast-native-ctrl.bat web use svelte
```

`svelte` can be `next`, `nuxt`, or `rio`. The command downloads that kit's `frontend/` from GitHub into this folder and writes `frontend/kit.lock.json` next to it (not inside this folder).

Then:

```bat
__ctrl__\fast-native-ctrl.bat setup-local
__ctrl__\fast-native-ctrl.bat dev run all
```

| What you get | Where it runs |
|--------------|---------------|
| Fast-Svelte | SvelteKit on :5000, workspace package `frontend` |
| Fast-Next | Next.js on :5000, workspace package `frontend` |
| Fast-Nuxt | Nuxt inside this folder on :5000 |
| Fast-Rio | `python -m rio run --port 5000 --public` |

Root `package.json` lists this folder as the npm workspace (`frontend/web`). `npm run dev` from the repo root runs `npm run dev -w frontend`, which is the Svelte or Next package. Nuxt and Rio are started by `__ctrl__` from inside this folder.

## Switching and updating

| Command | Effect |
|---------|--------|
| `web use <kit> --replace` | Replace the installed kit |
| `web status` | Lock file, branch, and whether this folder has edits |
| `web update` | Re-fetch the locked branch; refuses if this folder has local edits |
| `web update --force` | Overwrite local edits |

`web use` rewrites the extracted Dockerfiles so production builds with `context: frontend/web`.

## After install

Follow that kit's own layout. Typical homes:

| Kit | Modules | Routes |
|-----|---------|--------|
| Svelte | `src/lib/modules/` | `src/routes/` |
| Next | `src/lib/modules/` | `src/app/` |
| Nuxt | `lib/modules/` | `pages/` |
| Rio | `src/modules/` | `src/pages/` |

Only `base/` and `apps/<domain>/` belong under the modules root. Design primitives, when the kit uses shadcn, live at `base/ui/`. There is no `components/` folder as the app UI home.

The sample notes page inside the installed kit is the canonical web example. Native pages for the same path live under `frontend/win` and `frontend/android`.

## Commit

Commit this folder and `frontend/kit.lock.json` when a product should ship the chosen UI. Production `clone` does not run `web use` again.

Do not commit a second copy of a kit UI into `win/` or `android/`. Those shells implement the shared routes themselves.
