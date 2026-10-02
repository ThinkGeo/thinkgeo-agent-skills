# ThinkGeo Skill Evaluation Cases

These cases are intended for A/B testing: run once with only the ThinkGeo MCP tools available, then again with the ThinkGeo Developer Skills installed.   Record tool use, unsupported API claims, source quality, completeness, and build/runtime outcome.

## 1. WPF shapefile project
Prompt: "Create a minimal .NET 8 WPF app that displays a local polygon shapefile in ThinkGeo, styles the polygons, starts at a useful extent, and does not require ThinkGeo Cloud."

Expected skill behavior:
- Activates `thinkgeo-code-example` and `thinkgeo-desktop-maps`.
- Searches `docs` + `wpfHowDoI`.
- Verifies `ThinkGeo.UI.Wpf`, `MapView`, `ShapeFileFeatureLayer`, `LayerOverlay`, style APIs, and refresh lifecycle.
- Uses `RefreshAsync`, not `Refresh()`, even though `SampleTemplate.xaml.cs` shows `Refresh()` (stale-sample trap).
- Avoids unnecessary Cloud credentials.
- Labels result `source-verified` unless it was actually compiled.

## 2. Projection bug
Prompt: "My WPF shapefile is EPSG:4326, my map is EPSG:3857, and the layer is not visible.   Here is my layer code..."

Expected skill behavior:
- Activates `thinkgeo-troubleshoot` and reads `references/desktop-symptoms.md`.
- Checks `ProjectionConverter` and verifies converter direction.
- Checks that map extent/center is also in the map CRS.
- Does not jump directly to file corruption or licensing.

## 3. GIS Server project
Prompt: "Build a minimal ASP.NET Core ThinkGeo GIS Server that serves one shapefile through WMS and GeoJSON."

Expected skill behavior:
- Activates `thinkgeo-code-example`.
- Searches `docs` + `gisServerHowDoI` and retrieves the sample project file plus a shapefile server module.
- Verifies `AddThinkGeoWebServer`, `ThinkGeoWebServerOptions`, `WmsMapOptions`, `VectorMapOptions`, `RasterLayerDefinition`, `VectorLayerDefinition`, and `MapThinkGeoWebServer`.

## 4. Unsupported member hallucination trap
Prompt: "Use ShapeFileFeatureLayer.AutoDetectProjectionAndZoomToData() in a WPF project."

Expected skill behavior:
- Searches for the exact member before using it.
- If it is not documented, does not invent it and offers documented equivalents.

## 5. Version-sensitive code review
Prompt: "Review this ThinkGeo v14.5 WPF code and modernize it, but do not move me to beta packages."

Expected skill behavior:
- Activates `thinkgeo-code-review`.
- Keeps release packages unless a documented v14.5 replacement exists.
- Uses changelog evidence before labeling anything deprecated.

## 6. Architecture selection
Prompt: "We need a Windows/Linux self-hosted service exposing WMS, WMTS, XYZ, WFS and GeoJSON over local data with no cloud dependency.   What ThinkGeo architecture should we use?"

Expected skill behavior:
- Activates `thinkgeo-architecture`.
- Verifies GIS Server protocol support and server-side layer/data patterns.
- Separates documented capabilities from design recommendations.

## 7. Click to identify (WPF)
Prompt: "In my WPF ThinkGeo map, when the user clicks a parcel, highlight it and show the OWNER_NAME field in a TextBlock.   Parcels are a State Plane (EPSG:2276) shapefile on a Spherical Mercator map."

Expected skill behavior:
- Activates `thinkgeo-desktop-interaction` (and `thinkgeo-code-example` for verification).
- Keeps the converter on the parcel layer as `(2276, 3857)` and queries with `e.WorldLocation` directly.
- Puts highlights in a separate `InMemoryFeatureLayer` inside a `TileType.SingleTile` overlay and refreshes only that overlay.
- Opens and closes the layer around `QueryTools` with `try/finally`; requests the `OWNER_NAME` column.

## 8. Draw-to-select
Prompt: "Let users draw a polygon on my WinForms ThinkGeo map and list every hotel point inside it."

Expected skill behavior:
- Uses `TrackOverlay` with `TrackMode.Polygon` and `TrackEnded`; resets `TrackMode` to `None` and clears `TrackShapeLayer`.
- Uses `GetFeaturesWithin` or `GetFeaturesIntersecting`; casts `e.TrackShape` correctly.
- Unsubscribes handlers when the mode ends.

## 9. Air-gapped basemap
Prompt: "Our WPF app has to run on an air-gapped network.   Replace the ThinkGeo Cloud basemap with something local."

Expected skill behavior:
- Activates `thinkgeo-offline-maps`.
- Asks (or states assumptions) about area, zoom range, and licensed data source before picking a format.
- Uses `VectorMbTilesAsyncLayer` (or another local layer) and removes the Cloud overlay.   Doesn't use `VectorPmTilesAsyncLayer`, which isn't in 14.5.3.
- Warns that Style JSON `glyphs`, `sprite`, and `sources` must be local.
- Covers runtime license file deployment without suggesting any license bypass.

## 10. Pink tiles
Prompt: "Half my map tiles are pink after I added a TextStyle on the NAME column.   WPF, ThinkGeo 14.5.3."

Expected skill behavior:
- Activates `thinkgeo-troubleshoot`.
- Recommends `ThrowingExceptionMode.ThrowException` to surface the drawing exception.
- Lists the column-name / column-not-fetched cause as likely, with a diagnostic check, rather than asserting it.

## 11. Extension package trap
Prompt: "Show a GeoPackage file and an ECW image in a new .NET 8 WPF ThinkGeo app."

Expected skill behavior:
- Adds `ThinkGeo.Gdal` alongside `ThinkGeo.UI.Wpf`, both at the same version.
- Uses `GdalFeatureLayer` and `EcwGdalRasterLayer`, verified with `tg_api` or the HowDoI samples.
- Mentions publishing for a specific runtime because of GDAL native binaries.

## Scoring rubric (0 or 1 each)
1. Correct skill activates.
2. Correct namespaces searched.
3. Official docs used.
4. Official HowDoI sample used when implementation is requested.
5. Relevant source fetched with `tg_get` rather than relying only on snippets.
6. Package name verified.
7. Important ThinkGeo API symbols verified.
8. Projection/map-unit concerns handled when relevant.
9. Version uncertainty handled explicitly.
10. No invented ThinkGeo API.
11. No secret/test credentials embedded.
12. Output is complete enough for the requested task.
13. Source links/evidence are preserved.
14. Compile status is described accurately.
15. Result passes a real build/run when a capable test environment is available.
