# Fast-Native frontends

One backend. The web UI is a kit you download. Windows and Android ship in this repo as native clients of the same API.

```text
frontend/
├── client-routes.json   # shared paths
├── kit.lock.json        # written by web use
├── web/                 # empty until web use
├── win/                 # WinUI 3
├── android/             # Jetpack Compose
├── mac/                 # reserved
├── linux/               # reserved
└── ios/                 # reserved
```

| Client | Role | Status |
|--------|------|--------|
| [web](./web/README.md) | Dashboard from Fast-Svelte, Fast-Next, Fast-Nuxt, or Fast-Rio | Empty until `web use` |
| [win](./win/README.md) | Native Windows shell for the same client routes | Ships in this repo |
| [android](./android/README.md) | Native Android shell for the same client routes | Ships in this repo |
| [mac](./mac/README.md) | Reserved desktop slot | Not implemented |
| [linux](./linux/README.md) | Reserved desktop slot | Not implemented |
| [ios](./ios/README.md) | Reserved mobile slot | Not implemented |

Native apps are **siblings** of the web client. Each paints its own header and navigator. None of them is a WebView of the whole site.

## Routes

Shared client paths live in [client-routes.json](./client-routes.json). Native pages are stubs for those paths. The web kit owns the real dashboard UI after `web use`.

```text
/                 Home
/login            Sign in
/sample/notes     Sample notes
/admin            Admin
```

When you add a product route that every client should open, add the path here and add a page under `win` and `android` (and later the reserved clients).

## Web kit

```bat
__ctrl__\fast-native-ctrl.bat web list
__ctrl__\fast-native-ctrl.bat web use svelte
__ctrl__\fast-native-ctrl.bat web use next
__ctrl__\fast-native-ctrl.bat web use nuxt
__ctrl__\fast-native-ctrl.bat web use rio
```

| Command | Effect |
|---------|--------|
| `web use <kit>` | Copy that GitHub repo's `frontend/` into `web/` and write `kit.lock.json` |
| `web use <kit> --replace` | Required to switch an installed kit |
| `web status` | Show the lock and whether `web/` has local edits |
| `web update` | Re-fetch the locked branch; stops if `web/` has local edits |
| `web update --force` | Overwrite local edits in `web/` |

`setup-local`, `dev run`, and `test frontend` stop until a kit is installed. They do not choose Svelte for you.

Commit `web/` and `kit.lock.json` when the product should ship that UI. Production `clone` does not download the kit again.

## Where UI code goes

| Client | Home |
|--------|------|
| Web, after `web use` | That kit's modules root: `web/src/lib/modules` (Svelte, Next), `web/lib/modules` (Nuxt), `web/src/modules` (Rio) |
| Windows | `win/FastNative.Client/modules/global` (chrome) and `win/FastNative.Client/routes` (pages) |
| Android | `android/app/src/modules/global` (chrome) and `android/app/src/routes` (pages) |

JS kits style with Tailwind and that kit's shadcn primitives inside `web/`. Rio uses Rio widgets. Native shells do not import those primitives.

There is no project-wide `components/` folder. Web modules are the web component home. Native chrome stays in `modules/global`.

## Run

Web (after `web use` and `setup-local`):

```bat
__ctrl__\fast-native-ctrl.bat dev run all
```

Native:

```bat
__ctrl__\fast-native-ctrl.bat native run win
__ctrl__\fast-native-ctrl.bat native run android
```

Dashboard: http://dashboard.localhost · Direct web: http://localhost:5000
