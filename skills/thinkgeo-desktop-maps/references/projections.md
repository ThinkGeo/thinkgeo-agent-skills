# Projections and ProjectionConverter

Source: ProjectionConverter Guide, docs.thinkgeo.com Developer Guides.

## The one rule

`new ProjectionConverter(fileEpsg, mapEpsg)`

- **Internal** projection = where the data is stored (the file or table).
- **External** projection = what the map renders in (almost always EPSG:3857).

Getting these backwards moves features by thousands of kilometres or makes them vanish.   Unprojected data is the single most common cause of "I can't see my layer".

## Common EPSG codes

| EPSG | Name | Units | `MapUnit` if used as the map projection |
| --- | --- | --- | --- |
| 3857 | Web / Spherical Mercator (ThinkGeo Cloud, OSM, Google, Bing) | meters | `GeographyUnit.Meter` |
| 4326 | WGS84 lat/lon (GPS, GeoJSON) | degrees | `GeographyUnit.DecimalDegree` |
| 4269 | NAD83 lat/lon | degrees | `GeographyUnit.DecimalDegree` |
| 2276 | NAD83 / Texas North Central (State Plane) | US survey feet | `GeographyUnit.Feet` |
| 26914 | NAD83 / UTM Zone 14N | meters | `GeographyUnit.Meter` |
| 27700 | British National Grid | meters | `GeographyUnit.Meter` |

Helpers also exist for proj strings, for example `Projection.GetDecimalDegreesProjString()` and `Projection.GetSphericalMercatorProjString()`.

## Finding a dataset's projection

- Shapefiles: open the `.prj` file.   Tools like QGIS or `ogrinfo` report the EPSG code.
- Coordinates between -180 and 180 / -90 and 90 are almost certainly degrees (4326).
- Coordinates in the hundreds of thousands or millions are projected (State Plane, UTM, or 3857).   Ask the user or the data provider rather than guessing a State Plane zone.
- GeoJSON is 4326 by specification unless stated otherwise.

## Pattern 1: reproject a layer for display (most common)

```csharp
var hotels = new ShapeFileFeatureLayer(@".\Data\Hotels.shp");
hotels.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
// Assign before the layer opens, i.e. before the first refresh.
```

ThinkGeo opens and closes the converter with the feature source and handles both directions, so `QueryTools` calls accept map-space (3857) shapes and return map-space features.

## Pattern 2: convert one point or feature

```csharp
// GPS position (4326) to map space (3857)
var converter = new ProjectionConverter(4326, 3857);
converter.Open();
var mapFeature = converter.ConvertToExternalProjection(new Feature(-96.8345, 33.1501));
converter.Close();
MapView.CenterPoint = (PointShape)mapFeature.GetShape();
```

Standalone converters must be opened before use and closed afterwards.   Opening is relatively expensive, so convert in batches (`ConvertToExternalProjection(IEnumerable<Feature>)`) rather than opening per point.

## Pattern 3: map click back to data coordinates

```csharp
// e.WorldLocation is in map space (3857); data is in 2276
var converter = new ProjectionConverter(2276, 3857);
converter.Open();
var dataSpace = converter.ConvertToInternalProjection(new Feature(e.WorldLocation));
converter.Close();
```

Only needed when querying a `FeatureSource` that has no converter attached.   If the layer already has a converter, query with map-space shapes directly.

## Pattern 4: change projection at runtime

```csharp
var layer = MapView.FindFeatureLayer("World");
layer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326,
    "+proj=aea +lat_1=29.5 +lat_2=45.5 +lat_0=37.5 +lon_0=-96 +datum=NAD83 +units=m +no_defs");
layer.FeatureSource.ProjectionConverter.Open(); // required when the layer is already open
MapView.MapUnit = GeographyUnit.Meter;
await MapView.RefreshAsync();
```

## Rasters and tile layers

Vector converters use `ProjectionConverter`.   Raster and XYZ tile reprojection uses `GdalProjectionConverter`; see the "Project a Raster" and "Project the World (Raster)" samples.

## Pitfalls

1. Internal and external reversed.
2. `MapUnit` doesn't match the external projection (for example, a 3857 map left at `DecimalDegree`).
3. Converter assigned after the layer was opened without calling `Open()` on it.
4. One layer in a multi-layer map missing its converter.   Check every layer when debugging.
5. Precise topological queries (equal, touches) across an on-the-fly projection can fail because of floating-point rounding.   Pre-convert features into one coordinate space, put them in an `InMemoryFeatureLayer`, and query that.
6. Mixing datums (NAD27 vs NAD83/WGS84) without accounting for them causes offsets of tens to hundreds of metres.
