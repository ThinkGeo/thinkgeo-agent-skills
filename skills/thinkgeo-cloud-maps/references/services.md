# ThinkGeo Cloud Services and Their .NET Clients

Sources: ThinkGeo Cloud Quickstart and service pages, Client Keys page (docs.thinkgeo.com), the ThinkGeo.Core API reference, and the WPF HowDoI "ThinkGeo Cloud Integration" samples.   Names checked against the ThinkGeo 14.5 API reference and packages.

All clients live in `ThinkGeo.Core`, take a NativeConfidential key (`new XxxCloudClient(clientId, clientSecret)` or the `ClientId` and `ClientSecret` properties), and expose `TimeoutInSeconds` and `WebProxy`.   Each method below also has an `Async` version (`SearchAsync`, `GetRouteAsync`, and so on); use those from UI code.

| Service | Client | Main methods | Options class | Result type (check its `Exception`) |
| --- | --- | --- | --- | --- |
| Geocoding (US addresses) | `GeocodingCloudClient` | `Search` (one string or many) | `CloudGeocodingOptions` (`MaxResults`, `SearchMode`, `LocationType`, `BBox`, `ResultProjectionInSrid`) | `CloudGeocodingResult` (`Locations`, each with `LocationPoint` and `BoundingBox`) |
| Reverse geocoding | `ReverseGeocodingCloudClient` | `SearchPoint`, `SearchPoints`, `SearchLine`, `SearchArea`, each with an `...InDecimalDegree` variant | `CloudReverseGeocodingOptions` | `CloudReverseGeocodingResult` (`BestMatchLocation`, `NearbyLocations`) |
| Routing | `RoutingCloudClient` | `GetRoute`, `GetServiceArea`, `GetOptimizedRoute`, `GetTimeCostMatrix`, `GetDistanceCostMatrix` | `CloudRoutingGetRouteOptions` (`TurnByTurn`, `DistanceUnit`, `RouteType`, snap radius), `CloudRoutingGetServiceAreaOptions`, `CloudRoutingOptimizationOptions`, `CloudRoutingGetCostMatrixOptions` | `CloudRoutingGetRouteResult` (`RouteResult.Routes`, each with `Shape`, `Distance`, `Duration`, `Segments`) and matching result types for the other methods |
| Elevation | `ElevationCloudClient` | `GetElevationOfPoint`, `GetElevationOfPoints`, `GetElevationOfLine`, `GetElevationOfArea`, `GetGradeOfLine`, each with an `...InDecimalDegree` variant | (parameters on the method) | `CloudElevationResult`, `CloudGradeResult` |
| Maps Query (spatial queries on ThinkGeo's world data) | `MapsQueryCloudClient` | `GetFeaturesWithin`, `GetFeaturesContaining`, `GetFeaturesIntersecting`, `GetFeaturesNearest`, `GetFeaturesWithinDistance`, `GetLayers`, `GetAttributesOfLayer` | `CloudMapsQuerySpatialQueryOptions`, `CloudMapsQueryNearestQueryOptions`, `CloudMapsQueryCustomQueryOptions` | `CloudMapsQueryResult` |
| Projection | `ProjectionCloudClient` | `Project` | (parameters on the method) | Projected shapes or features |
| Time zones | `TimeZoneCloudClient` | `GetTimeZoneByCoordinate`, `GetAllTimeZoneNames`, `GetAllTimeZones` | (parameters on the method) | `CloudTimeZoneResult` |
| Colors | `ColorCloudClient` | `GetColorsInHueFamily`, `GetColorsInAnalogousFamily`, `GetColorsInComplementaryFamily`, and other families | (parameters on the method) | Color collections |
| Map tiles (single tiles) | `MapsCloudClient` | `GetRasterTile`, `GetVectorTile` | (parameters on the method) | Tile data |

For a basemap, use the map overlays in `SKILL.md` rather than `MapsCloudClient`; the overlays handle tiling, caching, and drawing.

## Coordinate system parameters

Without an SRID, services assume WGS84 longitude/latitude (EPSG:4326).   On a Spherical Mercator map:

- Routing: `GetRouteAsync(waypoints, 3857, options)`; the route `Shape` comes back in the same system.
- Reverse geocoding: `SearchPointAsync(x, y, 3857, searchRadius, DistanceUnit.Meter)`.
- Geocoding: `ResultProjectionInSrid = 3857` on `CloudGeocodingOptions`.
- Elevation: pass the SRID overload, or use `...InDecimalDegree` with longitude and latitude.

Overloads that take a Proj4 string instead of an SRID exist for systems without an EPSG code.

## Routing example

```csharp
using var router = new RoutingCloudClient(settings.ThinkGeoClientId, settings.ThinkGeoClientSecret);
var waypoints = new[] { startPoint, endPoint };                 // PointShapes in map coordinates (3857)
var result = await router.GetRouteAsync(waypoints, 3857, new CloudRoutingGetRouteOptions { TurnByTurn = true });

if (result.Exception != null) { ShowError(result.Exception.Message); return; }

var route = result.RouteResult.Routes[0];
routeLayer.InternalFeatures.Clear();
routeLayer.InternalFeatures.Add(new Feature(route.Shape));      // Shape is in 3857, matching the request
await routeOverlay.RefreshAsync();
```

Routing coverage, geocoding coverage (US addresses), and request limits are described on each service's page under docs.thinkgeo.com/products/cloud-maps/services/.   Search them with `tg_search` before promising coverage outside North America.
