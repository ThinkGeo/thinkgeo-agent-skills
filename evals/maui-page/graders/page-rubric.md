---
type: llm
focus:
  source: file
  path: "MauiMap/MainPage.xaml.cs"
---

This is the code-behind of a .NET MAUI ThinkGeo page: a State Plane (EPSG:2276) parcel shapefile shipped in the app, over a ThinkGeo Cloud basemap, with tap-to-highlight, for Android and iOS.

PASS only if all of these hold:
- The basemap is `ThinkGeoVectorOverlay` or `ThinkGeoRasterOverlay` (the MAUI classes) with ClientId/ClientSecret placeholders or configuration values, and `MapUnit` is meters with `ProjectionConverter(2276, 3857)` on the parcel layer.
- The parcel layer opens its file from a full path under `FileSystem.Current.AppDataDirectory`, not from `Resources\Raw` or a bare relative path.
- The map is initialized once after layout (`SizeChanged` with a guard, or `Loaded`).
- The tap handler uses `SingleTap`, converts the screen position with `ToWorldCoordinate` before querying, and refreshes the highlight overlay.

FAIL if any item is missing or wrong, or if the code uses desktop-only names such as `MapClick`, `e.WorldLocation`, or `ThinkGeoCloudVectorMapsOverlay`.
