# WPF HowDoI Sample Catalog

Every WPF sample in the ThinkGeo Desktop Maps HowDoI project, with the key API it demonstrates.   WinForms has a matching set under `samples/winforms/HowDoISample` with most of the same titles.

- Browse: https://gitlab.com/thinkgeo/public/thinkgeo-desktop-maps/-/tree/master/samples/wpf/HowDoISample
- With the ThinkGeo Documentation MCP server: `tg_find_sample(query: "<title or task>", platform: "wpf")`, then `tg_get` on the returned `docId` to read the code.

When the user asks for a feature, find the closest sample here, read it, and adapt it.   The samples use `SampleKeys.ClientId` / `SampleKeys.ClientSecret` and `./Data/...` paths; replace those with the user's own keys and data.

## Map Navigation

| Title | Key API |
| --- | --- |
| Map Navigation (zoom, pan, rotate) | `ZoomToAsync` |
| Zoom to the Black Hole (image layers at scales) | `GeoImageLayer` |
| Zoom to Extents | `ZoomToAsync` |
| Vehicle Navigation (follow GPS points) | `FeatureLayerWpfDrawingOverlay` |
| Resize the Map | `MapResizeMode` |
| Dynamic Rendering | `DynamicPointStyle` |
| Overview Map | `EditOverlay` |
| Restrict Map Extent | `RestrictExtent`, `MinimumScale`, `MaximumScale` |

## Map Online Data

| Title | Key API |
| --- | --- |
| ThinkGeo Raster Map | `ThinkGeoCloudRasterMapsOverlay` |
| ThinkGeo Vector Map | `ThinkGeoCloudVectorMapsOverlay` |
| Raster Map from XYZ Server | `ThinkGeoRasterMapsAsyncLayer` |
| Azure Map | `AzureMapsRasterOverlay` |
| Google Map | `GoogleMapsOverlay` |
| Open Street Map | `OpenStreetMapOverlay` |
| NOAA Weather Stations | `NoaaWeatherStationFeatureLayer` |
| NOAA Weather Warnings | `NoaaWeatherWarningsFeatureLayer` |
| OGC API Feature Server | `OgcApiFeatureLayer` |
| MVT / MVT with Local Fonts | `MvtTilesAsyncLayer` |
| WFS | `WfsV2AsyncLayer` |
| WMS | `WmsAsyncLayer` |
| WMTS | `WmtsOverlay` |

## Map Offline Data

| Title | Key API |
| --- | --- |
| Display a GeoPackage File | `GdalFeatureLayer` |
| Display Raster from MBTiles | `RasterMbTilesAsyncLayer` |
| Display Raster from File Tiles | `XyzFileTilesAsyncLayer` |
| Display Vector from MBTiles | `VectorMbTilesAsyncLayer` |
| Display a SQLite File | `SqliteFeatureLayer` |
| Display Common Raster Files (PNG, JPEG, BMP) | `SkiaRasterLayer` |
| Display an ECW File | `EcwGdalRasterLayer` |
| Display a ESRI Grid File | `GridFeatureLayer` |
| Display a Gdal File | `GdalRasterLayer` |
| Display a GeoTiff File | `GeoTiffRasterLayer` |
| Display a JPEG2000 File / MrSid File | `MrSidGdalRasterLayer` |
| Display a CAD File (.dwg) | `CadFeatureLayer` |
| Display a FileGeoDatabase | `FileGeoDatabaseFeatureLayer` |
| Display a GeoPdf File | `GeoPdfGdalFeatureLayer` |
| Display a GPX File | `GpxFeatureLayer` |
| Display Graticule Lines | `GraticuleLineStyle` |
| Display an InMemory File | `InMemoryFeatureLayer` |
| Display a KML File | `KmlGdalFeatureLayer` |
| Display a GeoJson File | `Feature.CreateFeaturesFromGeoJson` |
| Display a S57 (Nautical Charts) File | `NauticalChartsFeatureLayer` |
| Display a Shapefile File | `ShapeFileFeatureLayer` |
| Display a TAB File | `TabFeatureLayer` |
| Display a TinyGeo File | `TinyGeoFeatureLayer` |
| Display a table from Postgres | `PostgreSqlFeatureLayer` |
| Display a table from SQL Server | `SqlServerFeatureLayer` |
| Generate an ESRI Grid File | `GenerateGrid` |
| Grouping Layers Using LayerOverlay | `LayerOverlay` |

## XYZ Based Layers

| Title | Key API |
| --- | --- |
| Display Projected OSM Layer | `OpenStreetMapAsyncLayer`, `GdalProjectionConverter` |
| Display Raster from WMTS Server | `WmtsAsyncLayer` |
| Display Vector from MVT Server | `MvtTilesAsyncLayer` |
| Pre-Generate Cache for XYZ Layers | `GenerateTileCacheAsync` |

(The MBTiles and file-tile samples above also appear in this category.)

## Map Projection

| Title | Key API |
| --- | --- |
| Project Features | `ProjectionConverter` |
| Project a Raster | `GdalProjectionConverter` |
| Setting the Projection of a Layer | `ProjectionConverter` |
| Project the World (Vector) | `ProjectionConverter` |
| Project the World (Raster) | `GdalProjectionConverter` |

## Map Tools and Adornments

| Title | Key API |
| --- | --- |
| Map Tool Controls (logo, mouse coordinates, scale line, pan/zoom bar) | `MouseCoordinateMapTool` |
| Drag/Resize Adornments | `AdornmentLayer` |
| ScaleLine and ScaleBar | `ScaleLineAdornmentLayer` |
| Magnetic North | `MagneticDeclinationAdornmentLayer` |

## Markers and Popups

| Title | Key API |
| --- | --- |
| Markers | `SimpleMarkerOverlay` |
| Animated Marker | `SimpleMarkerOverlay` |
| Text Marker | `Marker` |
| Popups | `PopupOverlay` |

## Vector Data Styling

| Title | Key API |
| --- | --- |
| Render Points / Lines / Areas / Labels | `PointStyle`, `LineStyle`, `AreaStyle`, `TextStyle` |
| Render Based on Filters | `FilterStyle` |
| Render Based on Regex | `RegexItem` |
| Render Based on Scales | `ApplyUntilZoomLevel` |
| Render Based on Values | `ValueStyle` |
| Render Based on ClassBreaks | `ClassBreakStyle` |
| Display Cluster Points | `ClusterPointStyle` |
| Display Dot Density | `DotDensityStyle` |
| Display HeatMap | `HeatStyle` |
| Display ISOLine | `ClassBreakStyle` |
| Hatch Styles | `GeoHatchStyle` |
| Custom Styles | `TimeBasedPointStyle` (custom `Style` subclass) |
| Create a Flee Boolean Style | `FleeBooleanStyle` |
| Create a Multi-Column Text Style | `TextStyle` |

## Vector Data Editing

| Title | Key API |
| --- | --- |
| Edit Features | `EditOverlay` |
| Edit Features with Snapping | `VertexMovingEditInteractiveOverlayEventArgs` |
| Edit Map Events | `EditOverlay` |

## Vector Data Spatial Query

| Title | Key API |
| --- | --- |
| Check if Features are Equal | `GetFeaturesTopologicalEqual` |
| Find Containing Features | `GetFeaturesContaining` |
| Find Crossing Features | `GetFeaturesCrossing` |
| Find Disjoint Features | `GetFeaturesDisjointed` |
| Find Features Within a Distance | `GetFeaturesWithinDistanceOf` |
| Find Features Within a Feature | `GetFeaturesWithin` |
| Find Intersecting Features | `GetFeaturesIntersecting` |
| Find Overlapping Features | `GetFeaturesOverlapping` |
| Find Touching Features | `GetFeaturesTouching` |
| Get Data from All Features | `GetAllFeatures` |
| Get Data from One Feature | `GetFeaturesContaining` |

## Vector Data Geometric Operation

| Title | Key API |
| --- | --- |
| Union Shapes | `Union` |
| Buffer a Shape | `Buffer` |
| Simplify a Shape | `Simplify` |
| Rotate / Scale / Translate a Shape | `Rotate`, `ScaleTo`, `TranslateByDegree` |
| Calculate the Center Point / Area / Length | `GetCenterPoint`, `GetArea`, `GetLength` |
| Get Shortest Line | `GetShortestLineTo` |
| Get Line on a Line | `GetLineOnALine` |
| Clip Shape | `GetIntersection` |
| Get Shape Differences | `GetDifference` |
| Get Convex Hull | `GetConvexHull` |
| Get Envelope | `GetBoundingBox` |

## Vector Data Topological Validation

| Title | Key API |
| --- | --- |
| Validate Point / Line / Polygon Topology | `TopologyValidator` |

## ThinkGeo Cloud Integration

| Title | Key API |
| --- | --- |
| Color Utilities | `ColorCloudClient` |
| Elevation | `ElevationCloudClient` |
| Geocoding | `GeocodingCloudClient` |
| Projection | `ProjectionCloudClient` |
| Reverse Geocoding | `ReverseGeocodingCloudClient` |
| Routing / Service Area / Traveling Salesperson | `RoutingCloudClient` |
| Timezone | `TimeZoneCloudClient` |
| World Maps Query | `MapsQueryCloudClient` |

## Miscellaneous

| Title | Key API |
| --- | --- |
| Pre-Generate Cache for Tile Overlay | `GenerateTileCacheAsync` |
| Print the Map | `MapPrinterLayer` |
| Handle Exceptions | `ThrowingExceptionMode`, custom layer |
| Wrap the DateLine | `WrappingMode.WrapDateline` |
| Draw the map on an Image | `ThinkGeoRasterMapsAsyncLayer` |
| Get Map SnapShot | `GetSnapshot` |
| Perf Test: Refresh shapes (20,000 polygons per second) | `ValueStyle` |
| Custom Background | `GeoLinearGradientBrush` |
| Custom Feature Layer | Subclass `Layer` |
| Custom Feature Sources | Subclass `FeatureSource` |
| Navigate On TouchScreen | `IsManipulationEnabled` |
| Basic Map Events | `CurrentExtentChangedMapViewEventArgs` |
