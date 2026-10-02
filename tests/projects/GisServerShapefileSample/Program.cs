using ThinkGeo.Core;
using ThinkGeo.GisServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddThinkGeoWebServer();

builder.Services.Configure<ThinkGeoWebServerOptions>(options =>
{
    options.Wms.BasePath = "/wms";
    options.GeoJson.BasePath = "/geojson";

    var shapefilePath = Path.Combine(
        builder.Environment.ContentRootPath,
        "App_Data", "Shapefile", "Countries02.shp");

    var rasterLayers = new[]
    {
        new RasterLayerDefinition
        {
            Name = "countries",
            Title = "Countries",
            CreateLayer = () =>
            {
                var layer = new ShapeFileFeatureLayer(shapefilePath);
                layer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
                layer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                    AreaStyle.CreateSimpleAreaStyle(GeoColors.PapayaWhip, GeoColors.DarkGoldenrod, 1);
                layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                return layer;
            }
        }
    };

    var vectorLayers = new[]
    {
        new VectorLayerDefinition
        {
            Name = "countries",
            Title = "Countries",
            IsQueryable = true,
            CreateFeatureSource = () => new ShapeFileFeatureSource(shapefilePath)
        }
    };

    options.Wms.Maps["countries"] = new WmsMapOptions
    {
        Name = "countries",
        Title = "Countries",
        Abstract = "Countries shapefile rendered by ThinkGeo GIS Server.",
        Crs = "EPSG:3857",
        MapUnit = GeographyUnit.Meter,
        FullExtent = MaxExtents.SphericalMercator,
        BackgroundColor = GeoColors.White,
        RasterLayers = rasterLayers
    };

    options.GeoJson.Maps["countries"] = new VectorMapOptions
    {
        Name = "countries",
        Title = "Countries",
        Abstract = "Countries shapefile exposed as GeoJSON.",
        Crs = "EPSG:4326",
        MapUnit = GeographyUnit.DecimalDegree,
        FullExtent = MaxExtents.DecimalDegree,
        VectorLayers = vectorLayers
    };
});

var app = builder.Build();
app.MapThinkGeoWebServer();
app.Run();
