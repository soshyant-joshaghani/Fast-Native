# Dashboard UI conventions

FoxG foundation kits (**Fast-Next**, **Fast-Svelte**, **Fast-Nuxt**, **Fast-Rio**) share the same dashboard UX. Each kit implements the design in its own frontend stack — there is no shared UI package. **Fast-Native** installs one of those kits into `frontend/web` and draws the same routes in Windows and Android shells.

## Layout

- **Sidebar** (left): navigation links
- **Header** (top): theme toggle, user menu, logout
- **Main content**: page-specific UI

## Routes

| Path | Purpose | Auth |
|------|---------|------|
| `/login` | Centered login / signup | Public |
| `/` | Dashboard home (health checks) | Required |
| `/sample/notes` | Canonical notes CRUD | Required |
| `/admin` | Superuser placeholder | Superuser only |

## Visual language

- **Default theme:** dark
- **Toggle:** light/dark persisted per kit (localStorage, UserSettings, or equivalent)
- **Palette:** zinc/slate backgrounds, sky (`#0ea5e9`) primary accent
- **Sample Notes:** table/card CRUD styled like the [full-stack-fastapi-template](https://github.com/fastapi/full-stack-fastapi-template) Items page

## Navigation items

1. Dashboard → `/`
2. Sample Notes → `/sample/notes`
3. Admin → `/admin` (visible only when `is_superuser`)

## Auth

- JWT via `POST /base/login/access-token`
- Profile via `GET /base/login/me`
- Dev signup via `POST /private/users/` (local only)
- Unauthenticated users redirect to `/login`

## Kit-specific implementation

| Kit | Shell location |
|-----|----------------|
| Fast-Next | `frontend/src/lib/modules/base/`, `frontend/src/app/(dashboard)/` |
| Fast-Svelte | `frontend/src/lib/modules/base/`, `frontend/src/routes/(dashboard)/` |
| Fast-Nuxt | `frontend/lib/modules/base/`, `frontend/pages/(dashboard)/` |
| Fast-Rio | `frontend/src/modules/base/`, Rio pages |
| Fast-Native | Installed kit under `frontend/web/…`; native chrome in `frontend/win` and `frontend/android` |

After `web use`, follow that kit's primitives. JS kits keep shadcn under `…/modules/base/ui/`. Rio uses Rio widgets. Native shells do not import those primitives; they implement the same routes with their own chrome.

Do not copy UI code between kits or into the native shells. Match behavior and visuals only. Shared-layer (non-UI) changes transfer to the four UI kits and to Fast-Native — see [fast-template/README.md](../../README.md).
