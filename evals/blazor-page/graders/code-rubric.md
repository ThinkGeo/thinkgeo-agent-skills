---
type: llm
focus:
  source: file
  path: "BlazorMap/Components/Pages/Parcels.razor"
---

This is a Razor page for a .NET 8 Blazor Web App with a ThinkGeo map: a State Plane (EPSG:2276) parcel shapefile over a ThinkGeo Cloud basemap; clicking a parcel highlights it and shows OWNER_NAME.

PASS only if all of these hold:
- The page uses interactive server rendering (`@rendermode InteractiveServer` or equivalent) so the click event works.
- A `MapView` with `MapUnit` meters contains a Cloud basemap overlay using `ApiKey` (a placeholder or configuration value, never a ClientId/ClientSecret pair) and a `LayerOverlay` whose `Layers` holds a `ShapeFileFeatureLayer` with `ProjectionConverter(2276, 3857)`.
- The data path comes from the content root (for example `IWebHostEnvironment.ContentRootPath`), not a bare relative path or the current directory.
- `OnClick` takes `ClickedMapViewEventArgs`, builds a point from `WorldX`/`WorldY` without converting it manually, queries the layer for features containing it with the OWNER_NAME column, and opens and closes the layer around the query.
- The highlight goes into a separate layer or overlay, and the code calls `RedrawAsync()` on that overlay (through `@ref`) after changing it.

FAIL if any item is missing or wrong, or if the code uses a ThinkGeo API name that looks invented.
