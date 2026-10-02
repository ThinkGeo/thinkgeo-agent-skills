---
description: "Test case 5: review 14.5 WPF code with four planted errors; keep release packages."
tags: [full, review, wpf]
max_turns: 30
timeout_seconds: 600
allowed_tools: [Read, Glob, Grep, Skill]
---

Review this ThinkGeo v14.5 WPF code and modernize it, but do not move me to beta packages.

```xml
<PackageReference Include="ThinkGeo.UI.Wpf" Version="14.5.3" />
```

```csharp
using System.IO;
using System.Windows;
using ThinkGeo.Core;
using ThinkGeo.UI.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "Parcels.shp");
        ShapeFileFeatureLayer.BuildIndex(path);

        var parcels = new ShapeFileFeatureLayer(path, ShapeFileReadWriteMode.ReadWrite);
        parcels.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColors.LightGreen, GeoColors.DarkGreen);
        parcels.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var overlay = new LayerOverlay { TileType = TileType.MultipleTiles };
        overlay.Layers.Add(parcels);
        mapView.MapUnit = GeographyUnit.Meter;
        mapView.Overlays.Add(overlay);

        parcels.Open();
        mapView.CurrentExtent = parcels.GetBoundingBox();
        parcels.Close();

        mapView.Refresh();
    }
}
```
