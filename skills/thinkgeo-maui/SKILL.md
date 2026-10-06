---
name: thinkgeo-maui
description: Build .NET MAUI map apps with ThinkGeo (ThinkGeo.UI.Maui) for Android, iOS, Mac Catalyst, and Windows. Use this skill whenever the user mentions ThinkGeo MAUI, ThinkGeo.UI.Maui, a MAUI MapView, ThinkGeoVectorOverlay or ThinkGeoRasterOverlay, SingleTap, ToWorldCoordinate, MapScale, map rotation or tilt, GPS or current location on a ThinkGeo map, shipping shapefiles or MBTiles inside a mobile app, FileSystem.AppDataDirectory, or symptoms such as a map that works on Windows but not on Android or iOS, a license exception in the emulator, data files that can't be opened on a phone, or taps that don't select features.
---

# ThinkGeo for .NET MAUI

`ThinkGeo.UI.Maui` provides a `MapView` control for Android, iOS, Mac Catalyst, and Windows.   Layers, styles, projections, and queries use the same `ThinkGeo.Core` API as the desktop products, so most desktop knowledge carries over; the differences below are what trips people up.

Search the official samples with `tg_search` in the `mauiHowDoI` namespace, and check names with `tg_api` (MAUI types are in the `ThinkGeo.UI.Maui` namespace).

## Setup

- Package: `ThinkGeo.UI.Maui`, plus any extension packages the data needs, all at the same version.   Nothing ThinkGeo-specific goes in `MauiProgram.cs`.
- XAML: `xmlns:thinkgeo="clr-namespace:ThinkGeo.UI.Maui;assembly=ThinkGeo.UI.Maui"` and `<thinkgeo:MapView x:Name="mapView" SizeChanged="MapView_SizeChanged" />`.
- Code-behind: `using ThinkGeo.Core;` and `using ThinkGeo.UI.Maui;`.
- **Licensing is different on phones.** Android, iOS, and Mac Catalyst need a license file loaded with `LicenseLoader.LoadLicense` before any map draws, even in an emulator under the debugger; a developer license alone isn't enough there.   Follow `thinkgeo-code-example/references/licensing.md` (MAUI section) for every new MAUI project.

## Different from desktop

| Task | WPF / WinForms | MAUI |
| --- | --- | --- |
| Cloud basemap | `ThinkGeoCloudVectorMapsOverlay` | `ThinkGeoVectorOverlay` (raster: `ThinkGeoRasterOverlay`) |
| Zoom level of the view | `CurrentScale` | `MapScale` |
| Click or tap | `MapClick` with `e.WorldLocation` | `SingleTap` (also `DoubleTap`, `LongPress`) with screen `e.X`, `e.Y`; convert with `mapView.ToWorldCoordinate(e.X, e.Y)` |
| Data files | Next to the executable | Copied into `FileSystem.Current.AppDataDirectory` on first run (below) |
| Tile cache folder | Any writable folder | `FileSystem.Current.CacheDirectory` |
| Extras | | Map rotation (`IsRotationEnabled`, `MapRotation`) and tilt (`TiltAsync`) |

## A map page

```csharp
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

public partial class MainPage : ContentPage
{
    private bool _initialized;
    private ShapeFileFeatureLayer _parcels = null!;
    private InMemoryFeatureLayer _highlight = null!;
    private LayerOverlay _highlightOverlay = null!;

    public MainPage() => InitializeComponent();

    private async void MapView_SizeChanged(object? sender, EventArgs e)
    {
        if (_initialized) return;            // SizeChanged fires more than once
        _initialized = true;

        mapView.MapUnit = GeographyUnit.Meter;
        mapView.Overlays.Add(new ThinkGeoVectorOverlay
        {
            ClientId = AppSettings.ThinkGeoClientId,           // NativeConfidential key, from configuration
            ClientSecret = AppSettings.ThinkGeoClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "cloud_light")
        });

        _parcels = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Parcels.shp"));
        _parcels.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
        _parcels.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.DimGray, 1);
        _parcels.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        var parcelOverlay = new LayerOverlay();
        parcelOverlay.Layers.Add(_parcels);
        mapView.Overlays.Add(parcelOverlay);

        _highlight = new InMemoryFeatureLayer();
        _highlight.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.Orange), GeoColors.Orange, 2);
        _highlight.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        _highlightOverlay = new LayerOverlay();
        _highlightOverlay.Layers.Add(_highlight);
        mapView.Overlays.Add(_highlightOverlay);

        mapView.SingleTap += MapView_SingleTap;
        mapView.MapTools.Add(new ZoomMapTool());

        _parcels.Open();
        mapView.CenterPoint = _parcels.GetBoundingBox().GetCenterPoint();
        _parcels.Close();
        mapView.MapScale = 20000;
        await mapView.RefreshAsync();
    }

    private async void MapView_SingleTap(object? sender, SingleTapMapViewEventArgs e)
    {
        PointShape tapped = mapView.ToWorldCoordinate(e.X, e.Y);   // screen to map coordinates
        _parcels.Open();
        Collection<Feature> hits;
        try { hits = _parcels.QueryTools.GetFeaturesContaining(tapped, new[] { "OWNER_NAME" }); }
        finally { _parcels.Close(); }

        _highlight.InternalFeatures.Clear();
        foreach (var f in hits) _highlight.InternalFeatures.Add(f);
        await _highlightOverlay.RefreshAsync();   // refresh only the overlay that changed
    }
}
```

## Rules

1. **Initialize once, after layout.** Set up the map in `SizeChanged` with an `_initialized` guard (it fires repeatedly, including on rotation), or in `Loaded`.   `MapWidth` and `MapHeight` are zero before layout.
2. **Ship data as files in `AppDataDirectory`.** ThinkGeo file layers need a real, seekable file path.   On Android, files packaged as `MauiAsset` (`Resources\Raw`) are only available as non-seekable streams.   The official pattern: add data files as **embedded resources**, and on first start (for example in `App.OnStart`) copy them to `FileSystem.Current.AppDataDirectory` if they aren't there yet, then open them from there.   License files are the exception: they go in `Resources\Raw` and load with `LicenseLoader`.
3. **Taps give screen coordinates.** Convert `e.X`, `e.Y` with `ToWorldCoordinate` before querying.   A layer with a `ProjectionConverter` takes the converted point directly.   Finger taps are imprecise: for points and lines, query with a tolerance (`GetFeaturesWithinDistanceOf`) rather than `GetFeaturesContaining`.
4. **Projection:** Cloud basemaps are Spherical Mercator (`GeographyUnit.Meter`); give layers in other projections a `ProjectionConverter(dataEpsg, 3857)`.   GPS readings are WGS84 longitude/latitude: convert with `ProjectionConverter.Convert(4326, 3857, longitude, latitude)` before centering or drawing.
5. **Location:** use MAUI's `Geolocation.GetLocationAsync` (see the "Navigation" sample).   Each platform needs its location permission declared (Android manifest, iOS `Info.plist`) and requested at runtime; a missing declaration shows up as a permission exception or no location, not a ThinkGeo error.
6. **Refresh the overlay that changed** (`overlay.RefreshAsync()`), and keep changing data (highlights, GPS position, results) in its own overlay.   Update the map from the UI thread (`MainThread.BeginInvokeOnMainThread`) when results come from background work.
7. **Cloud keys:** `ThinkGeoVectorOverlay` and `ThinkGeoRasterOverlay` take a ClientId and ClientSecret; restrict the client by IP where possible and keep keys out of source code (see the `thinkgeo-cloud-maps` skill).
8. **Offline:** for maps without a connection, use a local basemap (MBTiles on 14.x, PMTiles on 15 and later) or a pre-generated tile cache; see the `thinkgeo-offline-maps` skill.   Plan app size: phones have limited storage, so prefer regional extracts.

## Troubleshooting quick list

| Symptom | Check first |
| --- | --- |
| License exception in the Android or iOS emulator | License file for the package name or bundle ID, loaded with `LicenseLoader` (`licensing.md`) |
| Works on Windows, data missing on Android or iOS | Data opened from `Resources\Raw` or a relative path instead of `AppDataDirectory` (rule 2) |
| Taps select nothing | Screen coordinates not converted (rule 3); tap tolerance for points and lines |
| Map blank or the wrong size | Initialized before layout (rule 1); `MapUnit` and converters (rule 4) |
| GPS position in the wrong place | Longitude/latitude not converted to 3857 (rule 4) |
| Blank Cloud basemap | ClientId/ClientSecret, network access, or quota (`thinkgeo-cloud-maps`) |
