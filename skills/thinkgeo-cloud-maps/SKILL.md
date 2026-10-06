---
name: thinkgeo-cloud-maps
description: Use ThinkGeo Cloud (Cloud Maps) services from ThinkGeo apps on any platform — WPF, WinForms, Blazor, MAUI, or plain .NET. Use this skill whenever the user mentions ThinkGeo Cloud, Cloud Maps, a cloud basemap (ThinkGeoCloudVectorMapsOverlay, ThinkGeoCloudRasterMapsOverlay, ThinkGeoVectorOverlay), ClientId and ClientSecret, an ApiKey, client keys, geocoding, reverse geocoding, routing, service areas, elevation, time zones, Maps Query, the Cloud console, quota or "hits per day", or errors such as a blank cloud basemap, 401 or unauthorized responses, or results landing in the wrong place on the map.
---

# ThinkGeo Cloud (Cloud Maps)

ThinkGeo Cloud is a set of hosted REST services: vector and raster map tiles, WMS, geocoding, reverse geocoding, routing, elevation, Maps Query, projection, colors, and time zones.   ThinkGeo apps use it through ready-made map overlays and .NET client classes, both authenticated with a client key from the ThinkGeo Cloud console (https://cloud.thinkgeo.com).

Cloud needs an internet connection.   For apps that must work without one, use the `thinkgeo-offline-maps` skill instead.   For API details, use the ThinkGeo Documentation MCP server: `tg_api` for class and member names, and `tg_find_sample` with `platform: "wpf"` for the "ThinkGeo Cloud Integration" samples (geocoding, reverse geocoding, routing, elevation, and more).

## Packages: nothing extra to install (v14)

In ThinkGeo 14, the Cloud client classes (`GeocodingCloudClient`, `RoutingCloudClient`, and the rest) ship inside `ThinkGeo.Core`, and the Cloud basemap overlays ship in each UI package.   **Don't add `ThinkGeo.Cloud.Client`.** The Cloud docs still point .NET developers to it, but it's the old Map Suite SDK (last release 10.6, plus 13.0 betas) and doesn't belong in a v14 project.

## Keys: which kind each platform needs

The Cloud console issues two kinds of client key.   New accounts get one of each.

| Key type | What it is | Used by |
| --- | --- | --- |
| NativeConfidential | `ClientId` + `ClientSecret`, exchanged for a token behind the scenes | WPF and WinForms overlays, MAUI overlays, every `*CloudClient`, and server-side code (including a Blazor server's own service calls) |
| JavaScript | One `ApiKey` string, sent with each request from the browser | The Blazor overlays (`ThinkGeoCloudVectorMapsOverlay` and `ThinkGeoCloudRasterMapsOverlay` in `ThinkGeo.UI.Blazor`).   Their only key property is a single `ApiKey`, so a NativeConfidential ClientId and ClientSecret pair can't be used there |

Rules for keys:

- **Never put real keys in source code or commit them.** Use placeholders (`YOUR_CLIENT_ID`, `YOUR_CLIENT_SECRET`, `YOUR_API_KEY`) in samples, and load real values from configuration (an app settings file, environment variables, or the platform's secret store).
- **Desktop and mobile apps embed their NativeConfidential key** in the shipped configuration.   Anyone with the app can extract it, so restrict the client by IP address or range in the Cloud console (NativeConfidential clients support IP restriction only).   IP restriction works when the app's users reach the internet from known networks, such as a company's offices; tell the user if their users don't.
- **Restrict JavaScript keys by domain** (for example `*.example.com`) in the Cloud console.
- **Use one client per application** so a leaked key can be revoked without breaking other apps.   Clients can also drop the "User" role if they only call services.
- Don't use the two pre-generated test keys from an account in shipped apps.

## Basemap overlays

| Platform | Vector basemap | Raster basemap | Key properties |
| --- | --- | --- | --- |
| WPF, WinForms | `ThinkGeoCloudVectorMapsOverlay` | `ThinkGeoCloudRasterMapsOverlay` | `ClientId`, `ClientSecret` |
| MAUI | `ThinkGeoVectorOverlay` | `ThinkGeoRasterOverlay` | `ClientId`, `ClientSecret` |
| Blazor | `ThinkGeoCloudVectorMapsOverlay` (component) | `ThinkGeoCloudRasterMapsOverlay` (component) | `ApiKey` |

Note the different MAUI class names.   All Cloud basemaps are Spherical Mercator, so set the map unit to `GeographyUnit.Meter`.

```csharp
// WPF / WinForms
MapView.MapUnit = GeographyUnit.Meter;
MapView.Overlays.Add("Basemap", new ThinkGeoCloudVectorMapsOverlay
{
    ClientId = settings.ThinkGeoClientId,          // from configuration, never hard-coded
    ClientSecret = settings.ThinkGeoClientSecret,
    MapType = ThinkGeoCloudVectorMapsMapType.Light,
    TileCache = new FileRasterTileCache(cacheFolder, "thinkgeo_vector_light")
});
```

```razor
@* Blazor (the page needs @using ThinkGeo.Core for the MapType enum) *@
<ThinkGeoCloudVectorMapsOverlay Id="VectorOverlay" ApiKey="@thinkGeoApiKey" MapType="ThinkGeoCloudVectorMapsMapType.Light" />
```

- **Vector map types:** `Light`, `Dark`, `TransparentBackground` (roads and labels to lay over imagery), and `CustomizedByStyleJson` (with `StyleJsonUri`).   Avoid `Default`, a legacy alias for `Light`.
- **Aerial and hybrid imagery are raster only.** The raster map types come in versioned names such as `Aerial2_V2_X2` and `Light_V2_X1`: `X1` tiles suit 100% display scaling, `X2` suit high-DPI screens and mobile.   The plain names (`Light`, `Aerial`, and so on) are legacy.   Check current names with `tg_get` on `ThinkGeoCloudRasterMapsMapType`, since `tg_api` doesn't list enum values.
- **Set a `TileCache` on desktop and mobile overlays.** It cuts requests (and quota use) and speeds up panning.   Give each map type its own cache ID.

## Service clients

Every client (`GeocodingCloudClient`, `ReverseGeocodingCloudClient`, `RoutingCloudClient`, `ElevationCloudClient`, `MapsQueryCloudClient`, `ProjectionCloudClient`, `ColorCloudClient`, `TimeZoneCloudClient`, and `MapsCloudClient` for single tiles) is in `ThinkGeo.Core`, takes a NativeConfidential `ClientId` and `ClientSecret`, has an `Async` version of each method, and is `IDisposable`.   Create one per service and reuse it; dispose it with the window or service that owns it.   Method names per service are in `references/services.md`.

### Coordinate systems: the most common mistake

Services work in WGS84 longitude/latitude (EPSG:4326) unless told otherwise.   ThinkGeo maps are usually Spherical Mercator (EPSG:3857).   A result used without conversion lands near 0,0 or far off the map.   Always say which system you mean:

- **Results:** set the projection on the options, for example `new CloudGeocodingOptions { ResultProjectionInSrid = 3857 }`.
- **Inputs:** use the overload that takes an SRID, for example `GetRouteAsync(waypoints, 3857, options)` or `SearchPointAsync(x, y, 3857, radius, DistanceUnit.Meter)`.
- **Lon/lat input:** the `...InDecimalDegree` methods (for example `GetElevationOfPointInDecimalDegreeAsync`) take longitude and latitude directly.

```csharp
using var geocoder = new GeocodingCloudClient(settings.ThinkGeoClientId, settings.ThinkGeoClientSecret);
var result = await geocoder.SearchAsync("6101 Frisco Square Blvd, Frisco, TX",
    new CloudGeocodingOptions { MaxResults = 5, ResultProjectionInSrid = 3857 });

if (result.Exception != null)                  // service errors come back here, not as thrown exceptions
{
    ShowError(result.Exception.Message);
    return;
}
foreach (var location in result.Locations)
    resultsLayer.InternalFeatures.Add(new Feature(location.LocationPoint));   // already in 3857
await resultsOverlay.RefreshAsync();
```

### Errors

- **Check `result.Exception` on every result.** Cloud results (`CloudGeocodingResult`, `CloudRoutingGetRouteResult`, `CloudReverseGeocodingResult`, and the others) report service failures in an `Exception` property instead of throwing.   Code that only uses `try`/`catch` silently shows nothing.   Still wrap calls in `try`/`catch` for network failures.
- **Blank cloud basemap:** check the key type for the platform (Blazor needs a JavaScript `ApiKey`, everything else a ClientId and ClientSecret), the client's IP or domain restrictions, and whether the evaluation or quota has run out.   To see the HTTP traffic, handle the overlay's `SendingHttpRequest` and `ReceivedHttpResponse` events, or set `ThinkGeoDebugger.LogType = ThinkGeoLogType.WebRequest`.
- **Quota:** free evaluation accounts are limited to 10,000 requests per day.   Usage is shown in the Cloud console.   Tile caches and reusing results reduce it.

## Caching Cloud tiles for offline use

Customers may pre-generate ThinkGeo Cloud tiles into a cache and ship it for offline use (confirmed by ThinkGeo).   Use the raster Cloud layer, which can fill its cache ahead of time:

```csharp
// Build machine, online: fill the cache for the area and zoom range the app needs.
var cloud = new ThinkGeoRasterMapsAsyncLayer(clientId, clientSecret)
{
    MapType = ThinkGeoCloudRasterMapsMapType.Light_V2_X1,
    TileCache = new FileRasterTileCache(cacheRoot, "cloud_light_v2_x1")
};
await cloud.OpenAsync();
await cloud.GenerateTileCacheAsync(areaInMeters, 0, 14);   // extent in 3857, start zoom, end zoom
await cloud.CloseAsync();

// Deployed app: same layer settings and cache ID, reading only from the cache.
cloud.IsCacheOnly = true;
```

Every generated tile is a Cloud request, and tile counts grow about four times per zoom level, so estimate the count against the account's quota first.   Ship the cache folder with the app; the `thinkgeo-offline-maps` skill covers packaging and cache gotchas.

## Boundaries

- Cloud is a separate subscription from the ThinkGeo developer license; Product Center doesn't manage Cloud keys.
- For the same street map without Cloud (offline, or to avoid per-request use), ThinkGeo sells the data as ThinkGeo Maps Streets, a vector tile file; see the `thinkgeo-offline-maps` skill.
- Verify any service method or option not listed here with `tg_api` before using it.

## Reference files

- `references/services.md`: each service's client, main methods, options class, and result type.
