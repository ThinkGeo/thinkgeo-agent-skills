# ThinkGeo Licensing by Platform

Sources: License Guide for ThinkGeo 15, Licensing page, Product Center page, Desktop and Web Deployment Guides, and MAUI License Guide (docs.thinkgeo.com); the MAUI HowDoI `App.xaml.cs`.

**Check the ThinkGeo version first.** ThinkGeo 15 replaced the whole licensing system, so read the version of the `ThinkGeo.*` packages in the project file and use the matching section:

- **ThinkGeo 15 and later:** a personal license key file per developer, checked when the project **builds**.   Deployed apps need nothing.   No Product Center, no runtime license files, no `LicenseLoader`.
- **ThinkGeo 14 and earlier:** Product Center installs a developer license, checked at run time, and deployed apps need a runtime license file.

For a new project, use the section for the version you install (the latest stable release on NuGet).   Licenses don't carry across: ThinkGeo 14 licenses don't license ThinkGeo 15 packages.   Never write code that bypasses, patches, or fakes license checks, and never put a key file or key text in source code, a repository, or a container image.   Send account problems to ThinkGeo support.

## ThinkGeo 15 and later

### How it works

1. Each developer keeps a **license key** file in their user profile folder.
2. Every build (`dotnet build`, Visual Studio, or an AI agent running the build) runs a check inside the `ThinkGeo.Core` package.   With a valid key, the build passes and stamps a signed, perpetual license token into the assembly.
3. At run time ThinkGeo reads the token from the assemblies.   Nothing is installed, copied, or activated on the machines the app is deployed to.

The check works offline: a build machine with no internet access needs only the key file.

A key covers one **product family**, and the build checks it against the UI packages the project references:

| Family | Packages | Key file |
| --- | --- | --- |
| Desktop | `ThinkGeo.UI.Wpf`, `ThinkGeo.UI.WinForms` | `desktop.thinkgeokey` |
| Web | `ThinkGeo.UI.Blazor`, `ThinkGeo.UI.WebApi` | `web.thinkgeokey` |
| Mobile | `ThinkGeo.UI.Maui` | `mobile.thinkgeokey` |

A project that uses several families needs a key for each; the build stamps one token per key.

### Developer setup

1. Sign in at https://helpdesk.thinkgeo.com and open the licensing link under the SDKs card.
2. Click **Generate Key** for each product family you work with, then **Download Key**.
3. Save the files with their names unchanged in the profile folder: `%LOCALAPPDATA%\ThinkGeo\` on Windows, `~/.thinkgeo/` on macOS and Linux.   Every `.thinkgeokey` file in the folder is read.
4. Build.   There is nothing to activate and nothing to restart, including Visual Studio.

The key belongs to the developer, not the machine: the same files work on their laptop, desktop, and virtual machines.   Keys must not be shared between developers or committed to a repository.   Downloading again returns the same key; **Revoke** on the Helpdesk page replaces it with a new one (use it if a key may have leaked).

If the page offers no key, it says why.   "Your contact is not enabled for downloads": the user's contact isn't marked as a developer, so they contact support@thinkgeo.com.   "Every seat your company holds is taken": they contact sales@thinkgeo.com.

**Trial:** no sign-up is needed.   The first build on a machine without a key starts a 30-day trial on that machine.   Builds pass with warning `TG0001` (which gives the end date and days left) and maps show a "Not Licensed for Runtime" watermark.   From day 30 the build fails with `TG0002`.   Apps built during the trial keep running, with the watermark.   For a longer evaluation, contact sales@thinkgeo.com.

**Non-map key** (`nonmap.thinkgeokey`, from the same page): for developers who build a solution but never work on its maps, such as a back-end service.   Every ThinkGeo project builds (message `TG0005`) and maps render blank.   It takes no seat.

### Build messages

The check prints these in the `dotnet build` output and the Visual Studio Error List, with the project name.   When a user pastes one, match the code:

| Code | Severity | Meaning | Fix |
| --- | --- | --- | --- |
| `TG0001` | warning | No key found; the machine's 30-day trial is running.   Maps show a watermark. | Put the key file in the profile folder |
| `TG0002` | error | No key, and the trial on this machine has ended | Put the key file in the profile folder |
| `TG0003` | error | The key doesn't cover this build: the subscription ended more than 14 days ago, or (perpetual license) the package was released after the Software Assurance date | Renew.   With a perpetual license, a package released before the Software Assurance date also builds |
| `TG0004` | error | The key is invalid, revoked, or malformed, or it doesn't cover a UI package the project uses (for example a Web key in a WPF project) | Download the key again, get the missing family's key, delete old key files.   In CI, check the `THINKGEO_LICENSE` value |
| `TG0005` | message | Building with a non-map key; maps render blank | Nothing |
| `TG0006` | warning | The subscription ends within 30 days, or ended less than 14 days ago and builds haven't stopped yet | Renew; a key file in the profile folder picks up the renewal on its own |
| `TG0007` | warning | The `ThinkGeo.Core` package isn't an official ThinkGeo build, so the key's date can't be checked | Use the official package from nuget.org |

**Dates and renewals:** with a subscription, the key's date is the end of the subscription; builds continue for 14 days after it with `TG0006`, then stop with `TG0003` until renewal.   With a perpetual license (legacy), the date is the end of Software Assurance; packages released before it build permanently.   Either way, apps already built and deployed keep running.   After a renewal, a key file in the profile folder refreshes itself on the next build.   If a machine never connects and still reports the old date, download the key again on a connected machine and copy it over.

### Deploying

Nothing to ship: the license travels inside the assembly.   There's nothing to install, copy, or activate on servers, desktops, or devices, and trimming, single-file publishing, and ReadyToRun keep the token.

- **WPF and WinForms:** no runtime license file next to the executable.
- **Blazor and WebAPI:** nothing on the server, including Linux and containers.
- **MAUI (Android, iOS, Mac Catalyst, Windows):** the key is used on the build machine; the device needs nothing.   Don't call `LicenseLoader.LoadLicense` and don't add `.mapsuitelicense` files: the call is obsolete in ThinkGeo 15 (a build warning now, an error in a later release).
- **Blazor WebAssembly:** the build is checked like any other project; the browser doesn't check the token.

### Build servers, CI, and containers

Only the machine that **compiles** needs a key.   Build agents usually have no profile folder, so pass the key in the `THINKGEO_LICENSE` environment variable: either the key text (the `tgk_...` line of a key file; one line per family is allowed) or the full path of a key file.   The variable is read before the profile folder.

- Use the key of a developer who holds a seat, or ask support@thinkgeo.com for a build account.   Store it as a CI secret, never in the repository or pipeline file.
- A key in `THINKGEO_LICENSE` is never refreshed automatically: replace the secret after a renewal, and when the developer it belongs to leaves (taking their seat back revokes the key).
- Without a key, every fresh agent or container starts a new trial and the build passes with `TG0001`, so a watermarked app can leave the pipeline unnoticed.   Add `-warnaserror:TG0001` to the build command to make that a failure.

GitHub Actions:

```yaml
- name: Build
  run: dotnet build -c Release -warnaserror:TG0001
  env:
    THINKGEO_LICENSE: ${{ secrets.THINKGEO_LICENSE }}
```

In GitLab CI, define `THINKGEO_LICENSE` as a masked variable (Settings > CI/CD > Variables).   In Azure Pipelines, secret variables aren't passed to scripts automatically, so map it in the step's `env:` block.

Docker, with a BuildKit secret so the key is never written into an image layer (here the variable holds the **path** of the mounted file):

```dockerfile
RUN --mount=type=secret,id=thinkgeo \
    THINKGEO_LICENSE=/run/secrets/thinkgeo dotnet publish -c Release -o /app
```

```bash
docker build --secret id=thinkgeo,src=$HOME/.thinkgeo/web.thinkgeokey -t myapp .
```

### Upgrading a project from ThinkGeo 14

- Each developer downloads their ThinkGeo 15 key files from the Helpdesk.   Product Center and its developer licenses aren't used.
- Remove runtime license files from the output and installers; they no longer do anything.
- MAUI: remove the `LicenseLoader.LoadLicense` calls and the `.mapsuitelicense` files.
- Build servers now need `THINKGEO_LICENSE` (ThinkGeo 14 didn't check builds).
- Apps that stay on ThinkGeo 14 packages keep using their ThinkGeo 14 licenses.

### Seats and accounts

Each purchase gives seats to a product family; a developer's key takes one seat of its family, and downloading again doesn't take another.   Only contacts marked as developers on the company account can download keys; support@thinkgeo.com adds or removes them.   For more seats, freeing a seat, or renewals, contact sales@thinkgeo.com.

## ThinkGeo 14 and earlier

Every platform needs a **developer license** on each developer's machine, installed with ThinkGeo Product Center.   Deployed apps and QA builds that run without a debugger need a **runtime license** file generated in Product Center.

| Platform | Product Center tab | Runtime license is generated for | Where the file goes |
| --- | --- | --- | --- |
| WPF, WinForms | WPF/WinForms/Blazor/WebAPI | The built executable (browse to the `.exe`) | Next to the executable |
| Blazor, WebAPI on Windows | WPF/WinForms/Blazor/WebAPI | The built executable (browse to the `.exe`) | Next to the executable, in the published output |
| MAUI on Android | Maui | The Android package name | `Resources\Raw`, Build Action `MauiAsset`, loaded in code |
| MAUI on iOS or Mac Catalyst | Maui | The bundle identifier | `Resources\Raw`, Build Action `MauiAsset`, loaded in code |
| MAUI on Windows | Maui (Windows Runtime License section) | The executable name, without `.exe` | Project root, Copy to Output Directory: Copy if newer |

For WPF and WinForms details, see `thinkgeo-desktop-maps/references/project-setup.md`.

### Developer license (all platforms)

1. Register at https://helpdesk.thinkgeo.com/register and download Product Center.
2. Sign in, open the tab for the product (for MAUI, the **Maui** tab), and click "Start Evaluation" (30-day trial) or "Activate" for a purchased license.

### Blazor and WebAPI

For building WebAPI tile services, see the `thinkgeo-web-api` skill; for Blazor pages, the `thinkgeo-blazor` skill.

Licensing works like the desktop: the same Product Center tab ("WPF/WinForms/Blazor/WebAPI"), a developer license for debugging, and a runtime license generated from the app's built executable for anything that runs without a debugger.   Generate the runtime license from the executable in the **published** output, and make sure the file is deployed with it.

**A server must have a runtime license, not just a developer license.** Unlike WPF and WinForms, a web app running on a machine with only a developer license shows a "subscription license has expired" watermark once the developer subscription ends.   Runtime licenses don't expire.   So a staging or production server that happens to have Product Center installed still needs the runtime license file.

**Linux and containers:** the current Deployment Guide covers only the Windows `.exe` case.   The Docker steps in the WebAPI Quick Start publish with `UseAppHost=false`, which produces no executable to browse to.   A command-line Product Center for Linux and macOS exists (see "Managing developer licenses" below), but no current guide explains how to license a container.   Don't invent a procedure; have the user confirm the supported approach with ThinkGeo support.

#### What Blazor and WebAPI users see

From ThinkGeo's license matrix on the Licensing page:

| What the user sees | When | Likely cause | Fix |
| --- | --- | --- | --- |
| Exception: "Welcome to ThinkGeo components! Please sign up at https://helpdesk.thinkgeo.com/register ..." | Debugging | No developer license on this machine | Install Product Center, start an evaluation or activate |
| Blank map, "Not Licensed for Map Development" watermark | Debugging | Only a runtime license is present (for example a teammate who doesn't develop the map) | Install a developer license on that machine, or run without the debugger |
| Exception: "Your ThinkGeo ... subscription license has expired ..." (the product name varies) | Debugging | Developer subscription expired | Renew the subscription |
| Map with "subscription license has expired" watermark | Running without a debugger | Relying on an expired developer license instead of a runtime license | Generate and deploy a runtime license |
| Map with "Not licensed for Runtime" watermark | Running without a debugger | No runtime license deployed, or generated for a different executable name | Regenerate from the published executable and deploy it with the app |
| Map with "XX Days Left" watermark | Any | Evaluation license | Activate a purchased license, then regenerate the runtime license |

### MAUI

For building MAUI map apps (data files, taps, GPS), see the `thinkgeo-maui` skill.

#### Android, iOS, and Mac Catalyst need a license file even to debug

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

#### Windows

A MAUI app on Windows works like a desktop app: the developer license covers debugging, and deployed builds need a runtime license.   In Product Center's Maui tab, type the executable name **without** the `.exe` extension (for example `MyMapApp`, not `MyMapApp.exe`) in the Windows Runtime License section and click "Create".   Copy the file to the project root and set Copy to Output Directory to "Copy if newer".

#### Upgrading from Xamarin or older mobile editions

MAUI licensing is different from earlier mobile editions: the license must be loaded in code with `LicenseLoader.LoadLicense`.   A license setup from an older edition won't be picked up.

#### What MAUI users see

From ThinkGeo's license matrix on the Licensing page, confirmed by ThinkGeo (October 2026):

| What the user sees | Platform | Likely cause | Fix |
| --- | --- | --- | --- |
| Exception: "Welcome to ThinkGeo components! ... A separate license file is required for each mobile project" | Android, iOS, Mac Catalyst | No license file loaded (or not loaded before the map rendered) | Generate the file for the package name or bundle identifier, add it as a `MauiAsset`, await `LoadLicense` before showing a map |
| Blank map with "Not Licensed for Runtime" watermark | Android, iOS, Mac Catalyst | Running without a license file | Same as above |
| "XX Days Left" watermark | Any | Evaluation license | Activate a purchased license, then regenerate the license files |
| Exception while debugging after the subscription ended, but release builds still show the map | Android, iOS, Mac Catalyst | Developer subscription expired; runtime licenses are perpetual but debugging needs an active subscription | Renew the subscription |
| Blank map with "Not licensed for Runtime" watermark | Windows | Runtime license missing, not copied to output, or generated for a different executable name | Regenerate for the exact executable name (no `.exe`), set Copy if newer |

If the file is loaded and the watermark persists, check that the package name or bundle identifier in the built app matches the one the license was generated for.

### Managing developer licenses

From the Product Center page on docs.thinkgeo.com.

| Task | How |
| --- | --- |
| Buy a license | Credit card: "Activate License" in Product Center, or the pricing page.   Purchase order or wire transfer: sales@thinkgeo.com |
| Move a dev license to a new machine or another developer | In Product Center, select the product and click "Deactivate License".   Then sign in to Product Center on the new machine (or as the other user) and activate it there |
| Product Center misbehaving after starting or stopping evaluations, or activating and deactivating licenses | Reset it (below).   Resetting doesn't affect licenses or the account |
| Work on Linux or macOS | Download the command-line Product Center from the Helpdesk.   It does the same things as the Windows version; the current Product Center page links to a command reference in the legacy deployment docs |
| Older Product Center for an older ThinkGeo version | "Legacy Downloads" on the Helpdesk main page |

Runtime licenses are free and perpetual, so deactivating or moving a dev license doesn't affect apps already deployed with a runtime license.

#### Resetting Product Center

1. Open Product Center.
2. If you have a purchased license, select each product marked "Activated" and click "Deactivate License".   (Evaluators skip this step.)
3. Click "Log Out" (upper right) and close Product Center.
4. Delete the folder `C:\ProgramData\ThinkGeo\Map Suite xx.x`, where `xx.x` is the version folder present on the machine.
5. Open Product Center, sign in, and reactivate your purchases or evaluations.
