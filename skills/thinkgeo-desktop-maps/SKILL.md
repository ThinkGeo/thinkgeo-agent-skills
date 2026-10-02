---
name: thinkgeo-desktop-maps
description: Build WPF and WinForms map applications with ThinkGeo Desktop Maps (ThinkGeo.UI.Wpf, ThinkGeo.UI.WinForms, ThinkGeo.Core). Use this skill whenever the user is writing or changing C# code that uses ThinkGeo, MapView, LayerOverlay, ShapeFileFeatureLayer, InMemoryFeatureLayer, ThinkGeoCloudVectorMapsOverlay, ZoomLevelSet, ProjectionConverter, or ThinkGeo styles, or asks how to add a map, basemap, shapefile, layer, label, or thematic style to a .NET desktop app, even if they don't say "ThinkGeo" explicitly but the project references ThinkGeo packages.
---

# ThinkGeo Desktop Maps (WPF & WinForms)

This skill helps you write correct, idiomatic code for ThinkGeo Desktop Maps, version 14 and later.   The API is shared between WPF and WinForms; only the host control and event wiring differ.

## Related skills

- `thinkgeo-desktop-interaction` covers click-to-identify, spatial queries, drawing, editing, markers, and popups.
- `thinkgeo-offline-maps` covers air-gapped and disconnected deployments.
- `thinkgeo-troubleshoot` covers blank maps, pink tiles, license watermarks, and other runtime problems.
- `thinkgeo-code-example` is the verification workflow to follow whenever you write ThinkGeo code; this skill supplies the desktop patterns.

## Verify against the docs when you can

If the ThinkGeo Documentation MCP server is connected (tools named `tg_search`, `tg_find_sample`, `tg_api`, `tg_get`), use it before writing anything non-trivial:

- `tg_find_sample` with `platform: "wpf"` or `"winforms"` finds a runnable HowDoI sample for a feature.   Prefer adapting a sample over inventing code.
- `tg_api` confirms that a class, member, or overload exists.   Use it whenever you are unsure of a name, because similar names exist across versions and platforms (for example, `TileType.MultiTile` on desktop, not `MultipleTiles`).
- `tg_search` with `namespaces: ["docs"]` finds developer guides; the "Developer Guides" pages contain "Common Pitfalls" sections that are worth reading.

If the server is not connected, rely on this skill and its references, and point the user to https://docs.thinkgeo.com and the HowDoI samples at https://gitlab.com/thinkgeo/public/thinkgeo-desktop-maps.   Avoid APIs marked Legacy (V13 and before) unless the user is on an old version.

## The object model in one picture

```
MapView                      MapUnit, CenterPoint, CurrentScale
└── Overlays (ordered, first added = bottom)
    ├── ThinkGeoCloudVectorMapsOverlay    basemap
    ├── LayerOverlay                      your data
    │   └── Layers (ordered)
    │       └── FeatureLayer              ShapeFile, InMemory, Sqlite, ...
    │           ├── FeatureSource
    │           │   └── ProjectionConverter
    │           └── ZoomLevelSet          styles per scale band
    └── TrackOverlay / EditOverlay        built-in, always on top
```

All GIS logic (layers, styles, features, projections, queries) lives in `ThinkGeo.Core` and is identical on every ThinkGeo platform.   The UI package only provides the control.

## Project setup

Read `references/project-setup.md` when creating a new project, adding the NuGet packages, setting up licensing, or preparing a deployment.   Key points:

- WPF: install `ThinkGeo.UI.Wpf`.   WinForms: install `ThinkGeo.UI.WinForms` and set `<UseWPF>true</UseWPF>` in the `.csproj` (the WinForms control depends on WPF assemblies).
- Target .NET 8 or later for new projects; .NET Framework 4.6.2+ is also supported.
- The first run without a license throws a "licenses not installed" exception.   This is expected; the developer activates an evaluation or purchased license in ThinkGeo Product Center.   Don't try to code around it.
- Starter files are in `assets/` (`MainWindow.xaml`, `MainWindow.xaml.cs`, `MainForm.cs`, and project files).   Copy and adapt them rather than writing from scratch.

## The initialization pattern

Every map follows the same order.   Getting the order wrong is the most common cause of blank maps.

```csharp
private bool _initialized;

private async void MapView_SizeChanged(object sender, SizeChangedEventArgs e)
{
    // Initialize once, after the control has a real size.
    if (_initialized || e.NewSize.Width <= 0 || e.NewSize.Height <= 0) return;
    _initialized = true;

    try
    {
        // 1. Map unit FIRST. Meter = Spherical Mercator (EPSG:3857), which matches ThinkGeo Cloud and most web basemaps.
        MapView.MapUnit = GeographyUnit.Meter;

        // 2. Basemap (bottom of the stack), always with a tile cache.
        MapView.Overlays.Add(new ThinkGeoCloudVectorMapsOverlay
        {
            ClientId = "YOUR_CLIENT_ID",          // from https://cloud.thinkgeo.com
            ClientSecret = "YOUR_CLIENT_SECRET",
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(@".\cache", "basemap_light")
        });

        // 3. Data layer: projection converter, then styles.
        var parcels = new ShapeFileFeatureLayer(@".\Data\Parcels.shp");
        parcels.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857); // (data EPSG, map EPSG)
        parcels.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.SteelBlue), GeoColors.SteelBlue, 1);
        parcels.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // 4. Layer into a keyed LayerOverlay, overlay into the map.
        var dataOverlay = new LayerOverlay();
        dataOverlay.Layers.Add("Parcels", parcels);
        MapView.Overlays.Add("DataOverlay", dataOverlay);

        // 5. Initial view, then refresh.
        MapView.CenterPoint = new PointShape(-10777290, 3908740);
        MapView.CurrentScale = 20000;
        await MapView.RefreshAsync();
    }
    catch (Exception ex)
    {
        // async void handlers can't propagate exceptions; log them.
        Debug.WriteLine(ex);
    }
}
```

The HowDoI samples initialize in `SizeChanged` with an `_initialized` guard so that sizes and extents are valid.   The WPF quick start uses `Loaded`, which also works for simple maps.   In WinForms, use the form's `Load` event.   Never read `MapView.MapWidth` or `MapHeight` before the control is laid out; they are zero.

## Rules that prevent most bugs

1. **Set `MapUnit` before adding overlays.** It must match the external projection of every layer: `Meter` for EPSG:3857, `DecimalDegree` for EPSG:4326, `Feet` for US State Plane in feet.
2. **Reproject every layer that isn't already in the map's projection.** `new ProjectionConverter(dataEpsg, mapEpsg)` — internal is the file, external is the map.   Assign it before the layer is opened.   Details in `references/projections.md`.
3. **Always set `ApplyUntilZoomLevel`.** A style on `ZoomLevel01` with no `ApplyUntilZoomLevel` only draws at zoom level 1, which is the most zoomed-out level.   `ZoomLevel01` is zoomed out and `ZoomLevel20` is zoomed in.   Details in `references/styling.md`.
4. **Use `TileType.SingleTile` for any overlay whose data changes at runtime** (`InMemoryFeatureLayer`, highlights, query results).   The default `MultiTile` mode caches tiles and shows stale content.
5. **Refresh the overlay you changed, not the whole map.** `await dataOverlay.RefreshAsync()` redraws one overlay without cancelling other draws.   `MapView.RefreshAsync()` redraws everything and cancels anything in flight.   Details in `references/refresh-and-async.md`.
6. **Give layers and overlays string keys** when you'll look them up later: `overlay.Layers.Add("Parcels", layer)`, then `MapView.FindFeatureLayer("Parcels")`.
7. **Open and close layers around direct queries.** Rendering opens layers automatically; `layer.QueryTools.*` calls outside rendering need `layer.Open()` / `layer.Close()` in a `try/finally`.
8. **Request the columns you use.** Styles and code that read `feature.ColumnValues["NAME"]` need that column fetched (`ReturningColumnsType.AllColumns` or an explicit list), and `InMemoryFeatureLayer` columns must be declared with `layer.Columns.Add(...)`.
9. **Dispose in WPF.** Windows or user controls that host a `MapView` should implement `IDisposable` and call `MapView.Dispose()`.   WinForms handles this through the control lifecycle.
10. **Never hard-code credentials in shared code.** Use placeholders for ThinkGeo Cloud keys and tell the user where to get their own.

## Choosing a layer type

| Data | Layer | Package / notes |
| --- | --- | --- |
| Shapefile | `ShapeFileFeatureLayer` | Build a spatial index for large files: `ShapeFileFeatureLayer.BuildIndexFile(path)` |
| Features created in code | `InMemoryFeatureLayer` | Put in a `SingleTile` overlay; declare columns first |
| GeoJSON | `Feature.CreateFeaturesFromGeoJson` into an `InMemoryFeatureLayer` | See the "Display a GeoJson File" sample |
| SQLite / GeoPackage | `SqliteFeatureLayer` / `GdalFeatureLayer` | GeoPackage needs `ThinkGeo.Gdal` |
| SQL Server / PostgreSQL | `SqlServerFeatureLayer` / `PostgreSqlFeatureLayer` | `ThinkGeo.SqlServer` / `ThinkGeo.PostgreSql`.   Filter in SQL where possible |
| GeoTIFF, ECW, MrSID, JPEG2000 | `GeoTiffRasterLayer`, `EcwGdalRasterLayer`, `MrSidGdalRasterLayer`, `Jpeg2000GdalRasterLayer` | The `...Gdal...` layers need `ThinkGeo.Gdal` |
| Vector tiles (MBTiles / MVT) | `VectorMbTilesAsyncLayer`, `MvtTilesAsyncLayer` | Call `await layer.OpenAsync()` before reading bounds.   PMTiles (`VectorPmTilesAsyncLayer`) needs ThinkGeo 15, not 14.5.3 |
| WMS / WMTS / WFS / OGC API | `WmsAsyncLayer`, `WmtsAsyncLayer`, `WfsV2AsyncLayer`, `OgcApiFeatureLayer` | |
| KML, GPX, TAB, CAD (.dwg), File Geodatabase, S-57 | `KmlGdalFeatureLayer`, `GpxFeatureLayer`, `TabFeatureLayer`, `CadFeatureLayer`, `FileGeoDatabaseFeatureLayer`, `NauticalChartsFeatureLayer` | KML needs `ThinkGeo.Gdal`; CAD, File Geodatabase and S-57 need `ThinkGeo.Cad`, `ThinkGeo.FileGeoDatabase`, `ThinkGeo.NauticalCharts` |

Use one version for every ThinkGeo package (14.5.3 is the current release as of October 2026).   The full type-to-package list is in the `thinkgeo-code-example` skill's `references/package-map.md`.   For anything else, check `references/sample-catalog.md`, which lists every WPF HowDoI sample with its key API.

## Overlay order

Add overlays bottom to top: basemap, static data, dynamic results, then interaction.   `TrackOverlay` and `EditOverlay` sit on top automatically in WPF.   Use `MapView.Overlays.Insert(0, overlay)` to force something to the bottom.

## Before you hand code back

Check the generated code against this list:

- `MapUnit` is set first and matches every layer's external projection.
- Every layer not in the map's projection has a `ProjectionConverter` with (data, map) order.
- Every style has a matching `ApplyUntilZoomLevel`.
- Dynamic overlays use `TileType.SingleTile`.
- Refreshes target the changed overlay.
- `async void` event handlers wrap their bodies in `try/catch`.
- Credentials are placeholders.
- Any API you weren't sure about was confirmed with `tg_api` or flagged to the user as unverified.

## Reference files

- `references/project-setup.md`: NuGet packages, `.csproj` settings, licensing, runtime license deployment.
- `references/projections.md`: EPSG codes, `ProjectionConverter` patterns, converting single points.
- `references/styling.md`: zoom levels, point/line/area/text styles, `ValueStyle`, `ClassBreakStyle`, clustering, heat maps.
- `references/refresh-and-async.md`: refresh rules, cancellation, `MapClick` timing.
- `references/sample-catalog.md`: every WPF HowDoI sample with its key API.
