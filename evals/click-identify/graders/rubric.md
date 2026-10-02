---
type: llm
---

Judge code for click-to-identify on a WPF ThinkGeo map: parcels are an EPSG:2276 shapefile shown on an EPSG:3857 map; clicking a parcel highlights it and shows OWNER_NAME in a TextBlock.

PASS only if all of these hold:
- The parcel layer's converter is `new ProjectionConverter(2276, 3857)`, and the query uses the click location (`e.WorldLocation` from `MapClick`) directly, without converting it manually (the layer's converter handles it).
- The query requests the OWNER_NAME column (by name or all columns), and the layer is opened before querying and closed afterwards, with try/finally or equivalent.
- The highlight goes into a separate `InMemoryFeatureLayer` in its own `LayerOverlay` with `TileType.SingleTile`, and only that overlay is refreshed (`overlay.RefreshAsync()`), not the whole map.
- Clicking empty space clears the highlight and the text, or the answer otherwise handles "no parcel found".

FAIL if any item is missing or wrong.
