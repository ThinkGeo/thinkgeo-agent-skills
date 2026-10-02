---
type: llm
---

Judge the final answer to a user whose EPSG:4326 shapefile is invisible on an EPSG:3857 (GeographyUnit.Meter) WPF ThinkGeo map.   Their code has no ProjectionConverter and sets CurrentExtent from the layer's bounding box.

PASS only if all of these hold:
- It identifies the missing ProjectionConverter as the cause and fixes it by assigning `new ProjectionConverter(4326, 3857)` (data projection first, map projection second) to the layer's `FeatureSource.ProjectionConverter`.
- It addresses the extent: without the converter, the bounding box is in degrees, so the map zooms to a tiny area near 0,0; with the converter assigned before opening the layer, `GetBoundingBox()` returns map (3857) coordinates.
- It does not blame file corruption, licensing, or the cloud basemap as the main cause.
- It shows corrected code.

FAIL if any item is missing or wrong, or if the converter arguments are reversed.
