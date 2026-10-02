---
name: thinkgeo-desktop-interaction
description: Add user interaction to ThinkGeo WPF and WinForms maps — click to identify or select features, highlight results, spatial queries (within distance, containing, intersecting), drawing shapes with TrackOverlay, editing geometry with EditOverlay, markers, and popups. Use this skill whenever the user wants a ThinkGeo desktop map to respond to clicks, select or highlight features, show feature attributes, let users draw or edit points/lines/polygons, measure, or add pins and popups, even if they just say "make the map clickable" or "let users draw a boundary".
---

# ThinkGeo Desktop Interaction

This skill builds on `thinkgeo-desktop-maps`.   Follow that skill's setup rules (map unit first, projection converters, `ApplyUntilZoomLevel`, per-overlay refresh).   If the ThinkGeo Documentation MCP server is connected, use `tg_find_sample` with `platform: "wpf"` or `"winforms"` to pull the matching HowDoI sample before writing code.

## The shape of every interactive feature

Almost every interaction follows the same loop:

1. **Input**: a `MapClick` (point in map coordinates), a shape drawn with `TrackOverlay`, or a UI control.
2. **Query or compute**: `layer.QueryTools.*` or a geometry operation on the shape.
3. **Show results**: put features into an `InMemoryFeatureLayer` that lives in its own `LayerOverlay` with `TileType.SingleTile`.
4. **Refresh only that overlay**: `await highlightOverlay.RefreshAsync()`.

Keep the results layer separate from the source data so the source never has to redraw.

## Map coordinates

`MapClick` gives you `e.WorldLocation` (a `PointShape`) in the map's projection, usually EPSG:3857.   If the queried layer has a `ProjectionConverter`, pass map-space shapes straight to `QueryTools`; ThinkGeo converts for you and returns map-space features.   Only convert manually when querying a raw `FeatureSource` with no converter (see `thinkgeo-desktop-maps/references/projections.md`).

## Click to identify

Polygons: find the feature containing the click.

```csharp
private async void MapView_MapClick(object sender, MapClickMapViewEventArgs e)
{
    try
    {
        var parcels = (FeatureLayer)MapView.FindFeatureLayer("Parcels");
        Collection<Feature> hits;
        parcels.Open();
        try
        {
            hits = parcels.QueryTools.GetFeaturesContaining(e.WorldLocation, ReturningColumnsType.AllColumns);
        }
        finally
        {
            parcels.Close();
        }

        await ShowHighlightAsync(hits);
        if (hits.Count > 0)
            InfoText.Text = hits[0].ColumnValues["OWNER_NAME"];
    }
    catch (Exception ex)
    {
        Debug.WriteLine(ex);
    }
}
```

Points and lines: a click never lands exactly on them, so search within a pixel tolerance.

```csharp
// Convert a pixel tolerance into map units at the current zoom.
double metersPerPixel = MapView.CurrentExtent.Width / MapView.MapWidth;
double toleranceMeters = 8 * metersPerPixel;

var hits = layer.QueryTools.GetFeaturesWithinDistanceOf(
    e.WorldLocation, MapView.MapUnit, DistanceUnit.Meter, toleranceMeters,
    ReturningColumnsType.AllColumns);
```

Use the attribute values you need from `feature.ColumnValues[...]`.   Request columns explicitly or use `AllColumns`; reading a column that wasn't requested throws `KeyNotFoundException`.

`MapClick` waits for the double-click interval by default, so it feels slightly delayed.   For instant selection set `MapView.ClickDoubleClickMode = MapClickDoubleClickMode.RaiseClickThenDoubleClick`.

## The highlight helper

```csharp
// Setup (once): a dynamic layer in its own SingleTile overlay, added after the data overlays.
var highlightLayer = new InMemoryFeatureLayer();
highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
    AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.OrangeRed), GeoColors.OrangeRed, 2);
highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.OrangeRed, 4, true);
highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.OrangeRed, 14, GeoColors.White);
highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

var highlightOverlay = new LayerOverlay { TileType = TileType.SingleTile };
highlightOverlay.Layers.Add("Highlight", highlightLayer);
MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

// Update (each time):
private async Task ShowHighlightAsync(IEnumerable<Feature> features)
{
    var overlay = (LayerOverlay)MapView.Overlays["HighlightOverlay"];
    var layer = (InMemoryFeatureLayer)overlay.Layers["Highlight"];

    layer.InternalFeatures.Clear();
    foreach (var f in features)
        layer.InternalFeatures.Add(f);

    await overlay.RefreshAsync();
}
```

If the highlight layer labels features with a `TextStyle` or uses a `ValueStyle`, declare those columns first: `layer.Open(); layer.Columns.Add(new FeatureSourceColumn("NAME")); layer.Close();`.

## Spatial queries

All on `layer.QueryTools`, all require the layer to be open:

| Question | Method |
| --- | --- |
| Which features contain this point/shape? | `GetFeaturesContaining` |
| Which features are inside this shape? | `GetFeaturesWithin` |
| Which features touch or overlap this shape at all? | `GetFeaturesIntersecting` |
| Which features are within X distance? | `GetFeaturesWithinDistanceOf(shape, mapUnit, DistanceUnit, distance, columns)` |
| Every feature | `GetAllFeatures` |
| Exact attribute match | `GetFeaturesByColumnValue(column, value, columns)` |
| Also available | `GetFeaturesCrossing`, `GetFeaturesOverlapping`, `GetFeaturesTouching`, `GetFeaturesDisjointed`, `GetFeaturesTopologicalEqual` |

For large data, `GetFeaturesIntersecting` with a drawn rectangle or polygon is the usual "select by area" tool.   For topological-equality or touch tests across projections, pre-convert features into one projection first; rounding during on-the-fly conversion causes misses.

## Drawing with TrackOverlay

`MapView.TrackOverlay` is built in.   Set a mode, the user draws, `TrackEnded` delivers the shape.

```csharp
// Start drawing (e.g. from a button)
MapView.TrackOverlay.TrackMode = TrackMode.Polygon;   // or Point, Line, Rectangle, Circle, ...
MapView.TrackOverlay.TrackEnded += TrackOverlay_TrackEnded;

private async void TrackOverlay_TrackEnded(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
{
    try
    {
        // 1. Stop drawing and clear the temporary shape
        MapView.TrackOverlay.TrackMode = TrackMode.None;
        MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        MapView.TrackOverlay.TrackEnded -= TrackOverlay_TrackEnded;

        // 2. Use the shape (here: select parcels inside it)
        var parcels = (FeatureLayer)MapView.FindFeatureLayer("Parcels");
        parcels.Open();
        Collection<Feature> selected;
        try { selected = parcels.QueryTools.GetFeaturesIntersecting(e.TrackShape, ReturningColumnsType.AllColumns); }
        finally { parcels.Close(); }

        await ShowHighlightAsync(selected);
    }
    catch (Exception ex) { Debug.WriteLine(ex); }
}
```

- Point: single click.   Line and polygon: click to add vertices, double-click to finish.   Middle-drag pans while drawing; in WPF, Shift constrains to north-south / east-west.
- Cast `e.TrackShape` to the type for the mode (`PolygonShape`, `LineShape`, `PointShape`).   Casting to the wrong type is a common bug.
- Always set `TrackMode = TrackMode.None` when finished, or the map stays in drawing mode and won't pan normally.
- `TrackShapeLayer` is transient.   Copy shapes you want to keep into your own `InMemoryFeatureLayer`.
- Live measurement while drawing: handle `TrackOverlay.MouseMoved` and read `e.AffectedFeature.GetShape()`.

Read `references/editing.md` for the full draw / edit / delete workflow.

## Editing with EditOverlay

`MapView.EditOverlay` lets users move, rotate, resize, and reshape existing features.   The essentials:

1. Move features **out of** your layer and **into** `MapView.EditOverlay.EditShapesLayer`, so they don't draw twice.
2. Call `MapView.EditOverlay.CalculateAllControlPoints()` every time you add or change features there.   Without it, no handles appear and the overlay looks dead.
3. Refresh the edit overlay and your layer's overlay together: `await MapView.RefreshAsync(new Overlay[] { MapView.EditOverlay, layerOverlay })`.
4. When the user finishes, move features back and clear `EditShapesLayer`.
5. WinForms: if you add overlays dynamically and need the edit handles on top, add `MapView.EditOverlay` to `MapView.Overlays` explicitly.

Full code, snapping, and saving edits back to a data source are in `references/editing.md`.

## Markers and popups

- **Markers** (`SimpleMarkerOverlay` with `Marker` objects) are UI elements: good for a handful of pins, draggable pins, or pins with custom images.   For hundreds or thousands of points, use a feature layer with a `PointStyle` instead; markers don't scale.
- **Popups** (`PopupOverlay` with `Popup` objects) show WPF/WinForms content anchored to a map location.

```csharp
// Marker (WPF)
var markers = new SimpleMarkerOverlay();
MapView.Overlays.Add("Markers", markers);
markers.Markers.Add(new Marker(point)
{
    ImageSource = new BitmapImage(new Uri("/Resources/marker.png", UriKind.RelativeOrAbsolute)),
    Width = 20, Height = 34, YOffset = -17   // anchor the pin tip on the point
});
await markers.RefreshAsync();

// Popup on click
var popups = new PopupOverlay();
MapView.Overlays.Add("Popups", popups);
popups.Popups.Clear();
popups.Popups.Add(new Popup(e.WorldLocation) { Content = feature.ColumnValues["NAME"] });
await popups.RefreshAsync();
```

## Tool modes

Apps with several tools (pan, identify, draw, edit) should model them as explicit modes:

- One method per mode transition that cleans up the previous mode: flush `TrackShapeLayer` and `EditShapesLayer`, set `TrackMode = None`, and unsubscribe handlers such as `MapClick` and `TrackEnded`.
- Only one of TrackOverlay or EditOverlay should be active at a time.
- Forgetting to unsubscribe a `MapClick` handler makes it fire in the wrong mode.

## Checklist before handing back code

- Results go to a separate `SingleTile` overlay, refreshed on its own.
- Layers are opened and closed around every `QueryTools` call, with `try/finally`.
- Point and line hit-testing uses a pixel-based tolerance, not an exact containment test.
- Track and edit modes are switched off and their temporary layers cleared when the user leaves them.
- `CalculateAllControlPoints()` follows every change to `EditShapesLayer`.
- Event handlers are `async void` with `try/catch`, and are unsubscribed when the mode ends.
