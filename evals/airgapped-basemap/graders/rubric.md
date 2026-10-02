---
type: llm
---

Judge the final answer for a WPF app on ThinkGeo 14.5.3 that must run on an air-gapped network.

PASS only if all of these hold:
- It replaces the ThinkGeo Cloud overlay with a local source that exists in 14.5.3 (for example VectorMbTilesAsyncLayer with an .mbtiles file, raster MBTiles, imagery, a pre-generated tile cache, or the user's own vector data).
- It does NOT recommend VectorPmTilesAsyncLayer (PMTiles) as the solution for 14.5.3.   Mentioning PMTiles is fine only if the answer clearly says it requires ThinkGeo 15 or a beta.
- If it uses vector tiles with a Style JSON, it warns that the style's sources, glyphs (fonts), and sprites must all be local files.
- It says the runtime license file must ship next to the application executable, and doesn't suggest bypassing, patching, or faking licensing.
- It mentions at least one of: the geographic area or zoom range to cover, the size of the data file, or that the user needs a license or source for the offline map data.

FAIL if any item is missing or wrong.
