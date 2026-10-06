---
type: llm
---

Judge code that adds ThinkGeo Cloud address search to a WPF ThinkGeo 14.5.3 map in Spherical Mercator (EPSG:3857).

PASS only if all of these hold:
- It uses `GeocodingCloudClient` (from `ThinkGeo.Core`, no extra package) with a ClientId and ClientSecret loaded from configuration or shown as placeholders, never real-looking keys.
- Results come back in the map's coordinate system: `ResultProjectionInSrid = 3857` on `CloudGeocodingOptions`, or an explicit conversion from 4326.
- It checks `result.Exception` for service errors (and handles an empty result) instead of relying only on try/catch.
- It marks the result on the map (for example a feature in an InMemoryFeatureLayer or a marker) and zooms to it, refreshing with `RefreshAsync` or `ZoomToAsync`, never a synchronous `Refresh()`.
- It reuses or disposes the client rather than leaking one per search.

FAIL if any item is missing or wrong, or if it says to install `ThinkGeo.Cloud.Client`.
