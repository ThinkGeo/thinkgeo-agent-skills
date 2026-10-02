using System.Diagnostics;
using System.Windows;
using ThinkGeo.Core;
using ThinkGeo.UI.Wpf;

namespace MapApp
{
    public partial class MainWindow : Window, IDisposable
    {
        private bool _initialized;

        public MainWindow()
        {
            InitializeComponent();
            Closed += (_, _) => Dispose();
        }

        private async void MapView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Initialize once, after the control has a real size.
            if (_initialized || e.NewSize.Width <= 0 || e.NewSize.Height <= 0) return;
            _initialized = true;

            try
            {
                // 1. Map unit first. Meter = Spherical Mercator (EPSG:3857).
                MapView.MapUnit = GeographyUnit.Meter;

                // 2. Basemap. Get keys at https://cloud.thinkgeo.com/clients.html
                //    For offline/air-gapped apps, replace this with a local tile layer
                //    (see the thinkgeo-offline-maps skill).
                MapView.Overlays.Add("Basemap", new ThinkGeoCloudVectorMapsOverlay
                {
                    ClientId = "YOUR_CLIENT_ID",
                    ClientSecret = "YOUR_CLIENT_SECRET",
                    MapType = ThinkGeoCloudVectorMapsMapType.Light,
                    TileCache = new FileRasterTileCache(@".\cache", "basemap_light")
                });

                // 3. Static data layer (replace path, EPSG code, and style).
                var dataLayer = new ShapeFileFeatureLayer(@".\Data\YourData.shp");
                dataLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857); // (data EPSG, map EPSG)
                dataLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                    AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.SteelBlue), GeoColors.SteelBlue, 1);
                dataLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle =
                    LineStyle.CreateSimpleLineStyle(GeoColors.SteelBlue, 2, true);
                dataLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                    PointStyle.CreateSimpleCircleStyle(GeoColors.SteelBlue, 8, GeoColors.White);
                dataLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                var dataOverlay = new LayerOverlay();
                dataOverlay.Layers.Add("Data", dataLayer);
                MapView.Overlays.Add("DataOverlay", dataOverlay);

                // 4. Dynamic layer for results/highlights (SingleTile so changes show immediately).
                var highlightLayer = new InMemoryFeatureLayer();
                highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                    AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(90, GeoColors.OrangeRed), GeoColors.OrangeRed, 2);
                highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
                    PointStyle.CreateSimpleCircleStyle(GeoColors.OrangeRed, 12, GeoColors.White);
                highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                var highlightOverlay = new LayerOverlay { TileType = TileType.SingleTile };
                highlightOverlay.Layers.Add("Highlight", highlightLayer);
                MapView.Overlays.Add("HighlightOverlay", highlightOverlay);

                // 5. Initial view: zoom to the data.
                dataLayer.Open();
                MapView.CurrentExtent = dataLayer.GetBoundingBox();
                dataLayer.Close();

                await MapView.RefreshAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "Map initialization failed");
            }
        }

        public void Dispose()
        {
            MapView.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
