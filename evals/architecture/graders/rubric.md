---
type: llm
---

Judge an architecture recommendation for a self-hosted Windows/Linux service that must expose WMS, WMTS, XYZ, WFS and GeoJSON over local data with no cloud dependency.

PASS only if all of these hold:
- It recommends ThinkGeo GIS Server (the `ThinkGeo.GisServer` package on ASP.NET Core), hosted on the customer's own Windows or Linux servers.
- It addresses each of the five protocols (WMS, WMTS, XYZ, WFS, GeoJSON), saying how GIS Server serves it or noting any gap.
- It keeps data local (shapefiles, databases, or other local sources) and avoids ThinkGeo Cloud.
- It mentions that the GIS Server package is currently released as a beta, or tells the user to check the package's release status.
- It separates what ThinkGeo documents from its own design advice (for example caching, scaling, or deployment suggestions are presented as recommendations).

FAIL if any item is missing or wrong.
