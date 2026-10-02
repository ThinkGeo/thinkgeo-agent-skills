using System.Diagnostics;
using System.Windows;
using ThinkGeo.Core;

namespace WpfShapefileSample;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closed += (_, _) => mapView.Dispose();
    }

    private async void MapView_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // This sample deliberately uses decimal degrees end-to-end so that it has no
            // cloud dependency and does not need reprojection for EPSG:4326 test data.
            mapView.MapUnit = GeographyUnit.DecimalDegree;

            var shapefilePath = Path.Combine(AppContext.BaseDirectory, "Data", "Countries02.shp");
            var countriesLayer = new ShapeFileFeatureLayer(shapefilePath);
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
                AreaStyle.CreateSimpleAreaStyle(
                    GeoColor.FromArgb(255, 233, 232, 214),
                    GeoColor.FromArgb(255, 118, 138, 69));
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            var overlay = new LayerOverlay();
            overlay.Layers.Add("Countries", countriesLayer);
            mapView.Overlays.Add("CountriesOverlay", overlay);

            countriesLayer.Open();
            mapView.CurrentExtent = countriesLayer.GetBoundingBox();
            countriesLayer.Close();

            // The desktop MapView exposes RefreshAsync; there is no synchronous Refresh()
            // in the current API reference (the HowDoI SampleTemplate still shows one).
            await mapView.RefreshAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            MessageBox.Show(ex.Message, "Map initialization failed");
        }
    }
}
