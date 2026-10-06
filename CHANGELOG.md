# Changelog

## Unreleased

Added:
- MIT license (`LICENSE`, and `"license": "MIT"` in both plugin manifests).
- README: install with `npx skills add ThinkGeo/thinkgeo-agent-skills` for Cursor, GitHub Copilot, Codex, Claude Code, and other assistants (tested: all twelve skills install with their reference files and starters), plus how to connect the MCP server, which that route doesn't install.
- `EVALUATION.md`: full A/B/C evaluation (no MCP, MCP only, MCP + skills; 18 cases, 2 runs each).   Average score 0.76 / 0.92 / 0.99; generated projects that build 2 / 6 / 10 out of 10.

Fixed:
- `evals/webapi-tiles`: removed the "converter" regex, which rejected correct code using named constants; the code rubric already checks the converter.

## 0.5.0

Knowledge skills for the last two platforms, Blazor and .NET MAUI, so every released ThinkGeo platform now has one.

Added:
- New skill `thinkgeo-blazor`: Blazor map pages with `ThinkGeo.UI.Blazor`.   Covers hosting (Blazor Server for ThinkGeo layers; WebAssembly and Hybrid only for client-drawn overlays such as `VectorTileOverlay`, per ThinkGeo support), interactive server rendering in .NET 8+ Blazor Web Apps, automatic client assets, the `MapView` settings blocks, `RedrawAsync` after data changes, click coordinates, markers, popups, `EditOverlay`, vector tiles, and navigation.   The example page builds with 0 warnings in a .NET 8 Blazor Web App on 14.5.5.
- `evals/`: cases 16 (Blazor map page) and 17 (Blazor WebAssembly with own data).
- New skill `thinkgeo-maui`: .NET MAUI map apps with `ThinkGeo.UI.Maui`.   A table of differences from desktop (`ThinkGeoVectorOverlay`, `MapScale`, `SingleTap` with `ToWorldCoordinate`, `AppDataDirectory`, rotation and tilt), licensing on phones, shipping data files (embedded resources copied to `AppDataDirectory`, because Android asset streams aren't seekable), tap tolerance, GPS with projection conversion, refreshing one overlay, Cloud keys, offline, and troubleshooting.   The example page builds for Windows against 14.5.5.
- `evals/`: cases 18 (MAUI map page) and 19 (MAUI data works on Windows, not Android).

## 0.4.0

Two new knowledge skills (`thinkgeo-cloud-maps` and `thinkgeo-web-api`), licensing for every released platform, ThinkGeo Maps Streets, and an automated evaluation suite.   The skills now tell assistants to use the latest stable ThinkGeo version from NuGet instead of naming one, and GIS Server is left out until it ships.

Fixed:
- `thinkgeo-code-example`: its description contained ": ", which broke YAML parsing, so Claude Code loaded the skill with no name or description (introduced after 0.3.0, so 0.3.0 itself is unaffected).   `validate_package.py` now rejects descriptions that would break YAML.
- `thinkgeo-code-review`: a review missed `TileType.MultipleTiles` because `tg_api` shows enums without their values.   The skill now says to read the enum's page with `tg_get`, lists the names that don't exist in 14.5.3 (with replacements), and checks for the desktop UI using.

Changed:
- Versions: the skills no longer name a "current" release.   New projects use the latest stable ThinkGeo version from NuGet; existing projects keep their version unless the user asks to upgrade; never mix versions or use betas.   Version-specific facts are phrased to survive releases (for example, PMTiles "needs ThinkGeo 15 or later").   The latest stable release turned out to be 14.5.5 (August 31, 2026), not 14.5.3: the sample project files lag behind.   All DLL-based findings were re-checked on 14.5.5.
- Starter and test projects moved to 14.5.5 (all build with 0 warnings).   Eval graders for new projects accept any stable version as long as the ThinkGeo packages match.
- `package-map.md`: `ThinkGeo.Ecw`, `ThinkGeo.MrSid`, and `ThinkGeo.Jpeg2000` stopped at 14.2.1; use `ThinkGeo.Gdal`, which contains the same layers.
- Removed GIS Server from the skills (platform lists, `gisServerHowDoI` namespace, the `ThinkGeo.GisServer` package row) while the product is unreleased and its API is expected to change.
- Evals and tests: retired the GIS Server eval (case 3) and the GIS Server test project; rewrote case 6 (architecture) around released products, an offline field app plus office review.

Added:
- ThinkGeo Maps Streets (`thinkgeo-offline-maps`): getting the data from the Helpdesk (PMTiles, world or regional extracts; MBTiles on request for ThinkGeo 14.x), its MapLibre `style.json`, Spherical Mercator only, display only (not routable), and sizing.   The legacy ThinkGeo StyleJSON format is noted as legacy.   The Cloud skill points to it as the offline alternative.
- New skill `thinkgeo-web-api`: ASP.NET Core map tile services with `ThinkGeo.UI.WebApi` and Leaflet or OpenLayers (the tile endpoint, Spherical Mercator, matching tile sizes, labels at tile edges, content-root data paths, concurrent drawing and `ThreadSafe`, SkiaSharp libraries on Linux, the browser side).   No recommendation yet on sharing layers across requests or on server tile caching.   Examples compile against 14.5.5.   The workflow skills now search the new `webApiHowDoI` sample namespace.
- `evals/`: cases 14 (WebAPI tile service) and 15 (WebAPI blank tiles after publishing).
- New skill `thinkgeo-cloud-maps`: ThinkGeo Cloud on any platform.   Covers which key type each platform needs (ClientId and ClientSecret vs the Blazor `ApiKey`), keeping keys out of code and restricting them by IP or domain, the overlay names per platform (MAUI uses `ThinkGeoVectorOverlay`), service clients in `ThinkGeo.Core` (not the old `ThinkGeo.Cloud.Client` package), coordinate systems, `result.Exception` error handling, and quotas.   Examples compile against 14.5.3.   Linked from the architecture, code-example, and offline skills.
- `evals/`: cases 12 (Cloud geocoding) and 13 (Cloud keys).
- Offline use of Cloud tiles: ThinkGeo allows customers to pre-cache and ship Cloud tiles.   `thinkgeo-cloud-maps` shows how (`ThinkGeoRasterMapsAsyncLayer.GenerateTileCacheAsync`, then `IsCacheOnly`), and `thinkgeo-offline-maps` lists it as a basemap option.
- `thinkgeo-code-example/references/licensing.md`: licensing by platform, starting with MAUI (license files for Android, iOS, and Mac Catalyst loaded with `LicenseLoader.LoadLicense`, required even to debug; MAUI on Windows; symptoms).   Linked from the troubleshoot, docs-research, and desktop skills.
- `licensing.md`: Blazor and WebAPI (same Product Center tab as desktop; servers need a runtime license because a web app on an expired developer license shows a watermark; symptom table).   Linux and container licensing isn't in the current docs, so the skill sends users to ThinkGeo support for now.
- `licensing.md`: managing developer licenses (buying, moving a license to another machine or developer, the command-line Product Center for Linux and macOS, legacy downloads) and the Product Center reset procedure.
- Desktop license symptoms: added "Not Licensed for Map Development" (debugging with only a runtime license); the expired-subscription row no longer says to regenerate the runtime license, since runtime licenses don't expire.
- `evals/`: the other eight test cases (2-8 and 10), so all 11 run with `--tag full`.
- `validate_package.py`: exempts `evals/code-review/prompt.md`, which contains wrong calls on purpose.

## 0.3.0

First release built and tested against ThinkGeo 14.5.3: the starter and test projects compile, API and package claims were checked against the 14.5.3 DLLs, and an automated evaluation pilot compares the skills with the MCP server alone.

Fixed:
- WPF test project and WPF starter now build (missing usings; missing `App.xaml`).
- `package-map.md` checked against the 14.5.3 package DLLs: added `Jpeg2000GdalRasterLayer`, `GeoTiffGdalRasterLayer`, and `PersonalGeoDatabaseGdalFeatureLayer`, listed the printer layers, and noted that the API reference (and so `tg_api`) is missing many extension-package classes.
- PMTiles: `VectorPmTilesAsyncLayer` isn't in 14.5.3 (only the 15.0 betas).   The offline-maps skill now defaults to MBTiles and marks PMTiles as ThinkGeo 15 only.
- `XyzFileTilesAsyncLayer`, `DynamicPointStyle`, and `FleeBooleanStyle` are now labeled as classes defined in HowDoI samples, not ThinkGeo APIs.
- Editing: `EditTools` reprojects on save when the layer has a `ProjectionConverter` (tested on 14.5.3), so `editing.md` now says to pass map coordinates and not to convert first.   It previously suggested converting, which moves features twice.
- Editing: shapefile layers must be opened with `FileAccess.ReadWrite` to commit edits.   `editing.md` previously said no mode was needed.   Added both problems to the troubleshooting symptom table.
- Offline licensing: `thinkgeo-offline-maps` now explains that never-connected machines need no activation (the runtime license is per executable name and perpetual), replacing the advice to ask support for an install-time procedure.
- Namespaces: generated desktop code often missed `using ThinkGeo.UI.Wpf;` (HowDoI samples hide it because they're declared inside `namespace ThinkGeo.UI.Wpf.HowDoI`).   `thinkgeo-desktop-maps`, `project-setup.md`, the code-generation checklist, the sample traps in `thinkgeo-docs-research`, and the troubleshooting table now say so, along with the WPF `System.IO` using.

Changed:
- Source ranking (`thinkgeo-docs-research`, `thinkgeo-code-example`): which source wins now depends on the question.   The API reference decides whether a member exists; current HowDoI samples rank ahead of developer guides for how to use it; guides cover concepts.   "How do I" questions search samples first.   Added the sample-defined helper class trap and the API reference gaps (extension packages, WinForms UI types).
- `Refresh()`: confirmed by compiling against 14.5.3 that the WPF `MapView` has no `Refresh()`, and that on WinForms it compiles as `Control.Refresh()` without redrawing the map.   Updated the troubleshoot skill, `refresh-and-async.md`, and the validator message.
- `README.md` install instructions lead with the GitHub mirror (`/plugin marketplace add ThinkGeo/thinkgeo-agent-skills`); the GitLab address is listed as an alternative.

Added:
- `.gitignore` for build output and copied test data.
- `evals/`: three automated `claude plugin eval` cases (test cases 1, 9, and 11), with results in `EVALUATION.md`.
- `deployment.md`: logging licensing messages with `ThinkGeoDebugger` to diagnose a license watermark on locked-down machines.

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
