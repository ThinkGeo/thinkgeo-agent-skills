# Changelog

## Unreleased

Fixed:
- WPF test project and WPF starter now build (missing usings; missing `App.xaml`).
- `package-map.md` checked against the 14.5.3 package DLLs: added `Jpeg2000GdalRasterLayer`, `GeoTiffGdalRasterLayer`, and `PersonalGeoDatabaseGdalFeatureLayer`, listed the printer layers, and noted that `tg_api` doesn't cover extension packages.
- PMTiles: `VectorPmTilesAsyncLayer` isn't in 14.5.3 (only the 15.0 betas).   The offline-maps skill now defaults to MBTiles and marks PMTiles as ThinkGeo 15 only.
- `XyzFileTilesAsyncLayer`, `DynamicPointStyle`, and `FleeBooleanStyle` are now labeled as classes defined in HowDoI samples, not ThinkGeo APIs.

Added:
- `.gitignore` for build output and copied test data.

## 0.2.0

Merged the v0.1 workflow skills with the WPF/WinForms knowledge skills.

Added:
- `thinkgeo-desktop-maps`, `thinkgeo-desktop-interaction`, and `thinkgeo-offline-maps` skills, with references and WPF/WinForms starter projects.
- `thinkgeo-troubleshoot/references/desktop-symptoms.md`: ranked causes for blank maps, pink tiles, license watermarks, stale tiles, exceptions, labels, performance, and editing problems.
- `thinkgeo-code-example/references/package-map.md`: which NuGet package each ThinkGeo type needs, and version rules.
- Claude Code plugin files: `.claude-plugin/plugin.json`, `.claude-plugin/marketplace.json`, `.mcp.json`.
- Five new test cases (click-to-identify, draw-to-select, air-gapped basemap, pink tiles, extension packages) and `tests/projects/get-test-data.ps1`.

Changed:
- `thinkgeo-troubleshoot` now combines the evidence-first workflow with the desktop symptom guide.
- `thinkgeo-code-example` requires checking members with `tg_api` even when an official sample uses them, verifying the containing NuGet package, and keeping all ThinkGeo packages on one version.
- Skill descriptions now name the symptoms and terms users actually type, so the right skill activates more reliably.
- `validate_package.py` no longer hard-codes the skill count or requires specific source text; it now checks name/folder match, cross-references, mixed package versions, credentials, and known-invalid API calls.

Fixed:
- WPF test project called `mapView.Refresh()`, which is not in the current API reference.   It now awaits `mapView.RefreshAsync()` inside `try/catch`, resolves the data path from the application folder, and disposes the map.
- Starter projects pin `14.5.3` instead of a floating `14.*`.
- Numbered-list formatting in reference files.

Documentation issues found in ThinkGeo's own docs (reported separately, not fixed here):
- WPF `SampleTemplate.xaml.cs` calls `mapView.Refresh()`.
- Architecture Guide: `ZoomLevel01` described as most zoomed in; `ShapeFileFeatureLayer.BuildIndex` (should be `BuildIndexFile`); `TileType.MultipleTiles` (should be `MultiTile`).
- ProjectionConverter Guide, Pattern 4: converter arguments reversed.

## 0.1.0

Initial five workflow skills, portable plugin manifests, WPF and GIS Server test projects, and evaluation plan.
