---
type: llm
focus:
  source: file
  path: "ParcelTiles/wwwroot/index.html"
---

This is the Leaflet page for a ThinkGeo WebAPI tile service.

PASS if it creates a Leaflet map and adds an `L.tileLayer` whose URL template ends in `{z}/{x}/{y}` and points at the service's tile route, without a tile size that conflicts with the server (the default 256, or a matching explicit size).   If it adds a ThinkGeo Cloud basemap, it uses a JavaScript API key placeholder, never a ClientId/ClientSecret.   FAIL otherwise.
