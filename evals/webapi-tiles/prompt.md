---
description: "Test case 14: ASP.NET Core WebAPI tile endpoint for a State Plane shapefile, shown in Leaflet. Must reproject to 3857, keep tile sizes consistent, and use content-root paths."
tags: [full, codegen, webapi]
max_turns: 40
timeout_seconds: 900
allowed_tools: [Read, Glob, Grep, Skill]
---

Build a minimal ASP.NET Core Web API that serves map tiles of a parcel shapefile (`App_Data\Parcels.shp`, State Plane EPSG:2276) with ThinkGeo, labelled with the OWNER_NAME column, plus a small Leaflet page that shows the tiles.   It will be deployed to IIS.

Write the project into a folder named `ParcelTiles` in the current directory, using exactly these files: `ParcelTiles/ParcelTiles.csproj`, `ParcelTiles/Program.cs`, `ParcelTiles/Controllers/TilesController.cs`, and `ParcelTiles/wwwroot/index.html`.   You can't run shell commands, so don't try to build; just write the files.
