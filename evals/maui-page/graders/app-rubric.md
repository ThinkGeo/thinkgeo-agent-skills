---
type: llm
focus:
  source: file
  path: "MauiMap/App.xaml.cs"
---

This is App.xaml.cs for a .NET MAUI ThinkGeo app (Android and iOS) that ships a parcel shapefile.

PASS only if both hold:
- On first run it copies the shapefile and its sidecar files (at least .shp, .shx, .dbf) to `FileSystem.Current.AppDataDirectory`, for example from embedded resources with `GetManifestResourceStream`, skipping files that already exist.
- It loads the ThinkGeo license file(s) with `LicenseLoader.LoadLicense` before the map is shown (or the code clearly leaves that step to MainPage).

FAIL otherwise.
