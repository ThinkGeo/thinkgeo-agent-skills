# ThinkGeo Licensing by Platform

Sources: Licensing page, Product Center page, Desktop and Web Deployment Guides, and MAUI License Guide (docs.thinkgeo.com); the MAUI HowDoI `App.xaml.cs`.

Every platform needs a **developer license** on each developer's machine, installed with ThinkGeo Product Center.   Deployed apps and QA builds that run without a debugger need a **runtime license** file generated in Product Center.   Never write code that bypasses, patches, or fakes license checks; send account problems to ThinkGeo support.

| Platform | Product Center tab | Runtime license is generated for | Where the file goes |
| --- | --- | --- | --- |
| WPF, WinForms | WPF/WinForms/Blazor/WebAPI | The built executable (browse to the `.exe`) | Next to the executable |
| Blazor, WebAPI on Windows | WPF/WinForms/Blazor/WebAPI | The built executable (browse to the `.exe`) | Next to the executable, in the published output |
| MAUI on Android | Maui | The Android package name | `Resources\Raw`, Build Action `MauiAsset`, loaded in code |
| MAUI on iOS or Mac Catalyst | Maui | The bundle identifier | `Resources\Raw`, Build Action `MauiAsset`, loaded in code |
| MAUI on Windows | Maui (Windows Runtime License section) | The executable name, without `.exe` | Project root, Copy to Output Directory: Copy if newer |

For WPF and WinForms details, see `thinkgeo-desktop-maps/references/project-setup.md`.

## Developer license (all platforms)

1. Register at https://helpdesk.thinkgeo.com/register and download Product Center.
2. Sign in, open the tab for the product (for MAUI, the **Maui** tab), and click "Start Evaluation" (30-day trial) or "Activate" for a purchased license.

## Blazor and WebAPI

For building WebAPI tile services, see the `thinkgeo-web-api` skill.

Licensing works like the desktop: the same Product Center tab ("WPF/WinForms/Blazor/WebAPI"), a developer license for debugging, and a runtime license generated from the app's built executable for anything that runs without a debugger.   Generate the runtime license from the executable in the **published** output, and make sure the file is deployed with it.

**A server must have a runtime license, not just a developer license.** Unlike WPF and WinForms, a web app running on a machine with only a developer license shows a "subscription license has expired" watermark once the developer subscription ends.   Runtime licenses don't expire.   So a staging or production server that happens to have Product Center installed still needs the runtime license file.

**Linux and containers:** the current Deployment Guide covers only the Windows `.exe` case.   The Docker steps in the WebAPI Quick Start publish with `UseAppHost=false`, which produces no executable to browse to.   A command-line Product Center for Linux and macOS exists (see "Managing developer licenses" below), but no current guide explains how to license a container.   Don't invent a procedure; have the user confirm the supported approach with ThinkGeo support.

### What Blazor and WebAPI users see

From ThinkGeo's license matrix on the Licensing page:

| What the user sees | When | Likely cause | Fix |
| --- | --- | --- | --- |
| Exception: "Welcome to ThinkGeo components! Please sign up at https://helpdesk.thinkgeo.com/register ..." | Debugging | No developer license on this machine | Install Product Center, start an evaluation or activate |
| Blank map, "Not Licensed for Map Development" watermark | Debugging | Only a runtime license is present (for example a teammate who doesn't develop the map) | Install a developer license on that machine, or run without the debugger |
| Exception: "Your ThinkGeo ... subscription license has expired ..." (the product name varies) | Debugging | Developer subscription expired | Renew the subscription |
| Map with "subscription license has expired" watermark | Running without a debugger | Relying on an expired developer license instead of a runtime license | Generate and deploy a runtime license |
| Map with "Not licensed for Runtime" watermark | Running without a debugger | No runtime license deployed, or generated for a different executable name | Regenerate from the published executable and deploy it with the app |
| Map with "XX Days Left" watermark | Any | Evaluation license | Activate a purchased license, then regenerate the runtime license |

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

## Managing developer licenses

From the Product Center page on docs.thinkgeo.com.

| Task | How |
| --- | --- |
| Buy a license | Credit card: "Activate License" in Product Center, or the pricing page.   Purchase order or wire transfer: sales@thinkgeo.com |
| Move a dev license to a new machine or another developer | In Product Center, select the product and click "Deactivate License".   Then sign in to Product Center on the new machine (or as the other user) and activate it there |
| Product Center misbehaving after starting or stopping evaluations, or activating and deactivating licenses | Reset it (below).   Resetting doesn't affect licenses or the account |
| Work on Linux or macOS | Download the command-line Product Center from the Helpdesk.   It does the same things as the Windows version; the current Product Center page links to a command reference in the legacy deployment docs |
| Older Product Center for an older ThinkGeo version | "Legacy Downloads" on the Helpdesk main page |

Runtime licenses are free and perpetual, so deactivating or moving a dev license doesn't affect apps already deployed with a runtime license.

### Resetting Product Center

1. Open Product Center.
2. If you have a purchased license, select each product marked "Activated" and click "Deactivate License".   (Evaluators skip this step.)
3. Click "Log Out" (upper right) and close Product Center.
4. Delete the folder `C:\ProgramData\ThinkGeo\Map Suite xx.x`, where `xx.x` is the version folder present on the machine.
5. Open Product Center, sign in, and reactivate your purchases or evaluations.
