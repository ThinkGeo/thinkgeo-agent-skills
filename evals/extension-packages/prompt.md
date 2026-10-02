---
description: "Test case 11: GeoPackage plus ECW needs the ThinkGeo.Gdal extension package at the same version as the UI package."
tags: [pilot, full, codegen, packages, wpf]
max_turns: 40
timeout_seconds: 900
allowed_tools: [Read, Glob, Grep, Skill]
---

Show a GeoPackage file (`Data\parcels.gpkg`) and an ECW image (`Data\ortho.ecw`) in a new .NET 8 WPF ThinkGeo app.   Both are in EPSG:3857.

Write the complete project into a folder named `GpkgEcwViewer` in the current directory, using exactly these files: `GpkgEcwViewer/GpkgEcwViewer.csproj`, `GpkgEcwViewer/App.xaml`, `GpkgEcwViewer/App.xaml.cs`, `GpkgEcwViewer/MainWindow.xaml`, and `GpkgEcwViewer/MainWindow.xaml.cs`.   You can't run shell commands, so don't try to build; just write the files.
