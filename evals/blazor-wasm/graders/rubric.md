---
type: llm
---

Judge the answer to a team that wants a Blazor WebAssembly ThinkGeo app showing their own shapefiles and a SQL Server layer with custom styles.

PASS only if all of these hold:
- It says clearly that ThinkGeo layers in a `LayerOverlay` (shapefiles, SQL Server, custom styles) are drawn on the server and are not supported in Blazor WebAssembly.
- It recommends Blazor Server (or a Blazor Web App with interactive server rendering) for this app.
- It mentions that only client-drawn overlays (such as `VectorTileOverlay` or `WmtsTileOverlay`) work in WebAssembly, or otherwise explains what WebAssembly can and can't show.
- It notes a consequence of server-side drawing, such as data files living on the server or server load growing with users, or that SQL Server credentials stay on the server.

FAIL if it says the app will work in WebAssembly as described, or if any item is missing.
