---
type: llm
focus:
  source: file
  path: "ParcelTiles/Controllers/TilesController.cs"
---

This is the tile controller of an ASP.NET Core ThinkGeo WebAPI service for a State Plane (EPSG:2276) parcel shapefile, labelled with OWNER_NAME, to be deployed to IIS and shown in Leaflet.

PASS only if all of these hold:
- A `{z}/{x}/{y}` action draws the tile: a `GeoImage` of a fixed size, `GeoCanvas.CreateDefaultGeoCanvas()`, `WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter)` (or an overload with a matching tile size), `BeginDrawing`/`Draw`/`EndDrawing`, and returns PNG bytes.
- The shapefile layer has `ProjectionConverter(2276, 3857)` and the canvas uses `GeographyUnit.Meter`.
- The data path comes from the content root or application base directory (for example `IWebHostEnvironment.ContentRootPath` or `AppContext.BaseDirectory`), not `Directory.GetCurrentDirectory()` or `../../..`.
- Labels use a `TextStyle` on OWNER_NAME, and the layer sets `DrawingMarginInPixel` (or otherwise handles labels clipped at tile edges).
- No ThinkGeo Cloud ClientId/ClientSecret appears in this server code unless loaded from configuration.

FAIL if any item is missing or wrong, or if the code uses a ThinkGeo API name that looks invented.
