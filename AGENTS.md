# ThinkGeo development guide for AI coding assistants

Rules and pointers for assistants writing, reviewing, or debugging .NET code that uses ThinkGeo (`ThinkGeo.Core`, `ThinkGeo.UI.Wpf`, `ThinkGeo.UI.WinForms`, `ThinkGeo.UI.Blazor`, `ThinkGeo.UI.Maui`, `ThinkGeo.UI.WebApi`, and the ThinkGeo Cloud clients).

This file is the condensed version of the ThinkGeo Agent Skills (https://github.com/ThinkGeo/thinkgeo-agent-skills).   If your assistant supports Agent Skills, install those instead (`npx skills add ThinkGeo/thinkgeo-agent-skills`); they hold the full patterns.   If it reads `AGENTS.md` but not skills, copy this file into your project root, or append it to your existing `AGENTS.md`.

## Source of truth

Connect the ThinkGeo Documentation MCP server: `https://ai.thinkgeo.com/mcp` (HTTP, no key).   Its tools:

- `tg_find_sample`: find an official HowDoI sample for a feature and platform.   Prefer adapting a sample over inventing code.
- `tg_api`: confirm that a class, member, or overload exists before using it.   Similar names exist across versions and platforms.
- `tg_search` and `tg_get`: search and read the developer guides, community forum, and blog.   Use `tg_get` to see enum values.

Without the server, use https://docs.thinkgeo.com and the HowDoI sample repositories on https://gitlab.com/thinkgeo/public.   Avoid APIs marked Legacy (v13 and earlier) unless the project is on an old version.

## Rules

- Use the latest stable ThinkGeo version from NuGet for new projects, and keep every ThinkGeo package on that one version.   In an existing project, keep its version.   Don't use beta packages.
- Desktop code-behind needs `using ThinkGeo.UI.Wpf;` (or `ThinkGeo.UI.WinForms;`) as well as `using ThinkGeo.Core;`.   Sample code hides the UI `using` because samples are declared inside `namespace ThinkGeo.UI.Wpf.HowDoI`.
- WinForms projects need `<UseWPF>true</UseWPF>` in the `.csproj`.
- Set `MapUnit` before adding content, and handle projections explicitly: if the data's EPSG code differs from the map's, give the feature source `new ProjectionConverter(dataEpsg, mapEpsg)`.   ThinkGeo Cloud basemaps are EPSG:3857 (`MapUnit = GeographyUnit.Meter`).
- Apply styles to a zoom level and propagate them with `ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20`, or nothing draws at other scales.   `ZoomLevel01` is the most zoomed out.
- Redraw with `await mapView.RefreshAsync()` (desktop and MAUI) or `RedrawAsync` (Blazor).   Refresh only the overlay that changed.
- Shapefile editing needs `FileAccess.ReadWrite` on the layer.   With a `ProjectionConverter` on the layer, `EditTools.Add` and `Update` take map coordinates and convert on their own; don't convert first.
- Use placeholders for credentials (`YOUR_CLIENT_ID`, `YOUR_CLIENT_SECRET`, `YOUR_API_KEY`).   Never paste keys from documentation or samples.
- Desktop apps need a ThinkGeo license from Product Center; deployed apps need a runtime license, which is tied to the executable name.   MAUI apps load a license file with `LicenseLoader.LoadLicense`, even in emulators.   Never bypass or fake licensing.
- Say whether code was compiled, checked against the docs, or is illustrative.

## Names that don't exist

| In the code | Use instead |
| --- | --- |
| `Refresh()` on the map view | `await mapView.RefreshAsync()`; WPF has no synchronous version, and on WinForms it is `Control.Refresh()`, which doesn't redraw the map |
| `BuildIndex` on a shapefile layer | `ShapeFileFeatureLayer.BuildIndexFile(...)` |
| `MultipleTiles` as a tile type | `TileType.MultiTile` (the default) |
| `ShapeFileReadWriteMode` (legacy) | `FileAccess.ReadWrite` |
| `VectorPmTilesAsyncLayer` on 14.x | Needs ThinkGeo 15 or later; use `VectorMbTilesAsyncLayer` on 14.x |
| `XyzFileTilesAsyncLayer`, `FleeBooleanStyle`, `DynamicPointStyle` | Not ThinkGeo classes; they're defined inside HowDoI samples.   Copy them from the sample or write your own |

## Platform notes

- **Blazor:** `LayerOverlay` (server-side drawing) works only in Blazor Server.   WebAssembly and Hybrid support only client-drawn overlays such as `VectorTileOverlay` and `WmtsTileOverlay`.
- **MAUI:** the basemap classes are `ThinkGeoVectorOverlay` and `ThinkGeoRasterOverlay`, taps use `SingleTap` with `ToWorldCoordinate`, and data files must be copied to `FileSystem.Current.AppDataDirectory` (Android asset streams aren't seekable files).
- **WebAPI:** each tile request draws a `{z}/{x}/{y}` tile on the server; use `WebApiExtentHelper.GetBoundingBoxForXyz` and keep data in EPSG:3857 or convert it.
- **Offline:** use local MBTiles or a tile cache instead of Cloud basemaps.
- **Pink or magenta tiles** mean a layer threw while drawing.   Set `ThrowingExceptionMode = ThrowingExceptionMode.ThrowException` on the overlay to see the real error.

## Agent Skills in this package

| Skill | Use it for |
| --- | --- |
| `thinkgeo-docs-research` | Authoritative answers about ThinkGeo APIs, packages, formats, and behavior |
| `thinkgeo-code-example` | Writing ThinkGeo code and small projects with every API and package verified; licensing setup |
| `thinkgeo-code-review` | Reviewing and modernizing existing ThinkGeo code |
| `thinkgeo-architecture` | Choosing products, rendering location, data access, and deployment design |
| `thinkgeo-troubleshoot` | Evidence-driven diagnosis, with a symptom guide for WPF and WinForms |
| `thinkgeo-desktop-maps` | WPF and WinForms setup, projections, styling, refresh rules, starter projects |
| `thinkgeo-desktop-interaction` | Click-to-identify, highlighting, spatial queries, drawing, editing, markers, popups |
| `thinkgeo-offline-maps` | Air-gapped and disconnected deployments, local basemaps, tile caches |
| `thinkgeo-blazor` | Blazor map pages, Server versus WebAssembly, overlays, clicks, editing |
| `thinkgeo-maui` | .NET MAUI map apps, licensing on phones, shipping data files, taps, GPS |
| `thinkgeo-web-api` | ASP.NET Core tile services with Leaflet or OpenLayers |
| `thinkgeo-cloud-maps` | ThinkGeo Cloud basemaps, geocoding, routing, elevation, keys, quotas |
