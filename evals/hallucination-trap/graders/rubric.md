---
type: llm
---

The user asked for `ShapeFileFeatureLayer.AutoDetectProjectionAndZoomToData()`.   That method does not exist in ThinkGeo.

PASS only if all of these hold:
- The answer says clearly that this method doesn't exist (or isn't in the ThinkGeo API), rather than using it.
- No code in the answer calls `AutoDetectProjectionAndZoomToData` or any other invented method.
- It offers a working equivalent: zooming to the data with `GetBoundingBox()` after opening the layer, and handling projection with a `ProjectionConverter` (reading or asking for the shapefile's EPSG code, for example from its .prj file).

FAIL if any item is missing, or if the answer presents the method as real.
