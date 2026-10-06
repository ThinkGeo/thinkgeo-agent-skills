# Draw, Edit, Delete, and Save

Sources: TrackOverlay Guide, EditOverlay Guide, ShapeFileFeatureLayer Guide (docs.thinkgeo.com Developer Guides), and the "Edit Features" HowDoI samples.

## The state machine

Keep one persistent `InMemoryFeatureLayer` (in a `SingleTile` overlay) as the working store.   The user switches between modes; every transition flushes the built-in overlays back into the store first.

```
Navigate  ──►  Draw (TrackOverlay)  ──►  Edit (EditOverlay)  ──►  Delete (MapClick)
   ▲                                                                    │
   └──────────────── every transition calls FlushOverlays() ◄───────────┘
```

## Setup

```csharp
private InMemoryFeatureLayer _featureLayer = null!;
private LayerOverlay _layerOverlay = null!;

private void SetUpEditing()
{
    _featureLayer = new InMemoryFeatureLayer();
    _featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 8, GeoColors.Black);
    _featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Blue, 4, true);
    _featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
        AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.Blue), GeoColors.Black);
    _featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

    _layerOverlay = new LayerOverlay { TileType = TileType.SingleTile };
    _layerOverlay.Layers.Add("Features", _featureLayer);
    MapView.Overlays.Add("FeatureOverlay", _layerOverlay);

    // WinForms only, when overlays are added dynamically later:
    // MapView.Overlays.Add("Edit Overlay", MapView.EditOverlay);
}
```

## Flush helper (call before every mode change)

```csharp
private async Task FlushOverlaysAsync()
{
    foreach (var f in MapView.TrackOverlay.TrackShapeLayer.InternalFeatures)
        _featureLayer.InternalFeatures.Add(f.Id, f);
    MapView.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();

    foreach (var f in MapView.EditOverlay.EditShapesLayer.InternalFeatures)
        _featureLayer.InternalFeatures.Add(f.Id, f);
    MapView.EditOverlay.EditShapesLayer.InternalFeatures.Clear();

    MapView.TrackOverlay.TrackMode = TrackMode.None;
    MapView.MapClick -= DeleteMode_MapClick;

    await MapView.RefreshAsync(new Overlay[] { MapView.TrackOverlay, MapView.EditOverlay, _layerOverlay });
}
```

## Modes

```csharp
private async Task EnterNavigateModeAsync() => await FlushOverlaysAsync();

private async Task EnterDrawModeAsync(TrackMode shapeType)
{
    await FlushOverlaysAsync();
    MapView.TrackOverlay.TrackMode = shapeType;   // Point, Line, Polygon, Rectangle, Circle, ...
}

private async Task EnterEditModeAsync()
{
    await FlushOverlaysAsync();

    // Move features OUT of the store and INTO the edit layer so they don't draw twice
    foreach (var f in _featureLayer.InternalFeatures)
        MapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(f.Id, f);
    _featureLayer.InternalFeatures.Clear();

    MapView.EditOverlay.CalculateAllControlPoints();   // required, or no handles appear
    await MapView.RefreshAsync(new Overlay[] { MapView.EditOverlay, _layerOverlay });
}

private async Task EnterDeleteModeAsync()
{
    await FlushOverlaysAsync();
    MapView.MapClick += DeleteMode_MapClick;
}

private async void DeleteMode_MapClick(object sender, MapClickMapViewEventArgs e)
{
    try
    {
        _featureLayer.Open();
        Collection<Feature> hits;
        try { hits = _featureLayer.QueryTools.GetFeaturesContaining(e.WorldLocation, ReturningColumnsType.NoColumns); }
        finally { _featureLayer.Close(); }

        foreach (var f in hits)
            _featureLayer.InternalFeatures.Remove(f.Id);

        await _layerOverlay.RefreshAsync();
    }
    catch (Exception ex) { Debug.WriteLine(ex); }
}
```

`GetFeaturesContaining` only finds polygons under the click.   To delete points and lines too, use `GetFeaturesWithinDistanceOf` with a pixel tolerance, as in the main skill.

## Editing existing data

Load features from any layer into `EditShapesLayer` directly, for example the feature the user clicked:

```csharp
MapView.EditOverlay.EditShapesLayer.InternalFeatures.Clear();
MapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(selectedFeature.Id, selectedFeature);
MapView.EditOverlay.CalculateAllControlPoints();
await MapView.RefreshAsync(new Overlay[] { MapView.EditOverlay });
```

Features can also be built from WKT (`new Feature("POLYGON((...))")`) or from shapes (`new Feature(lineShape)`).   Coordinates must be in the map's projection.

Hide the original while it is being edited (for example, with a `FilterStyle`, or by removing it from an in-memory store) so the user doesn't see two copies.

## Edit events

| Event | Use |
| --- | --- |
| `EditOverlay.VertexMoving` | Fires continuously during a drag. Set `e.TargetVertex` to override the position, which is how snapping works (see the "Edit Features with Snapping" sample). |
| `EditOverlay.VertexMoved` | Fires when a drag ends. Read `e.AffectedFeature.GetShape()` for live area or length. |
| `TrackOverlay.MouseMoved` | Live measurement while drawing. |
| `TrackOverlay.TrackEnded` | A shape was completed. |

The "Edit Map Events" sample shows the full event set.   Use `tg_find_sample("Edit Map Events", platform: "wpf")` to read it.

## Saving edits to a data source

Feature layers that support editing expose `EditTools`, which work in a transaction:

```csharp
// ReadWrite is required; the default is read-only and CommitTransaction throws.
var target = new ShapeFileFeatureLayer(@".\Data\Parcels.shp", FileAccess.ReadWrite);
// Same converter as the display layer (file EPSG, map EPSG), so edits in map coordinates
// are written back in the file's projection.
target.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
target.Open();
try
{
    target.EditTools.BeginTransaction();
    target.EditTools.Add(newFeature);          // new geometry
    target.EditTools.Update(editedFeature);    // same Id as the original
    target.EditTools.Delete(deletedFeatureId);
    TransactionResult result = target.EditTools.CommitTransaction();
    // Check result for failures and report them to the user
}
finally
{
    target.Close();
}
```

Notes:

- **Open the layer read-write.**   Pass `FileAccess.ReadWrite` (from `System.IO`) to the constructor.   Without it, `CommitTransaction` throws an `IOException` saying to open the file in ReadWrite mode.   The `ShapeFileReadWriteMode` enum from the old MapSuite API does not exist in `ThinkGeo.Core`; `FileAccess` replaces it.
- Committing also throws an `IOException` if another process or another layer instance holds the `.shp`/`.dbf` open.   Close display layers on the same file, or edit a separate layer instance and refresh the display afterwards.
- **Projection on save:** if the layer's `FeatureSource` has a `ProjectionConverter`, pass features to `Add` and `Update` in **map coordinates**, exactly as they come from `TrackOverlay` or `EditOverlay`.   `EditTools` converts them back to the file's projection on commit (verified on ThinkGeo 14.5 with a 4326 shapefile and a 3857 map).   **Do not** call `ConvertToInternalProjection` yourself first: the features get converted twice and land near 0,0.   Only convert manually if you write to a layer that has no converter while the file and map projections differ; setting the converter on that layer is simpler.
- Preserve attribute values by copying `ColumnValues` from the original feature into the edited one before updating.
- Back up user data or write to a copy first when building an editing workflow for the first time.
