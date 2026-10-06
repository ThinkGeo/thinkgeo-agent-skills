---
name: thinkgeo-web-api
description: Build ASP.NET Core map tile services with ThinkGeo WebAPI (ThinkGeo.UI.WebApi) and show them in Leaflet or OpenLayers. Use this skill whenever the user mentions ThinkGeo WebAPI, ThinkGeo.UI.WebApi, a map tile controller or endpoint, {z}/{x}/{y} tiles, WebApiExtentHelper, GetBoundingBoxForXyz, GeoCanvas or GeoImage on a server, Leaflet or OpenLayers with ThinkGeo, or symptoms such as blank or misplaced tiles, tiles offset from the basemap, labels cut off at tile edges, data that works locally but not after publishing, or a tile service failing on Linux or in Docker.
---

# ThinkGeo WebAPI (map tile services)

`ThinkGeo.UI.WebApi` lets an ASP.NET Core app draw map tiles on the server.   A controller action receives `{z}/{x}/{y}`, draws ThinkGeo layers into a tile image, and returns a PNG.   A JavaScript map in the browser (Leaflet or OpenLayers) requests and shows the tiles.   All GIS work (layers, styles, projections, queries) uses the same `ThinkGeo.Core` API as the desktop products.

Search the official samples with `tg_search` in the `webApiHowDoI` namespace (each sample comes in a Leaflet and an OpenLayers version), and check names with `tg_api`.

## Setup

- Project: ASP.NET Core Web API.   Package: `ThinkGeo.UI.WebApi`, plus any extension packages the data needs (see the `thinkgeo-code-example` skill's `references/package-map.md`), all at the same version.
- Usings: `ThinkGeo.Core` for layers and styles, `ThinkGeo.UI.WebApi` for `WebApiExtentHelper`.
- Licensing: a server needs a runtime license; a developer license isn't enough once the subscription ends.   See `thinkgeo-code-example/references/licensing.md` (Blazor and WebAPI section).

## The tile endpoint

```csharp
using Microsoft.AspNetCore.Mvc;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

[ApiController]
[Route("tiles")]
public class TilesController : ControllerBase
{
    private readonly string _dataFolder;

    public TilesController(IWebHostEnvironment env)
    {
        _dataFolder = Path.Combine(env.ContentRootPath, "App_Data");   // not Directory.GetCurrentDirectory()
    }

    [HttpGet("parcels/{z}/{x}/{y}")]
    public IActionResult GetParcelTile(int z, int x, int y)
    {
        var parcels = new ShapeFileFeatureLayer(Path.Combine(_dataFolder, "Parcels.shp"));
        parcels.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);   // (data EPSG, map EPSG)
        parcels.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.Red, 1);
        parcels.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var overlay = new LayerOverlay();
        overlay.Layers.Add(parcels);
        return DrawTile(overlay, z, x, y);
    }

    private IActionResult DrawTile(LayerOverlay overlay, int z, int x, int y)
    {
        using var image = new GeoImage(256, 256);
        var canvas = GeoCanvas.CreateDefaultGeoCanvas();
        RectangleShape tileExtent = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);

        canvas.BeginDrawing(image, tileExtent, GeographyUnit.Meter);
        overlay.Draw(canvas);
        canvas.EndDrawing();

        return File(image.GetImageBytes(GeoImageFormat.Png), "image/png");
    }
}
```

## Rules

1. **Tiles are Spherical Mercator.** XYZ tiles in Leaflet and OpenLayers are EPSG:3857, so draw with `GeographyUnit.Meter` and give every layer whose data is in another projection a `ProjectionConverter(dataEpsg, 3857)`.   Data drawn without one appears in the wrong place or not at all.
2. **Keep the tile size the same in three places:** `new GeoImage(256, 256)`, the bounding-box call, and the JavaScript layer.   `GetBoundingBoxForXyz(x, y, z, unit)` assumes 256-pixel tiles, which is also the Leaflet and OpenLayers default and what the samples use.   For another size, use the overload that takes `tileWidth` and `tileHeight`, set the same size in the JavaScript layer, and check the result against a known location.   A mismatch shows as tiles offset from the basemap or features at the wrong scale.
3. **Labels cut off at tile edges:** each tile is drawn on its own, so a label that crosses the edge is clipped.   Set `DrawingMarginInPixel` on the feature layer (the samples use values up to 300) so labels near the edge are drawn into neighbouring tiles too.
4. **Find data from the app's content root,** not the current directory.   `Directory.GetCurrentDirectory()` and paths like `../../../Data` only work when started from the project folder in Visual Studio; under IIS, a Windows service, or Docker they point elsewhere.   Use `IWebHostEnvironment.ContentRootPath` (or `AppContext.BaseDirectory`) and make sure the data is copied to the publish output.
5. **dBASE code pages:** if attribute text from a `.dbf` comes out garbled or throws an encoding error, call `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)` once at startup, as the samples do.
6. **Tiles are drawn concurrently.** A JavaScript map requests many tiles at once, so several requests can draw at the same moment.   The samples show two patterns: building the layer and overlay inside each request (simple, but the file is reopened for every tile), and keeping layers in `static` fields shared across requests.   If you share a layer, check its `ThreadSafe` property: a layer marked `ThreadSafetyLevel.Unsafe` must not be drawn or changed by two requests at once (the samples use `lock` around changes).   Choosing between the patterns and adding tile caching depend on the application; ask the user about traffic and how often the data changes rather than assuming.
7. **Linux and Docker:** ThinkGeo draws with SkiaSharp, whose native libraries need extra packages in Linux images (`libfontconfig1`, `libfreetype6`, and others; the WebAPI Quick Start's Dockerfile lists them).   A tile service that works on Windows but fails in a Linux container is often missing these.   Licensing for Linux and containers isn't covered by the current docs; see `licensing.md`.

## The browser side

The JavaScript map points a tile layer at the endpoint's URL template:

```javascript
// Leaflet
L.tileLayer('https://your-server/tiles/parcels/{z}/{x}/{y}').addTo(map);
```

```javascript
// OpenLayers
new ol.layer.Tile({ source: new ol.source.XYZ({ url: 'https://your-server/tiles/parcels/{z}/{x}/{y}' }) });
```

- A ThinkGeo Cloud basemap can be added in the browser directly from Cloud, using a **JavaScript** client key restricted to your domain (see the `thinkgeo-cloud-maps` skill).   Never put a ClientId and ClientSecret in browser code.
- Interaction such as identify or spatial queries uses separate endpoints that return JSON (for example GeoJSON from `Feature.GetGeoJson()` results); the "QueryTools" and "Drawing and Editing" samples show the round trip.   Check method names with `tg_api` before using them.

## Troubleshooting quick list

| Symptom | Check first |
| --- | --- |
| Blank tiles | Data path (rule 4); map unit and projection converter (rule 1); the layer's zoom-level styles cascade with `ApplyUntilZoomLevel.Level20` |
| Tiles in the wrong place or wrong scale | Tile size mismatch (rule 2); missing `ProjectionConverter` (rule 1) |
| Labels cut off | `DrawingMarginInPixel` (rule 3) |
| Works in Visual Studio, fails when published | Data path (rule 4); data files not copied to the publish output; runtime license |
| Fails only on Linux or in Docker | SkiaSharp native libraries (rule 7); licensing for containers (`licensing.md`) |
| Watermark on the tiles | Licensing (`licensing.md`) |
| Pink or error tiles | A layer threw while drawing; see the `thinkgeo-troubleshoot` skill |
