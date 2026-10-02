# Deploying to Disconnected Machines

Sources: Desktop Deployment Guide and Licensing pages (docs.thinkgeo.com).

## Runtime license

1. On a connected development machine with ThinkGeo Product Center and an activated license, open the "Runtime License" tab.
2. Browse to the application's built executable and click "Save".   The runtime license file is written next to the executable.
3. Include that file in the installer so it lands in the same folder as the executable on target machines.
4. Re-generate it whenever the executable's file name changes.

Symptoms on the target machine:

| What the user sees | Likely cause |
| --- | --- |
| Blank white map with "Not Licensed for Run Time" watermark | Runtime license file missing, in the wrong folder, or generated for a different executable name |
| "Your subscription license has expired" message | Evaluation or subscription expired; renew and regenerate |
| Map renders with a "days left" watermark | Running on an evaluation license |

### Machines that never connect

No extra procedure is needed.   Target machines never activate anything and never contact ThinkGeo:

- The runtime license is generated once, on the developer's machine, and ships as a file with the app.   ThinkGeo licenses developers, not end users, and allows unlimited deployments.
- The file is tied to the executable's name, not to a machine, so the same file works on every target machine.
- It's perpetual: apps keep running without a watermark after the developer subscription expires.   Regenerating it (for example, after renaming the executable) needs an active developer license.

So the build machine, or any developer machine with Product Center, generates the file, and the installer copies it next to the executable.   Don't generate the license at install time on the target machine.   If Product Center can't generate the file (for example, the development network is also offline), ThinkGeo support can create one from the executable's file name.

Never write code that bypasses, patches, or fakes license checks.

### Diagnosing a license watermark on a locked-down machine

If the watermark appears and the file looks correct, log ThinkGeo's licensing messages to a file the user can send back:

```csharp
// At startup, before the map is created
var log = new StreamWriter(Path.Combine(logDir, "thinkgeo-license.log")) { AutoFlush = true };
ThinkGeoDebugger.LogStreamWriter = log;
ThinkGeoDebugger.LogType = ThinkGeoLogType.Licensing;
ThinkGeoDebugger.LogLevel = ThinkGeoLogLevel.All;
```

Turn this off after diagnosing; logging slows rendering.   Common causes: the file isn't next to the executable, the executable was renamed after the license was generated, or a security tool or group policy blocked or altered the file during copy.

## Native dependencies

- If the project references `ThinkGeo.Dependency.MicrosoftVisualCRunTime140`, install the Microsoft Visual C++ Redistributable on target machines.   On air-gapped networks, bundle the redistributable installer with the app; it can't be downloaded at install time.
- GDAL-based layers (ECW, MrSID, GeoPDF, KML via GDAL, `GdalRasterLayer`) bring native libraries through NuGet.   Publish for the correct runtime (`win-x64` is typical) and confirm the native DLLs appear in the publish output.
- For a self-contained deployment that doesn't need .NET installed on the target: `dotnet publish -c Release -r win-x64 --self-contained true`.

## Installer layout suggestion

```
C:\Program Files\YourApp\              (read-only after install)
    YourApp.exe
    <runtime license file>
    ThinkGeo and other DLLs
C:\ProgramData\YourApp\Maps\           (data, installed or updated separately)
    region.mbtiles
    style.json
    fonts\, sprites\
    Shapefiles\*.shp/.shx/.dbf/.prj/.idx/.ids
%LocalAppData%\YourApp\cache\          (writable tile cache)
```

Make the data path configurable (app settings or a command-line argument) so administrators can relocate large datasets.

## Validation before release

- Install on a clean VM with networking disabled.
- Confirm the basemap, every data layer, labels, and icons render at the lowest and highest zoom the app allows.
- Confirm there is no license watermark.
- Check application logs for swallowed drawing exceptions (see the `thinkgeo-troubleshoot` skill for `ThrowingExceptionMode`).
