---
description: "Test case 19: MAUI shapefile works on Windows but fails on Android when loaded from Resources/Raw."
tags: [full, troubleshoot, maui]
max_turns: 20
timeout_seconds: 600
allowed_tools: [Read, Glob, Grep, Skill]
---

My ThinkGeo MAUI app shows our shapefile fine on Windows, but on Android the layer never appears.   I put the .shp, .shx and .dbf in Resources\Raw as MauiAsset and open the layer with `new ShapeFileFeatureLayer("Parcels.shp")`.   What's wrong?
