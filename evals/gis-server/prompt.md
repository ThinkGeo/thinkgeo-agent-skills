---
description: "Test case 3: minimal ASP.NET Core GIS Server serving one shapefile through WMS and GeoJSON."
tags: [full, codegen, gisserver]
max_turns: 40
timeout_seconds: 900
allowed_tools: [Read, Glob, Grep, Skill]
---

Build a minimal ASP.NET Core ThinkGeo GIS Server that serves one shapefile through WMS and GeoJSON.   The shapefile is `App_Data\Shapefile\Countries02.shp` in WGS84 (EPSG:4326).

Write the complete project into a folder named `GisServerDemo` in the current directory, using exactly these files: `GisServerDemo/GisServerDemo.csproj` and `GisServerDemo/Program.cs`.   You can't run shell commands, so don't try to build; just write the files.
