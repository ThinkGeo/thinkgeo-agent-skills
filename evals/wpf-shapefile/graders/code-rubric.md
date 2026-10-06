---
type: llm
focus:
  source: file
  path: "ShapefileViewer/MainWindow.xaml.cs"
---

This is the code-behind of a .NET 8 WPF ThinkGeo app that should show a local polygon shapefile (`Data\Countries02.shp`, WGS84 / EPSG:4326) with no ThinkGeo Cloud dependency.

PASS only if all of these hold:
- Map unit and data projection agree: either `MapUnit = GeographyUnit.DecimalDegree` with no conversion, or `GeographyUnit.Meter` with a `ProjectionConverter(4326, 3857)` assigned to the layer's `FeatureSource.ProjectionConverter`.
- A `ShapeFileFeatureLayer` gets an area style on a zoom level, applied through `ApplyUntilZoomLevel.Level20` (or an equivalent custom zoom level setup), and is added to a `LayerOverlay` in `MapView.Overlays`.
- The initial extent comes from the layer's bounding box, read after opening the layer, or is another sensible fixed extent.
- Setup runs after the control is loaded or sized (a `Loaded` or `SizeChanged` handler), and the map is drawn with `RefreshAsync`.

FAIL if any item is missing or wrong.   Don't penalize style choices, comments, or extra error handling.
