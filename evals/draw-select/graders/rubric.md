---
type: llm
---

Judge WinForms ThinkGeo code that lets the user draw a polygon and lists the hotel points inside it.

PASS only if all of these hold:
- It sets `mapView.TrackOverlay.TrackMode = TrackMode.Polygon` and handles `TrackOverlay.TrackEnded`.
- In the handler it takes the drawn shape from `e.TrackShape` (cast to `PolygonShape` or used as a `BaseShape`) and queries the hotel layer with `GetFeaturesWithin` or `GetFeaturesIntersecting`, requesting the NAME column, with the layer opened before and closed after.
- After drawing, it resets `TrackMode` to `TrackMode.None` and clears or manages `TrackShapeLayer` so old polygons don't pile up.
- It unsubscribes the TrackEnded handler when the mode ends, or explains why a single subscription is safe.
- It uses WinForms-appropriate code (`ThinkGeo.UI.WinForms`, updating the ListBox on the UI thread).

FAIL if any item is missing or wrong.
