---
type: llm
focus:
  source: file
  path: "MauiMap/App.xaml.cs"
---

This is App.xaml.cs for a .NET MAUI ThinkGeo app (Android and iOS) that ships a parcel shapefile.

PASS only if both hold:
- On first run it copies the shapefile and its sidecar files (at least .shp, .shx, .dbf) to `FileSystem.Current.AppDataDirectory`, for example from embedded resources with `GetManifestResourceStream`, skipping files that already exist.
- Licensing matches the ThinkGeo version the project uses: on ThinkGeo 14 and earlier, it loads the license file(s) with `LicenseLoader.LoadLicense` before the map is shown (or the code clearly leaves that step to MainPage); on ThinkGeo 15 and later, it does NOT call `LicenseLoader.LoadLicense` (the build checks the developer's license key instead).   If the version isn't visible, accept either.

FAIL otherwise.
