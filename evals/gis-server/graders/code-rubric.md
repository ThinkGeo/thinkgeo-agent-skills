---
type: llm
focus:
  source: file
  path: "GisServerDemo/Program.cs"
---

This is Program.cs for an ASP.NET Core ThinkGeo GIS Server that should serve one shapefile (`Countries02.shp`, EPSG:4326) through WMS and GeoJSON.

PASS only if all of these hold:
- Services are registered with `AddThinkGeoWebServer` and endpoints mapped with `MapThinkGeoWebServer`.
- A WMS map is configured (for example `options.Wms.Maps[...] = new WmsMapOptions { ... }`) with a raster layer definition that creates a styled `ShapeFileFeatureLayer`.
- A GeoJSON map is configured (for example `options.GeoJson.Maps[...] = new VectorMapOptions { ... }`) with a vector layer definition over the shapefile's feature source.
- Each map's CRS and map unit agree with its data: either a ProjectionConverter(4326, 3857) with a 3857/meter map, or EPSG:4326 with decimal degrees.
- No ThinkGeo Cloud keys or credentials appear.

FAIL if any item is missing or wrong, or if the code uses a ThinkGeo API name that looks invented.
