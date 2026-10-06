# ThinkGeo Package Map

Which NuGet package a ThinkGeo type comes from.   A missing extension package is a common reason generated code fails to build.

Evidence: every row below was checked against the public types in the ThinkGeo 14.5 package DLLs (14.5.3 and 14.5.5, October 2026).   The current WPF and WinForms HowDoI project files reference the same seven extension packages alongside the UI package.   Note that extension types still use the `ThinkGeo.Core` namespace, so the namespace doesn't tell you which package a type needs.   For a type not listed here, check the HowDoI project file with `tg_get` (`wpfHowDoI:HowDoI.csproj`, `winformHowDoI:HowDoI.csproj`) and the sample that uses the type.

Some HowDoI samples define their own helper classes (for example `XyzFileTilesAsyncLayer`, `DynamicPointStyle`, `FleeBooleanStyle`, `TimeBasedPointStyle`).   These aren't in any package.   If generated code uses one, copy the class from the sample or write it.

## UI and server packages (pick one per project)

| Package | Provides |
| --- | --- |
| `ThinkGeo.UI.Wpf` | WPF `MapView`, overlays.   Brings in `ThinkGeo.Core`. |
| `ThinkGeo.UI.WinForms` | WinForms `MapView`, overlays.   Needs `<UseWPF>true</UseWPF>`. |
| `ThinkGeo.UI.Blazor` | Blazor `MapView` component |
| `ThinkGeo.UI.Maui` | .NET MAUI `MapView` |
| `ThinkGeo.Core` | Engine only, for headless tools and services |

## Covered by ThinkGeo.Core (no extension package)

These ship in `ThinkGeo.Core`, which every UI package brings in.   If a build can't find one of these types, check for a missing `using ThinkGeo.Core;` or a version mismatch before adding packages.

Shapefile, TAB, TinyGeo, GPX, SQLite, ESRI Grid (`GridFeatureLayer`), GeoJSON (via `Feature.CreateFeaturesFromGeoJson`), `InMemoryFeatureLayer`, GeoTIFF (`GeoTiffRasterLayer`), common images (`SkiaRasterLayer`), MBTiles and MVT tile layers (`VectorMbTilesAsyncLayer`, `RasterMbTilesAsyncLayer`, `MvtTilesAsyncLayer`, `RasterXyzTileAsyncLayer`), WMS/WMTS/WFS/OGC API layers, all styles, `ProjectionConverter`, geometry and spatial queries.

**PMTiles needs ThinkGeo 15 or later.**   `VectorPmTilesAsyncLayer` isn't in any 14.x release (it first appears in the 15.0 betas), although the Vector Tiles Support guide describes it without a version note.   On 14.x, use MBTiles.

## Extension packages

| Package | Types (examples) |
| --- | --- |
| `ThinkGeo.Gdal` | `GdalFeatureLayer` (GeoPackage and other OGR formats), `GdalRasterLayer`, `GeoTiffGdalRasterLayer`, `EcwGdalRasterLayer`, `MrSidGdalRasterLayer`, `Jpeg2000GdalRasterLayer`, `KmlGdalFeatureLayer`, `GeoPdfGdalFeatureLayer`, `PersonalGeoDatabaseGdalFeatureLayer`, `GdalProjectionConverter` |
| `ThinkGeo.SqlServer` | `SqlServerFeatureLayer` |
| `ThinkGeo.PostgreSql` | `PostgreSqlFeatureLayer` |
| `ThinkGeo.FileGeoDatabase` | `FileGeoDatabaseFeatureLayer` |
| `ThinkGeo.Cad` | `CadFeatureLayer` (.dwg, .dxf) |
| `ThinkGeo.NauticalCharts` | `NauticalChartsFeatureLayer` (S-57) |
| `ThinkGeo.Printers` | `MapPrinterLayer`, `LegendPrinterLayer`, `ScaleBarPrinterLayer`, and other `...PrinterLayer` types (the `PrinterLayer` base class is in Core) |

The API reference doesn't list many classes from these packages (as of October 2026, `tg_api` finds none of the types in this table), so a "no match" for one of these types doesn't mean it's wrong.   Confirm extension types with `tg_find_sample` and the sample's code, or against this table.

GDAL is a third-party library with native binaries.   Publish for a specific runtime (`win-x64` is typical) and check the native DLLs reach the output folder.

## Versions

- Use one version for every ThinkGeo package in a project.
- New projects: the latest stable version on nuget.org.   ThinkGeo releases about monthly, and the HowDoI project files often lag behind, so don't copy the version from a sample.
- Existing projects: keep the project's version unless the user asks to upgrade, and don't cross a major version (for example 14 to 15) without checking the changelog.
- Beta builds have a suffix such as `-beta136`.   Don't use them unless the user asks.
- `ThinkGeo.Ecw`, `ThinkGeo.MrSid`, and `ThinkGeo.Jpeg2000` stopped at 14.2.1, and `ThinkGeo.Gdal` contains the same layers (`EcwGdalRasterLayer`, `MrSidGdalRasterLayer`, `Jpeg2000GdalRasterLayer`).   Use `ThinkGeo.Gdal` at the project's version instead of those packages.
