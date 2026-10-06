---
name: thinkgeo-blazor
description: Build Blazor map pages with ThinkGeo (ThinkGeo.UI.Blazor). Use this skill whenever the user mentions ThinkGeo Blazor, the Blazor MapView component, OverlaysSetting, LayerOverlay in a .razor page, RedrawAsync, OnClick with ClickedMapViewEventArgs, markers, popups, EditOverlay or BlazorTrackMode, VectorTileOverlay or WmtsTileOverlay, Blazor Server versus WebAssembly or Hybrid, render modes, or symptoms such as a map that never appears, layers that don't update after a change, clicks that do nothing, or a map that works in Blazor Server but not in WebAssembly.
---

# ThinkGeo for Blazor

`ThinkGeo.UI.Blazor` provides a `MapView` component for Razor pages.   Data layers use the same `ThinkGeo.Core` API as the desktop products (layers, styles, projections, queries).   The browser shows the map with an OpenLayers-based client; ThinkGeo's own layers are drawn into tiles **on the server**.

Search the official samples with `tg_search` in the `blazorHowDoI` namespace, and check names with `tg_api`.   Some newer components (for example `VectorTileOverlay`) aren't in the API reference yet; confirm them with a sample or the changelog.

## Hosting model: Blazor Server for ThinkGeo layers

| Hosting | ThinkGeo layers in `LayerOverlay` (shapefiles, databases, styles, `MvtTilesAsyncLayer`, ...) | Client-drawn overlays (`VectorTileOverlay`, `WmtsTileOverlay`) |
| --- | --- | --- |
| Blazor Server, or a .NET 8+ Blazor Web App with interactive server rendering | Supported | Supported |
| Blazor WebAssembly, Blazor Hybrid | **Not supported** (drawing would have to run inside the browser) | Supported |

- **Recommend Blazor Server** (interactive server rendering) for any map that shows the user's own data.   WebAssembly and Hybrid only suit maps built entirely from the client-drawn overlays.
- **Drawing happens on the server, per user.** Data files must be on the server, and server CPU and memory grow with the number of users viewing maps.   Read data from the app's content root (`IWebHostEnvironment.ContentRootPath`), not the current directory.
- **Interactivity:** in a .NET 8+ Blazor Web App, the page (or the app) needs an interactive server render mode, for example `@rendermode InteractiveServer`; without it the map renders statically and events don't fire.   The official samples use the older Blazor Server structure (`Startup.cs`, `_Host.cshtml`), which is interactive by default.
- **Client assets load automatically** (since 14.5).   Older guides add ThinkGeo's CSS and JavaScript from a CDN in `_Host.cshtml`; that still works but isn't needed.

## Setup

- Package: `ThinkGeo.UI.Blazor`, plus any extension packages the data needs, all at the same version (see `thinkgeo-code-example/references/package-map.md`).
- `_Imports.razor`: `@using ThinkGeo.Core` and `@using ThinkGeo.UI.Blazor`.
- Licensing: same Product Center tab as desktop; a server needs a runtime license (see `thinkgeo-code-example/references/licensing.md`).
- ThinkGeo Cloud basemaps take a JavaScript-type `ApiKey`, restricted to your domain (see the `thinkgeo-cloud-maps` skill).

## A map page

```razor
@page "/parcels"
@rendermode InteractiveServer
@inject IWebHostEnvironment Env

<MapView Id="map" MapUnit="GeographyUnit.Meter" Zoom="12"
         Center="@(new PointShape(-10776838, 3912346))"
         Width="100" Height="100" MapViewSizeUnitType="MapViewSizeUnitType.Percentage"
         OnClick="OnMapClick">
    <OverlaysSetting>
        <ThinkGeoCloudVectorMapsOverlay Id="basemap" ApiKey="@cloudApiKey" MapType="ThinkGeoCloudVectorMapsMapType.Light" />
        <LayerOverlay Id="parcels" Layers="@parcelLayers" />
        <LayerOverlay Id="highlight" @ref="highlightOverlay" Layers="@highlightLayers" />
    </OverlaysSetting>
</MapView>

<p>@ownerName</p>

@code {
    private readonly string cloudApiKey = "YOUR_API_KEY";   // load from configuration in a real app
    private GeoCollection<LayerBase> parcelLayers = new();
    private GeoCollection<LayerBase> highlightLayers = new();
    private LayerOverlay highlightOverlay = null!;     // set by @ref after the first render
    private ShapeFileFeatureLayer parcels = null!;
    private InMemoryFeatureLayer highlight = null!;
    private string? ownerName;

    protected override void OnInitialized()
    {
        parcels = new ShapeFileFeatureLayer(Path.Combine(Env.ContentRootPath, "App_Data", "Parcels.shp"));
        parcels.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
        parcels.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.DimGray, 1);
        parcels.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        parcelLayers.Add(parcels);

        highlight = new InMemoryFeatureLayer();
        highlight.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.Orange), GeoColors.Orange, 2);
        highlight.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        highlightLayers.Add(highlight);
    }

    private async Task OnMapClick(ClickedMapViewEventArgs e)
    {
        var clicked = new PointShape(e.WorldX, e.WorldY);   // map coordinates (3857 here)
        parcels.Open();
        Collection<Feature> hits;
        try { hits = parcels.QueryTools.GetFeaturesContaining(clicked, new[] { "OWNER_NAME" }); }
        finally { parcels.Close(); }

        highlight.InternalFeatures.Clear();
        foreach (var f in hits) highlight.InternalFeatures.Add(f);
        ownerName = hits.FirstOrDefault()?.ColumnValues["OWNER_NAME"];

        await highlightOverlay.RedrawAsync();   // the map doesn't redraw on its own after data changes
    }
}
```

## Rules

1. **Declare overlays in markup, data in code.** `MapView` holds settings blocks: `OverlaysSetting` (basemaps and `LayerOverlay`s), `MarkerOverlaySetting`, `PopupOverlaySetting`, `EditOverlaySetting`, `MapToolsSetting`, and `AdornmentOverlaySetting`.   A `LayerOverlay`'s layers come from a `GeoCollection<LayerBase>` field passed to `Layers`.
2. **Redraw after changing data or styles.** Changing a layer's features or styles in C# doesn't update the browser.   Keep a reference to the overlay with `@ref` and call `await overlay.RedrawAsync()`; redraw only the overlay that changed.   Put changing data (highlights, results) in its own `LayerOverlay` so the static data isn't redrawn too.
3. **Click coordinates are in map units.** `ClickedMapViewEventArgs.WorldX` and `WorldY` are in the map's `MapUnit` (meters for Spherical Mercator).   A layer with a `ProjectionConverter` takes those coordinates directly in its queries; don't convert them yourself.
4. **Projection:** cloud basemaps and most web tiles are Spherical Mercator, so use `GeographyUnit.Meter` and give every layer in another projection a `ProjectionConverter(dataEpsg, 3857)`.
5. **Set the map's size.** `Width` and `Height` with `MapViewSizeUnitType` (`Pixel`, `Percentage`, or `Em`), or size its container with CSS.   A map whose container has no height doesn't appear.
6. **Markers and popups:** `SimpleMarkerOverlay` binds `MarkerSource` to your list and renders each item through a `MarkersSetting` template; `PopupOverlay` shows `Popup` components positioned in map coordinates.   See the "Mark the Places" sample.
7. **Drawing and editing:** `EditOverlay` sets `TrackMode` (`BlazorTrackMode.Point`, `LineString`, `Polygon`, `Circle`, `Rectangle`, `Modify`, `None`) and reports results through `OnFeatureDrawn`, `OnFeatureModified`, and `OnFeatureClick`.   Check the event argument types with `tg_api` and the "Draw and Modify Geometries" sample before writing handlers.
8. **Vector tiles:** `VectorTileOverlay` (client-drawn, takes a MapLibre Style JSON URL or text in `StyleJson`) is fast and works in WebAssembly, but has no server tile cache.   `MvtTilesAsyncLayer` in a `LayerOverlay` is drawn on the server and supports caching.   Don't mix `VectorTileOverlay` and `WmtsTileOverlay` in one map: they use different renderers and misalign at world zoom.
9. **Navigation from code:** use the `MapView` methods `SetCenterAsync`, `ZoomToCenterAsync`, `PanToAsync`, `ZoomInAsync`, `ZoomOutAsync`, and `GetCurrentExtentAsync` through an `@ref` to the map.

## Troubleshooting quick list

| Symptom | Check first |
| --- | --- |
| Map never appears | Container height (rule 5); interactive render mode; browser console errors |
| Layer data doesn't show | Data path from the content root; `ProjectionConverter` (rule 4); zoom-level styles with `ApplyUntilZoomLevel.Level20` |
| Changes don't appear | `RedrawAsync` on the overlay (rule 2) |
| Clicks do nothing | Interactive render mode; `OnClick` handler signature takes `ClickedMapViewEventArgs` |
| Works on Blazor Server, blank in WebAssembly | `LayerOverlay` isn't supported in WebAssembly (hosting table) |
| Watermark | Runtime license on the server (`licensing.md`) |
