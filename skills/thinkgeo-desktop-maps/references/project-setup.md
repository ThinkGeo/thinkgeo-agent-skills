# Project Setup, Licensing, and Deployment

Sources: docs.thinkgeo.com Quick Start Guides (WPF, WinForms, VS Code) and the Desktop Deployment Guide.

## Packages

| Package | Use |
| --- | --- |
| `ThinkGeo.UI.Wpf` | WPF `MapView` control. Pulls in `ThinkGeo.Core`. |
| `ThinkGeo.UI.WinForms` | WinForms `MapView` control. Pulls in `ThinkGeo.Core`. |
| `ThinkGeo.Core` | GIS engine only (no UI). Use for console tools, services, or tile pre-generation. |
| `ThinkGeo.Dependency.MicrosoftVisualCRunTime140` | Optional VC++ runtime dependency. If used, deploy the Visual C++ Redistributable on target machines. |

Data formats such as GeoPackage, KML, ECW, MrSID, SQL Server, PostgreSQL, CAD, File Geodatabase, and S-57 need extension packages (`ThinkGeo.Gdal`, `ThinkGeo.SqlServer`, and so on).   See `references/package-map.md` in the `thinkgeo-code-example` skill, and keep every ThinkGeo package on the same version.

ThinkGeo publishes a release branch and a beta branch on NuGet.   Beta packages are prereleases; recommend the release branch for production unless the user needs a specific beta fix.

```
dotnet add package ThinkGeo.UI.Wpf
```

## WPF project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <!-- Current release as of October 2026; use the same version for every ThinkGeo package -->
    <PackageReference Include="ThinkGeo.UI.Wpf" Version="14.5.3" />
  </ItemGroup>
  <ItemGroup>
    <!-- Copy local data next to the executable -->
    <None Update="Data\**\*">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
</Project>
```

XAML namespace:

```xml
xmlns:thinkgeo="clr-namespace:ThinkGeo.UI.Wpf;assembly=ThinkGeo.UI.Wpf"
```

Code-behind needs both `using ThinkGeo.Core;` and `using ThinkGeo.UI.Wpf;`.   Any code that creates a `LayerOverlay` or touches `MapView` members by type uses the UI namespace.   HowDoI sample files don't show this using because they're declared inside `namespace ThinkGeo.UI.Wpf.HowDoI`; add it when you copy sample code into your own namespace.   WPF projects also leave `System.IO` out of the implicit usings (it clashes with `System.Windows.Shapes.Path`), so add `using System.IO;` for `Path` and `File`.

## WinForms project

The WinForms control relies on WPF assemblies (`WindowsBase`, `WindowsFormsIntegration`), so WPF support must be enabled.   In Visual Studio this is the "Enable WPF for this project" checkbox; in the project file it is `<UseWPF>true</UseWPF>` alongside `<UseWindowsForms>true</UseWindowsForms>`.

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net8.0-windows</TargetFramework>
  <UseWindowsForms>true</UseWindowsForms>
  <UseWPF>true</UseWPF>
</PropertyGroup>
<ItemGroup>
  <!-- Current release as of October 2026; use the same version for every ThinkGeo package -->
  <PackageReference Include="ThinkGeo.UI.WinForms" Version="14.5.3" />
</ItemGroup>
```

Create the control in code or the designer:

```csharp
using ThinkGeo.Core;
using ThinkGeo.UI.WinForms;

public partial class MainForm : Form
{
    private readonly MapView mapView;

    public MainForm()
    {
        InitializeComponent();
        mapView = new MapView { Dock = DockStyle.Fill };
        Controls.Add(mapView);
        Load += MainForm_Load;
    }

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        mapView.MapUnit = GeographyUnit.Meter;
        // ... same overlay/layer code as WPF ...
        await mapView.RefreshAsync();
    }
}
```

Where WPF and WinForms differ:

| Topic | WPF | WinForms |
| --- | --- | --- |
| Init event | `SizeChanged` with guard, or `Loaded` | Form `Load` |
| Declarative overlays | XAML supported | Code only |
| Disposal | Implement `IDisposable`, call `MapView.Dispose()` | Automatic |
| `EditOverlay` in draw order | Automatic | Add explicitly if you control overlay order: `mapView.Overlays.Add("Edit Overlay", mapView.EditOverlay)` |
| Concurrent overlay rendering | `DefaultOverlaysRenderSequenceType="Concurrent"` on `MapView` | Configured per overlay |

## Licensing

This section covers WPF and WinForms.   MAUI, Blazor, and other platforms differ; see `thinkgeo-code-example/references/licensing.md`.

ThinkGeo uses two kinds of license file:

- **Dev license**: installed on the developer machine by ThinkGeo Product Center.   Needed to build and debug.
- **Runtime license**: generated per application for deployment.

Steps for a developer:

1. Register at https://helpdesk.thinkgeo.com/register and download Product Center.
2. Sign in, open the WPF/WinForms/Blazor/WebAPI tab, and click "Start Evaluation" (30-day trial) or "Activate" for a purchased license.
3. Run the app.   Before a license is installed, the first run throws a "licenses not installed" exception and opens the registration site.   This is normal.

Steps to deploy:

1. In Product Center, open the "Runtime License" tab and browse to the application's built executable.
2. Click "Save".   A runtime license file is generated in the executable's folder.
3. Ship that file next to the executable.   Re-generate it if the executable name changes.
4. If the project uses `ThinkGeo.Dependency.MicrosoftVisualCRunTime140`, install the Visual C++ Redistributable on target machines.

A deployed app with a missing or invalid runtime license shows a blank white map with a "Not Licensed for Run Time" watermark rather than crashing.   Expired evaluation licenses show a "subscription license has expired" message.   See the `thinkgeo-troubleshoot` skill for diagnosis.

Do not write code that bypasses, patches, or fakes license checks.   If the user asks, explain the supported process above and point them to ThinkGeo support.

## ThinkGeo Cloud keys

`ThinkGeoCloudVectorMapsOverlay`, `ThinkGeoCloudRasterMapsOverlay`, and the Cloud clients (geocoding, routing, elevation, etc.) need a ClientId and ClientSecret from https://cloud.thinkgeo.com/clients.html.   The keys in ThinkGeo's quick-start guide are for testing only; tell users to create their own.   Load real keys from configuration, user secrets, or environment variables rather than source code.

Desktop apps that must run without internet access should not use Cloud overlays at all.   See the `thinkgeo-offline-maps` skill.
