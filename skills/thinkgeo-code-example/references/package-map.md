# ThinkGeo Package Map

Which NuGet package a ThinkGeo type comes from.   A missing extension package is a common reason generated code fails to build.

Evidence: the current WPF and WinForms HowDoI project files reference `ThinkGeo.Cad`, `ThinkGeo.FileGeoDatabase`, `ThinkGeo.Gdal`, `ThinkGeo.NauticalCharts`, `ThinkGeo.PostgreSql`, `ThinkGeo.Printers`, and `ThinkGeo.SqlServer` alongside the UI package, all at 14.5.3 (October 2026).   The type-to-package rows below follow ThinkGeo's naming; when in doubt, confirm with `tg_get` on the HowDoI project file (`wpfHowDoI:HowDoI.csproj`, `winformHowDoI:HowDoI.csproj`, `gisServerHowDoI:ThinkGeo.GisServer.Samples.csproj`) and the sample that uses the type.

## UI and server packages (pick one per project)

| Package | Provides |
| --- | --- |
| `ThinkGeo.UI.Wpf` | WPF `MapView`, overlays.   Brings in `ThinkGeo.Core`. |
| `ThinkGeo.UI.WinForms` | WinForms `MapView`, overlays.   Needs `<UseWPF>true</UseWPF>`. |
| `ThinkGeo.UI.Blazor` | Blazor `MapView` component |
| `ThinkGeo.UI.Maui` | .NET MAUI `MapView` |
| `ThinkGeo.GisServer` | ASP.NET Core OGC/XYZ/GeoJSON service host.   The official sample currently references a beta build; check before pinning. |
| `ThinkGeo.Core` | Engine only, for headless tools and services |

## Usually covered by ThinkGeo.Core (no extension package)

The HowDoI project files don't add an extension package for these, so they are expected to come with the UI package's `ThinkGeo.Core` dependency.   If a build can't find one of these types, check the sample's project file before adding packages.

Shapefile, TAB, TinyGeo, GPX, SQLite, ESRI Grid, GeoJSON (via `Feature.CreateFeaturesFromGeoJson`), `InMemoryFeatureLayer`, GeoTIFF, common images (`SkiaRasterLayer`), MBTiles/PMTiles/MVT vector and raster tile layers, WMS/WMTS/WFS/OGC API layers, all styles, `ProjectionConverter`, geometry and spatial queries.

## Extension packages

| Package | Types (examples) |
| --- | --- |
| `ThinkGeo.Gdal` | `GdalFeatureLayer` (GeoPackage and other OGR formats), `GdalRasterLayer`, `EcwGdalRasterLayer`, `MrSidGdalRasterLayer` (MrSID, JPEG2000), `KmlGdalFeatureLayer`, `GeoPdfGdalFeatureLayer`, `GdalProjectionConverter` |
| `ThinkGeo.SqlServer` | `SqlServerFeatureLayer` |
| `ThinkGeo.PostgreSql` | `PostgreSqlFeatureLayer` |
| `ThinkGeo.FileGeoDatabase` | `FileGeoDatabaseFeatureLayer` |
| `ThinkGeo.Cad` | `CadFeatureLayer` (.dwg, .dxf) |
| `ThinkGeo.NauticalCharts` | `NauticalChartsFeatureLayer` (S-57) |
| `ThinkGeo.Printers` | `MapPrinterLayer` and printing support |

GDAL is a third-party library with native binaries.   Publish for a specific runtime (`win-x64` is typical) and check the native DLLs reach the output folder.

## Versions

- Use one version for every ThinkGeo package in a project.
- Current release (from the HowDoI project files, October 2026): **14.5.3**.   Re-check the project files before pinning, since releases move.
- Beta builds are named like `14.5.0-beta072` or `15.0.0-beta136`.   Don't use them unless the user asks or the product ships only as beta.
