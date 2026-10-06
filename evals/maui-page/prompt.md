---
description: "Test case 18: MAUI page with a shapefile shipped in the app; tap to highlight. Must copy data to AppDataDirectory, convert tap coordinates, and use the MAUI basemap overlay."
tags: [full, codegen, maui]
max_turns: 40
timeout_seconds: 900
allowed_tools: [Read, Glob, Grep, Skill]
---

In my .NET MAUI app (Android and iOS), show a parcel shapefile that ships inside the app (`Parcels.shp` and its sidecar files, State Plane EPSG:2276) over a ThinkGeo Cloud basemap.   When the user taps a parcel, highlight it.

Write the page code-behind to `MauiMap/MainPage.xaml.cs` and any data-copy code to `MauiMap/App.xaml.cs` in the current directory.   You can't run shell commands, so don't try to build; just write the files.
