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

If the customer's process requires licensing at install time on machines that never connect (for example, from an MSI custom action), have them confirm the currently supported procedure with ThinkGeo support for their license type and version.   Don't invent a procedure, and never write code that bypasses, patches, or fakes license checks.

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
