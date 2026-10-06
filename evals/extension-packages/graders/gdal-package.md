---
type: regex
target:
  source: file
  path: "GpkgEcwViewer/GpkgEcwViewer.csproj"
pattern: 'Include="ThinkGeo\.UI\.Wpf"\s+Version="(\d+\.\d+\.\d+)"[\s\S]*Include="ThinkGeo\.Gdal"\s+Version="\1"|Include="ThinkGeo\.Gdal"\s+Version="(\d+\.\d+\.\d+)"[\s\S]*Include="ThinkGeo\.UI\.Wpf"\s+Version="\2"'
---
