# GIS Server Shapefile Sample

Status: **source-verified, not compiled in the generation environment**.

This minimal ASP.NET Core project publishes a local EPSG:4326 polygon shapefile as:
- WMS (server-rendered in EPSG:3857)
- GeoJSON (vector output in EPSG:4326)

Place `Countries02.shp` and its sidecar files under `App_Data/Shapefile/`.   Run `../get-test-data.ps1` to copy them from a local clone of ThinkGeo's desktop HowDoI repository.

Package note: this project pins `ThinkGeo.GisServer` 14.5.0-beta072 because the official GIS Server sample project references that beta build.   Switch to a release version when one is published, and keep every ThinkGeo package in the project on the same version.

The package/API pattern is verified against the current official GIS Server HowDoI project and shapefile module:
- https://gitlab.com/thinkgeo/public/thinkgeo-gis-server/-/blob/main/ThinkGeo.GisServer.Samples/ThinkGeo.GisServer.Samples.csproj
- https://gitlab.com/thinkgeo/public/thinkgeo-gis-server/-/blob/main/ThinkGeo.GisServer.Samples/Samples/Shapefile_WmsWmtsXyzWfsGeoJsonModule.cs
- https://gitlab.com/thinkgeo/public/thinkgeo-gis-server/-/blob/main/ThinkGeo.GisServer.Samples/Program.cs

Build on a machine with .NET 8 and NuGet access:

```bash
dotnet restore
dotnet build
dotnet run
```
