using System.Diagnostics;
using ThinkGeo.Core;
using ThinkGeo.UI.WinForms;

namespace MapApp
{
    public class MainForm : Form
    {
        private readonly MapView _mapView;

        public MainForm()
        {
            Text = "Map";
            Width = 1100;
            Height = 700;

            _mapView = new MapView { Dock = DockStyle.Fill };
            Controls.Add(_mapView);

            Load += MainForm_Load;
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            try
            {
                // 1. Map unit first. Meter = Spherical Mercator (EPSG:3857).
                _mapView.MapUnit = GeographyUnit.Meter;

                // 2. Basemap. Get keys at https://cloud.thinkgeo.com/clients.html
                _mapView.Overlays.Add("Basemap", new ThinkGeoCloudVectorMapsOverlay
                {
                    ClientId = "YOUR_CLIENT_ID",
                    ClientSecret = "YOUR_CLIENT_SECRET",
                    MapType = ThinkGeoCloudVectorMapsMapType.Light,
                    TileCache = new FileRasterTileCache(@".\cache", "basemap_light")
                });

                // 3. Data layer (replace path, EPSG code, and style).
                var dataLayer = new ShapeFileFeatureLayer(@".\Data\YourData.shp");
                dataLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857); // (data EPSG, map EPSG)
                dataLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                    AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.SteelBlue), GeoColors.SteelBlue, 1);
                dataLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                var dataOverlay = new LayerOverlay();
                dataOverlay.Layers.Add("Data", dataLayer);
                _mapView.Overlays.Add("DataOverlay", dataOverlay);

                // 4. Initial view: zoom to the data.
                dataLayer.Open();
                _mapView.CurrentExtent = dataLayer.GetBoundingBox();
                dataLayer.Close();

                await _mapView.RefreshAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "Map initialization failed");
            }
        }
    }
}
