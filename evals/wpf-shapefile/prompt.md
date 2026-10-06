---
description: "Test case 1: minimal WPF shapefile viewer that must build against the latest stable ThinkGeo release with no cloud dependency."
tags: [pilot, full, codegen, wpf]
max_turns: 40
timeout_seconds: 900
allowed_tools: [Read, Glob, Grep, Skill]
---

Create a minimal .NET 8 WPF app that displays a local polygon shapefile in ThinkGeo, styles the polygons, starts at a useful extent, and does not require ThinkGeo Cloud.   Assume the shapefile is `Data\Countries02.shp` in WGS84 (EPSG:4326).

Write the complete project into a folder named `ShapefileViewer` in the current directory, using exactly these files: `ShapefileViewer/ShapefileViewer.csproj`, `ShapefileViewer/App.xaml`, `ShapefileViewer/App.xaml.cs`, `ShapefileViewer/MainWindow.xaml`, and `ShapefileViewer/MainWindow.xaml.cs`.   You can't run shell commands, so don't try to build; just write the files.
