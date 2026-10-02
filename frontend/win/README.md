# Fast-Native — Windows client

WinUI 3 shell for the dashboard routes in [../client-routes.json](../client-routes.json). The app paints its own header and navigator. It is not a window around the website.

The web kit in `frontend/web` is a separate client of the same FastAPI backend. Do not host that site inside this window.

## Layout

```text
FastNative.Client/
├── App.xaml.cs
├── client-routes.json          # copy of the shared route list
├── modules/global/             # chrome, not product pages
│   ├── MainWindow.cs
│   ├── ShellNav.cs
│   ├── Theme.cs
│   ├── Messages.cs
│   └── Glass.cs
└── routes/                     # one page per shared path
    ├── Page.cs                 # /
    ├── login/Page.cs           # /login
    ├── sample/notes/Page.cs    # /sample/notes
    └── admin/Page.cs           # /admin
```

| Route | Page |
|-------|------|
| `/` | `routes/Page.cs` |
| `/login` | `routes/login/Page.cs` |
| `/sample/notes` | `routes/sample/notes/Page.cs` |
| `/admin` | `routes/admin/Page.cs` |

Shell code stays in `modules/global`. New shared routes get a folder under `routes/` and an entry in `frontend/client-routes.json` (and the copy next to the project). Product API calls belong with the page that needs them, not in a new top-level UI framework.

## Run

```bat
__ctrl__\fast-native-ctrl.bat native run win
__ctrl__\fast-native-ctrl.bat native build win
__ctrl__\fast-native-ctrl.bat native clean win
```

`native run win` builds `FastNative.Client` without an MSIX package and launches the exe. Windows App SDK 1.6 is required. `native list` shows this client and Android.

The API the pages call is the same stack as the web dashboard (`dev run all`, http://localhost:8000 or http://api.localhost). Start that stack when the page needs live data.
