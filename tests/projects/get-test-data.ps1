# Copies the Countries02 test shapefile from a local clone of ThinkGeo's desktop samples
# into the WPF test project.
#
#   git clone https://gitlab.com/thinkgeo/public/thinkgeo-desktop-maps.git
#   ./get-test-data.ps1 -SamplesRepo ..\..\..\thinkgeo-desktop-maps
#
param(
    [Parameter(Mandatory = $true)]
    [string]$SamplesRepo
)

$source = Join-Path $SamplesRepo "samples/wpf/HowDoISample/Data/Shapefile"
if (-not (Test-Path (Join-Path $source "Countries02.shp"))) {
    Write-Error "Countries02.shp not found under $source. Check the clone path."
    exit 1
}

$targets = @(
    (Join-Path $PSScriptRoot "WpfShapefileSample/Data")
)

foreach ($target in $targets) {
    New-Item -ItemType Directory -Force -Path $target | Out-Null
    Copy-Item (Join-Path $source "Countries02.*") $target -Force
    Write-Host "Copied Countries02.* to $target"
}
