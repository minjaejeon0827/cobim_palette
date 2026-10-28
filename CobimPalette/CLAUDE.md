# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`CobimPalette` is an **AutoCAD 2025 add-in** (.NET 8, `net8.0-windows`, WPF + WinForms) that docks a WebView2-based "COBIM 탐색기" (COBIM explorer) palette inside AutoCAD. The palette hosts the COBIM/HubBox web explorer (`http://explorer.cobim.kr:8100/...`) and exposes C# methods to that web page so it can trigger CDE operations (download, check-out, check-in, cancel work, open file) against the COBIM REST API. It cannot run standalone; it only runs when loaded into `acad.exe`.

Note: the git repository root is the parent directory (`D:\minjae\cobim_palette`); this solution lives in the `CobimPalette/` subfolder.

## Build / run

```powershell
dotnet build CobimPalette.sln            # or build in Visual Studio
```

- Requires AutoCAD 2025 installed at `C:\Program Files\Autodesk\AutoCAD 2025\` — `CobimPalette.csproj` references `accoremgd`, `acdbmgd`, `acmgd`, `AcCui`, `AcWindows`, `AdWindows` via hard-coded `HintPath` (`Private=False`, not copied).
- **Every project has a PostBuild step** that copies its output DLL to a hard-coded per-user bundle folder: `C:\Users\bhjeon\AppData\Roaming\Autodesk\ApplicationPlugins\CobimPalette.bundle\`. On a different machine/user the `copy` fails and breaks the build unless that path is adjusted (or the directory exists).
- Debugging: use the `AutoCAD2025` launch profile (`Properties/launchSettings.json`), which starts `acad.exe`. AutoCAD then loads the add-in from the bundle folder (or load the DLL manually with `NETLOAD`).
- There are no test projects and no lint configuration.

## Solution structure (dependency order)

- `CobimNet` — MVVM base: `BindableBase` (`Changed`, `SetAndNotify`, static `StaticChanged`), `PropertyChangedDisableSection`.
- `CobimUtil` — generic helpers/logger, used by Rest.Api.
- `CobimPalette.Data.Models` → CobimNet. Request/response data models per feature (`CobimBase/{Login,HubBox,WebView2Explorer,AutoCAD}`), each as a `T<Name>View<TData>` / `<Name>View` pair. Models are filled by **manual DI**: the VM calls `InputParamData.SetData(new RequestHeader(...), new ContainerPack(...), new FilePack(...))` before calling the REST layer.
- `CobimPalette.Rest.Api` → Data.Models, CobimUtil. Static `*RestServer` classes (`WebView2ExplorerRestServer`, `HubBoxRestServer`) that make the HTTP calls, plus `*Helper` classes holding endpoint URLs/constants (`HubBoxHelper` has the server addresses).
- `CobimPalette` (the add-in DLL) → all of the above.

## Add-in architecture (CobimPalette project)

- **Entry points**: `App.cs` is the `IExtensionApplication` (`[assembly: ExtensionApplication]`); `Command.cs` holds `[CommandMethod]` AutoCAD commands (`CmdWebExplorer` is the live one; others are commented out). 
- **Startup flow** (`App.Initialize`): hook `AppDomain.AssemblyResolve` → `LoadFromCobimPalette` (resolves NuGet DLLs like `AnimatedImage.Wpf` from the add-in's own folder — needed because AutoCAD 2025/.NET Core probes acad.exe's folder) → configure Serilog → `ConfigureAppSetting()` → build the ribbon (`Common/RibbonBase/Ribbon.CreateRibbonControl`, deferred via `ComponentManager.ItemInitialized` if the ribbon isn't ready) → subscribe to `DocumentCreated/Activated/BecameCurrent`, each of which re-shows the WebView2 palette.
- **Global state**: `Settings/AppSetting.Default` is a lazy singleton aggregating sub-settings (`HttpClientBase` – singleton `HttpClient`, `LoginBase`, `DirectoryBase`, `PaletteBase`, `WindowBase`, `ImagineBuilderBase`). Access everything through `AppSetting.Default.<X>Base`.
- **Palettes**: `PaletteSetting` + `Common/Extensions/PaletteExtensions` (`PaletteSetManager`, `CustomPaletteSet`) create singleton views and register/show them by title + GUID. Titles, GUIDs, view-type names and other constants live in `Common/CobimBase/CobimHelper.cs`. Adding a palette means: constant(s) in `CobimHelper`, a view field + cases in `PaletteSetting.CreatePalette/GetPalette/RegisterPalette`, and a register call in `App.ConfigureAppSetting`. The HubBox palette exists but its registration is commented out.
- **Ribbon → command**: ribbon buttons use `Commands/RibbonCommand`, which calls `SendStringToExecute(button.CommandParameter)`; the parameter strings (in `RibbonHelper`) must match a `[CommandMethod]` name in `Command.cs`.
- **MVVM layout**: `Views/` and `ViewModels/` mirror each other (`Pages/Palettes/...`, `Part/...`, `Windows/...`); views are suffixed `V`, view models `VM`. Base classes in `Services/` (`CobimScreen` → `PageBase`/`PageBase<TData>`, `PartBase`).
- **WebView2 ↔ C# bridge** (core of the app): `WebView2ExplorerV.xaml.cs` creates a `CoreWebView2Environment` (user-data folder `%LOCALAPPDATA%\TestWV`), navigates to `HubBoxHelper.requestLoginURL`, sets `DataContext = new WebView2ExplorerVM()` and calls `AddHostObjectToScript("boundAsync", vm)`. The web page's context menu then calls the VM's public async methods (`DownLoadAsync`, `CheckOutAsync`, `CheckInAsync`, `CancelWorkAsync`, `OpenFileAsync`). Constraints for these host-object methods:
  - The VM must stay `[ComVisible(true)]` + `[ClassInterface(AutoDual)]` and derive from **non-generic** `PageBase` — deriving from `PageBase<WebView2ExplorerView>` broke the binding.
  - Parameters must be `string` (JSON text, deserialized with `System.Text.Json`), not `object`.
  - They return `Task<string>`; UI work from them goes through `Dispatcher.Invoke`.

## Conventions

- Comments and user-facing strings are in Korean; keep that style. Most files are UTF-8 with BOM. `App.cs` and `CobimNet/BindableBase.cs` already contain mojibake (U+FFFD) in their Korean comments — don't "fix" encoding wholesale, and keep new edits UTF-8.
- Method pattern used everywhere: `var currentMethod = MethodBase.GetCurrentMethod();` then `try { ... } catch (Exception ex) { Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message); ... }`. Top-level handlers (events, commands, host-object methods) show `MessageBox.Show(ex.Message, CobimHelper.ErrorTitle)`; inner/settings methods `throw;` to the caller.
- Code is organized with `#region <Name>` / `#endregion <Name>` per member, ending with an empty `#region Sample`. Disabled features are kept as commented-out code with dated `// TODO : ... (yyyy.MM.dd minjae)` notes.
- Logs: Serilog file sink at `Documents\COBIM\CobimPalette\Logs\CobimPalette_.log` (30-file retention).
