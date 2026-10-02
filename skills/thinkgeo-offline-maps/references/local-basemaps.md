# Local Basemaps and Tile Caches

Sources: Vector Tiles Support guide (docs.thinkgeo.com), and the HowDoI samples "Display Vector from MBTiles", "Display Raster from MBTiles", "Display Raster from File Tiles", "MVT with Local Fonts", "Pre-Generate Cache for Tile Overlay", and "Pre-Generate Cache for XYZ Layers".

All tile layers below are async layers in EPSG:3857, so set `MapView.MapUnit = GeographyUnit.Meter`.   Call `await layer.OpenAsync()` before reading `GetBoundingBox()`.

## Vector tiles: MBTiles

```csharp
MapView.MapUnit = GeographyUnit.Meter;

var basemap = new VectorMbTilesAsyncLayer(Path.Combine(dataDir, "region.mbtiles"));
basemap.StyleJsonUri = Path.Combine(dataDir, "style.json");   // optional

var basemapOverlay = new LayerOverlay();
basemapOverlay.Layers.Add("Basemap", basemap);
MapView.Overlays.Insert(0, basemapOverlay);         // bottom of the stack

await basemap.OpenAsync();
MapView.CurrentExtent = basemap.GetBoundingBox();
await MapView.RefreshAsync();
```

Both `xyz` and `tms` tile schemes are detected from the MBTiles metadata.   To change the style at runtime, set `StyleJsonUri`, then `await basemap.CloseAsync(); await basemap.OpenAsync();` and refresh.

### Style JSON rules for offline use

The Style JSON follows the MapLibre Style Spec.   For offline use:

- `sources` must point at local files, not `https://` tile URLs.
- `glyphs` (fonts) must be local.   If glyphs are remote, labels disappear without an error.   The "MVT with Local Fonts" sample shows how to use local `.ttf` files.
- `sprite` (icons) must be local, or icons disappear.
- The style's `source-layer` names must match the layer names inside the tile data.   Styles written for the OpenMapTiles schema work with OpenMapTiles-schema data.   Otherwise edit `source-layer` values (Maputnik is a common editor).

Without a Style JSON, the MBTiles (and v15 PMTiles) layers draw the data with ThinkGeo's built-in default line, area, and point styles.   A remote-source `MvtTilesAsyncLayer` with no Style JSON draws nothing, because the style is what tells it where the tiles are.

## Vector tiles: PMTiles (ThinkGeo 15 only)

`VectorPmTilesAsyncLayer` is not in 14.5.3.   It first appears in the 15.0 beta builds (October 2026).   Use this only if the project already targets ThinkGeo 15; on 14.5.3, use MBTiles above.

```csharp
var basemap = new VectorPmTilesAsyncLayer(
    Path.Combine(dataDir, "region.pmtiles"),
    Path.Combine(dataDir, "style.json"));          // optional; omit for ThinkGeo's default styling

var basemapOverlay = new LayerOverlay();
basemapOverlay.Layers.Add("Basemap", basemap);
MapView.Overlays.Insert(0, basemapOverlay);

await basemap.OpenAsync();
MapView.CurrentExtent = basemap.GetBoundingBox();
await MapView.RefreshAsync();
```

## Raster tiles

```csharp
var raster = new RasterMbTilesAsyncLayer(Path.Combine(dataDir, "imagery.mbtiles"));
var overlay = new LayerOverlay();
overlay.Layers.Add("Imagery", raster);
MapView.Overlays.Insert(0, overlay);
await raster.OpenAsync();
```

For z/x/y folder tiles exported from QGIS or similar tools, subclass `RasterXyzTileAsyncLayer` and override `GetTileAsyncCore` to read `{z}\{x}\{y}` files.   The "Display Raster from File Tiles" sample has a ready-made class, `XyzFileTilesAsyncLayer`, defined in the sample itself (it isn't part of ThinkGeo).   Copy it and adjust the file extension.

Raster tiles look blurry when zoomed beyond their highest zoom level.   Check `MaxZoomOfTheData`, and use `MapView.MaximumScale` or `MinimumScale` to stop users zooming past usable detail.

## Imagery files

GeoTIFF, ECW, MrSID, and JPEG2000 use `GeoTiffRasterLayer`, `EcwGdalRasterLayer`, `MrSidGdalRasterLayer`, and `Jpeg2000GdalRasterLayer` (`GdalRasterLayer` handles other GDAL raster formats).   All but `GeoTiffRasterLayer` need `ThinkGeo.Gdal`.   If the imagery isn't in EPSG:3857, reproject it with `GdalProjectionConverter` (see the "Project a Raster" sample), or keep the map in the imagery's projection and set `MapUnit` to match.

## Pre-generating a tile cache

Use this to turn a slow, complex map (many detailed layers) into fast tiles.   Generate on a build machine, ship the cache folder, and read from it in the field.

```csharp
// Build machine
var overlay = new LayerOverlay();
overlay.Layers.Add(streetsLayer);
overlay.Layers.Add(parcelsLayer);

var cache = new FileRasterTileCache(cacheRoot, "basemap_v1");
overlay.TileCache = cache;

RectangleShape bbox = /* area to cover, in map units */;
overlay.TileMatrixSet = TileMatrixSet.CreateTileMatrixSet(512, MaxExtents.SphericalMercator, GeographyUnit.Meter);

// Static method on LayerBase (the HowDoI sample calls it as Layer.GenerateTileCacheAsync).
// An optional CancellationToken can be passed as the last argument.
await LayerBase.GenerateTileCacheAsync(
    overlay.Layers, cache, overlay.TileMatrixSet,
    bbox, GeographyUnit.Meter,
    startZoom: 10, endZoom: 17,
    progress => Console.WriteLine($"{progress.TilesCompleted}/{progress.TotalTileCount}"),
    1.0f,                     // scale factor; match the target screens' DPI scaling
    OverwriteMode.Overwrite);
```

```csharp
// Deployed app
var overlay = new LayerOverlay
{
    TileCache = new FileRasterTileCache(cacheRoot, "basemap_v1"),
    IsCacheOnly = true          // never render, only read cached tiles
};
overlay.TileMatrixSet = TileMatrixSet.CreateTileMatrixSet(512, MaxExtents.SphericalMercator, GeographyUnit.Meter);
MapView.ZoomScales = overlay.TileMatrixSet.GetScales();   // snap zooming to the cached levels
```

Gotchas:

- The deployed app must use the **same** tile size, tile matrix, and cache ID as the build.   A mismatch means every tile misses and the map looks blank.
- Tile counts grow about four times per zoom level.   Estimate size before generating street-level zooms over large areas.
- The scale factor (DPI) used at generation should match the screens it will be viewed on, or text and lines will look too small or too large.
- Version the cache ID (`basemap_v1`, `basemap_v2`) so updated caches don't mix with old tiles.
- Generating a cache from data you don't own (for example, a third-party basemap) may be restricted by that data's license.   Confirm before caching it.
- `LayerOverlay.GenerateTileCacheAsync` and `RasterXyzTileAsyncLayer.GenerateTileCacheAsync` also exist.   Check their signatures with `tg_api` if you prefer to generate from the overlay or a single tile layer.
