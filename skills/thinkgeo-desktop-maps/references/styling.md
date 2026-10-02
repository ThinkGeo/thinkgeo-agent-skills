# Styling and Zoom Levels

Sources: ZoomLevelSet and ZoomLevel Guide, ValueStyle Guide, ClassBreakStyle Guide, TextStyle Guide, and the Vector Data Styling HowDoI samples.

## How zoom levels work

Every `FeatureLayer` has a `ZoomLevelSet` with twenty levels, `ZoomLevel01` through `ZoomLevel20`.

- `ZoomLevel01` is the **most zoomed out** (largest scale denominator).
- `ZoomLevel20` is the **most zoomed in**.
- A style draws only at the level it is assigned to, unless `ApplyUntilZoomLevel` extends it.

Show at all scales:

```csharp
layer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(
    GeoColor.FromArgb(50, GeoColors.Blue), GeoColors.Blue);
layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
```

Show progressively more detail as the user zooms in:

```csharp
// Levels 12-13: small dots
hotels.ZoomLevelSet.ZoomLevel12.DefaultPointStyle =
    new PointStyle(PointSymbolType.Circle, 4, GeoBrushes.DarkRed, new GeoPen(GeoBrushes.White, 2));
hotels.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level13;

// Levels 14-20: larger dots plus labels
hotels.ZoomLevelSet.ZoomLevel14.DefaultPointStyle =
    new PointStyle(PointSymbolType.Circle, 10, GeoBrushes.DarkRed, new GeoPen(GeoBrushes.White, 2));
hotels.ZoomLevelSet.ZoomLevel14.DefaultTextStyle =
    new TextStyle("NAME", new GeoFont("Segoe UI", 10, DrawingFontStyles.Bold), GeoBrushes.DarkRed)
    {
        TextPlacement = TextPlacement.Lower,
        YOffsetInPixel = 4,
        HaloPen = new GeoPen(GeoBrushes.White, 2),
        DrawingLevel = DrawingLevel.LabelLevel
    };
hotels.ZoomLevelSet.ZoomLevel14.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
```

Keep scale bands contiguous.   A gap (for example, styles on 12-13 and 15-20 but nothing on 14) makes the layer disappear at that zoom.

## Default style slots vs `CustomStyles`

Each `ZoomLevel` has `DefaultPointStyle`, `DefaultLineStyle`, `DefaultAreaStyle`, and `DefaultTextStyle`, plus a `CustomStyles` collection.   Thematic styles (`ValueStyle`, `ClassBreakStyle`, `ClusterPointStyle`, `HeatStyle`, `FilterStyle`) go in `CustomStyles`.

If you rebuild styles at runtime, call `CustomStyles.Clear()` first; otherwise styles accumulate and draw on top of each other.

## Basic styles

```csharp
// Points
PointStyle.CreateSimpleCircleStyle(GeoColors.Red, 10, GeoColors.Black);
new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Blue, new GeoPen(GeoBrushes.White, 2));
new PointStyle(new GeoImage(@".\Resources\pin.png")) { ImageScale = 0.5 };

// Lines (color, width, antialias)
LineStyle.CreateSimpleLineStyle(GeoColors.Blue, 4, true);
// Road casing: outer pen, inner pen
new LineStyle(new GeoPen(GeoBrushes.Gray, 8), new GeoPen(GeoBrushes.White, 6));

// Areas (fill, outline, outline width)
AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(60, GeoColors.Green), GeoColors.DarkGreen, 1);
```

## Labels (`TextStyle`)

- The first argument is the column name to label with.   The column must be fetched (shapefiles fetch what styles need automatically; `InMemoryFeatureLayer` columns must be declared).
- Set `DrawingLevel = DrawingLevel.LabelLevel` so labels draw above features.
- Use `HaloPen` for legibility over basemaps.
- For streets, `SplineType = SplineType.StandardSplining` curves labels along lines.
- Labels across layers share collision detection, so overlapping labels are dropped automatically.   Use `GridSize` to control density.

## Categories: `ValueStyle`

```csharp
var valueStyle = new ValueStyle { ColumnName = "ZONE_TYPE" };
valueStyle.ValueItems.Add(new ValueItem("Residential",
    AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.Gold), GeoColors.Goldenrod)));
valueStyle.ValueItems.Add(new ValueItem("Commercial",
    AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColors.Red), GeoColors.DarkRed)));

layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);
layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
```

Matching is an exact string comparison on the column value.   A `ValueItem` whose `Value` is `""` acts as the fallback for unmatched values; add it first.

## Numeric ranges: `ClassBreakStyle`

```csharp
var classBreakStyle = new ClassBreakStyle("POP_DENSITY");
classBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue,
    AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(120, GeoColors.LightYellow), GeoColors.Gray)));
classBreakStyle.ClassBreaks.Add(new ClassBreak(1000,
    AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(120, GeoColors.Orange), GeoColors.Gray)));
classBreakStyle.ClassBreaks.Add(new ClassBreak(5000,
    AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(120, GeoColors.DarkRed), GeoColors.Gray)));

layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(classBreakStyle);
layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
```

Each break applies from its value up to the next break.   Start with `double.MinValue` so low values aren't dropped.   The column must be numeric or parse as a number.

## Other thematic styles

| Need | Style | HowDoI sample |
| --- | --- | --- |
| Many points at low zoom | `ClusterPointStyle` | Display Cluster Points |
| Density surface | `HeatStyle` | Display HeatMap |
| Draw only features matching a condition | `FilterStyle` | Render Based on Filters |
| Match by regular expression | `RegexStyle` / `RegexItem` | Render Based on Regex |
| Dot density by value | `DotDensityStyle` | Display Dot Density |
| Hatched fills | `GeoHatchStyle` brushes | Hatch Styles |
| Label from multiple columns | `TextStyle` with a format | Create a Multi-Column Text Style |
| Fully custom drawing | Subclass `Style` and override `DrawCore` | Custom Styles |

Confirm constructor signatures for these with `tg_api` before using them, or adapt the sample directly.

## Colors, brushes, pens

- `GeoColors.*` named colors; `GeoColor.FromArgb(alpha, color)` or `GeoColor.FromArgb(a, r, g, b)` for transparency.
- `GeoBrushes.*` and `new GeoSolidBrush(color)` for fills.
- `new GeoPen(color or brush, width)` for outlines.
- `GeoFont(name, size, DrawingFontStyles.Bold)` for text.   On machines without the font, the renderer falls back, so prefer common fonts or ship the font.
