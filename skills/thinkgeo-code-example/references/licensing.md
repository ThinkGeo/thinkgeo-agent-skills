# ThinkGeo Licensing by Platform

Sources: Licensing page, Desktop Deployment Guide, and MAUI License Guide (docs.thinkgeo.com); the MAUI HowDoI `App.xaml.cs`.

Every platform needs a **developer license** on each developer's machine, installed with ThinkGeo Product Center.   Deployed apps and QA builds that run without a debugger need a **runtime license** file generated in Product Center.   Never write code that bypasses, patches, or fakes license checks; send account problems to ThinkGeo support.

| Platform | Product Center tab | Runtime license is generated for | Where the file goes |
| --- | --- | --- | --- |
| WPF, WinForms | WPF/WinForms/Blazor/WebAPI | The built executable (browse to the `.exe`) | Next to the executable |
| MAUI on Android | Maui | The Android package name | `Resources\Raw`, Build Action `MauiAsset`, loaded in code |
| MAUI on iOS or Mac Catalyst | Maui | The bundle identifier | `Resources\Raw`, Build Action `MauiAsset`, loaded in code |
| MAUI on Windows | Maui (Windows Runtime License section) | The executable name, without `.exe` | Project root, Copy to Output Directory: Copy if newer |

For WPF and WinForms details, see `thinkgeo-desktop-maps/references/project-setup.md`.

## Developer license (all platforms)

1. Register at https://helpdesk.thinkgeo.com/register and download Product Center.
2. Sign in, open the tab for the product (for MAUI, the **Maui** tab), and click "Start Evaluation" (30-day trial) or "Activate" for a purchased license.

## MAUI

### Android, iOS, and Mac Catalyst need a license file even to debug

Because of Visual Studio restrictions, Android, iOS, and Mac Catalyst apps need a license file to run at all, including in an emulator or simulator under the debugger.   A developer license alone isn't enough on these targets.   This is the most common MAUI licensing surprise.

1. **Give the app a real ID.** Read the Android package name from `Platforms\Android\AndroidManifest.xml` and the bundle identifier from `Platforms\iOS\Info.plist` (Mac Catalyst: `Platforms\MacCatalyst\Info.plist`).   If the package name is still the Visual Studio default, `com.mycompany.myapp`, change it first; the default doesn't work with licensing.
2. **Generate the files.** In Product Center's Maui tab, paste the package name and bundle identifier into the boxes on the right and click "Create".   The files can have any name.
3. **Add them to the project.** Copy the files into `Resources\Raw` and set each file's Build Action to `MauiAsset`.
4. **Load them before any map renders**, for example in `App.OnStart()`:

```csharp
using ThinkGeo.UI.Maui;

protected override async void OnStart()
{
    // File names as added to Resources\Raw. The official sample loads both on every platform.
    await LicenseLoader.LoadLicense("myapp.android.mapsuitelicense");
    await LicenseLoader.LoadLicense("myapp.ios.mapsuitelicense");
    MainPage = new AppShell();
}
```

`LicenseLoader.LoadLicense` (namespace `ThinkGeo.UI.Maui`) is async; await it before the first page with a map is shown.   The license is tied to the package name or bundle identifier, so generate a new one whenever that ID changes.

### Windows

A MAUI app on Windows works like a desktop app: the developer license covers debugging, and deployed builds need a runtime license.   In Product Center's Maui tab, type the executable name **without** the `.exe` extension (for example `MyMapApp`, not `MyMapApp.exe`) in the Windows Runtime License section and click "Create".   Copy the file to the project root and set Copy to Output Directory to "Copy if newer".

### Upgrading from Xamarin or older mobile editions

MAUI licensing is different from earlier mobile editions: the license must be loaded in code with `LicenseLoader.LoadLicense`.   A license setup from an older edition won't be picked up.

### What MAUI users see

From ThinkGeo's license matrix on the Licensing page, confirmed by ThinkGeo (October 2026):

| What the user sees | Platform | Likely cause | Fix |
| --- | --- | --- | --- |
| Exception: "Welcome to ThinkGeo components! ... A separate license file is required for each mobile project" | Android, iOS, Mac Catalyst | No license file loaded (or not loaded before the map rendered) | Generate the file for the package name or bundle identifier, add it as a `MauiAsset`, await `LoadLicense` before showing a map |
| Blank map with "Not Licensed for Runtime" watermark | Android, iOS, Mac Catalyst | Running without a license file | Same as above |
| "XX Days Left" watermark | Any | Evaluation license | Activate a purchased license, then regenerate the license files |
| Exception while debugging after the subscription ended, but release builds still show the map | Android, iOS, Mac Catalyst | Developer subscription expired; runtime licenses are perpetual but debugging needs an active subscription | Renew the subscription |
| Blank map with "Not licensed for Runtime" watermark | Windows | Runtime license missing, not copied to output, or generated for a different executable name | Regenerate for the exact executable name (no `.exe`), set Copy if newer |

If the file is loaded and the watermark persists, check that the package name or bundle identifier in the built app matches the one the license was generated for.
