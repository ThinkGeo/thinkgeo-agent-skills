---
name: thinkgeo-offline-maps
description: Build ThinkGeo WPF and WinForms map applications that run without internet access — air-gapped networks, field laptops, vehicles, ships, secure facilities, and disconnected or low-bandwidth environments. Use this skill whenever the user mentions offline, air-gapped, disconnected, no internet, classified or secure network, SIPR/JWICS-style environments, field deployment, local basemaps, MBTiles, PMTiles, pre-caching tiles, or shipping map data with a ThinkGeo desktop app, even if they only ask "how do I get a basemap without the cloud".
---

# ThinkGeo Offline and Air-Gapped Maps (Desktop)

ThinkGeo's rendering engine runs entirely on the local machine, so a desktop map can work with no network connection at all.   The work is in choosing local data, packaging it, and making sure nothing in the app quietly reaches for the internet.

This skill builds on `thinkgeo-desktop-maps`; follow its setup and styling rules.   If the ThinkGeo Documentation MCP server is available on the development machine, use `tg_find_sample` and `tg_api` to confirm class names (the target machine itself won't need it).

## Step 1: ask what "offline" means for this user

Clarify these before writing code, because they change the design:

1. **Never connected** (air-gapped) or **sometimes connected** (field laptop that syncs at base)?
2. **What area and zoom range** must work offline?   A city at street level is a few hundred MB; a country at street level can be tens of GB.
3. **What basemap source** are they licensed to use offline?   (Their own data, a purchased MBTiles/PMTiles extract, OpenMapTiles-schema data, or imagery such as GeoTIFF/ECW/MrSID.)
4. **How is software moved onto the machines?** (Installer, removable media, a software center.)   This affects how the runtime license and data ship.

## Step 2: choose the basemap

| Option | Layer | Good for |
| --- | --- | --- |
| Vector tiles in one file (`.pmtiles`) | `VectorPmTilesAsyncLayer` | Large areas, small footprint, crisp at every zoom, restylable |
| Vector tiles in SQLite (`.mbtiles`) | `VectorMbTilesAsyncLayer` | Same, with a widely supported container |
| Raster tiles (`.mbtiles`) | `RasterMbTilesAsyncLayer` | Pre-rendered maps or scanned charts |
| Raster tiles in folders (z/x/y) | `XyzFileTilesAsyncLayer` | Tiles exported from QGIS or other tools |
| Imagery (GeoTIFF, ECW, MrSID, JPEG2000) | `GeoTiffRasterLayer`, `EcwGdalRasterLayer`, `MrSidGdalRasterLayer`, `GdalRasterLayer` | Aerial or satellite imagery |
| Your own vector data (shapefile, GeoPackage, SQLite, File Geodatabase) styled as a basemap | Feature layers + styles | Full control, uses data the customer already owns |
| Nautical charts (S-57) | `NauticalChartsFeatureLayer` | Maritime |
| Pre-generated tile cache of any overlay | `LayerOverlay` + `FileRasterTileCache` + `IsCacheOnly` | Freezing a complex, slow-to-render map into fast tiles |

Vector tiles (PMTiles or MBTiles) are usually the best default for a general-purpose street basemap.   Details and code are in `references/local-basemaps.md`.

Sources for vector tile data include MapTiler, OpenMapTiles extracts, and Protomaps builds.   Most need a license for production use; confirm the user has one.

## Step 3: remove every network dependency

Check the code for each of these and replace or remove it:

- `ThinkGeoCloudVectorMapsOverlay`, `ThinkGeoCloudRasterMapsOverlay`, and any `*CloudClient` (geocoding, routing, elevation).   These call ThinkGeo Cloud.
- `GoogleMapsOverlay`, `AzureMapsRasterOverlay`, `OpenStreetMapOverlay`, WMS/WMTS/WFS layers pointing at public servers.
- An `MvtTilesAsyncLayer` whose Style JSON or `sources` point at `http(s)` URLs.   Every URL inside a Style JSON must be local: tile sources, **glyphs (fonts), and sprites**.   Remote glyph or sprite URLs make labels and icons silently disappear offline.   See the "MVT with Local Fonts" sample.
- Fonts used by `TextStyle` / `GeoFont` that may not be installed on locked-down machines.   Prefer standard Windows fonts or ship the font.
- Any code that downloads data on first run.

Then test the way the user will deploy: on a machine or VM with the network adapter disabled.   Don't rely on a machine that has cached tiles from development.

## Step 4: package the data

- Put data under the application folder (for example, `Data\`) and mark it `CopyToOutputDirectory` in the `.csproj`, or install it to a known location (for example, `%ProgramData%\YourApp\Maps`) and make the path configurable.
- Resolve paths from `AppDomain.CurrentDomain.BaseDirectory` or configuration, not the current working directory; shortcuts and services often start in a different folder.
- For large data, ship it separately from the application installer so app updates don't re-copy gigabytes.
- Build shapefile spatial indexes (`ShapeFileFeatureLayer.BuildIndexFile`) before shipping, and ship the `.idx`/`.ids` files.   Building them on a read-only install folder fails.
- Tile caches must live in a **writable** folder if the app writes to them; use `%LocalAppData%` or `%ProgramData%` rather than `Program Files`.

## Step 5: licensing on disconnected machines

Read `references/deployment.md`.   In short: generate the runtime license on a connected development machine with ThinkGeo Product Center, then ship the license file next to the application executable.   Rebuild it if the executable name changes.   For a fully air-gapped installation process (for example, licensing at install time from an MSI), have the user confirm the current supported procedure with ThinkGeo support, since that procedure depends on their license type and version.   Never write code that bypasses or fakes license checks.

## Pre-generating a tile cache

When a map is built from many detailed layers, rendering on the fly can be slow on field hardware.   Render it once into tiles at build time, ship the tiles, and have the deployed app read only from cache:

```csharp
overlay.TileCache = new FileRasterTileCache(cacheFolder, "basemap_v1");
overlay.IsCacheOnly = true;   // deployed app: read tiles, never render
```

The generation code (`LayerBase.GenerateTileCacheAsync`) and its gotchas are in `references/local-basemaps.md`.

## Checklist

- No class or URL in the app requires internet (search the code for `http`, `Cloud`, `Google`, `Azure`, `OpenStreetMap`, `Wms`, `Wmts`, `Wfs`).
- Style JSON files reference only local tiles, glyphs, and sprites.
- `MapView.MapUnit` matches the basemap (vector and raster tile layers are EPSG:3857, `GeographyUnit.Meter`).
- Data paths are absolute or base-directory relative, and configurable.
- Caches and anything the app writes go to a writable location.
- Spatial indexes are built before shipping.
- The runtime license file ships next to the executable.
- The app was tested with networking disabled.
