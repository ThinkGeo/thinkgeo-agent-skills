# WPF Shapefile Sample

Status: **source-verified, not compiled in the generation environment**.

This is a minimal .NET 8 WPF project using `ThinkGeo.UI.Wpf` 14.5.3.   Place an EPSG:4326 polygon shapefile named `Countries02.shp` and its sidecar files (`.shx`, `.dbf`, `.prj`; `.idx`/`.ids` if present) in `Data/`.   ThinkGeo's WPF HowDoI repository ships this file under `samples/wpf/HowDoISample/Data/Shapefile/`; run `../get-test-data.ps1` to copy it from a local clone.

Changes from v0.1: replaced `mapView.Refresh()` with `await mapView.RefreshAsync()` (the current API reference lists only `RefreshAsync` on the desktop `MapView`), resolved the data path from the application folder, keyed the layer and overlay, and disposed the map when the window closes.

The implementation pattern is verified against:
- https://docs.thinkgeo.com/products/desktop-maps/quickstart-wpf/
- https://docs.thinkgeo.com/products/misc/Developer%20Guides/Desktop-Classes/desktop-refresh-and-cancellation/
- https://gitlab.com/thinkgeo/public/thinkgeo-desktop-maps/-/blob/master/samples/wpf/HowDoISample/SampleTemplate.xaml.cs (structure only; its `Refresh()` call is stale)
- https://gitlab.com/thinkgeo/public/thinkgeo-desktop-maps/-/blob/master/samples/wpf/HowDoISample/HowDoI.csproj

Build on Windows with a .NET 8 SDK and NuGet access:

```powershell
dotnet restore
dotnet build
```
