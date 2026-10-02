---
description: "Test case 2: shapefile in EPSG:4326 on an EPSG:3857 map is invisible. Missing ProjectionConverter, and the extent is read in degrees."
tags: [full, troubleshoot, wpf]
max_turns: 25
timeout_seconds: 600
allowed_tools: [Read, Glob, Grep, Skill]
---

My WPF shapefile is EPSG:4326, my map is EPSG:3857, and the layer is not visible.   ThinkGeo 14.5.3.   Here is my layer code:

```csharp
private async void MapView_Loaded(object sender, RoutedEventArgs e)
{
    MapView.MapUnit = GeographyUnit.Meter;
    MapView.Overlays.Add(new ThinkGeoCloudVectorMapsOverlay
    {
        ClientId = "YOUR_CLIENT_ID",
        ClientSecret = "YOUR_CLIENT_SECRET",
        MapType = ThinkGeoCloudVectorMapsMapType.Light
    });

    var parcels = new ShapeFileFeatureLayer(@"Data\Parcels.shp");
    parcels.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
        AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.Red, 2);
    parcels.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

    var overlay = new LayerOverlay();
    overlay.Layers.Add(parcels);
    MapView.Overlays.Add(overlay);

    parcels.Open();
    MapView.CurrentExtent = parcels.GetBoundingBox();
    parcels.Close();

    await MapView.RefreshAsync();
}
```

What's wrong, and what's the fix?
