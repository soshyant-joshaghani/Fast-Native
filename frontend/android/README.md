# Fast-Native — Android client

Jetpack Compose shell for the dashboard routes in [../client-routes.json](../client-routes.json). The activity keeps the route string and draws the header and navigator itself. The app is not a WebView of the website.

The web kit in `frontend/web` is a separate client of the same FastAPI backend.

## Layout

```text
app/src/
├── client/MainActivity.kt
├── modules/global/             # chrome, not product pages
│   ├── AppShell.kt
│   ├── ShellNav.kt
│   ├── Messages.kt
│   └── Glass.kt
└── routes/
    ├── Page.kt                 # /
    ├── login/Page.kt           # /login
    ├── sample/notes/Page.kt    # /sample/notes
    └── admin/Page.kt           # /admin
app/assets/client-routes.json   # copy of the shared route list
```

| Route | Page |
|-------|------|
| `/` | `app/src/routes/Page.kt` |
| `/login` | `app/src/routes/login/Page.kt` |
| `/sample/notes` | `app/src/routes/sample/notes/Page.kt` |
| `/admin` | `app/src/routes/admin/Page.kt` |

Shell code stays in `modules/global`. New shared routes get a folder under `routes/` and an entry in `frontend/client-routes.json` (and the asset copy).

## Run

```bat
__ctrl__\fast-native-ctrl.bat native run android
__ctrl__\fast-native-ctrl.bat native build android
__ctrl__\fast-native-ctrl.bat native clean android
```

`native run android` runs `gradlew assembleDebug` and, when adb sees a device, installs and launches `fastnative.client`. `native list` shows this client and Windows.

The API the pages call is the same stack as the web dashboard (`dev run all`, http://localhost:8000 or http://api.localhost). Start that stack when the page needs live data. A device or emulator must be reachable from adb.
