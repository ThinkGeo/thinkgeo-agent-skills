---
description: "Test case 16: Blazor page with a State Plane parcel layer, click to highlight and show OWNER_NAME. Must use interactive server rendering, RedrawAsync, and a converter."
tags: [full, codegen, blazor]
max_turns: 40
timeout_seconds: 900
allowed_tools: [Read, Glob, Grep, Skill]
---

In my .NET 8 Blazor Web App, add a page at `/parcels` with a ThinkGeo map of a parcel shapefile (`App_Data\Parcels.shp`, State Plane EPSG:2276) over a ThinkGeo Cloud basemap.   When the user clicks a parcel, highlight it and show its OWNER_NAME under the map.

Write the page to `BlazorMap/Components/Pages/Parcels.razor` in the current directory.   You can't run shell commands, so don't try to build; just write the file.
