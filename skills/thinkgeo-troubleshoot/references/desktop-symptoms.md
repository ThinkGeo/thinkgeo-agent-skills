# Desktop Symptom Guide (WPF & WinForms)

Use this file from the `thinkgeo-troubleshoot` skill when the platform is WPF or WinForms.   It lists the known causes for each symptom in the order they are most often responsible.   Treat each entry as a **likely cause** until the user's code, error text, or a diagnostic check confirms it, and verify any replacement API with `tg_api` before recommending it.   Correct setup patterns live in the `thinkgeo-desktop-maps` skill.

## First step for any rendering problem: make errors visible

By default, drawing exceptions are swallowed so one bad layer doesn't crash the app.   While debugging, surface them:

```csharp
var overlay = new LayerOverlay { ThrowingExceptionMode = ThrowingExceptionMode.ThrowException };
overlay.ThrowingException += (s, e) =>
{
    Debug.WriteLine($"Drawing error: {e.Exception}");
    e.Handled = true;   // log, don't crash
};
```

Also wrap `async void` handlers in `try/catch` and log; an exception in initialization otherwise disappears and leaves a half-built map.

## Symptom: blank map, or my layer doesn't show

Check in this order.   The first three account for most cases.

1. **Projection.** Is the data in the map's projection?   A shapefile in lat/lon (EPSG:4326) on a Spherical Mercator map (EPSG:3857) with no `ProjectionConverter` draws as a dot near 0,0, off the coast of Africa.   State Plane data without a converter lands far away.   Fix: `layer.FeatureSource.ProjectionConverter = new ProjectionConverter(dataEpsg, 3857)`.   Check the `.prj` file for the data's projection.
2. **Converter order.** Is it `(data, map)`?   `(3857, 4326)` on 4326 data is backwards.
3. **Zoom level styles.** Is there a style, and does `ApplyUntilZoomLevel` cover the current zoom?   A style on `ZoomLevel01` with no `ApplyUntilZoomLevel` draws only when fully zoomed out.   Also check for gaps between scale bands, and that the style type matches the geometry (an `AreaStyle` doesn't draw points).
4. **`MapUnit`.** Set before overlays are added, and matching the external projection (`Meter` for 3857).
5. **Extent.** Is the map looking at the data?   Temporarily set `MapView.CurrentExtent = layer.GetBoundingBox()` (open the layer first) and refresh.
6. **Overlay added and refreshed.** Was the `LayerOverlay` actually added to `MapView.Overlays`?   Was a refresh called after?
7. **Order.** Is an opaque overlay (a basemap added last, or an imagery layer) covering the data?   Overlays draw in the order added.
8. **Initialization timing.** Did setup run before the control had a size?   Use `SizeChanged` with an `_initialized` guard in WPF, or the form `Load` event in WinForms.
9. **File paths.** Relative paths resolve from the working directory, which differs between Visual Studio, shortcuts, and services.   Check the file exists at runtime; for shapefiles, the `.shx` and `.dbf` must sit beside the `.shp`.
10. **Basemap only is blank.** For `ThinkGeoCloudVectorMapsOverlay`, check the ClientId/ClientSecret, network or proxy access to ThinkGeo Cloud, and that the machine isn't offline.   For offline basemaps, see the `thinkgeo-offline-maps` skill.

## Symptom: pink or magenta tiles

A layer threw while drawing.   Turn on `ThrowingExceptionMode.ThrowException` (above) to see the exception.   Common causes:

- A style references a column that wasn't fetched or doesn't exist (check spelling and case).
- A `NullReferenceException` in a custom style or custom layer.
- A corrupt or locked data file, or a missing native dependency for GDAL-based layers.
- Index out of range in line labeling on degenerate geometry.

Subclasses can override `DrawExceptionCore(GeoCanvas canvas, Exception e)` to draw a friendlier error tile in production.

## Symptom: license watermark or license exception

| What they see | Cause | Fix |
| --- | --- | --- |
| Exception on first run: licenses not installed | No dev license on this machine | Install ThinkGeo Product Center, sign in, start an evaluation or activate the product |
| Blank white map, "Not Licensed for Run Time" (deployed machine) | Runtime license missing, not beside the exe, or generated for a different exe name | Regenerate with Product Center's Runtime License tab for the current exe and ship it beside the exe |
| "Your subscription license has expired" | Evaluation or subscription ended | Renew, then regenerate the runtime license |
| "N days left" watermark | Evaluation license | Activate a purchased license |

Point users to ThinkGeo support for license account issues.   Never suggest ways to bypass licensing.

## Symptom: map doesn't update after I change data

- The overlay holding an `InMemoryFeatureLayer` or other changing data uses the default `TileType.MultiTile`, which shows cached tiles.   Set `TileType = TileType.SingleTile`.
- No refresh after changing `InternalFeatures` or styles.   Add `await overlay.RefreshAsync()`.
- Two parts of the app call `MapView.RefreshAsync()` close together; the second cancels the first.   Have each part refresh its own overlay instead.
- A `FileRasterTileCache` on the data overlay is serving old tiles.   Clear it (`overlay.TileCache.ClearCache()`) or change the cache ID.
- `overlay.RefreshAsync()` was called on an overlay that isn't in `MapView.Overlays` yet; it silently does nothing.
- Styles were rebuilt by adding to `CustomStyles` without clearing it first, so old styles still draw.

## Symptom: exceptions

| Exception | Cause | Fix |
| --- | --- | --- |
| Build error CS0246: `LayerOverlay` (or `MapView`, another overlay) could not be found | Missing `using ThinkGeo.UI.Wpf;` (WinForms: `ThinkGeo.UI.WinForms`).   Common when code is copied from a HowDoI sample, which doesn't need the using because it lives in `namespace ThinkGeo.UI.Wpf.HowDoI` | Add the UI using alongside `using ThinkGeo.Core;` |
| Build error CS0103: `Path` does not exist (WPF) | WPF projects leave `System.IO` out of the implicit usings | Add `using System.IO;` |
| `TaskCanceledException` / `OperationCanceledException` from `ZoomToAsync`, `CenterAtAsync`, `ZoomInAsync` | A newer navigation or user pan/zoom superseded it. Navigation reports this by design. | Catch `OperationCanceledException` and ignore it, or fire-and-forget with `_ =`. `RefreshAsync` doesn't throw when superseded. |
| `KeyNotFoundException` on `feature.ColumnValues["X"]` | Column not requested in the query | Use `ReturningColumnsType.AllColumns` or include the column name |
| `InvalidOperationException`: the projection is not open | A standalone `ProjectionConverter` or a layer's source was used before opening | Call `layer.Open()` (or `converter.Open()`) first; close in `finally` |
| Exception calling `Clear()` or querying a layer | Layer not open | `layer.Open()` before, `layer.Close()` after |
| `IOException` when committing edits: "open the file with ReadWrite mode" | Layer constructed read-only (the default) | Construct it with `FileAccess.ReadWrite`, for example `new ShapeFileFeatureLayer(path, FileAccess.ReadWrite)` |
| `IOException` when committing edits (file in use) | Another process or layer instance has the file open | Close other handles to the same file before `CommitTransaction()` |
| Saved edits land near 0,0 or far from where they were drawn | Features converted with `ConvertToInternalProjection` before `EditTools.Add`/`Update` on a layer that already has a `ProjectionConverter`, so they were converted twice | Pass map coordinates straight to `EditTools`; the layer's converter handles the file projection |
| `NullReferenceException` inside `IsDrawingNeededCore` | Refresh before the map was laid out | Initialize in `SizeChanged` with a guard |

## Symptom: labels missing

- `TextStyle` column name wrong or not fetched; for `InMemoryFeatureLayer`, columns not declared with `layer.Columns.Add`.
- Labels are colliding and being dropped.   Reduce font size, lower `GridSize`, or show labels only at closer zoom levels.
- `DrawingLevel` isn't `LabelLevel`, so labels are hidden under features.
- Offline vector tiles: Style JSON `glyphs` URL is remote.   Use local fonts.
- The font isn't installed on the target machine.

## Symptom: slow rendering

- Large shapefile with no spatial index: build one with `ShapeFileFeatureLayer.BuildIndexFile(path)`.
- Basemap without a `TileCache`: add `FileRasterTileCache`.
- Static data in `SingleTile` mode re-renders on every pan: use the default `MultiTile` for static layers.
- Too many points at low zoom: use `ClusterPointStyle`, or only show the layer from a closer zoom level.
- Whole-map refresh on every update: refresh only the changed overlay.
- Database layers pulling everything: filter in SQL and request only needed columns.
- Hundreds of `Marker` objects: switch to a feature layer with a `PointStyle`.
- Complex static maps on slow hardware: pre-generate a tile cache (see `thinkgeo-offline-maps`).
- WPF: set `DefaultOverlaysRenderSequenceType="Concurrent"` on the `MapView` to draw overlays in parallel.

## Symptom: clicks feel delayed

`MapClick` waits for the double-click interval by default.   Set `MapView.ClickDoubleClickMode = MapClickDoubleClickMode.RaiseClickThenDoubleClick`, or handle `MapMouseDown` / `MapMouseUp`.

## Symptom: edit handles don't appear / drawn shapes duplicate

- `EditOverlay.CalculateAllControlPoints()` not called after filling `EditShapesLayer`.
- The feature is in both `EditShapesLayer` and the app's own layer, so it draws twice.
- WinForms: `MapView.EditOverlay` not added to `MapView.Overlays` when other overlays were added later.
- `TrackMode` left on after drawing, so the map won't pan.

## Escalation

Ask for the ThinkGeo package version, target framework, the setup code, the data's projection (`.prj` contents), and the exception with stack trace.   Build the smallest repro: one basemap and the problem layer only.   If it still fails, suggest posting the repro at https://community.thinkgeo.com or contacting ThinkGeo support.
